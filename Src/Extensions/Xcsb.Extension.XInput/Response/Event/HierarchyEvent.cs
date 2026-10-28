using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct HierarchyEvent : IXEvent<HierarchyEvent>
{
    private readonly byte[] _data;

    private HierarchyEvent(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (data.Length < 32)
            throw new ArgumentException("Buffer too small for Hierarchy Event.", nameof(data));
        _data = data;
    }

    public readonly byte ResponseType => _data[0];
    public readonly byte Extension => _data[1];
    public readonly ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);
    public readonly uint Length => Unsafe.As<byte, uint>(ref _data[4]);
    public readonly ushort EventType => Unsafe.As<byte, ushort>(ref _data[8]);
    public readonly InputDevice DeviceId => Unsafe.As<byte, ushort>(ref _data[10]);
    public readonly uint Time => Unsafe.As<byte, uint>(ref _data[12]);
    public readonly HierarchyMask Flags => Unsafe.As<byte, HierarchyMask>(ref _data[16]);
    private readonly ushort _informationLength => Unsafe.As<byte, ushort>(ref _data[20]);
    public readonly ReadOnlySpan<HierarchyInfo> Information => MemoryMarshal.Cast<byte, HierarchyInfo>(new ReadOnlySpan<byte>(_data, 32, (_informationLength * 12)));

    public HierarchyEvent FromBytes(byte[] response)
    {
        return new HierarchyEvent(response);
    }
}