namespace Xcsb.Connection.Response.Contract;

internal interface IXError<T> : IXBaseResponse<T> where T : struct
{
    string GetErrorMessage();
    bool Verify(in int sequence);
}