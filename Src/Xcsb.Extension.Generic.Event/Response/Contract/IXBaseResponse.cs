namespace Xcsb.Extension.Generic.Event.Response.Contract;

internal interface IXBaseResponse
{
    bool Verify(in int sequence);
}
