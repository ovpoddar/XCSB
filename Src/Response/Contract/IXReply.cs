namespace Xcsb.Response.Contract;

public interface IXReply
{
    T? GetReply<T>() where T : struct;
}