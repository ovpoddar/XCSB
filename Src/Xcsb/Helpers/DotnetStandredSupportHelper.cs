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
            buffer.CopyTo(array);
            while (total < buffer.Length)
                total += stream.Read(array, total, buffer.Length - total);

        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
    }
}
#endif