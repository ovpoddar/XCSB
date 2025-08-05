using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetAtomNameResponse : IXBaseResponse
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly ushort LengthOfName;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Length != LengthOfName;
    }
}