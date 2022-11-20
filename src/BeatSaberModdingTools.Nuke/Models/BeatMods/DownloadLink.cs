using System.Text.Json.Serialization;

namespace BeatSaberModdingTools.Nuke.Models.BeatMods
{
	public class DownloadLink
	{
		[JsonPropertyName("type")]
		public string TypeRaw { get; }

		[JsonIgnore]
		public DownloadType Type => Enum.Parse<DownloadType>(TypeRaw, true);

		[JsonPropertyName("url")]
		public string Url { get; }

		[JsonPropertyName("hashMd5")]
		public FileHash[] HashMd5 { get; }

		[JsonConstructor]
		public DownloadLink(string typeRaw, string url, FileHash[] hashMd5)
		{
			TypeRaw = typeRaw;
			Url = url;
			HashMd5 = hashMd5;
		}
	}
}