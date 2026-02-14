using Xcsb.Connection;
using Xcsb.Implementation;
using Xcsb.Infrastructure;

namespace Xcsb;

public static class GenericExtensation
{
    public static IXProto Initialized(this IXConnection xConnection)
    {
        return xConnection.Extensation is not IXExtensationInternal extensationInternal 
            ? throw new Exception() 
            : extensationInternal.GetOrCreate(() => new XProto((IXConnectionInternal)xConnection));
    }
}