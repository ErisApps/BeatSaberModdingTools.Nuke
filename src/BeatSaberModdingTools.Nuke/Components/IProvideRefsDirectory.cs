using Nuke.Common;
using Nuke.Common.IO;

namespace BeatSaberModdingTools.Nuke.Components;

public interface IProvideRefsDirectory : IProvideSourceDirectory
{
	[Parameter("Path to the Refs directory, defaults to a folder named 'Refs' in the source directory")]
	AbsolutePath RefsDirectory => TryGetValue(() => RefsDirectory) ?? SourceDirectory / "Refs";
}