using System.Runtime.InteropServices;

namespace Xcsb.Extension.Generic.Event.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
public struct Pixel
{
    public ushort Red;
    public ushort Green;
    public ushort Blue;
    public ushort Reserved;
}