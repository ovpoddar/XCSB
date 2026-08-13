using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ResizeRequestEvent : IXEvent<ResizeRequestEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Window;
    public ushort Width;
    public ushort Height;

    public ref readonly ResizeRequestEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ResizeRequestEvent>();
        if (result.ResponseHeader.Reply == ResponseType.ResizeRequest && result.ResponseHeader.GetValue() == 0||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}