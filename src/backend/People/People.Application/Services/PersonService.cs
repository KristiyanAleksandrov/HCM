using People.Application.Errors;
using People.Application.Interfaces;
using People.Application.RequestModels;
using People.Domain.Entities;

namespace People.Application.Services
{
    public class PersonService
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
    }
}
