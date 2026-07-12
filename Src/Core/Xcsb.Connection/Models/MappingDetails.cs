using Xcsb.Connection.Configuration;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Models;

internal record struct MappingDetails
{
    public MappingDetails(XResponseType responseType, XEventType? responseTypeDetails, bool isExtension)
    {
        ResponseType = responseType;
        ResponseTypeDetails = responseTypeDetails;
        IsExtension = isExtension;
    }

    public XResponseType ResponseType { get; }
    public XEventType? ResponseTypeDetails { get; }
    public bool IsExtension { get; }
    public ActionDelegates.ErrorMessageAction? ErrorMessageAction { get; private set; }

    public void SetErrorType<T>() where T : unmanaged, IXError
    {
        ErrorMessageAction = ErrorProcesser.GetErrorMessage<T>;
    }

    public void SetEventType<T>() where T : unmanaged, IXEvent
    {
        ErrorMessageAction = null;
    }
}