using Xcsb.Connection.Configuration;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Response.Errors;

public readonly struct GenericError
{
    private readonly XResponse _response;
    private readonly ActionDelegates.ErrorMessageAction _message;

    internal GenericError(XResponse response, ActionDelegates.ErrorMessageAction message)
    {
        _response = response;
        _message = message;
    }
    
    public string Message => _message(_response.Bytes);
}