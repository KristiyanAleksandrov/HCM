using People.Application.Errors;
using People.Application.Interfaces;
using People.Application.RequestModels;
using People.Application.ResponseModels;
using People.Domain.Entities;

namespace People.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository personRepository;

        public PersonService(IPersonRepository personRepository)
        {
            this.personRepository = personRepository;
        }

        public async Task<Guid> AddAsync(CreatePersonRequest req, CancellationToken ct)
        {
            var person = new Person(req.FirstName, req.LastName, req.Email, req.Position);

            await personRepository.AddAsync(person, ct);
            await personRepository.SaveChangesAsync(ct);
            return person.Id;
        }

        public async Task UpdateAsync(Guid id, UpdatePersonRequest req, CancellationToken ct)
        {
            var person = await personRepository.GetAsync(id, ct);

            if (person == null)
            {
                throw new NotFoundException("Person not found");
            }

            if (!string.IsNullOrWhiteSpace(req.FirstName))
                person.FirstName = req.FirstName;

            if (!string.IsNullOrWhiteSpace(req.LastName))
                person.LastName = req.LastName;

            if (!string.IsNullOrWhiteSpace(req.Position))
                person.Position = req.Position;

            if (!string.IsNullOrWhiteSpace(req.Email))
                person.Email = req.Email;

            personRepository.Update(person);
            await personRepository.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var person = await personRepository.GetAsync(id, ct);

            if (person == null)
            {
                throw new NotFoundException("Person not found");
            }

            personRepository.Delete(person);
            await personRepository.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<PersonResponse>> GetAllAsync(CancellationToken ct)
        {
            var people = await personRepository.GetAllAsync(ct);

            return people.Select(MapToResponse);
        }

        public async Task<PersonResponse?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var person = await personRepository.GetAsync(id, ct);

            if (person == null)
            {
                throw new NotFoundException("Person not found");
            }

            return MapToResponse(person);
        }

        private PersonResponse MapToResponse(Person p)
        {
            return new PersonResponse
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Position = p.Position
            };
        }
    }
}
