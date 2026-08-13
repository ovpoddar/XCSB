using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct VisibilityNotifyEvent : IXEvent<VisibilityNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Window;
    public Visibility State;

    public ref readonly VisibilityNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<VisibilityNotifyEvent>();
        if (result.ResponseHeader.Reply == ResponseType.VisibilityNotify && result.ResponseHeader.GetValue() == 0||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}