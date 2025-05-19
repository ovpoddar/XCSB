using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Helpers;
internal static class GenericHelper
{
    internal static ref T AsStruct<T>(this Span<byte> @bytes) where T : struct =>
        ref Unsafe.As<byte, T>(ref @bytes[0]);

    internal static T ToStruct<T>(this Span<byte> @bytes) where T : struct =>
        Unsafe.As<byte, T>(ref @bytes[0]);

    internal static int AddPadding(this int pad) =>
        pad + ((4 - (pad & 3)) & 3);

    internal static void SendMust(this Socket socket, Span<byte> buffer, SocketFlags socketFlags = SocketFlags.None)
    {
        while (socket.Connected)
        {
            var totalSend = socket.Send(buffer, socketFlags);
            if (totalSend == buffer.Length)
                break;
        }
    }
}
