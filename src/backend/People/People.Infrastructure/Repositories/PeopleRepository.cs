using Microsoft.EntityFrameworkCore;
using People.Application.Interfaces;
using People.Domain.Entities;
using People.Infrastructure.Data;

namespace People.Infrastructure.Repositories
{
    public class PeopleRepository : IPeopleRepository
    {
        private readonly PeopleDbContext dbContext;

        public PeopleRepository(PeopleDbContext dbContext)
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

        public async Task<IEnumerable<Person>> GetAllAsync(CancellationToken ct)
        {
            return await dbContext.People.ToListAsync(ct);
        }

        public void Delete(Person p)
        {
            dbContext.People.Remove(p);
        }
    }
}
