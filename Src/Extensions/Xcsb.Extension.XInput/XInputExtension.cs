using System;
using Xcsb.Connection;
using Xcsb.Extension.XInput.Implementation;
using Xcsb.Extension.XInput.Infrastructure;

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
            using var response = extensionInternal.QueryExtension("XInputExtension"u8);
            if (!response.Reply.Present) return null;
            var reply = response.Reply;
            return extensionInternal.GetOrCreate(() =>
            {
                extensionInternal.ActivateExtension(ExtensionName, reply);
                var result = new XInputProto(reply, extensionInternal);
                using var versionNegotiation = result.GetExtensionVersion("XInputExtension"u8);
                ExtensionMajorVersion = versionNegotiation.Reply.ServerMajor;
                ExtensionMinorVersion = versionNegotiation.Reply.ServerMinor;
                return result;
            });
        }
    }
}