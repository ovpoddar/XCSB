using System;
using Xcsb.Connection;

namespace Xcsb.SockAccesser
{
    public static class XConnectionExtensation
    {
        public static void SendData(this IXConnection connection, byte[] data)
        {
            if (connection is IXConnectionInternal connectionInternals)
                connectionInternals.Accessor.SendData(data.AsSpan(), System.Net.Sockets.SocketFlags.None);
        }

        public static void SendRequest(this IXConnection connection, byte[] request)
        {
            if (connection is IXConnectionInternal connectionInternals)
                connectionInternals.Accessor.SendRequest(request.AsSpan(), System.Net.Sockets.SocketFlags.None);
        }

        public static int GetSendRequestSequence(this IXConnection connection)
        {
            if (connection is IXConnectionInternal connectionInternals)
                return connectionInternals.Accessor.SendSequence;
            throw new NotSupportedException();
        }

        public static int GetReceivedRequestSequence(this IXConnection connection)
        {
            if (connection is IXConnectionInternal connectionInternals)
                return connectionInternals.Accessor.ReceivedSequence;
            throw new NotSupportedException();
        }
    }
}
