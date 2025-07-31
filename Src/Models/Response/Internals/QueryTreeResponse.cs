using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct QueryTreeResponse : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly uint Root;
    public readonly uint Parent;
    public readonly ushort WindowChildrenLenght;

    public bool Verify()
    {
        return this.ResponseHeader.Verify() && this.ResponseHeader.Length == this.WindowChildrenLenght;
    }
}