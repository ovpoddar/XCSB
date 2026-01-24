using System;
using Xcsb.Extension.Generic.Event.Implementation;
using Xcsb.Extension.Generic.Event.Infrastructure;
using Xcsb.Models.ServerConnection.Contracts;

namespace Xcsb.Extension.Generic.Event
{
    public static class GenericExtensation
    {
        public static IXProto Initialized(this IXConnection xConnection)
        {
            return new XProto((IXConnectionInternal)xConnection);
        }
    }
}
