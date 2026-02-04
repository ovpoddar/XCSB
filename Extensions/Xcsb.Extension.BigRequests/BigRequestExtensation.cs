using Xcsb.Connection;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Extension.BigRequests.Requests;
using Xcsb.Extension.BigRequests.Response;

namespace Xcsb.Extension.BigRequests
{

    public static class BigRequestExtensation
    {
        internal const string ExtensationName = "BIG-REQUESTS";
        private static IBigRequest? _request;

        public static IBigRequest? BigRequest(this IXExtensation extensation)
        {
            if (extensation is not IXExtensationInternal extensationInternal)
                return null;

            var response = extensationInternal.QueryExtension("BIG-REQUESTS"u8);
            if (!response.Present) return null;

            lock (ExtensationName)
            {
                if (_request is null)
                    _request = new BigRequestProto(response, extensationInternal);
                return _request;
            }
        }
    }
}