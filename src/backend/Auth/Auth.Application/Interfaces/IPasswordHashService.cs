namespace Auth.Application.Interfaces
{
    public interface IPasswordHashService
    {
        string Hash(string plaintext);

        bool Verify(string plaintext, string hash);
    }
}
