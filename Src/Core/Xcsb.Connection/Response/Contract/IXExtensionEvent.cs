namespace Xcsb.Connection.Response.Contract;

internal interface IXExtensionEvent : IXEvent
{
    T Create<T>(Span<byte> data) where T : struct;
}