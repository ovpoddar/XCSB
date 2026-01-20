using System.Buffers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Handlers;

internal class BufferProtoOut : ProtoBase
{
    private readonly List<byte> _buffer;
    internal readonly ProtoOut ProtoOut;
    internal int RequestLength;

    public BufferProtoOut(ProtoOut protoOut) : base(protoOut)
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
        RequestLength = 0;
    }

    protected override void SendExact(scoped in ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
    {
        base.SendExact(in buffer, socketFlags);
        ProtoOut.Sequence += RequestLength;
    }

}