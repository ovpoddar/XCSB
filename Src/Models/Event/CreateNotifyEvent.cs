using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CreateNotifyEvent
{
    private byte Pad0;
    public ushort Sequence;
    public uint Parent;
    public uint Window;
    public short X;
    public short Y;
    public ushort Width;
    public ushort Height;
    public ushort BorderWidth;
    public byte OverrideRedirect; // 1 true 0 false
}