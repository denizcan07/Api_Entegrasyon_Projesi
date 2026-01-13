using ApiEntegrasyon.Entity;

namespace ApiEntegrasyon.Repository
{
    public interface IUsersRepository
    {
        Task<Users?> GetByUsernameAndPassword(string username, string passwordHash);
    }
}
