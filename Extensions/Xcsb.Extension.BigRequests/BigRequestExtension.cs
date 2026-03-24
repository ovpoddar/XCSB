using Xcsb.Connection;

namespace Xcsb.Extension.BigRequests
{

    public static class BigRequestExtension
    {
        public const string ExtensionName = "BIG-REQUESTS";
        public static IBigRequest? BigRequest(this IXExtension extension)
        {
            if (extension is not IXExtensionInternal extensionInternal)
                return null;

            var response = extensionInternal.QueryExtension("BIG-REQUESTS"u8);
            return !response.Present
                ? null
                : extensionInternal.GetOrCreate(() => new BigRequestProto(response, extensionInternal));
        }
    }
}