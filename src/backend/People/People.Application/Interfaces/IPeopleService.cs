using People.Application.RequestModels;
using People.Application.ResponseModels;

namespace People.Application.Interfaces
{
    public interface IPeopleService
    {
        Task<Guid> AddAsync(CreatePersonRequest req, CancellationToken ct);
        Task UpdateAsync(Guid id, UpdatePersonRequest req, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task<IEnumerable<PersonResponse>> GetAllAsync(CancellationToken ct);
        Task<PersonResponse> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
