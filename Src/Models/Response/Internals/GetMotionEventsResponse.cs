using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetMotionEventsResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly uint NumberOfEvents;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Verify(in sequence) && (this.ResponseHeader.Length / 2) == this.NumberOfEvents;
    }
}