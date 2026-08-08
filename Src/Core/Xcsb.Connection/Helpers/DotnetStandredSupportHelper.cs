#if NETSTANDARD
using System.Buffers;
using System.Net.Sockets;

namespace Xcsb.Helpers;

internal static class DotnetStandardSupportHelper
{
    internal static void AddRange(this List<byte> list, ReadOnlySpan<byte> buffer)
    {
        var scratchBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
        list.AddRange(scratchBuffer.Take(buffer.Length));
        ArrayPool<byte>.Shared.Return(scratchBuffer);
    }

    internal static void ReadExactly(this Stream stream, Span<byte> buffer)
    {
        var total = 0;
        var array = ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            while (total < buffer.Length)
            {
                var read = stream.Read(array, total, buffer.Length - total);
                if (read == 0)
                    throw new EndOfStreamException();
                total += read;
            }
            array.AsSpan(0, total).CopyTo(buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
    }
}
#endif