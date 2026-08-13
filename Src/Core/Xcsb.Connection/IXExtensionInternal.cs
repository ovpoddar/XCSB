using Xcsb.Connection.Handlers;
using Xcsb.Connection.Models;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Replies;

namespace Xcsb.Connection;

internal interface IXExtensionInternal : IXExtension
{
    ISocketAccessor Transport { get; }

    void ActivateExtension(ReadOnlySpan<char> name, QueryExtensionReply reply);
    bool IsExtensionEnable(string name);

    T GetOrCreate<T>(Func<T> factory) where T : class;
    void Clear();

    void RegisterReply();
    void RegisterX1Event<T>(XEventType type, string extensionName = "") where T : unmanaged, IXEvent<T>;
    void RegisterX2Event<T>(XEventType type, string extensionName) where T : struct, IXEvent<T>;
    void RegisterError<T>(byte typeValue, XEventType type) where T : unmanaged, IXError;
}