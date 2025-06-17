using System.Runtime.InteropServices;
using Xcsb.Models.Event;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct QueryPointerReply
{
    public readonly byte Reply;
    private readonly byte _sameScreen;
    public readonly short SequenceNumber;
    public readonly int LengthReply;
    public readonly uint Root;
    public readonly uint child;
    public readonly short RootX;
    public readonly short RootY;
    public readonly short WinX;
    public readonly short WinY;
    public readonly KeyButMask Mask;
    private fixed byte _pad0[6];

    public readonly bool IsSameScreen =>
        _sameScreen == 1;
}
