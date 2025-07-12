using System.Buffers;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Helpers;

internal static class GenericHelper
{
    internal static ref T AsStruct<T>(this Span<byte> @bytes) where T : struct =>
        ref Unsafe.As<byte, T>(ref @bytes[0]);

    internal static T ToStruct<T>(this Span<byte> @bytes) where T : struct =>
        Unsafe.As<byte, T>(ref @bytes[0]);

    internal static T AddPadding<T>(this T pad) where T :
#if NETSTANDARD
    struct
    {
        object result;
        if (typeof(T) == typeof(byte))
            result = (byte)((byte)(object)pad + (4 - ((byte)(object)pad & 3) & 3));
        else if (typeof(T) == typeof(ushort))
            result = (ushort)((ushort)(object)pad + (4 - ((ushort)(object)pad & 3) & 3));
        else if (typeof(T) == typeof(int))
            result = (int)(object)pad + (4 - ((int)(object)pad & 3) & 3);
        else if (typeof(T) == typeof(uint))
            result = (uint)(object)pad + (4 - ((uint)(object)pad & 3) & 3); 
        else
            throw new ArgumentException($"Padding not implemented for type {typeof(T)}");
        return (T)result;
#else
        INumber<T>
    {
        var value = int.CreateChecked(pad);
        return T.CreateChecked(value + (4 - (value & 3) & 3));
#endif
    }

    internal static T Padding<T>(this T pad) where T :
#if NETSTANDARD
    struct
    {
        object result;

        if (typeof(T) == typeof(byte))
            result = (byte)((4 - ((byte)(object)pad & 3)) & 3);
        else if (typeof(T) == typeof(ushort))
            result = (ushort)((4 - ((ushort)(object)pad & 3)) & 3);
        else if (typeof(T) == typeof(int))
            result = (4 - ((int)(object)pad & 3)) & 3;
        else if (typeof(T) == typeof(uint))
            result = (4 - ((uint)(object)pad & 3)) & 3;
        else
            throw new ArgumentException($"Padding not implemented for type {typeof(T)}");

        return (T)result;
#else
        INumber<T>
    {
        return T.CreateChecked(4 - (int.CreateChecked(pad) & 3) & 3);

#endif
    }

    internal static void SendExact(this Socket socket, scoped ReadOnlySpan<byte> buffer,
        SocketFlags socketFlags = SocketFlags.None)
    {
        var total = 0;
#if NETSTANDARD
        var array = ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            buffer.CopyTo(array);
            while (total < buffer.Length)
            {
                var sent = socket.Send(array, total, buffer.Length - total, socketFlags);
                if (sent <= 0)
                    throw new SocketException();
                total += sent;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
#else
        while (total < buffer.Length)
        {
            var sent = socket.Send(buffer[total..], socketFlags);
            if (sent <= 0)
                throw new SocketException();
            total += sent;
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Send<T>(this Socket socket, scoped in T value) where T : unmanaged =>
        socket.SendExact(MemoryMarshal.AsBytes(value.AsReadOnlySpan()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe ReadOnlySpan<byte> AsReadOnlySpan<T>(this T @struct) where T : unmanaged =>
        new ReadOnlySpan<byte>(&@struct, sizeof(T));

    internal static void ReceiveExact(this Socket socket, Span<byte> buffer)
    {
        var total = 0;
#if NETSTANDARD
        var array = ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            buffer.CopyTo(array);
            while (total < buffer.Length)
                total += socket.Receive(array, total, buffer.Length - total, SocketFlags.None);

        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
#else
        while (socket.Connected)
        {
            total += socket.Receive(buffer[total..]);
            if (total == buffer.Length)
                break;
        }
#endif
    }

    internal static void Add<T>(this List<byte> list, scoped ref T value) where T : unmanaged
    {
#if NETSTANDARD
        var size = Unsafe.SizeOf<T>();
        using var buffer = new ArrayPoolUsing<byte>(size);
        Span<byte> bytes = buffer;
        Unsafe.WriteUnaligned(ref bytes[0], value);
        list.AddRange((byte[])buffer);
#else
        list.AddRange(MemoryMarshal.AsBytes(value.AsReadOnlySpan()));
#endif
    }

    //todo: verify
    internal static void WriteRequest<T>(this Span<byte> writeBuffer, ref T requestType, int size,
        ReadOnlySpan<byte> requestBody) where T : unmanaged
    {
#if NETSTANDARD
#else
        MemoryMarshal.Write(writeBuffer[0..size], in requestType);
        requestBody.CopyTo(writeBuffer[size..requestBody.Length]);
        if (requestBody.Length % 4 == 0)
            return;
        Debug.Assert(writeBuffer.Length - requestBody.Length >= 4);
        Debug.Assert(writeBuffer.Length - requestBody.Length == requestBody.Length.Padding());
        writeBuffer[requestBody.Length..writeBuffer.Length].Clear();
#endif
    }
}