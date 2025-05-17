using Auth.Application.Errors;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext dbContext;

        public RoleRepository(AuthDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Role> RequireByNameAsync(string roleName, CancellationToken ct)
        {
            var role = await dbContext.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName, ct);

            if (role == null)
            {
                throw new NotFoundException($"Role '{roleName}' not found.");
            }

            return role;
        }

        public async Task<IEnumerable<Role>> ListAsync(CancellationToken ct)
        {
            return await dbContext.Roles
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
