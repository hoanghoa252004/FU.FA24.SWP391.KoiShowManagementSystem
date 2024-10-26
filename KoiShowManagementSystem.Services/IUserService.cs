using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IUserService
    {
        Task<UserModel> Login(LoginModel dto);
        Task SignUp(SignUpModel dto);
        Task<UserModel> GetUser();
        Task<ProfileModel> EditProfile(EditProfileModel dto);
        Task<bool> ChangePassword(ChangePasswordModel dto);
        Task CreateUser(CreateUserRequest user);
        Task DeleteUser(int userId);
        Task<List<UserModel>> GetAllUser(int pageIndex, int pageSize, string? role);
    }
}
