using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct InternAtomReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly ATOM Atom;

    public bool Verify(in int sequence)
    {
        return (ResponseType)ResponseHeader.Reply  == ResponseType.Reply &&
               Length == 0;
    }
}