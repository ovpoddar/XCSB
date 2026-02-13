using Xcsb.Connection;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Extension.BigRequests.Requests;
using Xcsb.Extension.BigRequests.Response;

namespace Xcsb.Extension.BigRequests
{

    public static class BigRequestExtensation
    {
        public const string ExtensationName = "BIG-REQUESTS";
        public static IBigRequest? BigRequest(this IXExtensation extensation)
        {
            if (extensation is not IXExtensationInternal extensationInternal)
                return null;

            var response = extensationInternal.QueryExtension("BIG-REQUESTS"u8);
            if (!response.Present) return null;
            return extensationInternal.GetOrCreate(() => new BigRequestProto(response, extensationInternal));
        }
    }
}