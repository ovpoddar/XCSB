﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyText16Type(uint drawable, uint gc, ushort x, ushort y, int textLength)
{
    public readonly Opcode OpCode = Opcode.PolyText16;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(4 + (textLength.AddPadding() / 4));
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
    public readonly ushort X = x;
    public readonly ushort Y = y;
}
