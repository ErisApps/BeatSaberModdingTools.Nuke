using System.Text.Json.Serialization;

namespace BeatSaberModdingTools.Nuke.Models.BeatMods
{
	public class Author
	{
		[JsonPropertyName("_id")]
		public string Id { get; set; }  = null!;

		[JsonPropertyName("username")]
		public string Username { get; set; } = null!;

		[JsonPropertyName("lastLogin")]
		public DateTimeOffset LastLogin { get; set; }
	}
}