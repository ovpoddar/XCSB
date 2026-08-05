using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Handlers;
using Xcsb.Handlers.Direct;

namespace Xcsb.Handlers.Buffered;

internal sealed class BufferProtoOut
{
    private readonly List<byte> _buffer;
    private readonly ISocketOut _protoOut;
    private int _requestLength;

    public int Sequence => _protoOut.Sequence;

    public BufferProtoOut(ISocketOut protoOut)
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
        var buffers = MemoryMarshal.Cast<T, byte>(content);
        if (buffers.IsEmpty)
            return;
#if NETSTANDARD
        var array = new byte[buffers.Length];
        buffers.CopyTo(array);
        _buffer.AddRange(array);
#else
        // it might be the fastest. but might be slower than span copy directly.
        var start = _buffer.Count;
        CollectionsMarshal.SetCount(_buffer, start + buffers.Length);
        buffers.CopyTo(CollectionsMarshal.AsSpan(_buffer)[start..]);
#endif
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
        this.SendExact(buffer);
    }

    internal void Reset()
    {
        _buffer.Clear();
        _requestLength = 0;
    }

    private void SendExact(scoped in ReadOnlySpan<byte> buffer)
    {
        _protoOut.SendExact(in buffer);
        _protoOut.Sequence += _requestLength;
    }


}