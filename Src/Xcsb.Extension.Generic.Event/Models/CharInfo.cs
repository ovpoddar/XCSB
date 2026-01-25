using System.Runtime.InteropServices;

namespace Xcsb.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
public struct CharInfo(
    ushort leftSideBearing,
    ushort rightSideBearing,
    ushort characterWidth,
    ushort ascent,
    ushort descent,
    ushort attributes)
{
    public ushort LeftSideBearing = leftSideBearing;
    public ushort RightSideBearing = rightSideBearing;
    public ushort CharacterWidth = characterWidth;
    public ushort Ascent = ascent;
    public ushort Descent = descent;
    public ushort Attributes = attributes;
}