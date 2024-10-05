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
        Task<ProfileModel> GetProfile();
        Task<ProfileModel> EditProfile(EditProfileModel dto);
        Task<bool> ChangePassword(ChangePasswordModel dto);
    }
}
