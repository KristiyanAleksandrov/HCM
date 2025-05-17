using Auth.Domain.Entities;

namespace Auth.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user, CancellationToken ct);

        Task SaveChangesAsync(CancellationToken ct);

        Task<bool> ExistsAsync(string userName, CancellationToken ct);

        Task<User?> GetByUserNameAsync(string username, CancellationToken ct);
    }
}
