using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;

[StructLayout(LayoutKind.Sequential)]
public struct ConfigureRequestEvent
{
    public StackMode StackMode;
    public ushort Sequence;
    public uint Parent;
    public uint Window;
    public uint Sibling;
    public short X;
    public short Y;
    public ushort Width;
    public ushort Height;
    public ushort BorderWidth;
    public ushort ValueMask;
}