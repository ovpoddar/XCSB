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
        unmanaged
    {
        var value = Marshal.SizeOf<T>() switch
        {
            1 => Unsafe.As<T, byte>(ref pad),
            2 => Unsafe.As<T, ushort>(ref pad),
            4 => Unsafe.As<T, int>(ref pad),
            _ => throw new ArgumentException($"Padding not implemented for type {nameof(T)}")
        };

        T result = default;
        Unsafe.As<T, int>(ref result) = value + ((4 - (value & 3)) & 3);
        return result;
#else
        INumber<T>
    {
        var value = int.CreateChecked(pad);
        return T.CreateChecked(value + ((4 - (value & 3)) & 3));
#endif
    }

    internal static T Padding<T>(this T pad) where T :
#if NETSTANDARD
        unmanaged
    {
        if (typeof(T) == typeof(byte))
        {
            ref var padByte = ref Unsafe.As<T, byte>(ref pad);
            var result = (byte)(padByte + ((4 - (padByte & 3u)) & 3u));
            return Unsafe.As<byte, T>(ref result);
        }

        if (typeof(T) == typeof(ushort))
        {
            ref var padUShort = ref Unsafe.As<T, ushort>(ref pad);
            var result = (ushort)(padUShort + ((4 - (padUShort & 3u)) & 3u));
            return Unsafe.As<ushort, T>(ref result);
        }

        if (typeof(T) == typeof(short))
        {
            ref var padShort = ref Unsafe.As<T, short>(ref pad);
            var result = (short)(padShort + ((4 - (padShort & 3u)) & 3u));
            return Unsafe.As<short, T>(ref result);
        }

        if (typeof(T) == typeof(int))
        {
            ref var padInt = ref Unsafe.As<T, int>(ref pad);
            var result = padInt + (int)((4 - (padInt & 3u)) & 3u);
            return Unsafe.As<int, T>(ref result);
        }

        if (typeof(T) == typeof(uint))
        {
            ref var padUInt = ref Unsafe.As<T, uint>(ref pad);
            var result = padUInt + ((4 - (padUInt & 3u)) & 3u);
            return Unsafe.As<uint, T>(ref result);
        }

        throw new ArgumentException($"Padding not implemented for type {nameof(T)}");

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
    }

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

    internal static void EnsureReadSize(this Socket socket, int size)
    {
        while (true)
        {
            if (socket.Available >= size)
                break;
            socket.Poll(-1, SelectMode.SelectRead);
        }
    }


    internal static int CountFlags<T>(this T value) where T : struct, Enum
    {
        var v = Convert.ToUInt64(value);
        var count = 0;

        while (v != 0)
        {
            count += (int)(v & 1);
            v >>= 1;
        }

        return count;
    }
}