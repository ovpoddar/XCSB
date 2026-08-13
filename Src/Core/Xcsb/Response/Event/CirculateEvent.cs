using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct CirculateNotifyEvent : IXEvent<CirculateNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Event;
    public uint Window;
    private readonly uint _pad1;
    public Place Place;

    public ref readonly CirculateNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<CirculateNotifyEvent>();
        if (result.ResponseHeader.Reply != ResponseType.CirculateNotify && result.ResponseHeader.GetValue() == 0 &&
            result._pad1 == 0 || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}