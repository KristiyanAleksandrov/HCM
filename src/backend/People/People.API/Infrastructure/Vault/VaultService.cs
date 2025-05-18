using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace People.API.Infrastructure.Vault
{
    public class VaultService
    {
        private readonly IVaultClient _client;

        public VaultService()
        {
            //Not good for production
            var vaultUri = "http://vault:8200";
            var token = "root";

            var authMethod = new TokenAuthMethodInfo(token);
            var settings = new VaultClientSettings(vaultUri, authMethod);
            _client = new VaultClient(settings);
        }

        public async Task<IDictionary<string, object>> GetSecretAsync(string path)
        {
            var result = await _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path, null, "secret");

            return result.Data.Data;
        }
    }
}
