using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace KoiShowManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly Repository _repository;
        private readonly JwtServices _jwtServices;
        public UserService(JwtServices jwtServices, Repository repository)
        {
            _jwtServices = jwtServices;
            _repository = repository;
        }

        // 1. LOGIN:--------------------------------------
        public async Task<UserModel> Login(LoginModel dto)
        {
            if (dto == null 
                || dto.Email.IsNullOrEmpty() == true 
                || dto.Password.IsNullOrEmpty() == true) 
                throw new Exception("Invalid Login Information");
            var result =  await _repository.Users.GetAccount(dto);
            // Cấp Token:
            result.Token = _jwtServices.GenerateAccessToken(result);
            return result;  
        }

        
        // 2. SIGN UP: ------------------------------------
        public async Task SignUp(SignUpModel dto)
        {
            if (dto == null
                || dto.Email.IsNullOrEmpty() == true
                || dto.Password.IsNullOrEmpty() == true
                || dto.Name.IsNullOrEmpty() == true
                || dto.Phone.IsNullOrEmpty() == true
                )
                throw new Exception("Lack of Signup Information");
             await _repository.Users.AddUser(dto);
        }

        /*
        // 3. GET PROFILE:---------------------------------
        public async Task<object> GetProfile()
        {
            object result = null!;
            // 3.1 Lấy UserID từ Token:
            // Nếu lấy ra không được sẽ quăng exception.
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            // 3.2 Lấy User
            var user = await _unitOfWork.Users.GetById(id);
            if (user != null)
            {
                // 3.3 Lấy Role Title:
                var role = (await _unitOfWork.Roles.GetById(user.RoleId)).Title;
                // 3.4 Trích những thông tin cần lấy:
                result = new
                {
                    user.Name,
                    user.Email,
                    user.Phone,
                    user.Gender,
                    user.DateOfBirth,
                    Role = role,
                };
            }
            return result;
        }


        // 4. UPDATE PROFILE:------------------------------
        public async Task<bool> EditProfile(EditProfileModel dto)
        {
            bool result = false;
            // 4.1 Lấy UserID từ Token:
            // Nếu lấy ra không được sẽ quăng exception.
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            // 4.2 Lấy User:
            var user = await _unitOfWork.Users.GetById(id);
            if (user != null)
            {
                // 4.3 Update:
                user.Name = dto.Name!;
                user.Phone = dto.Phone!;
                user.Gender = dto.Gender!;
                user.DateOfBirth = dto.DateOfBirth!;
                // 4.4 Save Changes:
                await _unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }


        // 5. CHANGE PASSWORD:-----------------------------
        public async Task<bool> ChangePassword(ChangePasswordModel dto)
        {
            bool result = false;
            // 5.1 Lấy UserID từ Token:
            // Nếu lấy ra không được sẽ quăng exception.
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            // 5.2 Lấy User:
            var user = await _unitOfWork.Users.GetById(id);
            if (user != null)
            {
                // 5.3 Check Current Password:
                bool chkPassword = string.Equals(user.Password, dto.CurentPassword, StringComparison.Ordinal);
                if (chkPassword == true)
                {
                    // 5.4 Update:
                    user.Password = dto.NewPassword!;
                    // 5.5 Save changes:
                    await _unitOfWork.SaveAsync();
                    result = true;
                }
            }
            return result;
        }
        */
    }
}
