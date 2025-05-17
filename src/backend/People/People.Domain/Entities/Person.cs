namespace People.Domain.Entities
{
    public class Person
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get;  set; }

        public string Email { get; set; }

        public string Position { get; set; }

        public Person(string firstName, string lastName, string email, string position)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Position = position;
        }
    }
}
