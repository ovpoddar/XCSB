namespace Xcsb.Response.Contract;

public interface IXError
{
    T? GetError<T>() where T : struct;
}