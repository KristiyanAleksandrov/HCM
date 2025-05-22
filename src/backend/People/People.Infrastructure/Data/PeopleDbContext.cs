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
            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Position).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Person>().HasQueryFilter(x => x.IsDeleted == false);
        }
    }
}
