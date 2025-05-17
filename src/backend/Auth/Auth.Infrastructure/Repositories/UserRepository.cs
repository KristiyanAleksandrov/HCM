using Auth.Domain.Entities;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext dbContext;

        public UserRepository(AuthDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(User user, CancellationToken ct)
        {
            await dbContext.Users.AddAsync(user, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(string userName, CancellationToken ct)
        {
            return await dbContext.Users.AsNoTracking().AnyAsync(u => u.Username == userName, ct);
        }

        public async Task<User?> GetByUserNameAsync(string username, CancellationToken ct)
        => await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username, ct);
    }
}
