using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct SelectionClearEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Time;
    public uint Owner;
    public ATOM Selection;

    public bool Verify()
    {
        return (ResponseType)ResponseHeader.Reply == ResponseType.SelectionClear && ResponseHeader.GetValue() == 0;
    }
}