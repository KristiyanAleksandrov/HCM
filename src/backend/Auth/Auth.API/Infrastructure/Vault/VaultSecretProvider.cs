using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace Auth.API.Infrastructure.Vault
{
    public class VaultSecretProvider
    {
        private readonly IVaultClient client;

        //TODO: Think how to extract common things
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
