using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetMotionEventsResponse : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint NumberOfEvents;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.Reply &&
               this.Length / 2 == NumberOfEvents;
    }
}