namespace Xcsb.Connection.Response.Contract;

internal interface IXReply<T, InternalType> : IXBaseResponse<T> where T : struct where InternalType : IVerify
{
    
}