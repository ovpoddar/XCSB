using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Masks;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DeviceStateNotifyEvent : IXEvent<DeviceStateNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Time;
    public readonly byte KeysLength;
    public readonly byte ButtonsLength;
    public readonly byte ValuatorsLength;
    public readonly ClassesReportedMask ClassesReportedMask;
    private fixed byte Buttons[4];
    private fixed byte Keys[4];
    private fixed int Valuators[3];

    public readonly Span<byte> ButtonsSpan
    {
        get
        {
            fixed (byte* ptr = this.Buttons)
                return new Span<byte>(ptr, 4);
        }
    }

    public readonly Span<byte> KeysSpan
    {
        get
        {
            fixed (byte* ptr = this.Keys)
                return new Span<byte>(ptr, 4);
        }
    }

    public readonly Span<int> ValuatorsSpan
    {
        get
        {
            fixed (int* ptr = this.Valuators)
                return new Span<int>(ptr, 3);
        }
    }

    public ref readonly DeviceStateNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DeviceStateNotifyEvent>();
        if ((byte)result.ResponseHeader.Reply == (byte)XiInputEventType.DeviceStateNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}