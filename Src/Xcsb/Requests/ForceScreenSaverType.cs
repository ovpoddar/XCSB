using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ForceScreenSaverType(ForceScreenSaverMode mode)
{
    public readonly Opcode Opcode = Opcode.ForceScreenSaver;
    public readonly ForceScreenSaverMode Mode = mode;
    public readonly ushort Length = 1;
}