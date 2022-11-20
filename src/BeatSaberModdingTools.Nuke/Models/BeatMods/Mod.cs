using System.Text.Json.Serialization;

namespace BeatSaberModdingTools.Nuke.Models.BeatMods
{
	public class Mod : ModBase
	{
		/// <remark>
		/// Abstraction to superclass is a work-around to dependencies array that can either contain simple mod ids or full mod objects
		/// </remark>
		[JsonPropertyName("dependencies")]
		public List<ModBase> Dependencies { get; }

		[JsonConstructor]
		public Mod(string id, string name, string description, string versionRaw, string gameVersionRaw, string statusRaw, string authorId, Author author, DateTimeOffset updatedDate, DateTimeOffset uploadedDate, string link, string category,
			List<DownloadLink> downloads, bool required, List<ModBase> dependencies)
			: base(id, name, description, versionRaw, gameVersionRaw, statusRaw, authorId, author, updatedDate, uploadedDate, link, category, downloads, required)
		{
			Dependencies = dependencies;
		}
	}
}