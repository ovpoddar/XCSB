namespace Xcsb.Response.Contract;

public interface IXReply
{
    bool Verify(in int sequence);
}