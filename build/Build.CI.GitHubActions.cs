using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;

[GitHubActions(
	"pr",
	GitHubActionsImage.UbuntuLatest,
	AutoGenerate = true,
	OnPullRequestBranches = new [] { "main" },
	OnPullRequestIncludePaths = new[] { "**/*" },
	OnPullRequestExcludePaths = new[] { ".editorconfig", ".gitignore", "README.MD" },
	EnableGitHubToken = true,
	PublishArtifacts = false,
	InvokedTargets = new[] { nameof(Compile), nameof(Pack) },
	CacheKeyFiles = new[] { "src/**/*.csproj" })
]
[GitHubActions(
	"publish",
	GitHubActionsImage.UbuntuLatest,
	AutoGenerate = true,
	OnPushBranches = new [] { "main" },
	OnPushTags = new[] { "*.*.*" },
	OnPushIncludePaths = new[] { "**/*" },
	OnPushExcludePaths = new[] { ".editorconfig", ".gitignore", "README.MD" },
	EnableGitHubToken = true,
	PublishArtifacts = false,
	InvokedTargets = new[] { nameof(Compile),  nameof(Pack), nameof(PushToGithubPackagesRegistry) },
	CacheKeyFiles = new[] { "src/**/*.csproj" })
]
partial class Build
{
	[CI] readonly GitHubActions GitHubActions;
}