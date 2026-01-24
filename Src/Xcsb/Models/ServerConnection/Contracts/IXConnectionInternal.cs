using Xcsb.Handlers.Direct;
using Xcsb.Models.ServerConnection.Handshake;

namespace Xcsb.Models.ServerConnection.Contracts;

internal interface IXConnectionInternal : IXConnection
{
    ProtoOut ProtoOut { get; }
    ProtoIn ProtoIn { get; }
    HandshakeStatus HandshakeStatus { get; }

    bool EstablishConnection(ReadOnlySpan<byte> authName, ReadOnlySpan<byte> authData);
    void SetUpStatus();
}
