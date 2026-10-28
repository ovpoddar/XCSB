using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct RawButtonRelease : IXEvent<RawButtonRelease>
{
    private readonly byte[] _bytes;

    private RawButtonRelease(byte[] bytes)
    {
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));
        if (bytes.Length < 32)
            throw new ArgumentException("Buffer too small for RawButtonRelease.", nameof(bytes));
        _bytes = bytes;
    }

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
    public readonly PointerEventFlags Flags => Unsafe.As<byte, PointerEventFlags>(ref _bytes[24]);

    public readonly ReadOnlySpan<uint> ValuatorMask =>
        MemoryMarshal.Cast<byte, uint>(new ReadOnlySpan<byte>(_bytes, 32, (ValuatorsLength * 4)));

    public readonly ReadOnlySpan<Fp3232> AxisValues =>
        MemoryMarshal.Cast<byte, Fp3232>(new ReadOnlySpan<byte>(_bytes, (32 + (ValuatorsLength * 4)),
            (PopSum(ValuatorMask) * 8)));

    public readonly ReadOnlySpan<Fp3232> AxisValuesRaw => MemoryMarshal.Cast<byte, Fp3232>(new ReadOnlySpan<byte>(
        _bytes,
        ((32 + (ValuatorsLength * 4)) + (PopSum(ValuatorMask) * 8)), (PopSum(ValuatorMask) * 8)));

    private static int PopSum(ReadOnlySpan<uint> s)
    {
        var n = 0;
        foreach (var v in s) n += v.CountFlags();
        return n;
    }

    public RawButtonRelease FromBytes(byte[] response)
    {
        return new RawButtonRelease(response);
    }
}