using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Handlers.Direct;
using Xcsb.Helpers;
using Xcsb.Models.Infrastructure;
using Xcsb.Response.Contract;

namespace Xcsb.Models.Handshake;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
internal struct HandshakeResponseHead
{
    [FieldOffset(0)] public HandshakeStatus HandshakeStatus;
    [FieldOffset(0)] public HandshakeResponseHeadSuccess HandshakeResponseHeadSuccess;
    [FieldOffset(0)] public HandshakeResponseHeadFailed HandshakeResponseHeadFailed;
    [FieldOffset(0)] public HandshakeResponseHeadAuthenticate HandshakeResponseHeadAuthenticate;
}