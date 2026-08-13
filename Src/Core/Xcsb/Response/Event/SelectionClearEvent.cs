using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct SelectionClearEvent : IXEvent<SelectionClearEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Time;
    public uint Owner;
    public ATOM Selection;

    public ref readonly SelectionClearEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<SelectionClearEvent>();
        if (result.ResponseHeader.Reply == ResponseType.SelectionClear && result.ResponseHeader.GetValue() == 0||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}