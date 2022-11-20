using System.Text.Json.Serialization;
using Version = Hive.Versioning.Version;

namespace BeatSaberModdingTools.Nuke.Models.BeatMods
{
	public class ModBase
	{
		[JsonPropertyName("_id")]
		public string Id { get; }

		[JsonPropertyName("name")]
		public string Name { get; }

		[JsonPropertyName("description")]
		public string Description { get; }

		[JsonPropertyName("version")]
		public string VersionRaw { get; }

		[JsonIgnore]
		private Version? _version;

		[JsonIgnore]
		public Version? Version => _version ??= Version.TryParse(VersionRaw.Replace("v", string.Empty), out var parsedVersion) ? parsedVersion : null;

		[JsonPropertyName("gameVersion")]
		public string GameVersionRaw { get; }

		[JsonPropertyName("status")]
		public string StatusRaw { get; }

		[JsonIgnore]
		public ApprovalStatus Status => Enum.Parse<ApprovalStatus>(StatusRaw, true);

		[JsonPropertyName("authorId")]
		public string AuthorId { get; }

		[JsonPropertyName("author")]
		public Author Author { get; }

		[JsonPropertyName("updatedDate")]
		public DateTimeOffset UpdatedDate { get; }

		[JsonPropertyName("uploadedDate")]
		public DateTimeOffset UploadedDate { get; }

		[JsonPropertyName("link")]
		public string Link { get; }

		[JsonPropertyName("category")]
		public string Category { get; }

		[JsonPropertyName("downloads")]
		public List<DownloadLink> Downloads { get; }

		[JsonPropertyName("required")]
		public bool Required { get; }

		[JsonConstructor]
		public ModBase(string id, string name, string description, string versionRaw, string gameVersionRaw, string statusRaw, string authorId, Author author, DateTimeOffset updatedDate, DateTimeOffset uploadedDate, string link, string category,
			List<DownloadLink> downloads, bool required)
		{
			Id = id;
			Name = name;
			Description = description;
			VersionRaw = versionRaw;
			GameVersionRaw = gameVersionRaw;
			StatusRaw = statusRaw;
			AuthorId = authorId;
			Author = author;
			UpdatedDate = updatedDate;
			UploadedDate = uploadedDate;
			Link = link;
			Category = category;
			Downloads = downloads;
			Required = required;
		}

		public DownloadLink? GetUniversalDownloadLink()
		{
			return Downloads.FirstOrDefault(x => x.Type == DownloadType.Universal);
		}
	}
}