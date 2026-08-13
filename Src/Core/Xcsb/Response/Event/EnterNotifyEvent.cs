using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct EnterNotifyEvent : IXEvent<EnterNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, NotifyDetail> ResponseHeader;
    public uint Time;
    public uint Root;
    public uint Event;
    public uint Child;
    public short RootX;
    public short RootY;
    public short EventX;
    public short EventY;
    public ushort State;
    public NotifyMode Mode;
    private byte _sameScreenFocus;

    public bool IsSameScreenFocus => _sameScreenFocus == 1;

    public ref readonly EnterNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<EnterNotifyEvent>();
        if (result.ResponseHeader.Reply != ResponseType.EnterNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}