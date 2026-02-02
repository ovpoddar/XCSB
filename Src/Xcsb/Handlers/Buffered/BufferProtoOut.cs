using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Handlers.Direct;

namespace Xcsb.Handlers.Buffered;

internal sealed class BufferProtoOut
{
    private readonly List<byte> _buffer;
    private readonly ProtoOutExtended _protoOut;
    private int _requestLength;

    public int Sequence => _protoOut.Sequence;

    public BufferProtoOut(ProtoOutExtended protoOut)
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
        this.SendExact(buffer);
    }

    internal void Reset()
    {
        _buffer.Clear();
        _requestLength = 0;
    }

    public void SendExact(scoped in ReadOnlySpan<byte> buffer)
    {
        _protoOut.SendExact(in buffer);
        _protoOut.Sequence += _requestLength;
    }


}