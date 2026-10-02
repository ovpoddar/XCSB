namespace Xcsb.Connection.Response.Contract;

internal interface IVerify
{
    bool Verify(in int sequence);
}