using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryBestSizeReply : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort Width;
    public readonly ushort Height;

    public bool Verify()
    {
        return this.ResponseHeader.Verify() && this.ResponseHeader.Length == 0;
    }
}