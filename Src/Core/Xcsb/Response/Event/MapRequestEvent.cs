using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MapRequestEvent : IXEvent<MapRequestEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Parent;
    public uint Window;

    public ref readonly MapRequestEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<MapRequestEvent>();
        if (result.ResponseHeader.Reply == ResponseType.MapRequest && result.ResponseHeader.GetValue() == 0 ||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}