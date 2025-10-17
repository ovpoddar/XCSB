using System.Buffers;
using System.Runtime.CompilerServices;

namespace Xcsb.Helpers;

internal struct ArrayPoolUsing<T> : IDisposable
{
    public int Length { get; private set; }

    private readonly ArrayPool<T> _arrayPool;
    private readonly bool _clearArray;
    private T[]? _values;

    public ArrayPoolUsing(int length = 0, bool clearArray = false)
    {
        _arrayPool = ArrayPool<T>.Shared;
        _clearArray = clearArray;

        Rent(length);
    }

    public ArrayPoolUsing<T> Rent(int size)
    {
        if (size == 0) return this;
        if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));

        if (_values != null)
            _arrayPool.Return(_values, _clearArray);
        _values = _arrayPool.Rent(size);

        Length = size;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Dispose()
    {
        if (_values != null)
            _arrayPool.Return(_values, _clearArray);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T[](ArrayPoolUsing<T> arrayPoolUsing)
    {
        try
        {
            if (arrayPoolUsing._values == null) return [];
            var result = new T[arrayPoolUsing.Length];
            Array.Copy(arrayPoolUsing._values, result, result.Length);
            return result;
        }
        finally
        {
            arrayPoolUsing.Dispose();
        }
    }

    public static implicit operator ArraySegment<T>(ArrayPoolUsing<T> arrayPoolUsing) =>
        arrayPoolUsing._values is null
            ? []
            : new ArraySegment<T>(arrayPoolUsing._values, 0, arrayPoolUsing.Length);

    public static implicit operator Span<T>(ArrayPoolUsing<T> arrayPoolUsing) =>
        arrayPoolUsing._values.AsSpan(0, arrayPoolUsing.Length);

    public readonly Span<T> Slice(int start, int length)
    {
        if (length < 0 || length > Length)
            throw new ArgumentOutOfRangeException(nameof(length));
        return _values.AsSpan(start, length);
    }

    public readonly T this[int index]
    {
        get
        {
            if (_values is null)
                throw new ArgumentNullException(nameof(_values));
            if ((uint)index >= (uint)_values.Length)
                throw new IndexOutOfRangeException();
            return _values[index];
        }

        set
        {
            if (_values is null)
                throw new ArgumentNullException(nameof(_values));

            if ((uint)index >= (uint)_values.Length)
                throw new IndexOutOfRangeException();
            _values[index] = value;
        }
    }

    //todo: optamice with length veriable
    public readonly Span<T> this[Range range] =>
        _values is null
            ? []
            : _values.AsSpan(range);
}