using People.Application.Errors;
using People.Application.Interfaces;
using People.Application.RequestModels;
using People.Application.ResponseModels;
using People.Domain.Entities;
using System.Net;

namespace People.Application.Services
{
    public class PeopleService : IPeopleService
    {
        private readonly IPeopleRepository personRepository;

        public PeopleService(IPeopleRepository personRepository)
        {
            this.personRepository = personRepository;
        }

        public async Task<Guid> AddAsync(CreatePersonRequest req, CancellationToken ct)
        {
            await ValidatePersonInput(req.FirstName, req.LastName, req.Email, req.Position, ct);

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
            {
                if (req.FirstName.Length > 100)
                    throw new BadRequestException("First name must be under 100 characters.");

                person.FirstName = req.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(req.LastName))
            {
                if (req.LastName.Length > 100)
                    throw new BadRequestException("Last name must be under 100 characters.");

                person.LastName = req.LastName;
            }

            if (!string.IsNullOrWhiteSpace(req.Position))
            {
                if (req.Position.Length > 100)
                    throw new BadRequestException("Position must be under 100 characters.");

                person.Position = req.Position;
            }

            if (!string.IsNullOrWhiteSpace(req.Email))
            {
                if (person.Email != req.Email)
                {
                    if (req.Email.Length > 255)
                    {
                        throw new BadRequestException("Email must be under 255 characters.");
                    }
                    if (await personRepository.ExistsAsync(req.Email, ct))
                    {
                        throw new ConflictException("Person with this email already exists");
                    }
                    person.Email = req.Email;
                }
            }

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

        public async Task<PersonResponse> GetByIdAsync(Guid id, CancellationToken ct)
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

        private async Task ValidatePersonInput(string firstName, string lastName, string email, string position, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new BadRequestException("First name is required.");

            if (firstName.Length > 100)
                throw new BadRequestException("First name must be under 100 characters.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new BadRequestException("Last name is required.");

            if (lastName.Length > 100)
                throw new BadRequestException("Last name must be under 100 characters.");

            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Email is required.");

            if (email.Length > 255)
                throw new BadRequestException("Email must be under 255 characters.");

            if (string.IsNullOrWhiteSpace(position))
                throw new BadRequestException("Position is required.");

            if (position.Length > 100)
                throw new BadRequestException("Position must be under 100 characters.");

            if (await personRepository.ExistsAsync(email, ct))
            {
                throw new ConflictException("Person with this email already exists");
            }
        }
    }
}
