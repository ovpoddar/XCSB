using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct QueryColorsReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly Pixel[] Colors;
    internal QueryColorsReply(QueryColorsResponse result, Socket socket)
    {
        Reply = result.ResponseHeader.Reply;
        Sequence = result.ResponseHeader.Sequence;
        if (result.NumberOfColors == 0)
            Colors = [];
        else
        {
            var requiredSize = (int)result.NumberOfColors * Marshal.SizeOf<Pixel>();
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer);
            Colors = MemoryMarshal.Cast<byte, Pixel>(buffer[0..requiredSize]).ToArray();
        }
    }
}