using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetWindowAttributesType(uint window)
{
    public readonly Opcode OpCode = Opcode.GetWindowAttributes;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}
