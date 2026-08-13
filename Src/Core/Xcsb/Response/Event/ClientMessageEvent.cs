using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ClientMessageEvent : IXEvent<ClientMessageEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Window;
    public ATOM Type;
    public ClientMessageData Data;


    public ref readonly ClientMessageEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ClientMessageEvent>();
        if (result.ResponseHeader.Reply != ResponseType.ClientMessage || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}