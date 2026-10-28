using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct FocusOut : IXEvent<FocusOut>
{
     private readonly byte[] _data;
    public readonly byte ResponseType => _data[0];
    public readonly byte Extension => _data[1];
    public readonly ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);
    public readonly uint Length => Unsafe.As<byte, uint>(ref _data[4]);
    public readonly ushort EventType => Unsafe.As<byte, ushort>(ref _data[8]);
    public readonly int ByteSize => checked(32 + (int)Length * 4);
    public readonly InputDevice DeviceId => Unsafe.As<byte, InputDevice>(ref _data[10]);
    public readonly uint Time => Unsafe.As<byte, uint>(ref _data[12]);
    public readonly InputDevice SourceId => Unsafe.As<byte, InputDevice>(ref _data[16]);
    public readonly NotifyMode Mode => (NotifyMode)_data[18];
    public readonly NotifyDetail Detail => (NotifyDetail)_data[19];
    public readonly uint Root => Unsafe.As<byte, uint>(ref _data[20]);
    public readonly uint Event => Unsafe.As<byte, uint>(ref _data[24]);
    public readonly uint Child => Unsafe.As<byte, uint>(ref _data[28]);
    public readonly int RootX => Unsafe.As<byte, int>(ref _data[32]);
    public readonly int RootY => Unsafe.As<byte, int>(ref _data[36]);
    public readonly int EventX => Unsafe.As<byte, int>(ref _data[40]);
    public readonly int EventY => Unsafe.As<byte, int>(ref _data[44]);
    public readonly bool SameScreen => _data[48] != 0;
    public readonly bool Focus => _data[49] != 0;
    private readonly ushort ButtonsLength => Unsafe.As<byte, ushort>(ref _data[50]);
    public readonly ModifierInfo Mods => Unsafe.As<byte, ModifierInfo>(ref _data[52]);
    public readonly GroupInfo Group => Unsafe.As<byte, GroupInfo>(ref _data[68]);

    public readonly ReadOnlySpan<uint> Buttons =>
        MemoryMarshal.Cast<byte, uint>(new ReadOnlySpan<byte>(_data, 72, (ButtonsLength * 4)));

    private FocusOut(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (data.Length < 72)
            throw new ArgumentException("Buffer too small for FocusOut.", nameof(data));
        _data = data;
    }

    public FocusOut FromBytes(byte[] response)
    {
        return new FocusOut(response);
    }
}