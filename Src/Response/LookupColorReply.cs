using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct LookupColorReply : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort ExactRed;
    public readonly ushort ExactGreen;
    public readonly ushort ExactBlue;
    public readonly ushort VisualRed;
    public readonly ushort VisualGreen;
    public readonly ushort VisualBlue;
    public bool Verify(in int sequence)
    {
        return ResponseHeader.Verify(in sequence) && ResponseHeader.Length == 0;
    }
}