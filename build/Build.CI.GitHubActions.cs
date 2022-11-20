using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;

[GitHubActions(
	"pr",
	GitHubActionsImage.UbuntuLatest,
	AutoGenerate = true,
	FetchDepth = 0, // Only a single commit is fetched by default, for the ref/SHA that triggered the workflow. Make sure to fetch whole git history, in order to get GitVersion to work.
	OnPullRequestBranches = new [] { "main" },
	OnPullRequestIncludePaths = new[] { "**/*" },
	OnPullRequestExcludePaths = new[] { ".editorconfig", ".gitignore", "README.MD" },
	EnableGitHubToken = true,
	PublishArtifacts = false,
	InvokedTargets = new[] { nameof(Compile), nameof(Pack) },
	CacheKeyFiles = new string[0])
]
[GitHubActions(
	"publish",
	GitHubActionsImage.UbuntuLatest,
	AutoGenerate = true,
	FetchDepth = 0, // Only a single commit is fetched by default, for the ref/SHA that triggered the workflow. Make sure to fetch whole git history, in order to get GitVersion to work.
	OnPushBranches = new [] { "main" },
	OnPushTags = new[] { "*.*.*" },
	OnPushIncludePaths = new[] { "**/*" },
	OnPushExcludePaths = new[] { ".editorconfig", ".gitignore", "README.MD" },
	EnableGitHubToken = true,
	PublishArtifacts = false,
	InvokedTargets = new[] { nameof(Compile),  nameof(Pack), nameof(PushToGithubPackagesRegistry) },
	CacheKeyFiles = new string[0])
]
partial class Build
{
	[CI] readonly GitHubActions GitHubActions;
}