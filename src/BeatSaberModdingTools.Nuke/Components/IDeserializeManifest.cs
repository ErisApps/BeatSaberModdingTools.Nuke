using System.Text.Json;
using BeatSaberModdingTools.Nuke.Helpers.JSON;
using BeatSaberModdingTools.Nuke.Models.BSIPA;
using Nuke.Common;
using Nuke.Common.IO;
using Serilog;

namespace BeatSaberModdingTools.Nuke.Components;

public interface IDeserializeManifest : IProvideSourceDirectory
{
	static PluginManifest? Manifest { get; set; }

	[Parameter("Path to manifest.json")]
	AbsolutePath ManifestPath => TryGetValue(() => ManifestPath) ?? SourceDirectory / "manifest.json";

	Target DeserializeManifest => _ => _
		.Requires(() => !string.IsNullOrWhiteSpace(ManifestPath))
		.Executes((Func<Task>)(async () =>
		{
			Assert.FileExists(ManifestPath);

			Log.Information("Deserializing manifest");
			await using var fileStream = File.OpenRead(ManifestPath);
			Manifest = await JsonSerializer.DeserializeAsync(fileStream, BsipaSerializerContext.Default.PluginManifest).ConfigureAwait(false);
			Manifest.NotNull();

			Log.Information("Deserialized manifest | Name {Name} | Version {Version} | GameVersion {GameVersion}", Manifest!.Name, Manifest.Version, Manifest.GameVersion);
		}));
}