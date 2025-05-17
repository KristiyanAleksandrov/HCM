namespace People.Application.RequestModels
{
    public class UpdatePersonRequest
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Position { get; set; }
    }
}
