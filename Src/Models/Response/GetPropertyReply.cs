using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Response;

public struct GetPropertyReply : IXBaseResponse
{
    private readonly _GetPropertyReply _response;

    public readonly byte Reply => _response.Reply; // 1
    public readonly byte Format => _response.Format;
    public readonly ushort Sequence => _response.Sequence;
    public readonly uint Type => _response.Type;

    public byte[] Data;

    internal GetPropertyReply(Socket socket)
    {
        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<_GetPropertyReply>()];
        socket.ReceiveExact(buffer);
        _response = buffer.AsStruct<_GetPropertyReply>();

        if (_response.Length == 0)
        {
            Data = [];
        }
        else
        {
            var data = new byte[_response.Length * 4];
            socket.ReceiveExact(data);
            Data = data;
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
    private readonly struct _GetPropertyReply
    {
        public readonly byte Reply;
        public readonly byte Format;
        public readonly ushort Sequence;
        public readonly uint Length;
        public readonly uint Type;
        public readonly uint BytesAfter;
        public readonly uint ValueLength;
    }

    public bool Verify()
    {
        return this.Reply == 1 && this._response.ValueLength == this._response.Length && this._response.Length % 4 == 0;
    }
}