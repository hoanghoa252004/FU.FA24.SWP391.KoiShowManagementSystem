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
        public async Task AddUser(CreateUserRequest dto)
        {
            if(dto != null)
            {
                User newUser = new User()
                {
                    Name = dto.Name!,
                    Phone = dto.Phone,
                    DateOfBirth = dto.DateOfBirth,
                    Email = dto.Email!,
                    Password = dto.Password!,
                    RoleId = dto.RoleId ?? (int)ROLE_ID.MEMBER,
                    Gender = dto.Gender,
                    Status = ACTIVE_STATUS
                };
                await _context.Set<User>().AddAsync(newUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            UserModel result = null!;
            if(email != null)
            {
                var user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(user => user.Email.Equals(email));
                if(user != null && user.Email.Equals(email, StringComparison.OrdinalIgnoreCase) == true)
                    result = new UserModel()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        Role = user.Role.Title,
                        Password = user.Password,
                        Status = user.Status,
                    };
            }
            return result;
        }

        public async Task<ProfileModel> UpdateUser(int userId, EditProfileModel dto)
        {
            var user = await _context.Set<User>().SingleOrDefaultAsync(user => user.Id == userId);
            if (user != null)
            {
                // Update: 
                if (dto.Name != null) user.Name = dto.Name;
                if(dto.Phone != null) user.Phone = dto.Phone;
                if (dto.DateOfBirth != null) user.DateOfBirth = dto.DateOfBirth;
                if (dto.Gender != null) user.Gender = dto.Gender;
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

        public async Task UpdatePasswordById(int id, string newPassword)
        {
            var user = await _context.Set<User>().SingleOrDefaultAsync(user => user.Id == id);
            // Update:
            if(user != null)
            {
                user.Password = newPassword;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserModel> GetUserById(int userId)
        {
            var user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(user => user.Id == userId);
            if(user != null)
                return new UserModel()
                {
                    Id = userId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role.Title,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender
                };
            return null!;
        }

        public async Task<List<UserModel>> GetAllUser()
        {
            var result = await _context.Users.Include(u => u.Role).Select(u => new UserModel()
            {
                Id=u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role.Title,
                DateOfBirth = u.DateOfBirth,
                Password = u.Password
            }).ToListAsync();
            return result;
        }

        public async Task DeleteUser(int userId)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            if(user != null)
            {
                user.Status = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
