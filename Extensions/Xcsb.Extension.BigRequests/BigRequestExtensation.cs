using Xcsb.Connection;

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
            return !response.Present 
                ? null 
                : extensationInternal.GetOrCreate(() => new BigRequestProto(response, extensationInternal));
        }
    }
}