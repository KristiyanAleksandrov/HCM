using Auth.Application.Common;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Data
{
    public class AuthDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> opts)
            : base(opts) { }


        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>(x =>
            {
                x.HasIndex(u => u.Username).IsUnique();
            });

            b.Entity<Role>().HasData(
              new Role { Id = -1, Name = RoleNames.Employee },
              new Role { Id = -2, Name = RoleNames.Manager },
              new Role { Id = -3, Name = RoleNames.HRAdmin });
        }
    }
}
