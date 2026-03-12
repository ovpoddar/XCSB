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

    public readonly Span<byte> GetRawResponse() =>
        _response.Bytes;
    
    internal readonly XResponse GetResponse() => _response;
}