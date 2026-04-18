using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Models;

public readonly struct XEvent
{
    private readonly XResponse _response;
    private readonly MappingDetails _mappingDetails;

    internal XEvent(XResponse response, MappingDetails mappingDetails)
    {
        _response = response;
        _mappingDetails = mappingDetails;
    }

    public readonly XEventType ReplyType => _mappingDetails.ResponseTypeDetails!;

    public readonly unsafe ref readonly T As<T>() where T : struct =>
        ref _response.Bytes.AsStruct<T>();

    public readonly GenericError? Error =>
        _mappingDetails.ResponseType != XResponseType.Error || _mappingDetails.ErrorMessageAction is null
            ? null
            : new GenericError(_response, _mappingDetails.ErrorMessageAction);

    public readonly GenericEvent? Event =>
        _mappingDetails.ResponseType is XResponseType.Event or XResponseType.Notify
            ? new GenericEvent(_response, _mappingDetails.ResponseTypeDetails!)
            : null;

    public Span<byte> GetRawResponse() =>
        _response.Bytes;

}