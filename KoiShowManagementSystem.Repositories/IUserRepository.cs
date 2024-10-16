using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IUserRepository 
    {
        Task<UserModel> GetUserByEmail(string email);
        Task AddUser(CreateUserRequest dto);
        Task<ProfileModel> UpdateUser(int userId, EditProfileModel dto);
        Task UpdatePasswordById(int id, string newPassword);
        Task<UserModel> GetUserById(int userId);
        Task<List<UserModel>> GetAllUser();
    }
}
