using System.Buffers;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Models;

public struct ReplyLease<T> : IDisposable where T : struct, IXBaseResponse<T>
{
    private byte[]? _buffer;
    private readonly ArrayPool<byte> _pool;

    public readonly T Reply;

    public readonly ReadOnlySpan<byte> Buffer => _buffer ?? throw new ObjectDisposedException(nameof(ReplyLease<T>));

    internal ReplyLease(byte[] buffer, ArrayPool<byte> pool)
    {
        Reply = default(T).FromBytes(buffer);
        _buffer = buffer;
        _pool = pool;
    }

    public void Dispose()
    {
        var buffer = _buffer;

        if (buffer is null)
            return;

        _buffer = null;
        _pool.Return(buffer);
    }
}

public struct ReplyLease : IDisposable
{
    private byte[]? _buffer;
    private readonly ArrayPool<byte> _pool;


    public readonly XEvent Reply;
    public readonly ReadOnlySpan<byte> Buffer => _buffer ?? throw new ObjectDisposedException(nameof(ReplyLease));
    
    internal ReplyLease(byte[] buffer, MappingDetails mappingDetails, ArrayPool<byte> pool)
    {
        Reply = new XEvent(buffer, mappingDetails);
        _buffer = buffer;
        _pool = pool;
    }

    public void Dispose()
    {
        var buffer = _buffer;

        if (buffer is null)
            return;

        _buffer = null;
        _pool.Return(buffer);
    }
}