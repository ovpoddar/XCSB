using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 52)]
internal unsafe struct GetKeyboardControlResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly AutoRepeatMode AutoRepeatMode;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint LedMask;
    public readonly byte KeyClickPercent;
    public readonly byte BellPercent;
    public readonly ushort BellPitch;
    public readonly ushort BellDuration;
    private readonly ushort _pad0;
    public fixed byte Repeats[32];

    public bool Verify(in int sequence)
    {
        return this.Reply == 1 && this.Length == 5 && this.Sequence == sequence;
    }
}