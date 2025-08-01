using System.Runtime.InteropServices;

namespace Xcsb.Response.Contract;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
public readonly struct ResponseHeader : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return Reply == 1 && Sequence == sequence;
    }
}