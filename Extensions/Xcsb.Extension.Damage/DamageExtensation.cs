using System;
using Xcsb.Connection;

namespace Xcsb.Extension.Damage
{
    public static class DamageExtensation
    {
        internal const string ExtensationName = "DAMAGE";
        internal static uint ExtensationMajorVersion = 1;
        internal static uint ExtensationMinorVersion = 1;

        public static IDamageRequest? Damage(this IXExtensation extensation)
        {

            if (extensation is not IXExtensationInternal extensationInternal)
                return null;

            var response = extensationInternal.QueryExtension("DAMAGE"u8);
            if (!response.Present) return null;

            return extensationInternal.GetOrCreate(() =>
            {
                extensationInternal.ActivateExtensation(ExtensationName, response, 1, 1);
                var request = new DamageRequestProto(response, extensationInternal);
                var versionNegostion = request.QueryVersion(ExtensationMajorVersion, ExtensationMinorVersion);
                ExtensationMajorVersion = versionNegostion.MajorVersion;
                ExtensationMinorVersion = versionNegostion.MinorVersion;
                return request;
            });
        }
    }
}