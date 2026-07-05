using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct BarrierReleasePointerInfo(InputDevice deviceId, uint barrier, uint eventId)
{
    public readonly InputDevice DeviceId = deviceId;
    private readonly ushort _pad0 = 0;
    public readonly uint Barrier = barrier;
    public readonly uint EventId = eventId;
}