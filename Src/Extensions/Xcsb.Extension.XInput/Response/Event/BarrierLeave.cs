using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BarrierLeave : IXEvent<BarrierLeave>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort EventType;
    public readonly InputDevice DeviceId;
    public readonly uint Time;
    public readonly uint EventId;
    public readonly uint Root;
    public readonly uint Event;
    public readonly uint Barrier;
    public readonly uint DeltaTime;
    public readonly BarrierFlags Flags;
    public readonly InputDevice SourceId;
    private readonly ushort _pad0;
    public readonly uint RootX;
    public readonly uint RootY;
    public readonly Fp3232 DX;
    public readonly Fp3232 DY;
    public ref readonly BarrierLeave Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}