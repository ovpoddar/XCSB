using System.Runtime.InteropServices;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListHostsResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly AccessControlMode Mode;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly ushort NumberOfHosts;

    public bool Verify()
    {
        return this.Reply == 1 && this.Length % 4 == 0;
    }
}