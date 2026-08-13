using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ReParentNotifyEvent : IXEvent<ReParentNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Event;
    public uint Window;
    public uint Parent;
    public short X;
    public short Y;
    public bool OverrideRedirect;

    public ref readonly ReParentNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ReParentNotifyEvent>();
        if (result.ResponseHeader.Reply == ResponseType.ReParentNotify && result.ResponseHeader.GetValue() == 0||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}