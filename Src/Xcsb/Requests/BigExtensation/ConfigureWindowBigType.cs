using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ConfigureWindowBigType(uint window, ConfigureValueMask mask, int argsLength)
{
    public readonly Opcode opcode = Opcode.ConfigureWindow;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(4 + argsLength);
    public readonly uint Window = window;
    public readonly ConfigureValueMask Mask = mask;
    private readonly ushort _pad1 = 0;
}