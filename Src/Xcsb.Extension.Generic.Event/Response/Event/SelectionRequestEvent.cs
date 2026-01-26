using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct SelectionRequestEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Time; // 0 -> current time
    public uint Owner;
    public uint Requestor;
    public ATOM Selection;
    public ATOM Target;
    public ATOM Property;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.SelectionRequest && ResponseHeader.GetValue() == 0;
    }
}