using People.Domain.Entities;

namespace People.Application.Interfaces
{
    public interface IPeopleRepository
    {
        Task SaveChangesAsync(CancellationToken ct);

        Task AddAsync(Person person, CancellationToken ct);

        void Update(Person person);

        void Delete(Person person);

        Task<Person?> GetAsync(Guid id, CancellationToken ct);

        Task<IEnumerable<Person>> GetAllAsync(CancellationToken ct);

        Task<bool> ExistsAsync(string email, CancellationToken ct);
    }
}
