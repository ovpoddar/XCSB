using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Masks;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ConfigureWindowType(uint window, ConfigureValueMask mask, int argsLength)
{
    public readonly Opcode opcode = Opcode.ConfigureWindow;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = (ushort)(3 + argsLength);
    public readonly uint Window = window;
    public readonly ConfigureValueMask Mask = mask;
    private readonly ushort _pad1 = 0;
}