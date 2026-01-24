using Xcsb.Models.ServerConnection.Handshake;

namespace Xcsb.Models.ServerConnection.Contracts;

public interface IXConnection : IDisposable
{
    HandshakeSuccessResponseBody? HandshakeSuccessResponseBody { get; }
    string FailReason { get; }

    uint NewId();
}
