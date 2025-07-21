using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct QueryTreeResponse : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint Root;
    public readonly uint Parent;
    public readonly ushort WindowChildrenLenght;

    public bool Verify()
    {
        return this.Reply == 1 && this.Length == this.WindowChildrenLenght;
    }
}