using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RawTouchUpdate : IXEvent<RawTouchUpdate>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort EventType;
    public readonly InputDevice DeviceId;
    public readonly uint Time;
    public readonly uint Detail;
    public readonly InputDevice SourceId;
    private readonly ushort ValuatorsLength;
    public readonly TouchEventFlags Flags;
    private readonly int _pad0;
    public readonly uint[] ValuatorsMask;
    public readonly Fp3232[] AxisValues;
    public readonly Fp3232[] AxisValuesRaw;
    
    public ref readonly RawTouchUpdate Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}