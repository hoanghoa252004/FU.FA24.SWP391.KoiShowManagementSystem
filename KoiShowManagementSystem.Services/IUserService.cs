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
        Task<object> Login(LoginModel dto);
        Task<bool> SignUp(SignUpModel dto);
        Task<object> GetProfile();
        Task<bool> EditProfile(EditProfileModel dto);
        Task<bool> ChangePassword(ChangePasswordModel dto);
    }
}
