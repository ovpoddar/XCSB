using System;
using System.Collections.Generic;
using System.Text;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Models;

internal record struct MappingDetails
{
    public MappingDetails(XResponseType responseType, XEventType? responseTypeDetails)
    {
        ResponseType = responseType;
        ResponseTypeDetails = responseTypeDetails;
    }

    // public Type Type { get; private set; }
    public XResponseType ResponseType { get; }
    public XEventType? ResponseTypeDetails { get; }
    public ActionDelegates.ErrorMessageAction? ErrorMessageAction { get; private set; }

    public void SetErrorType<T>() where T : unmanaged, IXError
    {
        // Type = typeof(T);
        ErrorMessageAction = ErrorProcesser.GetErrorMessage<T>;
    }
    
    public void SetEventType<T>() where T : unmanaged, IXEvent
    {
        // Type = typeof(T);
        ErrorMessageAction = null;
    }
}