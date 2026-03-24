namespace Xcsb.Connection.Response.Contract;

internal interface IXBaseResponse
{
    bool Verify(in int sequence);
}
