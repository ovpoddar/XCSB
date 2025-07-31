using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct AllocColorPlanesReply
{
    public readonly byte Reply;
    public readonly ushort Sequence;
    public readonly uint RedMask;
    public readonly uint GreenMask;
    public readonly uint BlueMask;
    public uint[] Pixels;

    internal AllocColorPlanesReply(AllocColorPlanesResponse response, Socket stream)
    {
        this.Reply = response.ResponseHeader.Reply;
        this.Sequence = response.ResponseHeader.Sequence;
        this.RedMask = response.RedMask;
        this.GreenMask = response.GreenMask;
        this.BlueMask = response.BlueMask;
        if (response.NumberOfPixels == 0)
            this.Pixels = [];
        else
        {
            var requiredSize = (int)response.ResponseHeader.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            stream.ReceiveExact(buffer[0..requiredSize]);
            this.Pixels = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
        }
    }
}