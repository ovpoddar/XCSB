﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DeletePropertyType(uint window, uint atom)
{
    public readonly Opcode opcode = Opcode.DeleteProperty;
    private readonly byte _pad0;
    public readonly ushort Length = 3;
    public readonly uint Window = window;
    public readonly uint Atom = atom;
}