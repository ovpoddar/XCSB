using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Masks;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct DeviceStateNotifyEvent : IXEvent
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

    public bool Verify()
    {
        throw new System.NotImplementedException();
    }
}