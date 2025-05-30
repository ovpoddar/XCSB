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

    internal static T AddPadding<T>(this T pad) where T : INumber<T>
    {
        var value = int.CreateChecked(pad);
        return T.CreateChecked(value + (4 - (value & 3) & 3));
    }

    internal static T Padding<T>(this T pad) where T : INumber<T> =>
        T.CreateChecked(4 - (int.CreateChecked(pad) & 3) & 3);

    internal static void SendExact(this Socket socket, scoped ReadOnlySpan<byte> buffer, SocketFlags socketFlags = SocketFlags.None)
    {
        var total = 0;
        while (socket.Connected)
        {
            total += socket.Send(buffer[total..], socketFlags);
            if (total == buffer.Length)
                break;
        }
    }
#if true
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe static void Send<T>(this Socket socket, scoped ref T value) where T : struct
    {
        var valueAsSpan = new ReadOnlySpan<byte>(
           Unsafe.AsPointer(ref value),
           Unsafe.SizeOf<T>());
        socket.SendExact(valueAsSpan);
    }
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Send<T>(this Socket socket, scoped ref T value) where T : struct =>
        socket.SendExact(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)));
#endif

    internal static void ReceiveExact(this Socket socket, Span<byte> buffer)
    {
        var total = 0;
        while (socket.Connected)
        {
            total += socket.Receive(buffer[total..]);
            if (total == buffer.Length)
                break;
        }
    }
}
