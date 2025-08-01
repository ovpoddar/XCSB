using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct TranslateCoordinatesReply : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _sameScreen;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint Window;
    public readonly ushort DestinationX;
    public readonly ushort DestinationY;

    public bool Verify(in int sequence)
    {
        return Reply == 1 && Length == 0 && Sequence == sequence;
    }
    public bool SameScreen => _sameScreen == 1;
}