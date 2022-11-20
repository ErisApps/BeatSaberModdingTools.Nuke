﻿using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;

[GitHubActions(
	"pr",
	GitHubActionsImage.MacOsLatest,
	GitHubActionsImage.UbuntuLatest,
	GitHubActionsImage.WindowsLatest,
	AutoGenerate = true,
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
	GitHubActionsImage.MacOsLatest,
	GitHubActionsImage.UbuntuLatest,
	GitHubActionsImage.WindowsLatest,
	AutoGenerate = true,
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