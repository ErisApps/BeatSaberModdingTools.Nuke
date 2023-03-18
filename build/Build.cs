using System.Collections.Generic;
using Helpers;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Helpers.DotNetNugetUpdateSourceTask;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

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
			if (GitHubActions != null)
			{
				DotNetNuGetUpdateSource(s => s
					.SetName("Atlas-Rhythm GH Packages")
					.SetUsername(GitHubActions.RepositoryOwner)
					.SetPassword(GitHubActions.Token)
					.EnableStorePasswordInClearText());
			}

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
				.SetVersion(GitVersion.FullSemVer)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion));
		});

	Target Pack => _ => _
		.DependsOn(Clean, Compile)
		.Produces(ArtifactsDirectory / "*.nupkg")
		.Executes(() =>
		{
			DotNetPack(s => s
				.EnableNoRestore()
				.EnableNoBuild()
				.SetProject(Solution.BeatSaberModdingTools_Nuke)
				.SetConfiguration(Configuration)
				.SetVersion(GitVersion.NuGetVersion)
				.SetProperty("RepositoryBranch", GitRepository.Branch)
				.SetProperty("RepositoryCommit", GitRepository.Commit)
				.SetOutputDirectory(ArtifactsDirectory));
		});

	Target PushToGithubPackagesRegistry => _ => _
		.DependsOn(Pack)
		/*.OnlyWhenDynamic(() => GitHubActions != null &&
		                       GitHubActions.EventName == "push" &&
		                       GitHubActions.Ref != null && GitHubActions.Ref.Contains("/refs/tags/"))*/
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