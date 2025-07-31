using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AllocNamedColorReply : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly uint Pixel;
    public readonly ushort ExactRed;
    public readonly ushort ExactGreen;
    public readonly ushort ExactBlue;
    public readonly ushort VisualRed;
    public readonly ushort VisualGreen;
    public readonly ushort VisualBlue;
    public bool Verify()
    {
        return this.ResponseHeader.Verify() && this.ResponseHeader.Length == 0;
    }
}