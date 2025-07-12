using System;
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
    internal unsafe static void Send<T>(this Socket socket, scoped ref T value) where T : unmanaged
    {
#if NETSTANDARD
        fixed (byte* pointer = &Unsafe.As<T, byte>(ref value))
        {
            var buffer = new ReadOnlySpan<byte>(pointer, Unsafe.SizeOf<T>());
            socket.SendExact(buffer);
        }
#else
        var buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1));
        socket.SendExact(buffer);
#endif
    }

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

    internal unsafe static void Add<T>(this List<byte> list, scoped ref T value) where T : unmanaged
    {
#if NETSTANDARD
        fixed (byte* pointer = &Unsafe.As<T, byte>(ref value))
        {
            var buffer = new ReadOnlySpan<byte>(pointer, Marshal.SizeOf<T>());
            list.AddRange(buffer);
        }
#else
        var buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1));
        list.AddRange(buffer);
#endif
    }

    internal static void WriteRequest<T>(this Span<byte> writeBuffer, ref T requestType, int size,
        ReadOnlySpan<byte> requestBody) where T : unmanaged
    {
#if NETSTANDARD
#else
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(writeBuffer), requestType);
        requestBody.CopyTo(writeBuffer[size..]);
        var remainder = requestBody.Length.Padding();
        if (remainder != 0)
        {
            var paddingStart = size + requestBody.Length;
            var paddingCount = 4 - remainder;

            ref byte paddingRef = ref writeBuffer[paddingStart];
            if (paddingCount >= 1) paddingRef = 0;
            if (paddingCount >= 2) Unsafe.Add(ref paddingRef, 1) = 0;
            if (paddingCount >= 3) Unsafe.Add(ref paddingRef, 2) = 0;
        }
#endif
    }
}