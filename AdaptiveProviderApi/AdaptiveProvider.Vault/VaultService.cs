using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AdaptiveProvider.Vault
{
    public class VaultService
    {
        private readonly string _storeServiceName;

        public VaultService()
        {

        }

        public VaultService(string connectionString)
        {
            this._storeServiceName = connectionString;
        }

        //public Dictionary<string, object> RequiredServices { get; set; }

        //[JsonIgnore]
        public Dictionary<string, string> ConfigurationVariables { get; set; }

        public string GetSecret(string token)
        {
            //todo: read the configuration variables from a VariablesService
            //if (!RequiredServices.ContainsKey(_storeServiceName))
            //{
            //    throw new ArgumentException($"Service {_storeServiceName} is not regitered to the Vault Service");
            //}

            //dynamic service = RequiredServices[_storeServiceName];

            if (!ConfigurationVariables.ContainsKey(token))
            {
                return null;
            }            

            return Decrypt(ConfigurationVariables[token]);
        }

        public string Encrypt(string secret)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(secret), null, DataProtectionScope.CurrentUser));
        }

        public string Decrypt(string cypher)
        {
            return Encoding.Unicode.GetString(
                ProtectedData.Unprotect(
                     Convert.FromBase64String(cypher), null, DataProtectionScope.CurrentUser));
        }
    }
}
