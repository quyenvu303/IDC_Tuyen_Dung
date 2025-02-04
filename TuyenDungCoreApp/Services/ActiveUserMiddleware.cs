using TuyenDungCoreApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace TuyenDungCoreApp.Services
{
    public class ActiveUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public ActiveUserMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Lấy thông tin từ JWT Token
            var userIdClaim = context.User?.FindFirst("userId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AdminDbContext>();

                    // Kiểm tra trạng thái isActive của user
                    var user = await dbContext.ManagementUser.FindAsync(userId);
                    if (user == null || user.IsActive == 0)
                    {
                        context.Response.StatusCode = 403; // Forbidden
                        await context.Response.WriteAsync("User account is inactive.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

}
