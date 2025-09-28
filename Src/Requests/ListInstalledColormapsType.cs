using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ListInstalledColormapsType(uint window)
{
    public readonly Opcode Opcode = Opcode.ListInstalledColormaps;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}