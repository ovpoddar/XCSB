using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetModifierMappingResponse : IXBaseResponse
{
    public readonly ResponseHeader<byte> ResponseHeader;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Length == KeycodesPerModifier * 2 && ResponseHeader.Sequence == sequence;
    }
    
    public byte KeycodesPerModifier => ResponseHeader.GetValue();
}