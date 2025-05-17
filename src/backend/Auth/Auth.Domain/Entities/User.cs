namespace Auth.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }

        public string Username { get; private set; }

        public string Email { get; private set; }

        public string PasswordHash { get; private set; }

        public ICollection<Role> Roles { get; private set; } = new List<Role>();

        private User() { }

        public User(string userName, string email, string password)
        {
            Id = Guid.NewGuid();
            Username = userName;
            Email = email;
            PasswordHash = password;
        }

        public void AddRole(Role role) => Roles.Add(role);
    }
}
