using System;
using Xcsb.Connection;

namespace Xcsb.Extension.XInput
{
    public static class XInputExtension
    {
        internal const string ExtensionName = "XInputExtension";
        internal static uint ExtensionMajorVersion = 2;
        internal static uint ExtensionMinorVersion = 3;             

        public static IXinputRequest? XInput(this IXExtension extension)
        {
            if (extension is not IXExtensionInternal extensionInternal)
                return null;
            var response = extensionInternal.QueryExtension("XInputExtension"u8);
            if (!response.Present) return null;
            
            
            return extensionInternal.GetOrCreate(() =>
            {
                extensionInternal.ActivateExtension(ExtensionName, response);
                var result = new XInputProto(response, extensionInternal);
                var versionNegosiation = result.GetExtensionVersion("XInputExtension"u8);
                ExtensionMajorVersion = versionNegosiation.ServerMajor;
                ExtensionMinorVersion = versionNegosiation.ServerMinor;
                return result;
            });
        }
    }
}