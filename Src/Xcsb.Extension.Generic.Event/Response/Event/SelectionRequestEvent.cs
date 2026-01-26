using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

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