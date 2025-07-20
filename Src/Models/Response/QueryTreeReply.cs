using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Response;

public struct QueryTreeReply : IXBaseResponse
{
    public byte Reply;
    public ushort Sequence;
    public uint Root;
    public uint Parent;
    public uint[] WindowChildren;

    public QueryTreeReply(Socket socket)
    {
        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<_QueryTreeReply>()];
        socket.ReceiveExact(buffer);
        ref var queryTreeReply = ref buffer.AsStruct<_QueryTreeReply>();
        Reply = queryTreeReply.Reply;
        Sequence = queryTreeReply.Sequence;
        Root = queryTreeReply.Root;
        Parent = queryTreeReply.Parent;

        var windowChildrenLenght = queryTreeReply.WindowChildrenLenght * 4;
        if (windowChildrenLenght == 0)
            WindowChildren = [];
        else
        {
            var windowChildren = new ArrayPoolUsing<byte>(windowChildrenLenght);
            socket.ReceiveExact(windowChildren);
            WindowChildren = MemoryMarshal.Cast<byte, uint>(windowChildren).ToArray();
        }
    }

    public bool Verify()
    {
        return this.Reply == 1;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
    private readonly struct _QueryTreeReply
    {
        public readonly byte Reply;
        private readonly byte _pad0;
        public readonly ushort Sequence;
        public readonly uint Length;
        public readonly uint Root;
        public readonly uint Parent;
        public readonly ushort WindowChildrenLenght;
    }
}