using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Response;

public struct QueryTreeReply : IXBaseResponse
{
    private readonly _QueryTreeReply _response;
    public readonly byte Reply => _response.Reply;
    public readonly ushort Sequence => _response.Sequence;
    public readonly uint Root => _response.Root;
    public readonly uint Parent => _response.Parent;
    public uint[] WindowChildren;

    public QueryTreeReply(Socket socket)
    {
        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<_QueryTreeReply>()];
        socket.ReceiveExact(buffer);
        _response = buffer.AsStruct<_QueryTreeReply>();

        if (_response.WindowChildrenLenght == 0)
            WindowChildren = [];
        else
        {
            using var windowChildren = new ArrayPoolUsing<byte>(_response.WindowChildrenLenght * 4);
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