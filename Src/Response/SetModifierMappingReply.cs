using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct SetModifierMappingReply : IXBaseResponse
{
    public readonly ResponseHeader<MappingStatus> ResponseHeader;

    public bool Verify(in int sequence)
    {
        // Status
        return ResponseHeader.Length == 0;
    }
}