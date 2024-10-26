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
            UserModel user = await _repository.Users.GetUserByEmail(dto.Email);
            if (user != null && user.Password!.Equals(dto.Password) == true)
            {
                if (user.Status == true)
                {
                    user.Token = _jwtServices.GenerateAccessToken(user);
                    user.Id = null;
                    user.Password = null;
                    user.Status = null;
                }
                else
                    throw new Exception("Failed: Your account has been banned !");
            }
            else
                throw new Exception("Failed: Incorrect Email or Password !");
            return user;
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
                throw new Exception("Failed: Lack of Information to Sign Up !");
            // V01: Check Email:
            var users = await _repository.Users.GetAllUser();
            var check = from user in users
                        where user.Email == dto.Email
                        select user;
            if (!check.Any() == true)
                await _repository.Users.AddUser(new CreateUserRequest()
                {
                    DateOfBirth = dto.DateOfBirth,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Password = dto.Password,
                    Name = dto.Name,
                    Gender = dto.Gender,
                });
            else
                throw new Exception("Failed: Email has already exited !");
        }

        
        // 3. GET PROFILE:---------------------------------
        public async Task<UserModel> GetUser()
        {
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            return await _repository.Users.GetUserById(id);
        }

        // 4. UPDATE PROFILE:------------------------------
        public async Task<ProfileModel> EditProfile(EditProfileModel dto)
        {
            if (dto != null 
                && dto.Name.IsNullOrEmpty()== true
                && dto.Phone.IsNullOrEmpty() == true
                && dto.DateOfBirth == null
                && dto.Gender == null)
                throw new Exception("Failed: No thing to update.");

            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            ProfileModel result = await _repository.Users.UpdateUser(id, dto!);
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
                var user = await _repository.Users.GetUserById(id);
                if (user != null && user.Password!.Equals(dto.CurentPassword))
                {
                    await _repository.Users.UpdatePasswordById(id, dto.NewPassword!);
                    result = true;
                }
                else
                    throw new Exception("Failed: Your Current Password is incorrect !");
            }
            else
                throw new Exception("Failed: No thing to update.");
            return result;
        }

        // 6. CREATE USER:------------------------------
        public async Task CreateUser(CreateUserRequest user)
        {
            if(user == null 
                || user.Email.IsNullOrEmpty() == true 
                || user.Name.IsNullOrEmpty() == true
                || user.Password.IsNullOrEmpty() == true
                || user.RoleId == null)
                throw new Exception("Failed: Lack of basic information to create user !");
            // V01: Check Email:
            var users = await _repository.Users.GetAllUser();
            var check = from u in users
                        where u.Email == user.Email
                        select user;
            if (!check.Any() == true)
                await _repository.Users.AddUser(user);
            else
                throw new Exception("Failed: Email has already exited !");
        }

        // 7. DELETE USER:----------------------------------
        public async Task DeleteUser(int userId)
        {
            var actor = _jwtServices.GetIdAndRoleFromToken();
            var user = await _repository.Users.GetUserById(userId);
            if (user != null)
            {
                // V01: Tự xóa mình:
                if (actor.userId == user.Id)
                    throw new Exception("Failed: Sorry, you're not able to perform this behavior !");
                if (actor.role.Equals("Staff", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if(user.Role!.Equals("Manager") == true
                    || user.Role!.Equals("Referee") == true
                    || user.Role!.Equals("Staff") == true)
                        throw new Exception("Failed: Staff does not have permission to delete Manager, Referee or other Staff !");
                    else
                        await _repository.Users.DeleteUser(userId);
                } 
                else if (actor.role.Equals("Manager", StringComparison.OrdinalIgnoreCase) == true)
                    await _repository.Users.DeleteUser(userId);
                else
                    throw new Exception("Failed: You do not have permission to delete any user !");
            }
            else
                throw new Exception("Failed: User does not exit !");
        }

        public async Task<List<UserModel>> GetAllUser(int pageIndex, int pageSize, string? role)
        {
            if (role != null)
            {
                return (await _repository.Users.GetAllUser())
                    .Where(u => u.Role!.Equals(role, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(u => u.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize).ToList();
            }
            else
            {
                return (await _repository.Users.GetAllUser())
                    .OrderByDescending(u => u.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize).ToList();
            }
        }
    }
}
