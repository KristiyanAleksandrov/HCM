using System.Data;

namespace Auth.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
