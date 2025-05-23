using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public unsafe struct ClientMessageData
{
    [FieldOffset(0)]
    public fixed byte Data8[20];
    [FieldOffset(0)]
    public fixed ushort Data16[10];
    [FieldOffset(0)]
    public fixed uint Data32[5];
}