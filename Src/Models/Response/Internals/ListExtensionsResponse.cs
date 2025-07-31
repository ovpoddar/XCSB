using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListExtensionsResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly byte NumberOfExtensions;
    public readonly ushort Sequence;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return this.Reply == 1 && this.Length * 4 >= NumberOfExtensions && this.Sequence == sequence;
    }
}