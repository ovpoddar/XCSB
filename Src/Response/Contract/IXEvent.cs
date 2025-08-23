namespace Xcsb.Response.Contract;

public interface IXEvent
{
    T? GetEvent<T>() where T : struct;
}