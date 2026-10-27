using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchUpdate : IXEvent<TouchUpdate>
{
    private readonly byte[] _bytes;
    public readonly byte ResponseType => _bytes[0];
    public readonly byte Extension => _bytes[1];
    public readonly ushort Sequence => Unsafe.As<byte, ushort>(ref _bytes[2]);
    public readonly uint Length => Unsafe.As<byte, uint>(ref _bytes[4]);
    public readonly ushort EventType => Unsafe.As<byte, ushort>(ref _bytes[8]);
    public readonly int ByteSize => checked(32 + (int)Length * 4);
    public readonly InputDevice DeviceId => Unsafe.As<byte, InputDevice>(ref _bytes[10]);
    public readonly uint Time => Unsafe.As<byte, uint>(ref _bytes[12]);
    public readonly uint Detail => Unsafe.As<byte, uint>(ref _bytes[16]);
    public readonly uint Root => Unsafe.As<byte, uint>(ref _bytes[20]);
    public readonly uint Event => Unsafe.As<byte, uint>(ref _bytes[24]);
    public readonly uint Child => Unsafe.As<byte, uint>(ref _bytes[28]);
    public readonly int RootX => Unsafe.As<byte, int>(ref _bytes[32]);
    public readonly int RootY => Unsafe.As<byte, int>(ref _bytes[36]);
    public readonly int EventX => Unsafe.As<byte, int>(ref _bytes[40]);
    public readonly int EventY => Unsafe.As<byte, int>(ref _bytes[44]);
    private readonly ushort ButtonsLength => Unsafe.As<byte, ushort>(ref _bytes[48]);
    private readonly ushort ValuatorsLength => Unsafe.As<byte, ushort>(ref _bytes[50]);
    public readonly InputDevice SourceId => Unsafe.As<byte, InputDevice>(ref _bytes[52]);
    public readonly TouchEventFlags Flags => Unsafe.As<byte, TouchEventFlags>(ref _bytes[56]);
    public readonly ModifierInfo Mods => Unsafe.As<byte, ModifierInfo>(ref _bytes[60]);
    public readonly GroupInfo Group => Unsafe.As<byte, GroupInfo>(ref _bytes[76]);

    public readonly ReadOnlySpan<uint> ButtonMask =>
        MemoryMarshal.Cast<byte, uint>(new ReadOnlySpan<byte>(_bytes, 80, (ButtonsLength * 4)));
    
    public readonly ReadOnlySpan<uint> ValuatorMask => MemoryMarshal.Cast<byte, uint>(
        new ReadOnlySpan<byte>(_bytes, (80 + (ButtonsLength * 4)), (ValuatorsLength * 4)));

    public readonly ReadOnlySpan<Fp3232> AxisValues => MemoryMarshal.Cast<byte, Fp3232>(new ReadOnlySpan<byte>(_bytes, 
        80 + ButtonsLength * 4 + ValuatorsLength * 4, (PopSum(ValuatorMask) * 8)));

    private static int PopSum(ReadOnlySpan<uint> s)
    {
        var n = 0;
        foreach (var v in s) n += v.CountFlags();
        return n;
    }

    private TouchUpdate(byte[] bytes)
    {
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));
        if (bytes.Length < 80)
            throw new ArgumentException("Buffer too small for TouchUpdate.", nameof(bytes));
        _bytes = bytes;
    }

    public TouchUpdate FromBytes(byte[] response)
    {
        return new TouchUpdate(response);
    }
}