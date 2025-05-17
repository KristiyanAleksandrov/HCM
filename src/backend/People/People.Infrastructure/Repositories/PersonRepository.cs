using Microsoft.EntityFrameworkCore;
using People.Application.Interfaces;
using People.Domain.Entities;
using People.Infrastructure.Data;

namespace People.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly PeopleDbContext dbContext;

        public PersonRepository(PeopleDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await dbContext.SaveChangesAsync(ct);
        }

        public async Task AddAsync(Person p, CancellationToken ct)
        {
            await dbContext.People.AddAsync(p, ct);
        }

        public void Update(Person p)
        {
             dbContext.People.Update(p);
        }

        public async Task<Person?> GetAsync(Guid id, CancellationToken ct)
        {
           return await dbContext.People.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task DeleteAsync(Person p, CancellationToken ct)
        {
            dbContext.People.Remove(p);
            return Task.CompletedTask;
        }
    }
}
