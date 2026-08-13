using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct SelectionRequestEvent : IXEvent<SelectionRequestEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Time; // 0 -> current time
    public uint Owner;
    public uint Requestor;
    public ATOM Selection;
    public ATOM Target;
    public ATOM Property;

    public ref readonly SelectionRequestEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<SelectionRequestEvent>();
        if (result.ResponseHeader.Reply == ResponseType.SelectionRequest && result.ResponseHeader.GetValue() == 0||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}