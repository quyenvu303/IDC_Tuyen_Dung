using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using TuyenDungCoreApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace TuyenDungCoreApp.Services
{
    public class ClearTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public ClearTokenMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Kiểm tra nếu đường dẫn là "/login" hoặc "/logout"
            if (context.Request.Path.StartsWithSegments("/login", StringComparison.OrdinalIgnoreCase) ||
                context.Request.Path.StartsWithSegments("/logout", StringComparison.OrdinalIgnoreCase))
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AdminDbContext>();

                    // Lấy Refresh Token từ cookie
                    var refreshToken = context.Request.Cookies["RefreshToken"];
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        // Tìm Refresh Token trong DB
                        var token = await dbContext.RefreshToken
                            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                        if (token != null)
                        {
                            // Kiểm tra trạng thái isActive của user
                            var user = await dbContext.ManagementUser.FindAsync(token.UserId);
                            if (user != null && user.IsActive == 0)
                            {
                                // Nếu tài khoản không hoạt động, vô hiệu hóa tất cả Refresh Token của user
                                var userTokens = dbContext.RefreshToken.Where(rt => rt.UserId == user.Id);
                                foreach (var t in userTokens)
                                {
                                    t.Revoked = true;
                                    t.RevokedAt = DateTime.Now;
                                }

                                await dbContext.SaveChangesAsync();

                                // Xóa Refresh Token khỏi Cookie
                                context.Response.Cookies.Delete("RefreshToken");
                                context.Response.Cookies.Delete("JwtToken");

                                // Ngăn user tiếp tục truy cập
                                context.Response.StatusCode = 403; // Forbidden
                                await context.Response.WriteAsync("User account is inactive.");
                                return;
                            }

                            // Nếu user vẫn hoạt động, chỉ vô hiệu hóa token hiện tại
                            token.Revoked = true;
                            token.RevokedAt = DateTime.Now;
                            await dbContext.SaveChangesAsync();
                        }
                    }

                    // Xóa JWT Token khỏi Cookie
                    if (context.Request.Cookies.ContainsKey("JwtToken"))
                    {
                        context.Response.Cookies.Delete("JwtToken");
                    }

                    // Xóa Refresh Token khỏi Cookie
                    if (context.Request.Cookies.ContainsKey("RefreshToken"))
                    {
                        context.Response.Cookies.Delete("RefreshToken");
                    }
                }
            }

            await _next(context);
        }
    }

}
