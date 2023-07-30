using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Configuration;

namespace DataAccessLayer
{
    public partial class ServicesAPIContext
    {
        private static bool IsInitialized;
        private static ClientCredential _ClientCredential;
        private static string _clientId;
        private static string _clientSecret;

        public ServicesAPIContext(DbContextOptions<ServicesAPIContext> options, AzureIdentity identity)
        : base(options)
        {
            _clientId = identity.ClientId;
            _clientSecret = identity.ClientSecret;

            if (!IsInitialized)
                { IsInitialized = true; InitializeAzureKeyVaultProvider(); }
        }


        private static void InitializeAzureKeyVaultProvider()
        {
            _ClientCredential = new ClientCredential(_clientId, _clientSecret);

            SqlColumnEncryptionAzureKeyVaultProvider azureKeyVaultProvider =
              new SqlColumnEncryptionAzureKeyVaultProvider(GetToken);

            Dictionary<string, SqlColumnEncryptionKeyStoreProvider> providers =
              new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>();

            providers.Add(SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureKeyVaultProvider);
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);
        }

        private static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, _ClientCredential);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the access token");
            return result.AccessToken;
        }
    }
}
