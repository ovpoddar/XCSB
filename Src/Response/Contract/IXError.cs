namespace Xcsb.Response.Contract;

public interface IXError
{
    bool Verify(in int sequence);
}