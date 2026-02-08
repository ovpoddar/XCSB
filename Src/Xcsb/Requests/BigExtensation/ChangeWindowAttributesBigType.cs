using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Masks;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeWindowAttributesBigType(uint window, ValueMask mask, int argsLength)
{
    public readonly Opcode opcode = Opcode.ChangeWindowAttributes;
    private readonly byte _pad0 = 0;
    private readonly ushort _pad1 = 0;
    public readonly uint Length = (uint)(4 + argsLength);
    public readonly uint Window = window;
    public readonly ValueMask Mask = mask;
}