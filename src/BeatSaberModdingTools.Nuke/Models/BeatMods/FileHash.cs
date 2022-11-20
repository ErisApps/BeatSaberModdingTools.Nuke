using System.Text.Json.Serialization;

namespace BeatSaberModdingTools.Nuke.Models.BeatMods
{
	public readonly struct FileHash
	{
		[JsonPropertyName("file")]
		public string File { get; }

		[JsonPropertyName("hash")]
		public string Hash { get; }

		[JsonConstructor]
		public FileHash(string file, string hash)
		{
			File = file;
			Hash = hash;
		}
	}
}