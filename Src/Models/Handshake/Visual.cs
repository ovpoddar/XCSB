using System.Runtime.InteropServices;

namespace Xcsb.Models.Handshake;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
public struct Visual
{
    public uint VisualId; /* visual id of this visual */
    public VisualClass Class; /* class of screen (monochrome, etc.) */
    public byte BitsPerRgb; /* log base 2 of distinct color values */
    public ushort MapEntries; /* color map entries */
    public uint RedMask;
    public uint GreenMask;
    public uint BlueMask; /* mask values */
    public int Pad0;
}