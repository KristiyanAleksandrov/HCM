using Microsoft.EntityFrameworkCore;
using People.Application.Interfaces;
using People.Infrastructure.Data;
using People.Infrastructure.Interceptors;

namespace People.API.Infrastructure.Persistence
{
    public static class PersistenceRegistration
    {
        public static IServiceCollection AddPeopleDb(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<PeopleDbContext>((sp, opts) =>
            {
                var vault = sp.GetRequiredService<IVaultService>();
                var db = vault.GetSecretAsync("hcm/db")
                                 .GetAwaiter().GetResult();

                opts.UseNpgsql(db["PeopleDb"]!.ToString())
                    .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>());
            });

            return services;
        }
    }
}
