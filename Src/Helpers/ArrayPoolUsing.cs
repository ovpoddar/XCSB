using System.Buffers;
using System.Runtime.CompilerServices;

namespace Xcsb.Helpers;
internal struct ArrayPoolUsing<T> : IDisposable
{
    private readonly ArrayPool<T> _arrayPool;
    private readonly int _length;
    private readonly bool _clearArray;
    private T[]? _values;

    public ArrayPoolUsing(int length = 0, bool clearArray = false)
    {
        _arrayPool = ArrayPool<T>.Shared;
        _length = length;
        _clearArray = clearArray;

        Rent(length);
    }

    public ArrayPoolUsing<T> Rent(int size)
    {
        if (size == 0) return this;
        ArgumentOutOfRangeException.ThrowIfNegative(size);

        if (_values != null)
            _arrayPool.Return(_values, _clearArray);
        _values = _arrayPool.Rent(size);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Dispose()
    {
        if (_values != null)
            _arrayPool.Return(_values, _clearArray);
    }

    public static implicit operator T[](ArrayPoolUsing<T> arrayPoolUsing) =>
        arrayPoolUsing._values ?? [];

    public static implicit operator Span<T>(ArrayPoolUsing<T> arrayPoolUsing) =>
        arrayPoolUsing._values.AsSpan(0, arrayPoolUsing._length);

    public readonly Span<T> Slice(int length)
    {
        if (length < 0 || length >= _length)
            throw new ArgumentOutOfRangeException(nameof(length));
        return _values.AsSpan(0, length);
    }

    public readonly Span<T> Slice(int start, int length)
    {
        if (length < 0 || length > _length)
            throw new ArgumentOutOfRangeException(nameof(length));
        return _values.AsSpan(start, length);
    }

    public readonly T this[int index]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_values);
            if ((uint)index >= (uint)_values.Length)
                throw new IndexOutOfRangeException();
            return _values[index];
        }

        set
        {
            ArgumentNullException.ThrowIfNull(_values);
            if ((uint)index >= (uint)_values.Length)
                throw new IndexOutOfRangeException();
            _values[index] = value;
        }
    }

    public readonly Span<T> this[Range range]
    {
        get
        {
            if (_values is null) return [];
            return _values.AsSpan(range);
        }
    }
}
