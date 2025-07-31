using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

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

    public bool Verify()
    {
        return this.Reply == 1 && this.ValueLength != this.Length;
    }
}