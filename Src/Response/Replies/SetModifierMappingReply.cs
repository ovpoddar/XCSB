using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct SetModifierMappingReply : IXReply
{
    public readonly ResponseHeader<MappingStatus> ResponseHeader;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return this.Length == 0;
    }
    
    public MappingStatus Status => ResponseHeader.GetValue();
}