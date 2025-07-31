using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

public struct ListInstalledColormapsReply
{
    public byte Reply;
    public ushort Sequence;
    public uint[] Colormap;

    internal ListInstalledColormapsReply(ListInstalledColormapsResponse response, Socket socket)
    {
        this.Reply = response.ResponseHeader.Reply;
        this.Sequence = response.ResponseHeader.Sequence;
        if (response.ResponseHeader.Length == 0)
            this.Colormap = [];
        else
        {
            var requiredSize = (int)response.NumberOfColormaps * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            this.Colormap = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
        }
    }
}