namespace People.Application.RequestModels
{
    public class CreatePersonRequest
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required string Position { get; set; }
    }
}
