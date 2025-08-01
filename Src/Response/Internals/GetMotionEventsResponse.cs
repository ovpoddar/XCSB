using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetMotionEventsResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly uint NumberOfEvents;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Verify(in sequence) && ResponseHeader.Length / 2 == NumberOfEvents;
    }
}