using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MotionNotifyEvent : IXEvent<MotionNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, Motion> ResponseHeader;
    public uint Time;
    public uint Root;
    public uint Window;
    public uint Child;
    public short RootX;
    public short RootY;
    public short EventX;
    public short EventY;
    public ushort State;
    private sbyte _sameScreen;

    public bool IsSameScreen => _sameScreen == 1;

    public ref readonly MotionNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<MotionNotifyEvent>();
        if (result.ResponseHeader.Reply == ResponseType.MotionNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}