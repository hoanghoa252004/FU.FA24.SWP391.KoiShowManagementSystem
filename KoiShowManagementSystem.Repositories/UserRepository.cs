using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace KoiShowManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private KoiShowManagementSystemContext _context;
        public UserRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task AddUser(SignUpModel dto)
        {
            try
            {
                User newUser = new User()
                {
                    Name = dto.Name!,
                    Phone = dto.Phone!,
                    DateOfBirth = dto.DateOfBirth,
                    Email = dto.Email!,
                    Password = dto.Password!,
                    RoleId = 4,
                    Gender = dto.Gender,
                    Status = true
                };
                await _context.Set<User>().AddAsync(newUser);
                await _context.SaveChangesAsync();
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
        }

        public async Task<UserModel> GetAccount(LoginModel dto)
        {
            var user =  await _context.Set<User>().SingleOrDefaultAsync(user => user.Email == dto.Email);
            if (user == null || !string.Equals(user.Password, dto.Password, StringComparison.Ordinal))
                throw new Exception("Email or password is incorrect");
            else if(user != null && user.Status == false)
                throw new Exception("Your account is banned");
            var role = (await _context.Set<Role>().SingleOrDefaultAsync(role => role.Id == user!.RoleId))!.Title;
            return new UserModel()
            {
                Id = user!.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user!.Phone,
                Role = role,
            };
        }
    }
}
