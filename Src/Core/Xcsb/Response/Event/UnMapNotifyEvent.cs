using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct UnMapNotifyEvent : IXEvent<UnMapNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Event;
    public uint Window;
    private byte _fromConfigure;
    
    public readonly bool FromConfigure => _fromConfigure == 1;
    
    public ref readonly UnMapNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<UnMapNotifyEvent>();
        if (result.ResponseHeader.Reply == ResponseType.UnMapNotify && result.ResponseHeader.GetValue() == 0||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}