using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct RawKeyRelease : IXEvent<RawKeyRelease>
{
    private readonly byte[] _data;
    public readonly byte ResponseType => _data[0];
    public readonly byte Extension => _data[1];
    public readonly ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);
    public readonly uint Length => Unsafe.As<byte, uint>(ref _data[4]);
    public readonly ushort EventType => Unsafe.As<byte, ushort>(ref _data[8]);
    public readonly InputDevice DeviceId => Unsafe.As<byte, InputDevice>(ref _data[10]);
    public readonly uint Time => Unsafe.As<byte, uint>(ref _data[12]);
    public readonly uint Detail => Unsafe.As<byte, uint>(ref _data[16]);
    public readonly InputDevice SourceId => Unsafe.As<byte, InputDevice>(ref _data[20]);
    private readonly ushort ValuatorsLength => Unsafe.As<byte, ushort>(ref _data[22]);
    public readonly KeyEventFlags Flags => Unsafe.As<byte, KeyEventFlags>(ref _data[24]);

    public readonly ReadOnlySpan<uint> ValuatorMask =>
        MemoryMarshal.Cast<byte, uint>(new ReadOnlySpan<byte>(_data, 32, (ValuatorsLength * 4)));

    public readonly ReadOnlySpan<Fp3232> AxisValues =>
        MemoryMarshal.Cast<byte, Fp3232>(new ReadOnlySpan<byte>(_data, (32 + (ValuatorsLength * 4)),
            (PopSum(ValuatorMask) * 8)));

    public readonly ReadOnlySpan<Fp3232> AxisValuesRaw => MemoryMarshal.Cast<byte, Fp3232>(new ReadOnlySpan<byte>(_data,
        ((32 + (ValuatorsLength * 4)) + (PopSum(ValuatorMask) * 8)), (PopSum(ValuatorMask) * 8)));


    private static int PopSum(ReadOnlySpan<uint> s)
    {
        var n = 0;
        foreach (var v in s) n += v.CountFlags();
        return n;
    }

    private RawKeyRelease(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (data.Length < 32)
            throw new ArgumentException("Buffer too small for RawKeyRelease.", nameof(data));
        _data = data;
    }

    public RawKeyRelease FromBytes(byte[] response)
    {
        return new RawKeyRelease(response);
    }
}