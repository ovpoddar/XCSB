using Xcsb.Connection.Models.Handshake;

namespace Xcsb.Connection;

public interface IXConnection : IDisposable
{
    HandshakeSuccessResponseBody? HandshakeSuccessResponseBody { get; }
    string FailReason { get; }

    uint NewId();
}
