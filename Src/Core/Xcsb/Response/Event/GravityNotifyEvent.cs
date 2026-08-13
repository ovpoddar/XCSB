using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct GravityNotifyEvent : IXEvent<GravityNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Event;
    public uint Window;
    public short X;
    public short Y;
    
    public ref readonly GravityNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<GravityNotifyEvent>();
        if (result.ResponseHeader.Reply != ResponseType.GravityNotify && result.ResponseHeader.GetValue() == 0 ||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}