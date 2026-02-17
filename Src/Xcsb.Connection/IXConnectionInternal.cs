using Xcsb.Connection.Handlers;
using Xcsb.Connection.Models.Handshake;

namespace Xcsb.Connection;

internal interface IXConnectionInternal : IXConnection
{
    ISoccketAccesser Accesser { get; }
    HandshakeStatus HandshakeStatus { get; }

    bool EstablishConnection(ReadOnlySpan<byte> authName, ReadOnlySpan<byte> authData);
    void SetUpStatus();
}
