using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Helpers;
internal static class GenericHelper
{
    internal static ref T AsStruct<T>(this Span<byte> @bytes) where T : struct =>
        ref Unsafe.As<byte, T>(ref @bytes[0]);

    internal static T ToStruct<T>(this Span<byte> @bytes) where T : struct =>
        Unsafe.As<byte, T>(ref @bytes[0]);

    internal static T AddPadding<T>(this T pad) where T : INumber<T>
    {
        var value = int.CreateChecked(pad);
        return T.CreateChecked(value + ((4 - (value & 3)) & 3));
    }

    internal static void SendExact(this Socket socket, Span<byte> buffer, SocketFlags socketFlags = SocketFlags.None)
    {
        while (socket.Connected)
        {
            var totalSend = socket.Send(buffer, socketFlags);
            if (totalSend == buffer.Length)
                break;
        }
    }

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
