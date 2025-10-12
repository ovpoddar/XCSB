using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct AllocColorPlanesResponse : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort NumberOfPixels;
    private readonly ushort _pad1;
    public readonly uint RedMask;
    public readonly uint GreenMask;
    public readonly uint BlueMask;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               _pad1 == 0 && Length == NumberOfPixels;
    }
}