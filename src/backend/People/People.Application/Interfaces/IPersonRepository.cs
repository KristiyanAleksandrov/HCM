using People.Domain.Entities;

namespace People.Application.Interfaces
{
    public interface IPersonRepository
    {
        Task SaveChangesAsync(CancellationToken ct);

        Task AddAsync(Person person, CancellationToken ct);

        void Update(Person person);

        Task DeleteAsync(Person person, CancellationToken ct);

        Task<Person?> GetAsync(Guid id, CancellationToken ct);
    }
}
