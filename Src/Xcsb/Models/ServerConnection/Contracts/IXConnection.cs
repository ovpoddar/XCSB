using System;
using System.Collections.Generic;
using System.Text;
using Xcsb.Models.ServerConnection.Handshake;

namespace Xcsb.Models.ServerConnection.Contracts;

public interface IXConnection : IDisposable
{
    HandshakeSuccessResponseBody? HandshakeSuccessResponseBody { get; }
    string FailReason { get; }

    bool IsEventAvailable();
    void WaitForEvent();
    uint NewId();
}
