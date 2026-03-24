using Xcsb.Connection;
using Xcsb.Implementation;
using Xcsb.Infrastructure;

namespace Xcsb;

public static class GenericExtension
{
    public static IXProto Initialized(this IXConnection xConnection)
    {
        return xConnection.Extension is not IXExtensionInternal extensionInternal
            ? throw new Exception()
            : extensionInternal.GetOrCreate(() => new XProto((IXConnectionInternal)xConnection));
    }
}