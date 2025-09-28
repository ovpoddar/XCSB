using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetGeometryReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint Root;
    public readonly ushort X;
    public readonly ushort Y;
    public readonly ushort Width;
    public readonly ushort Height;
    public readonly ushort BorderWidth;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.Reply &&
               this.Length == 0;
    }

    public byte Depth => ResponseHeader.GetValue();
}