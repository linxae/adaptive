using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AdaptiveProvider.Core.Configuration
{
    public static class ConfigurationExtensions
    {
        public static string Encrypt(this string text)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(text), null, DataProtectionScope.CurrentUser));
        }

        public static string Decrypt(this string text)
        {
            return Encoding.Unicode.GetString(
                ProtectedData.Unprotect(
                     Convert.FromBase64String(text), null, DataProtectionScope.CurrentUser));
        }
    }
}
