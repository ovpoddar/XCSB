using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangePropertyType(
    PropertyMode mode,
    uint window,
    uint property,
    uint type,
    int argsLength,
    byte size)
{
    public readonly Opcode OpCode = Opcode.ChangeProperty;
    public readonly PropertyMode Mode = mode;
    public readonly ushort Length = (ushort)(6 + argsLength.AddPadding() / 4);
    public readonly uint Window = window;
    public readonly uint Property = property;
    public readonly uint Type = type;
    public readonly byte Size = size;
    private readonly byte _pad0 = 0;
    private readonly byte _pad1 = 0;
    private readonly byte _pad2 = 0;
    public readonly int ArgsLength = argsLength;
}