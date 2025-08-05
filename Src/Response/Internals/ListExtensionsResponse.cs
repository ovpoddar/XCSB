using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListExtensionsResponse : IXBaseResponse
{
    public readonly ResponseHeader<byte> ResponseHeader;

    public bool Verify(in int sequence)
    {
        // NumberOfExtensions
        return ResponseHeader.Length * 4 >= ResponseHeader.Value;
    }
}