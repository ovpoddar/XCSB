using Xcsb.Connection.Handlers;
using Xcsb.Connection.Response.Replies;

namespace Xcsb.Connection;

internal interface IXExtensationInternal : IXExtensation
{
    ISocketAccessor Transport { get; }

    void ActivateExtensation(ReadOnlySpan<char> name, QueryExtensionReply reply);
    bool IsExtensationEnable(string name);

    T GetOrCreate<T>(Func<T> factory) where T : class;
    void Clear();
}
