using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TuyenDungCoreApp.Models;
using TuyenDungCoreApp.Services;
using TuyenDungCoreApp.Utils;
using TuyenDungModel.Custom;
using TuyenDungModel.DataTable;


namespace TuyenDungCoreApp.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly AdminDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly RedisService _redisService;
        public LoginController(AdminDbContext context, TokenService tokenService, IConfiguration configuration, RedisService redisService)
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
            _redisService = redisService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Authen")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authen(string txt_username, string txt_password, string txt_IpAddress, string returnUrl = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_username) || string.IsNullOrWhiteSpace(txt_password))
                {
                    ViewBag.Error = "Username and password cannot be empty.";
                    return View("Index");
                }

                string hashedPassword = HashPassword(txt_password);

                var user = Get_User_Login(txt_username, hashedPassword);

                if (user != null && user.Id != 0) // Kiểm tra nếu thông tin người dùng hợp lệ
                {
                    //JWT
                    var jwtToken = _tokenService.GenerateJwtToken(user);
                    var refreshToken = _tokenService.GenerateRefreshToken();
                    var existingToken = _context.RefreshToken.FirstOrDefault(rt => rt.UserId == user.Id);

                    if (existingToken != null)
                    {
                        //existingToken.Token = refreshToken;
                        //existingToken.ExpiryDate = DateTime.Now.AddDays(2);

                        existingToken.Revoked = true;
                        existingToken.RevokedAt = DateTime.Now;

                    }
                    else
                    {
                        var tokenEntry = new RefreshToken
                        {
                            Token = refreshToken,
                            ExpiryDate = DateTime.Now.AddDays(2),
                            UserId = user.Id,
                            CreatedBy = user.Id.ToString(),
                            CreatedTime = DateTime.Now,
                            Revoked = false
                        };
                        _context.RefreshToken.Add(tokenEntry);
                    }

                    await _context.SaveChangesAsync();
                    await CleanupExpiredTokens();
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
                        new Claim(ClaimTypes.Name, user.UserName) // Claim mặc định của ASP.NET Core
                    };

                    Response.Cookies.Append("JwtToken", jwtToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddMinutes(60) // JWT hết hạn trong 15 phút
                    });

                    Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddDays(2) // Refresh Token hết hạn trong 2 ngày
                    });

                    // Chuyển hướng đến ReturnUrl nếu có, ngược lại chuyển về trang chính
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        if (returnUrl == "/Shared/404")
                        {
                            returnUrl = "/";
                        }
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                return (IActionResult)Logger.LogError(ex);
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(tokenRequest.Token);
                var userId = int.Parse(principal.FindFirst("userId").Value);

                var savedRefreshToken = _context.RefreshToken.SingleOrDefault(rt => rt.Token == tokenRequest.RefreshToken);
                if (savedRefreshToken == null || savedRefreshToken.UserId != userId || savedRefreshToken.ExpiryDate <= DateTime.Now || savedRefreshToken.Revoked == true)
                {
                    return Unauthorized();
                }

                var user = await _context.EmployeesViewLogin.FindAsync(userId);

                // Tạo JWT Token mới
                var newJwtToken = _tokenService.GenerateJwtToken(user);

                // Thực hiện Refresh Token Rotation
                // Vô hiệu hóa Refresh Token cũ
                savedRefreshToken.Revoked = true;
                savedRefreshToken.RevokedAt = DateTime.Now;

                var newRefreshToken = _tokenService.GenerateRefreshToken();

                var newRefreshTokenEntry = new RefreshToken
                {
                    Token = newRefreshToken,
                    UserId = user.Id,
                    ExpiryDate = DateTime.Now.AddDays(7), // Thời hạn của token mới
                    CreatedTime = DateTime.Now,
                    CreatedBy = user.Id.ToString(),
                    Revoked = false
                };
                // Lưu Refresh Token mới vào DB
                _context.RefreshToken.Add(newRefreshTokenEntry);
                await _context.SaveChangesAsync();

                //savedRefreshToken.Token = newRefreshToken;
                //savedRefreshToken.ExpiryDate = DateTime.Now.AddDays(7);
                //await _context.SaveChangesAsync();

                return Ok(new { Token = newJwtToken, RefreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                return (IActionResult)Logger.LogError(ex);
            }

        }

        private async Task CleanupExpiredTokens()
        {
            try
            {
                var expiredTokens = _context.RefreshToken.Where(rt => rt.ExpiryDate <= DateTime.Now || rt.Revoked == null);
                _context.RefreshToken.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private string HashPassword(string password)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public EmployeesViewLogin Get_User_Login(string login_name, string password)
        {
            try
            {
                string sql = "[dbo].[Get_User_Login] @LoginName,@Password";
                var _LoginName = new SqlParameter("@LoginName", SqlDbType.NVarChar, 50); _LoginName.Value = login_name;
                var _Password = new SqlParameter("@Password", SqlDbType.NVarChar, 500); _Password.Value = password;
                var results = _context.EmployeesViewLogin.FromSqlRaw(sql, _LoginName, _Password).AsEnumerable().ToList();
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return (EmployeesViewLogin)Logger.LogError(ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied"); // Hiển thị trang từ chối quyền truy cập nếu cần
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {

                // Xóa Refresh Token khỏi Cookie
                if (HttpContext.Request.Cookies.ContainsKey("RefreshToken"))
                {
                    HttpContext.Response.Cookies.Delete("RefreshToken");
                }

                // Xóa JWT token khỏi cookie (nếu bạn lưu trong cookie)
                HttpContext.Response.Cookies.Delete("JwtToken");

                // Xóa refresh token khỏi cơ sở dữ liệu
                var userId = User.FindFirst("userId")?.Value;  // Lấy userId từ claims
                if (!string.IsNullOrEmpty(userId))
                {
                    var refreshToken = await _context.RefreshToken.FirstOrDefaultAsync(rt => rt.UserId == int.Parse(userId));
                    if (refreshToken != null)
                    {
                        // Cập nhật trạng thái Revoked
                        refreshToken.Revoked = true;
                        refreshToken.RevokedAt = DateTime.Now;

                        //_context.RefreshTokens.Remove(refreshToken);  // Xóa refresh token trong DB
                        //await _context.SaveChangesAsync();  // Lưu thay đổi
                    }
                }

                // Xóa các session nếu có
                HttpContext.Session.Clear();

                // Chuyển hướng về trang Login
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
