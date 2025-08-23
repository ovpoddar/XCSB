namespace Xcsb.Response.Contract;

internal interface IXBaseResponse
{
    bool Verify(in int sequence);
    bool IsEvent();
    bool IsError();
    bool IsReply();
    T? GetReply<T>() where T : struct;
    T? GetError<T>() where T : struct;
    T? GetEvent<T>() where T : struct;
}