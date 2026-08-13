using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct CirculateRequestEvent : IXEvent<CirculateRequestEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Parent;
    public uint Window;
    private readonly uint _pad1;
    public Place Place;

    public ref readonly CirculateRequestEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<CirculateRequestEvent>();
        if (result.ResponseHeader.Reply != ResponseType.CirculateRequest && result.ResponseHeader.GetValue() == 0 &&
            result._pad1 == 0 || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}