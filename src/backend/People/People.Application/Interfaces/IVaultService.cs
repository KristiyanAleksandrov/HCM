namespace People.Application.Interfaces
{
    public interface IVaultService
    {
        Task<IDictionary<string, object>> GetSecretAsync(string path);
    }
}
