﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UninstallColormapType(uint colormapId)
{
    public readonly Opcode Opcode = Opcode.UninstallColormap;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint ColorMapId = colormapId;
}