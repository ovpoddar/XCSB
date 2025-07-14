using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Response;
public struct GetPropertyReply
{
    public byte Reply; // 1
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

        var data = new byte[propertyReply.Length * 4];
        if (data.Length != 0)
            socket.ReceiveExact(data);
        Data = data;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
    private readonly unsafe struct _GetPropertyReply
    {
        public readonly byte Reply;
        public readonly byte Format;
        public readonly ushort Sequence;
        public readonly uint Length;
        public readonly uint Type;
        public readonly uint BytesAfter;
        public readonly uint ValueLength;
    }
}