using System.Text.Json.Serialization;
using BeatSaberModdingTools.Nuke.Helpers.Converters;
using Hive.Versioning;
using Version = Hive.Versioning.Version;

namespace BeatSaberModdingTools.Nuke.Models.BSIPA
{
	public class PluginManifest
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = null!;

		[JsonPropertyName("name")]
		public string Name { get; set; } = null!;

		[JsonPropertyName("description")]
		[JsonConverter(typeof(MultilineStringConverter))]
		public string? Description { get; set; }

		[JsonPropertyName("version")]
		public string VersionRaw { get; set; } = null!;

		[JsonIgnore]
		public Version? Version => Version.TryParse(VersionRaw, out var parsedVersion) ? parsedVersion : null;

		[JsonPropertyName("gameVersion")]
		public string GameVersion { get; set; } = null!;

		[JsonPropertyName("author")]
		public string Author { get; set; }  = null!;

		[JsonPropertyName("dependsOn")]
		public Dictionary<string, string> DependenciesRaw { get; set; } = new();

		[JsonIgnore]
		public Dictionary<string, VersionRange> Dependencies => DependenciesRaw.ToDictionary(x => x.Key, x => new VersionRange(x.Value));

		[JsonPropertyName("conflictsWith")]
		public Dictionary<string, string> ConflictsRaw { get; set; } = new();

		[JsonIgnore]
		public Dictionary<string, VersionRange> Conflicts => ConflictsRaw.ToDictionary(x => x.Key, x => new VersionRange(x.Value));

		[JsonPropertyName("loadBefore")]
		public string[] LoadBefore { get; set; } = Array.Empty<string>();

		[JsonPropertyName("loadAfter")]
		public string[] LoadAfter { get; set; } = Array.Empty<string>();

		[JsonPropertyName("icon")]
		public string? IconPath { get; set; }

		[JsonPropertyName("files")]
		public string[] Files { get; set; } = Array.Empty<string>();

		[Serializable]
		public class LinksObject
		{
			[JsonPropertyName("project-home")]
			[JsonConverter(typeof(NullableUriConverter))]
			public Uri? ProjectHome { get; set; } = null;

			[JsonPropertyName("project-source")]
			[JsonConverter(typeof(NullableUriConverter))]
			public Uri? ProjectSource { get; set; } = null;

			[JsonPropertyName("donate")]
			[JsonConverter(typeof(NullableUriConverter))]
			public Uri? Donate { get; set; } = null;
		}

		[JsonPropertyName("links")]
		public LinksObject? Links { get; set; }

		[Serializable]
		public class MiscObject
		{
			[JsonPropertyName("plugin-hint")]
			public string? PluginMainHint { get; set; } = null;
		}

		[JsonPropertyName("misc")]
		public MiscObject? Misc { get; set; }
	}
}