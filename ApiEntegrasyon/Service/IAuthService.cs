namespace ApiEntegrasyon.Service
{
    public interface IAuthService
    {
        Task<bool> Login(string username, string password);
    }
}
