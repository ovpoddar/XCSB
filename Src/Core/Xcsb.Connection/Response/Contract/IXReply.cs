namespace Xcsb.Connection.Response.Contract;

internal interface IXReply<T> : IXBaseResponse<T> where T : struct
{
    bool Verify(in int sequence);
}