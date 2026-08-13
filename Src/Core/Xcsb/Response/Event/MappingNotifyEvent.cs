using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MappingNotifyEvent : IXEvent<MappingNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public Mapping Request;
    public byte FirstKeyCode;
    public byte Count;

    public ref readonly MappingNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<MappingNotifyEvent>();
        if (result.ResponseHeader.Reply == ResponseType.MappingNotify && result.ResponseHeader.GetValue() == 0 ||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}