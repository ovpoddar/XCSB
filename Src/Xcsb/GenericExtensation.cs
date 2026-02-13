using Xcsb.Connection;
using Xcsb.Implementation;
using Xcsb.Infrastructure;

namespace Xcsb
{
    public static class GenericExtensation
    {
        public static IXProto Initialized(this IXConnection xConnection)
        {
            if (xConnection.Extensation is not IXExtensationInternal extensationInternal)
                throw new Exception();

            return extensationInternal.GetOrCreate(() => new XProto((IXConnectionInternal)xConnection));
        }
    }
}
