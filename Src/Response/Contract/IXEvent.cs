namespace Xcsb.Response.Contract;

public interface IXEvent
{
    bool Verify(in int sequence);
}