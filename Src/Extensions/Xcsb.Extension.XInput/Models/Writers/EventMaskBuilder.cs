using System;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models.Writers;

public class EventMaskBuilder
{
    internal uint[] m_data;
    internal int m_length;

    public static EventMaskBuilder Create()
    {
        var result = new EventMaskBuilder()
        {
            m_data = Array.Empty<uint>(),
            m_length = 0
        };
        return result;
    }

    public EventMaskBuilder AddEventMask(InputDevice deviceId, ReadOnlySpan<XiEventMask> mask)
    {
        var position = m_data.Length;
        var newBuffer = new uint[position + 1 + mask.Length];
        m_data.CopyTo(newBuffer, 0);
        var header = ((uint)mask.Length << 16) | (ushort)deviceId.Id;
        newBuffer[position++] = header;
        MemoryMarshal.Cast<XiEventMask, uint>(mask)
            .CopyTo(newBuffer.AsSpan(position));
        m_data = newBuffer;
        m_length++;
        return this;
    }
}