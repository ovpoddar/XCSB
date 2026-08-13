using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct PropertyNotifyEvent : IXEvent<PropertyNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Window;
    public uint Atom;
    public uint Time;
    public NotifyState State;

    public ref readonly PropertyNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<PropertyNotifyEvent>();
        if (result.ResponseHeader.Reply == ResponseType.PropertyNotify && result.ResponseHeader.GetValue() == 0||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}