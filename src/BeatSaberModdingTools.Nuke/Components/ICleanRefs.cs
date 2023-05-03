using Nuke.Common;
using static Nuke.Common.IO.FileSystemTasks;

namespace BeatSaberModdingTools.Nuke.Components;

public interface ICleanRefs : IProvideRefsDirectory
{
	Target CleanRefs => _ => _
		.Executes(() => EnsureCleanDirectory(RefsDirectory));
}