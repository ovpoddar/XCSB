using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangePointerControlType(
    ushort accelerationNumerator,
    ushort accelerationDenominator,
    ushort threshold,
    byte doAcceleration,
    byte doThreshold
)
{
    public readonly Opcode opcode = Opcode.ChangePointerControl;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 3;
    public readonly ushort AccelerationNumerator = accelerationNumerator;
    public readonly ushort AccelerationDenominator = accelerationDenominator;
    public readonly ushort Threshold = threshold;
    public readonly byte DoAcceleration = doAcceleration;
    public readonly byte DoThreshold = doThreshold;
}