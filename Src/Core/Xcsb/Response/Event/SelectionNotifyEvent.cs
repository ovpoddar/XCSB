using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct SelectionNotifyEvent : IXEvent<SelectionNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Time;
    public uint Requestor;
    public ATOM Selection;
    public ATOM Target;
    public ATOM Property;

    public ref readonly SelectionNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<SelectionNotifyEvent>();
        if (result.ResponseHeader.Reply == ResponseType.SelectionNotify && result.ResponseHeader.GetValue() == 0||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}