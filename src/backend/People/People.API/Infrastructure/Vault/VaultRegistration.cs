using People.Application.Interfaces;

namespace People.API.Infrastructure.Vault
{
    public static class VaultRegistration
    {
        public static IServiceCollection AddVault(this IServiceCollection services)
        {
            services.AddSingleton<IVaultService, VaultService>();
            return services;
        }
    }
}
