using Nuke.Common;
using static Nuke.Common.IO.FileSystemTasks;

namespace BeatSaberModdingTools.Nuke.Components;

public interface IClean : IProvideRefsDirectory
{
	Target Clean => _ => _
		.Executes(() => EnsureCleanDirectory(RefsDirectory));
}