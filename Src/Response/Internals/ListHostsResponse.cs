using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListHostsResponse : IXBaseResponse
{
    public readonly ResponseHeader<AccessControlMode> ResponseHeader;
    public readonly ushort NumberOfHosts;

    public bool Verify(in int sequence)
    {
        // Mode
        return true;
    }
}