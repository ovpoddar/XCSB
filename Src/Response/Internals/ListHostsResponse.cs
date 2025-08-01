using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListHostsResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly AccessControlMode Mode;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly ushort NumberOfHosts;

    public bool Verify(in int sequence)
    {
        return Reply == 1 && Sequence == sequence;
    }
}