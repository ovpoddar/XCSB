using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct RotatePropertiesType(uint window, int propertiesLength, ushort delta)
{
    public readonly Opcode opcode = Opcode.RotateProperties;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + propertiesLength);
    public readonly uint Window = window;
    public readonly ushort PropertiesLength = (ushort)propertiesLength;
    public readonly ushort Delta = delta;
}