using System.Runtime.InteropServices;
using Xcsb.Event;
using Xcsb.Response.Errors;

namespace Xcsb.Response.Contract;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
internal unsafe partial struct XResponse<T> where T : unmanaged
{
    [FieldOffset(0)] public ResponseHeader<T> ResponseHeader;
    [FieldOffset(4)] public fixed byte Padding[28];

    [FieldOffset(0)] private fixed byte _data[32];

    [FieldOffset(0)] internal XGenericEvent XGenericEvent;
    [FieldOffset(0)] internal XGenericError GenericError;
}
