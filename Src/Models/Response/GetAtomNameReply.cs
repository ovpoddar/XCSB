using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Helpers;

namespace Xcsb.Models.Response;

public struct GetAtomNameReply : IXBaseResponse
{
    private readonly _GetAtomNameReply _response;
    public readonly byte Reply => _response.Reply;
    public readonly ushort Sequence => _response.Sequence;
    public readonly string Name;

    // todo: try to move the first read to caller 
    // leave the calling here so can avoid the private
    // storing of response and also call the verify and
    // verify the sequence 
    public GetAtomNameReply(Socket socket)
    {
        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<_GetAtomNameReply>()];
        socket.ReceiveExact(buffer);
        _response = buffer.AsStruct<_GetAtomNameReply>();
        using var nameBuffer = new ArrayPoolUsing<byte>((int)_response.Length);
        socket.ReceiveExact(nameBuffer);

        Name = Encoding.ASCII.GetString(nameBuffer, 0, _response.LengthOfName);
    }

    public bool Verify()
    {
        return this.Reply == 1 && this._response.Length == this._response.LengthOfName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
    private readonly struct _GetAtomNameReply
    {
        public readonly byte Reply;
        private readonly byte _pad0;
        public readonly ushort Sequence;
        public readonly uint Length;
        public readonly ushort LengthOfName;
    }
}