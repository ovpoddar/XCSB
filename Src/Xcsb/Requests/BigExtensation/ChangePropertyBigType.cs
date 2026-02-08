using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;


[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangePropertyBigType(
    PropertyMode mode,
    uint window,
    ATOM property,
    ATOM type,
    int argsLength,
    int size)
{
    public readonly Opcode OpCode = Opcode.ChangeProperty;
    public readonly PropertyMode Mode = mode;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(7 + (argsLength * size).AddPadding() / 4);
    public readonly uint Window = window;
    public readonly ATOM Property = property;
    public readonly ATOM Type = type;
    public readonly byte Size = (byte)(size * 8);
    private readonly byte _pad0 = 0;
    private readonly byte _pad1 = 0;
    private readonly byte _pad2 = 0;
    public readonly int ArgsLength = argsLength;
}