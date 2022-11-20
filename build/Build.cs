using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

[ShutdownDotNetAfterServerBuild]
partial class Build : NukeBuild
{
	/// Support plugins are available for:
	///   - JetBrains ReSharper        https://nuke.build/resharper
	///   - JetBrains Rider            https://nuke.build/rider
	///   - Microsoft VisualStudio     https://nuke.build/visualstudio
	///   - Microsoft VSCode           https://nuke.build/vscode
	public static int Main() => Execute<Build>();

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution(GenerateProjects = true)] readonly Solution Solution;

	[GitRepository] readonly GitRepository GitRepository;

	[GitVersion] readonly GitVersion GitVersion;

	AbsolutePath SourceDirectory => RootDirectory / "src";
	AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			EnsureCleanDirectory(ArtifactsDirectory);
		});

	Target Restore => _ => _
		.Executes(() =>
		{
			/*if (GitHubActions != null)
			{
				NuGetSourcesUpdate(s => s
					.SetName("Atlas-Rhythm GH Packages")
					.SetUserName(GitHubActions.RepositoryOwner)
					.SetPassword(GitHubActions.Token));
			}*/

			DotNetRestore(s => s
				.SetProjectFile(Solution));
		});

	Target Compile => _ => _
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetBuild(s => s
				.EnableNoRestore()
				.SetProjectFile(Solution)
				.SetConfiguration(Configuration)
				.SetDeterministic(IsServerBuild)
				.SetContinuousIntegrationBuild(IsServerBuild)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion));
		});

	Target Pack => _ => _
		.DependsOn(Clean, Compile)
		.Executes(() =>
		{
			DotNetPack(s => s
				.EnableNoRestore()
				.EnableNoBuild()
				.SetProject(Solution.BeatSaberModdingTools_Nuke)
				.SetProperty("RepositoryBranch", GitRepository.Branch)
				.SetProperty("RepositoryCommit", GitRepository.Commit)
				.SetConfiguration(Configuration)
				.SetOutputDirectory(ArtifactsDirectory));
		});

	Target PushToGithubPackagesRegistry => _ => _
		.DependsOn(Pack)
		.Executes(() =>
		{
			IEnumerable<AbsolutePath> artifactPackages = ArtifactsDirectory.GlobFiles("*.nupkg");

			var ghPackageRegistryUrl = $"https://nuget.pkg.github.com/{GitRepository.GetGitHubOwner()}/index.json";

			DotNetNuGetPush(s => s
				.SetSource(ghPackageRegistryUrl)
				.SetApiKey(GitHubActions.Token)
				.EnableSkipDuplicate()
				.CombineWith(artifactPackages, (_, v) => _
					.SetTargetPath(v)));
		});
}