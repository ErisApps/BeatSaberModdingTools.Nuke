using System.IO.Compression;
using System.Net.Http.Json;
using BeatSaberModdingTools.Nuke.Helpers;
using BeatSaberModdingTools.Nuke.Helpers.JSON;
using BeatSaberModdingTools.Nuke.Models.BeatMods;
using Hive.Versioning;
using Nuke.Common;
using Serilog;

namespace BeatSaberModdingTools.Nuke.Components;

public interface IDownloadBeatModsDependencies : IProvideRefsDirectory
{
	private const string BEAT_MODS_VERSIONS = "https://versions.beatmods.com/versions.json";
	private const string BEAT_MODS_ALIAS = "https://alias.beatmods.com/aliases.json";

	private const string BEAT_MODS_BASE_URL = "https://beatmods.com";
	private const string BEAT_MODS_API_URL = $"{BEAT_MODS_BASE_URL}/api/v1/";

	private static Dictionary<string, VersionRange> Dependencies => IDeserializeManifest.Manifest!.Dependencies;

	Target DownloadDependencies => _ => _
		.TryAfter<IClean>()
		.DependsOn<IDeserializeManifest>(x => x.DeserializeManifest)
		.OnlyWhenDynamic(() => Dependencies.Count > 0).Executes((Func<Task>)(async () =>
		{
			Log.Information("Retrieving BeatMods version information");
			var httpClient = new HttpClient();
			var versionsTask = httpClient.GetFromJsonAsync(BEAT_MODS_VERSIONS, BeatModsSerializerContext.Default.ListString);
			var aliasesTask = httpClient.GetFromJsonAsync(BEAT_MODS_ALIAS, BeatModsSerializerContext.Default.DictionaryStringListString);

			await Task.WhenAll(versionsTask, aliasesTask);

			var manifestGameVersion = IDeserializeManifest.Manifest!.GameVersion;
			var aliasBaseVersion = versionsTask.Result?.Find(x => x == manifestGameVersion) ?? aliasesTask.Result?.GetValueOrDefault(manifestGameVersion)?.Find(x => x == manifestGameVersion);
			if (aliasBaseVersion == null)
			{
				// Log.Fatal("Could not find alias base version for {manifestVersion}", manifestVersion);
				Assert.Fail($"Could not find alias base version for {manifestGameVersion}");
			}

			Log.Information("Fetching mod metadata for alias base version {AliasBaseVersion}", aliasBaseVersion);

			var urlBuilder = new UrlBuilder(BEAT_MODS_API_URL + "mod")
				.AppendQueryParam("gameVersion", aliasBaseVersion!);

			var modMetadata = await httpClient.GetFromJsonAsync(urlBuilder, BeatModsSerializerContext.Default.ListMod).ConfigureAwait(false);
			modMetadata.NotNull();

			async Task DownloadAndExtract(ModBase modBase)
			{
				Log.Information("Downloading {ModName} with version {ModVersion}", modBase.Name, modBase.Version);
				var universalDownloadLink = modBase.GetUniversalDownloadLink();
				universalDownloadLink.NotNull();

				var beatModsDependencyResponse = await httpClient.GetAsync(BEAT_MODS_BASE_URL + universalDownloadLink!.Url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
				beatModsDependencyResponse.EnsureSuccessStatusCode();

				Log.Debug("Extracting {ModName} with version {ModVersion}", modBase.Name, modBase.Version);
				await using var beatModsDependencyStream = await beatModsDependencyResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
				using var zipArchive = new ZipArchive(beatModsDependencyStream);
				zipArchive.ExtractToDirectory(RefsDirectory);
			}

			foreach (var (modName, versionRange) in Dependencies)
			{
				var dependency = modMetadata!.Find(x => x.Name == modName && x.Version is not null && versionRange.Matches(x.Version));
				if (dependency != null)
				{
					await DownloadAndExtract(dependency).ConfigureAwait(false);
				}
				else
				{
					Log.Warning("Mod {DependencyName} version {DependencyVersionRange} not found", modName, versionRange);
				}
			}
		}));
}