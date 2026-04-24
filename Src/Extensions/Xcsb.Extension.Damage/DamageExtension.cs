using Xcsb.Connection;
using Xcsb.Extension.Damage.Infrastructure;

namespace Xcsb.Extension.Damage
{
    public static class DamageExtension
    {
        internal const string ExtensionName = "DAMAGE";
        internal static uint ExtensionMajorVersion = 1;
        internal static uint ExtensionMinorVersion = 1;

        public static IDamageRequest? Damage(this IXExtension extension)
        {
            if (extension is not IXExtensionInternal extensionInternal)
                return null;

            var response = extensionInternal.QueryExtension("DAMAGE"u8);
            if (!response.Present) return null;

            return extensionInternal.GetOrCreate(() =>
            {
                extensionInternal.ActivateExtension(ExtensionName, response);
                var request = new DamageProto(response, extensionInternal);
                var versionNegostion = request.QueryVersion(ExtensionMajorVersion, ExtensionMinorVersion);
                ExtensionMajorVersion = versionNegostion.MajorVersion;
                ExtensionMinorVersion = versionNegostion.MinorVersion;
                return request;
            });
        }
    }
}