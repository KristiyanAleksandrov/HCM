namespace Auth.Application.RequestModels
{
    public class RegisterRequestModel
    {
        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public required string[] Roles { get; set; }
    }
}
