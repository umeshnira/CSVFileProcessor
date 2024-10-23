using Dapper;
using FileProcessing.Domain.Entities;
using FileProcessing.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Infrastructure.DataAccess.Repositories
{
    public class UserRecordRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRecordRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task AddUserRecordAsync(RegisterModel registerModel)
        {
            var query = "INSERT INTO Users (Username, Password, Email) VALUES (@Username, @Password, @Email)";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { registerModel.Username, registerModel.Email, registerModel.Password });
            }
        }

        public async Task<bool> GetUserRecordAsync(LoginModel loginModel)
        {
            var query = "SELECT * FROM Users WHERE Username = @Username and Password= @Password";
            using (var connection = _context.CreateConnection())
            {
                LoginModel? loginResult = await connection.QuerySingleOrDefaultAsync<LoginModel>(query, new { Username = loginModel.Username, Password= loginModel.Password });
                return loginResult?.Username != null;
            }
        }
    }
}
