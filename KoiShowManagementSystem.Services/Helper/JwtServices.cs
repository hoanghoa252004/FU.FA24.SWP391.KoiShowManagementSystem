
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using KoiShowManagementSystem.DTOs.BusinessModels;

namespace KoiShowManagementSystem.Services.Helper;

public class JwtServices
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public JwtServices(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    // 1. GENERATE ACCESS TOKEN:---------------------------------------------------
    public string GenerateAccessToken(UserModel dto)
    {
        // Tạo Claims cho Payload:
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, dto.Email!),
            new Claim(JwtRegisteredClaimNames.Name, dto.Name!),
            new Claim("id", dto.Id.ToString()),
            new Claim(ClaimTypes.Role, dto.Role!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Encode SecretKey:
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));

        // Encrypt để Signature: cần Encoded SecretKey và Thuật toán encrypt.
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Tạo Access Token:
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60*24), //**** TEST *****//
            signingCredentials: signingCredentials
            );

        // Dùng Handler để lấy chuỗi Token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


     // 2. GET IDENTITY:------------------------------------------------------
    public (int userId, string role) GetIdAndRoleFromToken()
    {
        var user = _httpContextAccessor.HttpContext!.User;
        var userId = user.FindFirst("id")?.Value;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            throw new UnauthorizedAccessException("Invalid Token Claims");
        return (int.Parse(userId), role);
    }
}
