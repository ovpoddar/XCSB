using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ClientMessageEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Window;
    public ATOM Type;
    public ClientMessageData Data;


    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.ClientMessage;
    }
}