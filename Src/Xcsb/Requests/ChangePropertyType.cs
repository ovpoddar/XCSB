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
    ATOM property,
    ATOM type,
    int argsLength,
    int size)
{
    public readonly Opcode OpCode = Opcode.ChangeProperty;
    public readonly PropertyMode Mode = mode;
    public readonly ushort Length = (ushort)(6 + (argsLength * size).AddPadding() / 4);
    public readonly uint Window = window;
    public readonly ATOM Property = property;
    public readonly ATOM Type = type;
    public readonly byte Size = (byte)(size * 8);
    private readonly byte _pad0 = 0;
    private readonly byte _pad1 = 0;
    private readonly byte _pad2 = 0;
    public readonly int ArgsLength = argsLength;
}