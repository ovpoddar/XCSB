using Xcsb.Connection.Handlers;
using Xcsb.Connection.Response.Replies;

namespace Xcsb.Connection;

internal interface IXExtensionInternal : IXExtension
{
    ISocketAccessor Transport { get; }

    void ActivateExtension(ReadOnlySpan<char> name, QueryExtensionReply reply);
    bool IsExtensionEnable(string name);

    T GetOrCreate<T>(Func<T> factory) where T : class;
    void Clear();
}
