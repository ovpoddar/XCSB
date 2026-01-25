using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Handlers;
using Xcsb.Handlers.Direct;

namespace Xcsb.Extension.Generic.Event.Handlers.Buffered;

internal sealed class BufferProtoOut : ProtoBase
{
    private readonly List<byte> _buffer;
    private readonly ProtoOut _protoOut;
    private int _requestLength;

    public int Sequence => _protoOut.Sequence;

    public BufferProtoOut(ProtoOut protoOut) : base(protoOut, protoOut.Configuration)
    {
        _protoOut = protoOut;
        _buffer = new List<byte>();
        _requestLength = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Add<T>(scoped ref T value) where T : unmanaged
    {
        AddRange(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)));
        _requestLength++;
    }

    internal void AddRange<T>(ReadOnlySpan<T> content) where T : struct
    {
        ReadOnlySpan<byte> buffers = MemoryMarshal.Cast<T, byte>(content);
        foreach (var item in buffers)
            _buffer.Add(item);
    }

    internal void Add(byte value) =>
        _buffer.Add(value);

    internal void Flush()
    {
        var buffer =
#if NETSTANDARD
        _buffer.ToArray();
#else
        CollectionsMarshal.AsSpan(_buffer);
#endif
        this.SendExact(buffer, SocketFlags.None);
    }

    internal void Reset()
    {
        _buffer.Clear();
        _requestLength = 0;
    }

    protected override void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        base.SendExact(in buffer, socketFlags);
        _protoOut.Sequence += _requestLength;
    }

}