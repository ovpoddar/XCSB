using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Response.Contract;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
public readonly struct ResponseHeader<T> : IXBaseResponse where T : struct
{
    public readonly byte Reply;
    public readonly T Value;
    public readonly ushort Sequence;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return Reply == 1 && Sequence == sequence && Unsafe.SizeOf<T>() == 1;
    }
}