namespace BeatSaberModdingTools.Nuke.Models.BeatMods
{
	[Flags]
	public enum ApprovalStatus
	{
		Approved = 1 << 0,
		Pending = 1 << 1,
		Declined = 1 << 2,
		Inactive = 1 << 3
	}
}