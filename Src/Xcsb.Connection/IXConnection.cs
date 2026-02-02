using Xcsb.Connection.Models.Handshake;

namespace Xcsb.Connection;

public interface IXConnection : IDisposable
{
    HandshakeSuccessResponseBody? HandshakeSuccessResponseBody { get; }
    IXExtensation Extensation { get; }
    string FailReason { get; }

    uint NewId();
}
