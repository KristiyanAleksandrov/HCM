using People.Application.Interfaces;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace People.API.Infrastructure.Vault
{
    public class VaultSecretProvider
    {
        private readonly IVaultClient client;
        public VaultSecretProvider(string vaultUri, string vaultToken)
        {
            var authMethod = new TokenAuthMethodInfo(vaultToken);
            var settings = new VaultClientSettings(vaultUri, authMethod);
            client = new VaultClient(settings);
        }

        public async Task<IDictionary<string, object>> GetSecretAsync(string path)
        {
            var result = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path, null, "secret");

            return result.Data.Data;
        }
    }
}
