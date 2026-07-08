using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;

namespace Xcsb.Extension.XInput.Models;

public struct DeviceTimeCoord
{
    public uint Time;
    public int[] AxisValues;

    public static DeviceTimeCoord Parse(Span<byte> buffer) =>
        new DeviceTimeCoord()
        {
            Time = MemoryMarshal.Read<uint>(buffer[0..4]),
            AxisValues = MemoryMarshal.Cast<byte, int>(buffer.Slice(4)).ToArray(),
        };
}