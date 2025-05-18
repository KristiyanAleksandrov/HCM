using Microsoft.EntityFrameworkCore;
using People.Domain.Entities;

namespace People.Infrastructure.Data
{
    public class PeopleDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public PeopleDbContext(DbContextOptions<PeopleDbContext> opts)
        : base(opts) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasQueryFilter(x => x.IsDeleted == false);
        }
    }
}
