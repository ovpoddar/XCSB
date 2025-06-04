using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xcsb.Helpers;

namespace Xcsb.Models;
public unsafe struct GetPropertyReply
{
    public byte Reply;
    public byte Format;
    public ushort Sequence;
    public uint Type;

    public byte[] Data;
    internal GetPropertyReply(Socket socket)
    {
        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<_GetPropertyReply>()];
        socket.ReceiveExact(buffer);

        ref var propertyReply = ref buffer.AsStruct<_GetPropertyReply>();
        Reply = propertyReply.Reply;
        Format = propertyReply.Format;
        Sequence = propertyReply.Sequence;
        Type = propertyReply.Type;

        var data = new byte[propertyReply.Length];
        if (data.Length != 0)
            socket.ReceiveExact(data);
        this.Data = data;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
    private readonly unsafe struct _GetPropertyReply
    {
        public readonly byte Reply;
        public readonly byte Format;
        public readonly ushort Sequence;
        public readonly uint Length;
        public readonly uint Type;
        public readonly uint bytes_after;
        public readonly uint value_len;
    }
}