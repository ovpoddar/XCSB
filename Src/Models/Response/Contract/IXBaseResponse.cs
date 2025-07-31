namespace Xcsb.Models.Response.Contract;

internal interface IXBaseResponse
{
    bool Verify(in int sequence);
}