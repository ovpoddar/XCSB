using System.Buffers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Handlers;

internal class BufferProtoOut
{
    private readonly List<byte> _buffer;
    internal readonly ProtoOut ProtoOut;
    internal int RequestLength;

    public BufferProtoOut(ProtoOut protoOut)
    {
        ProtoOut = protoOut;
        _buffer = new List<byte>();
        RequestLength = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Add<T>(scoped ref T value) where T : unmanaged
    {
        AddRange(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)));
        RequestLength++;
    }

    internal void AddRange(ReadOnlySpan<byte> buffer)
    {
        var scratchBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
        buffer.CopyTo(scratchBuffer);
        _buffer.AddRange(scratchBuffer.Take(buffer.Length));
        ArrayPool<byte>.Shared.Return(scratchBuffer);
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
        ProtoOut.SendExact(buffer);
        ProtoOut.Sequence += RequestLength;
    }

    internal void Reset()
    {
        _buffer.Clear();
        RequestLength = 0;
    }
}