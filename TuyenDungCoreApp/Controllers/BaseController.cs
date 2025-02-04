using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System;
using TuyenDungCoreApp.Models;
using TuyenDungModel.Custom;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using TuyenDungModel.DataTable;
using TuyenDungCoreApp.Utils;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using TuyenDungCoreApp.Services;

namespace TuyenDungCoreApp.Controllers
{
    public class BaseController : Controller
    {
        public string RequiredPermission { get; set; }
        private readonly IMemoryCache _cache;
        private readonly AdminDbContext _context;
        //private readonly RedisService _redisService;
        public BaseController(IMemoryCache cache, AdminDbContext context)
        {
            _cache = cache;
            _context = context;
            //_redisService = redisService;
        }
        //Kiểm tra người dùng đã login chưa, nưa login đẩy về LoginController
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Kiểm tra người dùng đã xác thực hay chưa
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Nếu chưa xác thực, chuyển hướng đến trang login
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { area = "", controller = "Login", action = "Index" })
                );
                return;
            }
            else
            {
                // Lấy thông tin từ JWT Token
                var userId = context.HttpContext.User.FindFirst("userId")?.Value;
                var fullName = context.HttpContext.User.FindFirst("fullName")?.Value;
                var avatarPath = context.HttpContext.User.FindFirst("avatar")?.Value;
                var departmentName = context.HttpContext.User.FindFirst("departmentName")?.Value;

                // Kiểm tra null hoặc rỗng cho avatar
                var baseUrl = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}";
                var avatar = string.IsNullOrEmpty(avatarPath)
                    ? $"{baseUrl}/assets/img/avatars/user_default.png" // Avatar mặc định
                    : $"{baseUrl}{avatarPath}"; // Ghép base URL với đường dẫn avatar

                // Gán thông tin vào ViewBag
                ViewBag.FullName = fullName ?? "Guest User";
                ViewBag.Avatar = avatar;
                ViewBag.DepartmentName = departmentName;
                ViewBag.UserName = context.HttpContext.User.Identity.Name ?? "guest";

                if (HttpContext.Session.GetString("MenuTree") == null)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        string sql = "[dbo].[Get_List_Menu] @user_id";
                        var d_userId = new SqlParameter("@user_id", SqlDbType.Int);
                        d_userId.Value = int.Parse(userId);

                        var rs = _context.MenuItems.FromSqlRaw(sql, d_userId).ToList();
                        var menuTree = BuildMenuTree(rs);

                        // Lưu menuTree vào Session dưới dạng JSON
                        HttpContext.Session.SetString("MenuTree", JsonConvert.SerializeObject(menuTree));
                    }
                }

                // Đọc MenuTree từ Session và gán vào ViewData
                var menuTreeFromSession = HttpContext.Session.GetString("MenuTree");
                if (!string.IsNullOrEmpty(menuTreeFromSession))
                {
                    var menuTree = JsonConvert.DeserializeObject<List<MenuItem>>(menuTreeFromSession);
                    ViewData["MenuTree"] = menuTree;
                }
                ////đọc số lượng user đnag truy cập
                //int onlineDeviceCount = _context.UserDevices
                //                        .Where(u => u.IsOnline == true) // Điều kiện: IsOnline = 1
                //                        .Select(u => u.UserId)          // Chỉ lấy UserId
                //                        .Distinct()                     // Loại bỏ các UserId trùng lặp
                //                        .Count();


            }

            base.OnActionExecuting(context);
        }
        private List<MenuItem> BuildMenuTree(List<MenuItem> menuItems)
        {
            // Nhóm các mục menu theo Parent_Id
            var lookup = menuItems.ToLookup(m => m.parent_id);

            // Lấy các mục root (Parent_Id = null)
            var rootItems = lookup[null].ToList();

            // Duyệt qua từng mục root và thiết lập quan hệ con
            foreach (var item in rootItems)
            {
                BuildChildren(item, lookup);
            }

            return rootItems;
        }
        private void BuildChildren(MenuItem parent, ILookup<int?, MenuItem> lookup)
        {
            parent.Children = lookup[parent.id].ToList();
            foreach (var child in parent.Children)
            {
                BuildChildren(child, lookup);
            }
        }
        //Hàm lấy thông tin user login
        public EmployeesViewLogin Get_Info_User_Login(AdminDbContext _context)
        {
            try
            {
                var _userId = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
                string sql = "[dbo].[Get_Info_User_Login] @userId";
                var d_userId = new SqlParameter("@userId", SqlDbType.Int); d_userId.Value = CheckNullParameterStore(d_userId, _userId);  //_LoginName.Value = User.Identity.Name;
                var rs = _context.EmployeesViewLogin.FromSqlRaw(sql, d_userId).AsEnumerable().Take(1).SingleOrDefault();
                return rs;
            }
            catch (Exception ex)
            {
                return (EmployeesViewLogin)Logger.LogError(ex);
            }
        }
        // convert tham số thành null khi không hợp lệ
        public object CheckNullParameterStore(SqlParameter p, dynamic d)
        {
            try
            {
                if (d == null)
                    return DBNull.Value;
                else
                    return d;
            }
            catch (Exception ex)
            {
                return (List<string>)Logger.LogError(ex);
            }

        }
        // HÀm dùng chung lưu file vào wwwroot/uploads
        public List<string> SaveFiles(IFormFileCollection files)
        {
            var savedFilePaths = new List<string>();
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            foreach (var file in files)
            {
                try
                {
                    var filePath = Path.Combine(folderPath, file.FileName);

                    // Nếu file tồn tại, kiểm tra xem file có bị khóa không
                    if (System.IO.File.Exists(filePath))
                    {
                        if (IsFileLocked(filePath))
                        {
                            Console.WriteLine($"File is locked: {filePath}");
                            throw new IOException($"File is being used by another process: {filePath}");
                        }
                        System.IO.File.Delete(filePath); // Xóa file nếu không bị khóa
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        file.CopyTo(stream);
                    }

                    var publicPath = $"/uploads/{file.FileName}";
                    savedFilePaths.Add(publicPath);
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine($"File access issue: {ioEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected issue: {ex.Message}");
                }
            }

            return savedFilePaths;
        }
        bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    return false; // File không bị khóa
                }
            }
            catch (IOException)
            {
                return true; // File đang bị khóa
            }
        }
        // Hàm mã hóa theo SHA256
        public string HashPassword(string password)
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
                return (string)Logger.LogError(ex);
            }

        }
        public List<ManagementPermission> Get_Permission_User_Login(AdminDbContext _context, string user_name, string controller)
        {
            try
            {
                string sql = "[dbo].[Get_Permission_User_Login] @User_name,@Controller";
                var _user_name = new SqlParameter("@User_name", SqlDbType.NVarChar) { Value = user_name };
                var _Controller = new SqlParameter("@Controller", SqlDbType.NVarChar, 500) { Value = controller };

                var results = _context.ManagementPermission.FromSqlRaw(sql, _user_name, _Controller).AsEnumerable().ToList();
                return results;
            }
            catch (Exception ex)
            {

                return new List<ManagementPermission>();
            }
        }
    }
}
