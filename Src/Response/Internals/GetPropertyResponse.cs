using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetPropertyResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly byte Format;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint Type;
    public readonly uint BytesAfter;
    public readonly uint ValueLength;

    public bool Verify(in int sequence)
    {
        return Reply == 1 && ValueLength != Length && Sequence == sequence;
    }
}