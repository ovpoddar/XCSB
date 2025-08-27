using System.Runtime.InteropServices;
using Xcsb.Event;
using Xcsb.Helpers;
using Xcsb.Response.Errors;

namespace Xcsb.Response.Contract;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
internal unsafe partial struct XResponse
{
    [FieldOffset(0)] private ResponseType Reply;
    [FieldOffset(0)] private fixed byte _data[32];
}
