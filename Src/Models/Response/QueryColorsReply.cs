using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryColorsReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly Pixel[] Colors;
    internal QueryColorsReply(QueryColorsResponse result, Socket socket)
    {
        this.Reply = result.Reply;
        this.Sequence = result.Sequence;
        if (result.NumberOfColors == 0)
            this.Colors = [];
        else
        {
            var requiredSize = (int)result.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer);
            this.Colors = MemoryMarshal.Cast<byte, Pixel>(buffer[0..requiredSize]).ToArray();
        }
    }
}