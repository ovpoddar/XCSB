using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;

namespace Xcsb.Extension.XInput.Models;

public struct ValuatorInfo : IInputInfo
{
    public readonly ClassId ClassId { get; }
    public readonly byte Length { get; }
    public readonly byte AxisCount;
    public readonly byte DeviceMode;
    public readonly uint MotionBufferSize;
    public readonly AxisInfo[] AxisInfos;

    public ValuatorInfo(Span<byte> span)
    {
        ref readonly var axisInfo = ref span.AsStruct<_AxisInfo>();
        ClassId = axisInfo.ClassId;
        Length = axisInfo.Length;
        AxisCount = axisInfo.AxisCount;
        DeviceMode = axisInfo.DeviceMode;
        MotionBufferSize = axisInfo.MotionBufferSize;
        var axislength = AxisCount * Unsafe.SizeOf<AxisInfo>();
        AxisInfos = MemoryMarshal.Cast<byte, AxisInfo>(span.Slice(8, axislength)).ToArray();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
file readonly struct _AxisInfo
{
    public readonly ClassId ClassId ;
    public readonly byte Length ;
    public readonly byte AxisCount;
    public readonly byte DeviceMode;
    public readonly uint MotionBufferSize;
}
