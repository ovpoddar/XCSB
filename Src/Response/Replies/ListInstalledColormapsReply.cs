using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public struct ListInstalledColormapsReply
{
    public ResponseType Reply;
    public ushort Sequence;
    public uint[] Colormap;

    internal ListInstalledColormapsReply(ListInstalledColormapsResponse response, Socket socket)
    {
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.Length == 0)
            Colormap = [];
        else
        {
            var requiredSize = response.NumberOfColormaps * 4;
            using var buffer = new ArrayPoolUsing<byte>(requiredSize);
            socket.ReceiveExact(buffer[0..requiredSize]);
            Colormap = MemoryMarshal.Cast<byte, uint>(buffer[0..requiredSize]).ToArray();
        }
    }
}