using System;
using System.Net;
using AdaptiveProvider.Core.Configuration;

namespace AdaptiveProvider.TowerAnsible
{
    public static class ConnectionStringParser
    {
        private static readonly char[] ConnectionStringSeparators = { '>', '#', '@', '!' };

        private static string GetPart(this string connectionString, int index)
        {
            //todo: validate using regex

            var parts = (connectionString ?? string.Empty).Split(ConnectionStringSeparators);

            if (parts.Length != 5)
            {
                throw new ArgumentException("TowerAnsible API incorrect connection string format");
            }

            return parts[index];
        }


        public static AuthenticationMethod AuthenticationMethod(this string cs)
        {
            return TowerAnsible.AuthenticationMethod.Basic;
        }

        public static NetworkCredential Credential(this string cs)
        {
            return new NetworkCredential(cs.GetPart(1), cs.GetPart(2).Decrypt());
        }

        public static string Url(this string cs)
        {
            return cs.GetPart(3);
        }

        public static bool SkipCertificateCheck(this string cs)
        {
            return string.Compare("y", cs.GetPart(4), ignoreCase:true) == 0;
        }
    }
}
