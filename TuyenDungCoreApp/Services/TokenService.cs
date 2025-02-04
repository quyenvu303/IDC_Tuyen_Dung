using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System;
using TuyenDungCoreApp.Utils;
using TuyenDungModel.Custom;

namespace TuyenDungCoreApp.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(EmployeesViewLogin user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim("userName", user.UserName ?? string.Empty),
                    new Claim("fullName", user.FullName ?? string.Empty),
                    new Claim("role", user.Role ?? string.Empty),
                    new Claim("departmentId", user.DepartmentId.ToString() ?? string.Empty),
                    new Claim("departmentName", user.DepartmentName ?? string.Empty),
                    new Claim("jobTitleId", user.JobTitleId.ToString() ?? string.Empty),
                    new Claim("jobTitleName", user.JobTitleName ?? string.Empty),
                    new Claim("email", user.Email ?? string.Empty),
                    new Claim("phone", user.Phone ?? string.Empty),
                    new Claim("avatar", user.Avatar ?? string.Empty),
                    new Claim("isActive", user.IsActive.ToString() ?? string.Empty),
                    new Claim("status", user.Status.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Name, user.UserName), // Claim mặc định của ASP.NET Core
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Thêm jti
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                return (string)Logger.LogError(ex);
            }

        }

        public string GenerateRefreshToken()
        {
                var randomNumber = new byte[64]; // Tạo mảng byte có kích thước 64
            try
            {
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber); // Điền dữ liệu ngẫu nhiên vào mảng
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            return Convert.ToBase64String(randomNumber); // Chuyển mảng byte sang chuỗi Base64
        }


        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false, 
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception ex)
            {
                return (ClaimsPrincipal)Logger.LogError(ex);
            }
            
        }

    }
}
