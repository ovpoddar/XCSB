using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct Motion : IXEvent<Motion>
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
    public readonly uint Root => Unsafe.As<byte, uint>(ref _data[20]);
    public readonly uint Event => Unsafe.As<byte, uint>(ref _data[24]);
    public readonly uint Child => Unsafe.As<byte, uint>(ref _data[28]);
    public readonly int RootX => Unsafe.As<byte, int>(ref _data[32]);
    public readonly int RootY => Unsafe.As<byte, int>(ref _data[36]);
    public readonly int EventX => Unsafe.As<byte, int>(ref _data[40]);
    public readonly int EventY => Unsafe.As<byte, int>(ref _data[44]);
    private readonly ushort ButtonsLength => Unsafe.As<byte, ushort>(ref _data[48]);
    private readonly ushort ValuatorsLength => Unsafe.As<byte, ushort>(ref _data[50]);
    public readonly InputDevice SourceId => Unsafe.As<byte, InputDevice>(ref _data[52]);
    public readonly PointerEventFlags Flags => Unsafe.As<byte, PointerEventFlags>(ref _data[56]);
    public readonly ModifierInfo Mods => Unsafe.As<byte, ModifierInfo>(ref _data[60]);
    public readonly GroupInfo Group => Unsafe.As<byte, GroupInfo>(ref _data[76]);

    public readonly ReadOnlySpan<uint> ButtonMask =>
        MemoryMarshal.Cast<byte, uint>(new ReadOnlySpan<byte>(_data, 80, (((int)ButtonsLength) * 4)));

    public readonly ReadOnlySpan<uint> ValuatorMask =>
        MemoryMarshal.Cast<byte, uint>(new ReadOnlySpan<byte>(_data, (80 + (ButtonsLength * 4)),
            (ValuatorsLength * 4)));

    public readonly ReadOnlySpan<Fp3232> AxisValues =>
        MemoryMarshal.Cast<byte, Fp3232>(new ReadOnlySpan<byte>(_data,
            ((80 + (ButtonsLength * 4)) + (ValuatorsLength * 4)), (PopSum(ValuatorMask) * 8)));

    public Motion FromBytes(byte[] response)
    {
        return new Motion(response);
    }

    private Motion(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (data.Length < 80)
            throw new ArgumentException("Buffer too small for Motion.", nameof(data));
        _data = data;
    }

    private static int PopSum(ReadOnlySpan<uint> s)
    {
        var n = 0;
        foreach (var v in s) n += v.CountFlags();
        return n;
    }
}