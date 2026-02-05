using System;
using Xcsb.Connection;

namespace Xcsb.Extension.Damage
{
    public static class DamageExtensation
    {
        internal const string ExtensationName = "DAMAGE";
        public static IDamageRequest? Damage(this IXExtensation extensation)
        {

            if (extensation is not IXExtensationInternal extensationInternal)
                return null;
            
            var response = extensationInternal.QueryExtension("DAMAGE"u8);
            if (!response.Present) return null;

            lock (ExtensationName)
            {
                return null;
            }
        }
    }
}