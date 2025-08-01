using System.Runtime.InteropServices;
using Xcsb.Event;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryPointerReply : IXBaseResponse
{
    public readonly byte Reply; // 1
    private readonly byte _sameScreen;
    public readonly short Sequence;
    public readonly int LengthReply;
    public readonly uint Root;
    public readonly uint child;
    public readonly short RootX;
    public readonly short RootY;
    public readonly short WinX;
    public readonly short WinY;
    public readonly KeyButMask Mask;

    public readonly bool IsSameScreen => _sameScreen == 1;

    public bool Verify(in int sequence)
    {
        return Reply == 1 && LengthReply == 0 && Sequence == sequence;
    }
}