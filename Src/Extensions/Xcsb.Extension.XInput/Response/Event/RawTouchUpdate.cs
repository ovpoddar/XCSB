using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct RawTouchUpdate : IXEvent<RawTouchUpdate>
{
    private readonly byte[] _bytes;
    public readonly byte ResponseType => _bytes[0];
    public readonly byte Extension => _bytes[1];
    public readonly ushort Sequence => Unsafe.As<byte, ushort>(ref _bytes[2]);
    public readonly uint Length => Unsafe.As<byte, uint>(ref _bytes[4]);
    public readonly ushort EventType => Unsafe.As<byte, ushort>(ref _bytes[8]);
    public readonly InputDevice DeviceId => Unsafe.As<byte, InputDevice>(ref _bytes[10]);
    public readonly uint Time => Unsafe.As<byte, uint>(ref _bytes[12]);
    public readonly uint Detail => Unsafe.As<byte, uint>(ref _bytes[16]);
    public readonly InputDevice SourceId => Unsafe.As<byte, InputDevice>(ref _bytes[20]);
    private readonly ushort ValuatorsLength => Unsafe.As<byte, ushort>(ref _bytes[22]);
    public readonly TouchEventFlags Flags => Unsafe.As<byte, TouchEventFlags>(ref _bytes[24]);

    public readonly ReadOnlySpan<uint> ValuatorMask => MemoryMarshal.Cast<byte, uint>(new ReadOnlySpan<byte>(_bytes,
        32, (ValuatorsLength * 4)));

    public ReadOnlySpan<Fp3232> Axisvalues => MemoryMarshal.Cast<byte, Fp3232>(new ReadOnlySpan<byte>(_bytes,
        32 + (ValuatorsLength * 4), (PopSum(ValuatorMask) * 8)));

    public ReadOnlySpan<Fp3232> AxisvaluesRaw => MemoryMarshal.Cast<byte, Fp3232>(new ReadOnlySpan<byte>(_bytes,
        (32 + (ValuatorsLength * 4) + (PopSum(ValuatorMask) * 8)), (PopSum(ValuatorMask) * 8)));


    private static int PopSum(ReadOnlySpan<uint> s)
    {
        var n = 0;
        foreach (var v in s) n += v.CountFlags();
        return n;
    }

    private RawTouchUpdate(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (data.Length < 32)
            throw new ArgumentException("Buffer too small for RawTouchUpdate.", nameof(data));
        _bytes = data;
    }

    public RawTouchUpdate FromBytes(byte[] response)
    {
        return new RawTouchUpdate(response);
    }
}