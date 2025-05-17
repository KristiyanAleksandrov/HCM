using Auth.Domain.Entities;

namespace Auth.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> RequireByNameAsync(string roleName, CancellationToken ct);

        Task<IEnumerable<Role>> ListAsync(CancellationToken ct);
    }
}
