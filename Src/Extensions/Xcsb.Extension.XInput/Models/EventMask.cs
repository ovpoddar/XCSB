using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;

namespace Xcsb.Extension.XInput.Models;

public struct EventMask
{
    public DeviceInfo DeviceId;
    public XiEventMask[] Mask;

    public static EventMask Parse(Span<byte> buffer, ref int index)
    {
        ref readonly var eventMask = ref buffer.AsStruct<_eventMask>();
        index += Unsafe.SizeOf<_eventMask>();
        var length = eventMask.Length * Unsafe.SizeOf<XiEventMask>();
        var result = new EventMask()
        {
            DeviceId = eventMask.DeviceId,
            Mask = MemoryMarshal.Cast<byte, XiEventMask>(buffer.Slice(index, length)).ToArray()
        };
        index += length;
        return result;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
file readonly struct _eventMask
{
    public readonly DeviceInfo DeviceId;
    public readonly ushort Length;
}