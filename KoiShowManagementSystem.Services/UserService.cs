using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace KoiShowManagementSystem.Services
{
    /* USER SERVICE */
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly JwtServices _jwtServices;
        public UserService(UnitOfWork unitOfWork, JwtServices jwtServices)
        {
            _unitOfWork = unitOfWork;
            _jwtServices = jwtServices;
        }

        // 1. LOGIN:--------------------------------------
        public async Task<object> Login(LoginModel dto)
        {
            object result = null!;
            // 1.1. Get User:
            var user = await _unitOfWork.Users.GetByEmail(dto.Email);
            // 1.2 Check Status:
            if (user != null && user.Status == true)
            {
                // 1.3. Check Password:
                bool chkPassword = string.Equals(user.Password, dto.Password, StringComparison.Ordinal);
                if (chkPassword == true)
                {
                    // 1.4 Get Role Title:
                    var role = await _unitOfWork.Roles.GetById(user.RoleId);
                    result = new 
                    {
                        Id = user!.Id,
                        Name = user!.Name,
                        Email = user.Email,
                        Role = role!.Title,
                        Phone = user!.Phone,
                        Token = _jwtServices.GenerateAccessToken(user.Email, user.Name, user.Id, role.Title)
                    };
                }
            }
            else if (user != null && user.Status == false)
                throw new Exception("Your account is banned");
            // 1.5 Return result
            return result;
        }


        //2. SIGN UP: ------------------------------------
        public async Task<bool> SignUp(SignUpModel dto)
        {
            bool result = false;
            try
            {
                await _unitOfWork.Users.Add(new Entities.User()
                {
                    Name = dto.Name!,
                    Phone = dto.Phone!,
                    DateOfBirth = dto.DateOfBirth,
                    Email = dto.Email!,
                    Password = dto.Password!,
                    RoleId = 4,
                    Gender = dto.Gender,
                    Status = true
                });
                await _unitOfWork.SaveAsync();
                result = true;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx!.Number == 2627
                        || sqlEx.Number == 2601
                        && sqlEx.Message.Contains("UQ__User__Email"))
                    {
                        throw new Exception("Email has already existed");
                    }
                }
            }
            return result;
        }
    }
}
