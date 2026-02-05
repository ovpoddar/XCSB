using Xcsb.Connection;
using Xcsb.Implementation;
using Xcsb.Infrastructure;

namespace Xcsb
{
    public static class GenericExtensation
    {
        private static IXProto? _globalProto;
        private static readonly object _lock = new();
        public static IXProto Initialized(this IXConnection xConnection)
        {
            lock (_lock)
            {
                _globalProto ??= new XProto((IXConnectionInternal)xConnection);
                return _globalProto;
            }
        }
    }
}
