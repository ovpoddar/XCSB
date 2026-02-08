using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct RotatePropertiesBigType(uint window, int propertiesLength, ushort delta)
{
    public readonly Opcode opcode = Opcode.RotateProperties;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(4 + propertiesLength);
    public readonly uint Window = window;
    public readonly ushort PropertiesLength = (ushort)propertiesLength;
    public readonly ushort Delta = delta;
}