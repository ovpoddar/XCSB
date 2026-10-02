namespace Xcsb.Connection.Response.Contract;

internal interface IXError<T> : IVerify, IXBaseResponse<T> where T : struct
{
    string GetErrorMessage();
}