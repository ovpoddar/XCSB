using Xcsb.Connection;
using Xcsb.Implementation;
using Xcsb.Infrastructure;

namespace Xcsb
{
    public static class GenericExtensation
    {
        public static IXProto Initialized(this IXConnection xConnection)
        {
            return new XProto((IXConnectionInternal)xConnection);
        }
    }
}
