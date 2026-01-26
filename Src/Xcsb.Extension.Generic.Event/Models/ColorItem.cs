using System.Runtime.InteropServices;

namespace Xcsb.Extension.Generic.Event.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
public struct ColorItem
{
    public uint Pixel;
    public ushort Red;
    public ushort Green;
    public ushort Blue;
    public ColorFlag Flags;
}