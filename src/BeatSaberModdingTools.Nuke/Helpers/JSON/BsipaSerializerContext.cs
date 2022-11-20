using System.Text.Json.Serialization;
using BeatSaberModdingTools.Nuke.Models.BSIPA;

namespace BeatSaberModdingTools.Nuke.Helpers.JSON
{
	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
	[JsonSerializable(typeof(PluginManifest))]
	internal partial class BsipaSerializerContext : JsonSerializerContext
	{
	}
}
