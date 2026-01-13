using Dapper;
using System.Data;
using ApiEntegrasyon.Entity;

namespace ApiEntegrasyon.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IDbConnection _context;

        public UsersRepository(IDbConnection context)
        {
            _context = context;
        }

        public async Task<Users> GetByUsernameAndPassword(string username, string passwordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Username", username, DbType.String);      
            parameters.Add("PasswordHash", passwordHash, DbType.String);

            return await _context.QueryFirstOrDefaultAsync<Users>(
                "sp_user_get_by_username",
                parameters,
                commandType: CommandType.StoredProcedure
            );

        }
    }
}
