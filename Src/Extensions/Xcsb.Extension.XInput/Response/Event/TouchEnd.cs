using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TouchEnd : IXEvent<TouchEnd>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort EventType;
    public readonly InputDevice DeviceId;
    public readonly uint Time;
    public readonly uint Detail;
    public readonly uint RootId;
    public readonly uint Event;
    public readonly uint Child;

    public readonly uint RootX;
    public readonly uint RootY;
    public readonly uint EventX;
    public readonly uint EventY;
    private readonly ushort ButtonsLength;
    public readonly ushort ValuatorsLength;
    public readonly InputDevice SourceId;
    private readonly ushort _pad;
    public readonly TouchEventFlags Flags;
    public readonly ModifierInfo Modifier;
    public readonly GroupInfo Group;
    public readonly uint[] Buttons;
    public readonly uint[] Valuators;
    public readonly Fp3232[] AxisValues;
    public ref readonly TouchEnd Cast(Span<byte> response)
    {
        throw new NotImplementedException();
    }
}