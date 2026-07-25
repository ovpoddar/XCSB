namespace Xcsb.Connection.Response.Contract;

internal interface IXExtensionEvent<out T> : IXEvent where T : struct
{
    T Create(Span<byte> data);
}