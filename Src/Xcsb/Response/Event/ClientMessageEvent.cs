using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ClientMessageEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Window;
    public ATOM Type;
    public ClientMessageData Data;


    public readonly bool Verify()
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.ClientMessage;
    }
}