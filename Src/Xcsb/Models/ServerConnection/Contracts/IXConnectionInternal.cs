using Xcsb.Handlers;
using Xcsb.Models.ServerConnection.Handshake;

namespace Xcsb.Models.ServerConnection.Contracts;

internal interface IXConnectionInternal : IXConnection
{
    SoccketAccesser Accesser { get; }
    HandshakeStatus HandshakeStatus { get; }

    bool EstablishConnection(ReadOnlySpan<byte> authName, ReadOnlySpan<byte> authData);
    void SetUpStatus();
}
