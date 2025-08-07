using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetPropertyResponse : IXBaseResponse
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Type;
    public readonly uint BytesAfter;
    public readonly uint ValueLength;

    public bool Verify(in int sequence)
    {
        return ValueLength != ResponseHeader.Length;
    }
    
    public byte Format => ResponseHeader.GetValue();
}