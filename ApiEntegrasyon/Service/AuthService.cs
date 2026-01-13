using ApiEntegrasyon.Repository;
using System.Threading.Tasks;

namespace ApiEntegrasyon.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUsersRepository _usersRepository;

        public AuthService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await _usersRepository.GetByUsernameAndPassword(username, password);
            return user != null;
        }

    }
}
