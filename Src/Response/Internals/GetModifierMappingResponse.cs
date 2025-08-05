using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetModifierMappingResponse : IXBaseResponse
{
    public readonly ResponseHeader<byte> ResponseHeader;

    public bool Verify(in int sequence)
    {
        //KeycodesPerModifier
        return ResponseHeader.Length == ResponseHeader.Value * 2 && ResponseHeader.Sequence == sequence;
    }
}