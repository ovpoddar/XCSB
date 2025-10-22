using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies;
using Xcsb.Response.Replies.Internals;


#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Helpers;

internal static class GenericHelper
{
    internal static ref readonly T AsStruct<T>(this Span<byte> bytes) where T : struct =>
        ref Unsafe.As<byte, T>(ref bytes[0]);

    internal static T ToStruct<T>(this Span<byte> bytes) where T : struct =>
        Unsafe.As<byte, T>(ref bytes[0]);

    internal static T Make<T, I>(this Span<byte> bytes, I value) where T : struct
        where I : struct
    {
        Debug.Assert(Unsafe.SizeOf<I>() == bytes.Length);
        Unsafe.As<byte, I>(ref bytes[0]) = value;
        return bytes.ToStruct<T>();
    }

    internal static T AddPadding<T>(this T pad) where T :
#if NETSTANDARD
    struct
    {
        object result;
        if (typeof(T) == typeof(byte))
            result = (byte)((byte)(object)pad + (4 - ((byte)(object)pad & 3) & 3));
        else if (typeof(T) == typeof(ushort))
            result = (ushort)((ushort)(object)pad + (4 - ((ushort)(object)pad & 3) & 3));
        else if (typeof(T) == typeof(short))
            result = (short)((short)(object)pad + (4 - ((short)(object)pad & 3) & 3));
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
        return T.CreateChecked(value + ((4 - (value & 3)) & 3));
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
        else if (typeof(T) == typeof(short))
            result = (short)((4 - ((short)(object)pad & 3)) & 3);
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
        return T.CreateChecked((4 - (int.CreateChecked(pad) & 3)) & 3);

#endif
    }

    internal static void SendExact(this Socket socket, scoped ReadOnlySpan<byte> buffer,
        SocketFlags socketFlags = SocketFlags.None)
    {
        var total = 0;
        while (total < buffer.Length)
        {
            var sent = socket.Send(buffer[total..], socketFlags);
            if (sent <= 0)
                throw new SocketException();
            total += sent;
        }
#if DEBUGSEND
        Debug.WriteLine("[SEND] : " + string.Join(" ", buffer.ToArray()));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Send<T>(this Socket socket, scoped ref T value) where T : unmanaged =>
        socket.SendExact(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)));

    internal static void ReceiveExact(this Socket socket, Span<byte> buffer)
    {
        if (buffer.Length == 0)
            return;

        var total = 0;
        while (socket.Connected)
        {
            total += socket.Receive(buffer[total..]);
            if (total == buffer.Length)
                break;

            if (socket.Available == 0 && total < buffer.Length)
                socket.Poll(-1, SelectMode.SelectRead);
        }
#if DEBUGRECV
        Debug.WriteLine("[RECV] : " + string.Join(" ", buffer.ToArray()));
#endif
    }

    internal static void WriteRequest<T>(this Span<byte> writeBuffer, ref T requestType, int size,
        ReadOnlySpan<byte> requestBody) where T : unmanaged
    {
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(writeBuffer), requestType);
        requestBody.CopyTo(writeBuffer[size..]);
        var remainder = requestBody.Length.Padding();
        if (remainder == 0) return;
        writeBuffer.Slice(size + requestBody.Length, remainder).Clear();
    }

#if !NETSTANDARD
    [SkipLocalsInit]
#endif
    private static T ReceivedResponse<T>(this Socket socket) where T : unmanaged
    {
        var resultLength = Unsafe.SizeOf<T>();
        Span<byte> buffer = stackalloc byte[resultLength];
        socket.EnsureReadSize(resultLength);
        socket.ReceiveExact(buffer);
        return buffer.ToStruct<T>();
    }

    internal static void EnsureReadSize(this Socket socket, int size)
    {
        while (true)
        {
            if (socket.Available >= size)
                break;
            socket.Poll(-1, SelectMode.SelectRead);
        }
    }

}