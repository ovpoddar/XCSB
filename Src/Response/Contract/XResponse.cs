using System.Runtime.InteropServices;

namespace Xcsb.Response.Contract;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
internal unsafe partial struct XResponse<T> : IXBaseResponse where T : struct
{
    [FieldOffset(0)] public ResponseType Type;
    [FieldOffset(1)] public T Value;
    [FieldOffset(2)] public ushort Sequence;
    [FieldOffset(4)] public fixed byte Padding[28];
    
    [FieldOffset(0)] private fixed byte _data[32];
}
