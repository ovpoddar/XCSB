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
    void RegisterEvent<T>(XEventType type, byte? typeValue = null) where T : unmanaged, IXEvent;
    void RegisterError<T>(byte typeValue, XEventType type) where T : unmanaged, IXError;
}
