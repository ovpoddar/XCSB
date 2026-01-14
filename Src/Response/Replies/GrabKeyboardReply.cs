using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GrabKeyboardReply : IXReply
{
    public readonly ResponseHeader<GrabStatus> ResponseHeader;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length == 0;
    }

    public GrabStatus Status => ResponseHeader.GetValue();
}