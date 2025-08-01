namespace Xcsb.Response.Contract;

internal interface IXBaseResponse
{
    bool Verify(in int sequence);
}