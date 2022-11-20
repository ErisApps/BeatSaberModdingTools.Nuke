using Nuke.Common;
using Nuke.Common.IO;

namespace BeatSaberModdingTools.Nuke.Components;

public interface IProvideSourceDirectory : INukeBuild
{
	[Parameter("Path to the source directory")]
	AbsolutePath SourceDirectory => TryGetValue(() => SourceDirectory) ?? RootDirectory;
}