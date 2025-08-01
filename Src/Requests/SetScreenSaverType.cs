using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetScreenSaverType(
    short timeout,
    short interval,
    TriState preferBlanking,
    TriState allowExposures)
{
    public readonly Opcode OpCode = Opcode.SetScreenSaver;
    private readonly byte _pad0;
    public readonly ushort Length = 3;
    public readonly short TimeOut = timeout;
    public readonly short Interval = interval;
    public readonly TriState PreferBlanking = preferBlanking;
    public readonly TriState AllowExposures = allowExposures;
}