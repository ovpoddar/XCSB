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

    internal static Task<int> ReceiveAsync(this Socket socket, byte[] buffer)
    {
        var tcs = new TaskCompletionSource<int>();
        var args = new SocketAsyncEventArgs();
        args.SetBuffer(buffer, 0, buffer.Length);

        args.Completed += (sender, e) =>
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                    tcs.SetResult(e.BytesTransferred);
                else
                    tcs.SetException(new SocketException((int)e.SocketError));
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            finally
            {
                e.Dispose();
            }
        };

        if (!socket.ReceiveAsync(args))
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    var result = args.BytesTransferred;
                    args.Dispose();
                    tcs.SetResult(result);
                }
                else
                {
                    var error = new SocketException((int)args.SocketError);
                    args.Dispose();
                    tcs.SetException(error);
                }
            }
            catch (Exception ex)
            {
                args.Dispose();
                tcs.SetException(ex);
            }
        }

        return tcs.Task;
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