using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public struct AllocColorCellsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public uint[] Pixels;
    public ushort[] Masks;

    internal AllocColorCellsReply(AllocColorCellsResponse result, Socket socket)
    {
        Reply = result.ResponseHeader.Reply;
        Sequence = result.ResponseHeader.Sequence;
        var requiredSize = (int)result.Length * 4;
        using var buffer = new ArrayPoolUsing<byte>(requiredSize);
        socket.ReceiveExact(buffer);
        Pixels = result.NumberOfPixels == 0
            ? []
            : MemoryMarshal.Cast<byte, uint>(buffer[0..(result.NumberOfPixels * 4)]).ToArray();

        Masks = result.NumberOfMasks == 0
            ? []
            : MemoryMarshal.Cast<byte, ushort>(buffer[(result.NumberOfPixels * 4)..requiredSize]).ToArray();
    }
}