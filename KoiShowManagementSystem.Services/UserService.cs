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
                throw new Exception("Failed: Invalid Login Information");
            UserModel result = await _repository.Users.GetAccount(dto);
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
                || dto.DateOfBirth.HasValue == false
                || dto.Gender.HasValue == false
                )
                throw new Exception("Failed: Lack of Information");

             await _repository.Users.AddUser(dto);
        }

        
        // 3. GET PROFILE:---------------------------------
        public async Task<ProfileModel> GetProfile()
        {
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            return await _repository.Users.GetProfile(id);
        }

        
        // 4. UPDATE PROFILE:------------------------------
        public async Task<ProfileModel> EditProfile(EditProfileModel dto)
        {
            if (dto == null)
                throw new Exception("Failed: No thing to update.");

            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            ProfileModel result = await _repository.Users.EditProfile(id, dto);
            return result;
        }

        
        // 5. CHANGE PASSWORD:-----------------------------
        public async Task<bool> ChangePassword(ChangePasswordModel dto)
        {
            bool result = false;
            if (dto != null
                && !dto.NewPassword.IsNullOrEmpty()
                && !dto.CurentPassword.IsNullOrEmpty())
            {
                int id = _jwtServices.GetIdAndRoleFromToken().userId;
                string currentPassword = await _repository.Users.GetPasswordById(id);
                if (currentPassword != null && currentPassword.Equals(dto.CurentPassword))
                {
                    await _repository.Users.UpdatePasswordById(id, dto.NewPassword!);
                    result = true;
                }
            }
            else
                throw new Exception("Failed: No thing to update.");
            return result;
        }

        


    }
}
