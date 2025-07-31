using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryBestSizeReply : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort Width;
    public readonly ushort Height;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Verify(in sequence) && this.ResponseHeader.Length == 0;
    }
}