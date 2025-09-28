using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public struct AllocColorPlanesReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly uint RedMask;
    public readonly uint GreenMask;
    public readonly uint BlueMask;
    public uint[] Pixels;

    internal AllocColorPlanesReply(AllocColorPlanesResponse response, Socket stream)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        RedMask = response.RedMask;
        GreenMask = response.GreenMask;
        BlueMask = response.BlueMask;
        if (response.NumberOfPixels == 0)
            Pixels = [];
        else
        {
            var requiredSize = (int)response.Length * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            stream.ReceiveExact(buffer[0..requiredSize]);
            Pixels = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
        }
    }
}