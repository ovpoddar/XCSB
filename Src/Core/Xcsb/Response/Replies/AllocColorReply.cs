using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AllocColorReply : IXReply<AllocColorReply, AllocColorReply>, IVerify
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort Red;
    public readonly ushort Green;
    public readonly ushort Blue;
    private readonly ushort _pad1;
    public readonly uint Pixel;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply && ResponseHeader.Verify(in sequence) &&
               _pad1 == 0 && Length == 0;
    }
    
    public AllocColorReply FromBytes(Span<byte> response)
    {
        return  response.ToStruct<AllocColorReply>();
    }
}