using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AllocColorReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort Red;
    public readonly ushort Green;
    public readonly ushort Blue;
    private readonly ushort _pad1;
    public readonly uint Pixel;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.Reply && this.ResponseHeader.Sequence == sequence &&
               _pad1 == 0 && this.Length == 0;
    }
}