using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct AllocColorCellsReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public uint[] Pixels;
    public ushort[] Masks;

    internal AllocColorCellsReply(AllocColorCellsResponse result, Socket socket)
    {
        this.Reply = result.ResponseHeader.Reply;
        this.Sequence = result.ResponseHeader.Sequence;
        var requiredSize = (int)result.ResponseHeader.Length * 4;
        using var buffer = new ArrayPoolUsing<byte>(requiredSize);
        socket.ReceiveExact(buffer);
        this.Pixels = result.NumberOfPixels == 0
            ? []
            : MemoryMarshal.Cast<byte, uint>(buffer[0..(result.NumberOfPixels * 4)]).ToArray();

        this.Masks = result.NumberOfMasks == 0
            ? []
            : MemoryMarshal.Cast<byte, ushort>(buffer[(result.NumberOfPixels * 4)..requiredSize]).ToArray();
    }
}