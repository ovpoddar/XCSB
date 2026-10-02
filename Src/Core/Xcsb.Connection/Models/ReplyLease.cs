using System.Buffers;

namespace Xcsb.Models;

public ref struct ReplyLease<T> where T : struct,IXBaseResponse<T>
{
    private byte[]? _buffer;
    private readonly ArrayPool<byte> _pool;

    public readonly T Reply;

    public readonly ReadOnlySpan<byte> Buffer => _buffer ?? throw new ObjectDisposedException(nameof(ReplyLease<T>));

    internal ReplyLease(byte[] buffer, ArrayPool<byte> pool)
    {
        var type = default(T);
        Reply = type.FromBytes(buffer);
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