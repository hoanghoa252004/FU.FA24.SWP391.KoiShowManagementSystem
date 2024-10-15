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
        Task<UserModel> GetAccount(LoginModel dto);
        Task AddUser(SignUpModel dto);
        Task<ProfileModel> GetProfile(int id);
        Task<ProfileModel> EditProfile(int userId, EditProfileModel dto);
        Task<string> GetPasswordById(int id);
        Task UpdatePasswordById(int id, string newPassword);
        Task<UserModel> GetUserById(int userId);
    }
}
