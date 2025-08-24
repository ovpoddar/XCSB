namespace Xcsb.Response.Contract;

public interface IXBaseResponse
{
    bool Verify(in int sequence);
}