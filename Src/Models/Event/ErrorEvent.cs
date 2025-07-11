using System.Runtime.InteropServices;

namespace Xcsb.Models.Event;
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 31)]
public unsafe struct ErrorEvent
{
    public ErrorCode ErrorCode;
    public fixed byte Padding[6];
    public ushort MinorOpCode;
    public ushort MajorOpCode;
}
