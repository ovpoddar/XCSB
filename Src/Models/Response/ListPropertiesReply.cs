using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Helpers;

namespace Xcsb.Models.Response;

public struct ListPropertiesReply : IXBaseResponse
{
    private readonly _ListPropertiesReply _response;
    public readonly byte Reply => _response.Reply;
    public readonly ushort Sequence => _response.Sequence;
    public uint[] Atoms;

    public ListPropertiesReply(Socket socket)
    {
        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<_ListPropertiesReply>()];
        socket.ReceiveExact(buffer);
        _response = buffer.ToStruct<_ListPropertiesReply>();
        var atoms = new ArrayPoolUsing<byte>((int)_response.NumberOfProperties * 4);
        socket.ReceiveExact(atoms);
        Atoms = MemoryMarshal.Cast<byte, uint>(atoms).ToArray();
    }

    public bool Verify()
    {
        return this.Reply == 1 && this._response.Length == this._response.NumberOfProperties;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
    private readonly struct _ListPropertiesReply
    {
        public readonly byte Reply;
        private readonly byte _pad0;
        public readonly ushort Sequence;
        public readonly uint Length;
        public readonly ushort NumberOfProperties;
    }
}