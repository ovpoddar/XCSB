using Xcsb.Connection.Models;

namespace Xcsb.Connection.Response.Contract;

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