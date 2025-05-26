using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ExposeEvent
{
    private byte Pad0;
    public uint Window;
    public ushort X;
    public ushort Y;
    public ushort Width;
    public ushort Height;
    public ushort Count;
}