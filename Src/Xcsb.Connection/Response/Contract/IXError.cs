namespace Xcsb.Connection.Response.Contract;

internal interface IXError : IXBaseResponse
{
    string GetErrorMessage();
}