using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Models;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;

namespace Xcsb.Response.Event;

public struct GenericEvent
{
    private readonly XResponse _response;
    private readonly XEventType _eventType;

    internal GenericEvent(XResponse response, XEventType eventType)
    {
        _response = response;
        _eventType = eventType;
    }

}