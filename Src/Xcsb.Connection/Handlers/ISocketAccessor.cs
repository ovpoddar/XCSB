using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Models;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Handlers;

internal interface ISocketAccessor
{
    int AvailableData { get; }

    ISocketIn SocketIn { get; }
    ISocketOut SocketOut { get; }

    bool PollRead(int timeout = -1);
    bool HasEventToProcesses();
    void WaitForEventArrival();
    void SkipErrorForSequence(int sequence, bool shouldThrow, [CallerMemberName] string name = "");
}