using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TuyenDungCoreApp.Filters;
using TuyenDungCoreApp.Models;
using TuyenDungModel.DataTable;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using TuyenDungCoreApp.Services;
using TuyenDungCoreApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Http.Features;
using StackExchange.Redis;

namespace TuyenDungCoreApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Đọc secret key từ cấu hình (appsettings.json)
            var secretKey = Configuration["Jwt:Key"];

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // Giới hạn 100MB
            });

            services.AddHttpContextAccessor(); // Đăng ký IHttpContextAccessor
            services.Configure<IConfiguration>(Configuration);

            WebContext.Configure(
                services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>(),
                Configuration.GetSection("AppSettings")
            );

            // Đăng ký Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(Configuration.GetSection("Redis:ConnectionString").Value, true);
                configuration.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(configuration);
            });
            // Đăng ký RedisService
            services.AddSingleton<RedisService>();

            services.AddControllersWithViews();
            services.AddMemoryCache(); // Đăng ký MemoryCache để lưu data Menu

            // Đăng ký DbContext
            services.AddDbContext<AdminDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //Đăng ký các controller
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.DefaultContractResolver());
            services.AddControllers();

            //Cấu hình xác thực và JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
                // Xử lý khi người dùng chưa xác thực
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse(); // Chặn xử lý mặc định
                        context.Response.Redirect("/Login"); // Chuyển hướng tới LoginController
                        return Task.CompletedTask;
                    }
                };
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login"; // Đường dẫn đến trang Login
                options.AccessDeniedPath = "/Login/AccessDenied"; // Đường dẫn khi bị từ chối truy cập
            });
            services.AddAuthorization();
            services.AddScoped<TokenService>();
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN"; // Đảm bảo rằng tên header CSRF Token là chính xác
            });

            // Đảm bảo rằng middleware này được kích hoạt


            // Đăng ký Session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60); // Thời gian session tồn tại (30 phút)
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //Thêm DynamicAuthorizeAttribute
            services.AddScoped<DynamicAuthorizeAttribute>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            // Tùy chỉnh chuyển hướng HTTPS
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect; // Chuyển hướng vĩnh viễn
                options.HttpsPort = 44333; // Cổng HTTPS
            });

            Logger.Configure(Configuration);
        }
        private void InitializeDatabase(IServiceProvider serviceProvider)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
                    // Kiểm tra và thêm user mặc định
                    var adminUser = context.ManagementUser.FirstOrDefault(u => u.UserName == "admin");
                    if (adminUser == null)
                    {
                        adminUser = new ManagementUser
                        {
                            user_no = "admin",
                            UserName = "admin",
                            Password = HashPassword("1234"), // Mã hóa mật khẩu
                            FullName = "Administrator",
                            DepartmentName = "Administrator", // Cập nhật theo giá trị của bạn
                            JobTitleName = "Administrator",  // Cập nhật theo giá trị của bạn
                            Email = "admin@example.com",
                            Phone = "1234",
                            Role = "Administrator",
                            Avatar = "/assets/img/avatars/user_default.png",
                            IsActive = 1,
                            CreatedTime = DateTime.Now,
                            CreatedBy = "System"
                        };
                        context.ManagementUser.Add(adminUser);
                        context.SaveChanges();
                    }
                    // Kiểm tra và thêm menu "Home"
                    var homeMenu = context.ManagementMenu.FirstOrDefault(m => m.MenuName == "Trang chủ");
                    if (homeMenu == null)
                    {
                        homeMenu = new ManagementMenu
                        {
                            ParentId = null,
                            MenuName = "Trang chủ",
                            Areas = null,
                            Controller = "Home",
                            Action = "Index",
                            Icon = null,
                            //UserCreator = adminUser.Id, // Hoặc giá trị phù hợp của bạn
                            Status = 1
                        };
                        context.ManagementMenu.Add(homeMenu);
                        context.SaveChanges();
                    }
                    // Kiểm tra và thêm menu "Quản lý công cụ"
                    var toolManagementMenu = context.ManagementMenu.FirstOrDefault(m => m.MenuName == "Quản lý công cụ");
                    if (toolManagementMenu == null)
                    {
                        toolManagementMenu = new ManagementMenu
                        {
                            ParentId = null,
                            MenuName = "Quản lý công cụ",
                            Areas = "Admin",
                            Controller = "ManagementCategory",
                            Action = "Index",
                            Icon = null,
                            //UserCreator = adminUser.Id, // Hoặc giá trị phù hợp của bạn
                            Status = 1
                        };
                        context.ManagementMenu.Add(toolManagementMenu);
                        context.SaveChanges(); // Lưu để lấy Id của bản ghi vừa thêm
                    }

                    // Kiểm tra và thêm menu "Management_permission" với ParentId là Id của "Quản lý công cụ"
                    var managementPermissionMenu = context.ManagementMenu.FirstOrDefault(m => m.MenuName == "Quản lý người dùng");
                    if (managementPermissionMenu == null && toolManagementMenu != null)
                    {
                        managementPermissionMenu = new ManagementMenu
                        {
                            ParentId = toolManagementMenu.Id, // Id của "Quản lý công cụ"
                            MenuName = "Quản lý người dùng",
                            Areas = "Admin",
                            Controller = "ManagementUser",
                            Action = "Index",
                            Icon = null,
                            //UserCreator = adminUser.Id, // Hoặc giá trị phù hợp của bạn
                            Status = 1
                        };
                        context.ManagementMenu.Add(managementPermissionMenu);
                        context.SaveChanges();
                    }
                    // Kiểm tra và thêm menu "Management_Menu" với ParentId là Id của "Quản lý công cụ"
                    var Management_Menu = context.ManagementMenu.FirstOrDefault(m => m.MenuName == "Quản lý danh mục");
                    if (Management_Menu == null && toolManagementMenu != null)
                    {
                        Management_Menu = new ManagementMenu
                        {
                            ParentId = toolManagementMenu.Id, // Id của "Quản lý công cụ"
                            MenuName = "Quản lý danh mục",
                            Areas = "Admin",
                            Controller = "ManagementMenu",
                            Action = "Index",
                            Icon = null,
                            //UserCreator = adminUser.Id, // Hoặc giá trị phù hợp của bạn
                            Status = 1
                        };
                        context.ManagementMenu.Add(Management_Menu);
                        context.SaveChanges();
                    }
                    // Kiểm tra và thêm quyền mặc định cho "Home" menu
                    if (!context.ManagementPermission.Any(p => p.UserId == adminUser.Id && p.MenuId == homeMenu.Id))
                    {
                        var adminPermissionHome = new ManagementPermission
                        {
                            UserId = adminUser.Id,
                            MenuId = homeMenu.Id,
                            FullControl = 1,
                            Access = 1,
                            View = 1,
                            Edit = 1,
                            Insert = 1,
                            Delete = 1,
                            Status = 1
                        };
                        context.ManagementPermission.Add(adminPermissionHome);
                        context.SaveChanges();
                    }
                    // Kiểm tra và thêm quyền mặc định cho "Quản lý công cụ"
                    if (!context.ManagementPermission.Any(p => p.UserId == adminUser.Id && p.MenuId == toolManagementMenu.Id))
                    {
                        var adminPermissionHome = new ManagementPermission
                        {
                            UserId = adminUser.Id,
                            MenuId = toolManagementMenu.Id,
                            FullControl = 1,
                            Access = 1,
                            View = 1,
                            Edit = 1,
                            Insert = 1,
                            Delete = 1,
                            Status = 1
                        };
                        context.ManagementPermission.Add(adminPermissionHome);
                        context.SaveChanges();
                    }

                    // Kiểm tra và thêm quyền mặc định cho "Management_permission" menu
                    if (!context.ManagementPermission.Any(p => p.UserId == adminUser.Id && p.MenuId == Management_Menu.Id))
                    {
                        var adminPermissionManagementPermission = new ManagementPermission
                        {
                            UserId = adminUser.Id,
                            MenuId = Management_Menu.Id,
                            FullControl = 1,
                            Access = 1,
                            View = 1,
                            Edit = 1,
                            Insert = 1,
                            Delete = 1,
                            Status = 1
                        };
                        context.ManagementPermission.Add(adminPermissionManagementPermission);
                        context.SaveChanges();
                    }
                    // Kiểm tra và thêm quyền mặc định cho "Management_permission" menu
                    if (!context.ManagementPermission.Any(p => p.UserId == adminUser.Id && p.MenuId == managementPermissionMenu.Id))
                    {
                        var adminPermissionManagementPermission = new ManagementPermission
                        {
                            UserId = adminUser.Id,
                            MenuId = managementPermissionMenu.Id,
                            FullControl = 1,
                            Access = 1,
                            View = 1,
                            Edit = 1,
                            Insert = 1,
                            Delete = 1,
                            Status = 1
                        };
                        context.ManagementPermission.Add(adminPermissionManagementPermission);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }
        private string HashPassword(string password)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                    foreach (byte b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            return builder.ToString();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AdminDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            // Kiểm tra kết nối database
            try
            {
                if (dbContext.Database.CanConnect())
                {
                    Console.WriteLine("Database connected successfully.");
                }
                else
                {
                    Console.WriteLine("Unable to connect to database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
            }

            // Dọn dẹp Refresh Token hết hạn khi ứng dụng khởi động
            using (var scope = app.ApplicationServices.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetService<AdminDbContext>();
                if (dbContext.RefreshToken.Any())
                {
                    CleanupExpiredTokensOnStartup(dbContext).GetAwaiter().GetResult();
                }
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            //seeding tai khoan admin vào DB
            InitializeDatabase(app.ApplicationServices);

            //Thêm Middleware để đọc token từ Cookie
            app.Use(async (context, next) =>
            {
                var token = context.Request.Cookies["JwtToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Add("Authorization", $"Bearer {token}");
                }
                await next();
            });
            app.UseCookiePolicy();

            // Middleware kiểm tra trạng thái isActive của user
            app.UseMiddleware<ActiveUserMiddleware>();

            // Middleware xóa token khi gọi LoginController hoặc Logout
            app.UseMiddleware<ClearTokenMiddleware>();

            // Middleware xác thực (Authentication)
            app.UseAuthentication();

            // Middleware phân quyền (Authorization)
            app.UseAuthorization();

            // Xử lý lỗi 404
            app.UseStatusCodePagesWithReExecute("/404");

            // Middleware xử lý lỗi 404, 403
            // Middleware xử lý lỗi 404 và 403
            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;
                var request = context.HttpContext.Request;

                // Kiểm tra mã trạng thái lỗi
                switch (response.StatusCode)
                {
                    case 404:
                        response.ContentType = "text/html";
                        await response.WriteAsync($"<script>alert('Page not found (404)'); window.location='/';</script>");
                        break;

                    case 403:
                        response.ContentType = "text/html";
                        await response.WriteAsync($"<script>alert('Access denied (403)'); window.location='/';</script>");
                        break;

                    case 401:
                        response.ContentType = "text/html";
                        await response.WriteAsync($"<script>alert('Unauthorized access (401)'); window.location='/';</script>");
                        break;

                    case 500:
                        response.ContentType = "text/html";
                        await response.WriteAsync($"<script>alert('Internal server error (500)'); window.location='/';</script>");
                        break;

                    default:
                        response.ContentType = "text/html";
                        await response.WriteAsync($"<script>alert('An error occurred ({response.StatusCode})'); window.location='/';</script>");
                        break;
                }
            });

            app.UseEndpoints(endpoints =>
            {
                // Định tuyến cho Areas
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapFallbackToFile("/index.html");

                // Định tuyến mặc định cho các controller không thuộc Areas
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        private async Task CleanupExpiredTokensOnStartup(AdminDbContext context)
        {
            var expiredTokens = context.RefreshToken.Where(rt => rt.ExpiryDate <= DateTime.Now).ToList();
            if (expiredTokens.Any())
            {
                context.RefreshToken.RemoveRange(expiredTokens);
                await context.SaveChangesAsync();
            }
        }
    }
}
