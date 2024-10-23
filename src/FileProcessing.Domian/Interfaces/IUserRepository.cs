using FileProcessing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserRecordAsync(RegisterModel registerModel);
        Task<bool> GetUserRecordAsync(LoginModel loginModel);
    }
}
