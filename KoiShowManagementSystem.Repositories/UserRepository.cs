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
        private readonly KoiShowManagementSystemContext _context;
        private enum ROLE_ID
        {
            MANAGER = 1,
            STAFF = 2, 
            REFEREE = 3,
            MEMBER = 4,
        }
        private readonly bool ACTIVE_STATUS = true;
        public UserRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        // IMPLEMENTATION:---------------------------
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
                    RoleId = (int) ROLE_ID.MEMBER,
                    Gender = dto.Gender,
                    Status = ACTIVE_STATUS
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
            var role = await _context.Set<Role>().SingleOrDefaultAsync(role => role.Id == user!.RoleId);
            return new UserModel()
            {
                Id = user!.Id,
                Name = user!.Name,
                Email = user.Email,
                Phone = user!.Phone,
                Role = role!.Title,
            };
        }

        public async Task<ProfileModel> GetProfile(int id)
        {
            ProfileModel result = null!;
            var user =  await _context.Set<User>().SingleOrDefaultAsync(user => user.Id == id);
            if (user != null)
            {
                var role = await _context.Set<Role>().SingleOrDefaultAsync(role => role.Id == user!.RoleId);
                result = new ProfileModel()
                {
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Role = role!.Title,
                };
            }
            else
                throw new Exception("User does not exist");
            return result;
        }

        public async Task<ProfileModel> EditProfile(int userId, EditProfileModel dto)
        {
            var user = await _context.Set<User>().SingleOrDefaultAsync(user => user.Id == userId);
            if (user != null)
            {
                // Update: 
                user.Name = dto.Name!;
                user.Phone = dto.Phone!;
                user.DateOfBirth = dto.DateOfBirth;
                user.Gender = dto.Gender;
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
                // Return new Information:
                var role = await _context.Set<Role>().SingleOrDefaultAsync(role => role.Id == user!.RoleId);
                return new ProfileModel()
                {
                    Name = user.Name!,
                    Phone = user.Phone!,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Email = user.Email,
                    Role = role!.Title,
                };
            }
            else
                throw new Exception("User does not exist");
        }

        public async Task<string> GetPasswordById(int id)
        {
            var user =  await _context.Set<User>().SingleOrDefaultAsync(user => user.Id == id);
            return user!.Password;
        }

        public async Task UpdatePasswordById(int id, string newPassword)
        {
            var user = await _context.Set<User>().SingleOrDefaultAsync(user => user.Id == id);
            // Update:
            user!.Password = newPassword;
            await _context.SaveChangesAsync();
        }
    }
}
