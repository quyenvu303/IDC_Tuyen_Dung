using TuyenDungCoreApp.Controllers;
using TuyenDungCoreApp.Filters;
using TuyenDungCoreApp.Models;
using TuyenDungCoreApp.Services;
using TuyenDungCoreApp.Utils;
using TuyenDungModel.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Newtonsoft.Json.Linq;
using TuyenDungModel.Custom;

namespace GearMarketBackend.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ManagementUserController : BaseController
    {
        private readonly AdminDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ManagementUserController> _logger;

        public ManagementUserController(
            ILogger<ManagementUserController> logger,
            IMemoryCache cache,
            AdminDbContext context,
            TokenService tokenService,
            IConfiguration configuration) 
            : base(cache, context) 
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
        }
        [DynamicAuthorize(RequiredPermission = EAction.Access)]
        public IActionResult Index()
        {
            return View();
        }

        public class DataCustom
        {
            public int? Id { get; set; }
            public int? IsActive { get; set; }
            public int? Status { get; set; }
        }
        public class Data
        {
            public string Search { get; set; }
            public int IsActive { get; set; }
            public string Department_id { get; set; }
            public int pageIndex { get; set; }
            public int pageSize { get; set; }
        }

        [DynamicAuthorize(RequiredPermission = EAction.Access)]
        [HttpPost("/ManagementUser/GetUserPermissions")]
        public JsonResult GetUserPermissions()
        {
            var _user_name = User.FindFirst("userName")?.Value;
            var controller = RouteData.Values["controller"]?.ToString();

            var permissions = Get_Permission_User_Login(_context, _user_name, controller);

            var userPermissions = new
            {
                Access = permissions.Any(p => p.Access == 1),
                View = permissions.Any(p => p.View == 1),
                Edit = permissions.Any(p => p.Edit == 1),
                Insert = permissions.Any(p => p.Insert == 1),
                Delete = permissions.Any(p => p.Delete == 1)
            };

            return Json(userPermissions);
        }

        [DynamicAuthorize(RequiredPermission = EAction.Access)]
        [HttpPost("/ManagementUser/GetNumberUserNoAuto")]
        public JsonResult GetNumberUserNoAuto()
        {
            try
            {
                string sql = "[dbo].[GetNumberUserNoAuto]";
                var nextUserNo = _context.Set<UserNoResult>().FromSqlRaw(sql).AsEnumerable().FirstOrDefault();
                return Json(nextUserNo);
            }
            catch (Exception ex)
            {
                return (JsonResult)Logger.LogError(ex);
            }
        }
       
        [DynamicAuthorize(RequiredPermission = EAction.View)]
        [HttpPost("/ManagementUser/JTable")]
        public object JTable([FromBody] Data jTablePara)
        {
            try
            {
                var userlogin = Get_Info_User_Login(_context);
                string sql = "[dbo].[Management_user_Get_Table]" +
                "@Search," +
                "@Department_id," +
                "@Status," +
                "@pageIndex," +
                "@pageSize," +
                "@output OUTPUT ";
                var _Search = new SqlParameter("@Search", SqlDbType.NVarChar); _Search.Value = CheckNullParameterStore(_Search, jTablePara.Search);
                var _Department_id = new SqlParameter("@Department_id", SqlDbType.NVarChar); _Department_id.Value = CheckNullParameterStore(_Department_id, jTablePara.Department_id);
                var _Status = new SqlParameter("@Status", SqlDbType.Int); _Status.Value = CheckNullParameterStore(_Status, jTablePara.IsActive);
                var _pageIndex = new SqlParameter("@pageIndex", SqlDbType.Int); _pageIndex.Value = CheckNullParameterStore(_pageIndex, jTablePara.pageIndex);
                var _pageSize = new SqlParameter("@pageSize", SqlDbType.Int); _pageSize.Value = CheckNullParameterStore(_pageSize, jTablePara.pageSize);
                var _output = new SqlParameter("@output", SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                var data = _context.ManagementUser.FromSqlRaw(
                    sql,
                    _Search,
                    _Department_id,
                    _Status,
                    _pageIndex,
                    _pageSize,
                    _output)
                .ToList();
                int totalRecords = (int)_output.Value;
                return new
                {
                    Error = false,
                    Title = "Success",
                    Data = data,
                    TotalRecords = totalRecords,
                    PageIndex = jTablePara.pageIndex,
                    PageSize = jTablePara.pageSize
                };
            }
            catch (Exception ex)
            {
                return Logger.LogError(ex);
            }
        }

        [HttpPost("/ManagementUser/GetDepartment")]
        public IActionResult GetDepartment()
        {
            try
            {
                string sql = "[dbo].[Management_user_GetDepartment]";
                var data = _context.Management_user_GetDepartment.FromSqlRaw(sql).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                return (IActionResult)Logger.LogError(ex);
            }

        }

        [HttpPost("/ManagementUser/GetJobTitle")]
        public IActionResult GetJobTitle()
        {
            try
            {
                var data = _context.ManagementJobTitle.Where(x => x.Status == 1).Select(x => new { x.Id, x.PositionName }).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                return (IActionResult)Logger.LogError(ex);
            }

        }

        [DynamicAuthorize(RequiredPermission = EAction.Insert)]
        [HttpPost("/ManagementUser/Insert")]
        public object Insert()
        {
            var data = Request.Form["submit"];
            ManagementUser obj = JsonConvert.DeserializeObject<ManagementUser>(data);
            obj.CreatedTime = DateTime.Now;
            var userlogin = Get_Info_User_Login(_context);
            obj.CreatedBy = userlogin.UserName;
            obj.Password = HashPassword("1234");
            try
            {
                // Lưu file(s) nếu có
                List<string> savedFiles = new List<string>();
                if (Request.Form.Files.Count > 0)
                {
                    savedFiles = SaveFiles(Request.Form.Files); // Gọi hàm lưu file
                }

                // Nếu có file, lưu đường dẫn file đầu tiên vào obj.Avatar (hoặc ghép nhiều đường dẫn nếu cần)
                if (savedFiles.Count > 0)
                {
                    obj.Avatar = string.Join(";", savedFiles); // Ghép các đường dẫn file (nếu nhiều file)
                }

                string sql = "[dbo].[Management_user_Insert]" +
                    "@user_no," +
                    "@UserName," +
                    "@Password," +
                    "@FullName," +
                    "@Department_id," +
                    "@JobTitle_id," +
                    "@Email," +
                    "@Phone," +
                    "@Avatar," +
                    "@IsActive," +
                    "@CreatedBy," +
                    "@ReturnCode OUT";

                var _user_no = new SqlParameter("@user_no", SqlDbType.NVarChar);
                _user_no.Value = CheckNullParameterStore(_user_no, obj.user_no);

                var _UserName = new SqlParameter("@UserName", SqlDbType.NVarChar);
                _UserName.Value = CheckNullParameterStore(_UserName, obj.UserName);

                var _Password = new SqlParameter("@Password", SqlDbType.NVarChar);
                _Password.Value = CheckNullParameterStore(_Password, obj.Password);

                var _FullName = new SqlParameter("@FullName", SqlDbType.NVarChar);
                _FullName.Value = CheckNullParameterStore(_FullName, obj.FullName);

                var _Department_id = new SqlParameter("@Department_id", SqlDbType.Int);
                _Department_id.Value = CheckNullParameterStore(_Department_id, obj.DepartmentId);

                var _JobTitle_id = new SqlParameter("@JobTitle_id", SqlDbType.Int);
                _JobTitle_id.Value = CheckNullParameterStore(_JobTitle_id, obj.JobTitleId);

                var _Email = new SqlParameter("@Email", SqlDbType.NVarChar, 200);
                _Email.Value = CheckNullParameterStore(_Email, obj.Email);

                var _Phone = new SqlParameter("@Phone", SqlDbType.NVarChar, 50);
                _Phone.Value = CheckNullParameterStore(_Phone, obj.Phone);

                var _Avatar = new SqlParameter("@Avatar", SqlDbType.NVarChar, 500);
                var obj_Avatar = "";
                if (obj.Avatar == null || obj.Avatar == "")
                {
                    obj_Avatar = "/assets/img/avatars/user_default.png";
                }
                else
                {
                    obj_Avatar = obj.Avatar;
                }
                _Avatar.Value = CheckNullParameterStore(_Avatar, obj_Avatar);

                var _IsActive = new SqlParameter("@IsActive", SqlDbType.Int);
                _IsActive.Value = CheckNullParameterStore(_IsActive, obj.IsActive);

                var _CreatedBy = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100);
                _CreatedBy.Value = CheckNullParameterStore(_CreatedBy, obj.CreatedBy);

                var ReturnCode = new SqlParameter("@ReturnCode", SqlDbType.NVarChar, 1000) { Direction = System.Data.ParameterDirection.Output };
                ReturnCode.Value = "";

                var rs = _context.Database.ExecuteSqlRaw(sql, _user_no, _UserName, _Password, _FullName, _Department_id, _JobTitle_id, _Email, _Phone, _Avatar, _IsActive, _CreatedBy, ReturnCode);
                if (ReturnCode.Value.ToString() == "TRUNGMA")
                {
                    return new { Title = "Thêm mới thành công. Mã nhân viên " + _user_no.Value + " đã tồn tại và tự động tăng lên ", Error = false };
                }
                else if (ReturnCode.Value.ToString() == "success")
                {
                    return new { Title = "Thêm mới thành công", Error = false };
                }
                else
                {
                    return new { Title = "Thêm mới không thành công", Error = true };
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Json(new { Title = "Đã xảy ra lỗi: " + ex.Message, Error = true });
            }
        }

        [HttpPost("/ManagementUser/GetItem")]
        public IActionResult GetItem([FromBody] DataCustom request)
        {
            try
            {
                var data = _context.ManagementUser.Where(x => x.Id == request.Id).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                return (IActionResult)Logger.LogError(ex);
            }

        }

        [DynamicAuthorize(RequiredPermission = EAction.Edit)]
        [HttpPost("/ManagementUser/Update")]
        public object Update()
        {
            var data = Request.Form["submit"];
            ManagementUser obj = JsonConvert.DeserializeObject<ManagementUser>(data);
            obj.CreatedTime = DateTime.Now;
            var userlogin = Get_Info_User_Login(_context);
            try
            {
                // Lưu file(s) nếu có
                List<string> savedFiles = new List<string>();
                if (Request.Form.Files.Count > 0)
                {
                    savedFiles = SaveFiles(Request.Form.Files); // Gọi hàm lưu file
                }

                // Nếu có file, lưu đường dẫn file đầu tiên vào obj.Avatar (hoặc ghép nhiều đường dẫn nếu cần)
                if (savedFiles.Count > 0)
                {
                    obj.Avatar = string.Join(";", savedFiles); // Ghép các đường dẫn file (nếu nhiều file)
                }
                string sql = "[dbo].[Management_user_Update]" +
                    "@Id," +
                    "@UserName," +
                    "@FullName," +
                    "@Department_id," +
                    "@JobTitle_id," +
                    "@Email," +
                    "@Phone," +
                    "@Avatar," +
                    "@IsActive," +
                    "@ReturnCode OUT";
                var Id = new SqlParameter("@Id", SqlDbType.Int);
                Id.Value = CheckNullParameterStore(Id, obj.Id);

                var UserName = new SqlParameter("@UserName", SqlDbType.NVarChar);
                UserName.Value = CheckNullParameterStore(UserName, obj.UserName);

                var FullName = new SqlParameter("@FullName", SqlDbType.NVarChar);
                FullName.Value = CheckNullParameterStore(FullName, obj.FullName);

                var Department_id = new SqlParameter("@Department_id", SqlDbType.Int);
                Department_id.Value = CheckNullParameterStore(Department_id, obj.DepartmentId);

                var JobTitle_id = new SqlParameter("@JobTitle_id", SqlDbType.Int);
                JobTitle_id.Value = CheckNullParameterStore(JobTitle_id, obj.JobTitleId);

                var Email = new SqlParameter("@Email", SqlDbType.NVarChar, 200);
                Email.Value = CheckNullParameterStore(Email, obj.Email);

                var Phone = new SqlParameter("@Phone", SqlDbType.NVarChar, 50);
                Phone.Value = CheckNullParameterStore(Phone, obj.Phone);

                var Avatar = new SqlParameter("@Avatar", SqlDbType.NVarChar, 500);
                var obj_Avatar = "";
                if (obj.Avatar == null || obj.Avatar == "")
                {
                    obj_Avatar = "/assets/img/avatars/user_default.png";
                }
                else
                {
                    obj_Avatar = obj.Avatar;
                }
                Avatar.Value = CheckNullParameterStore(Avatar, obj_Avatar);

                var IsActive = new SqlParameter("@IsActive", SqlDbType.Int);
                IsActive.Value = CheckNullParameterStore(IsActive, obj.IsActive);

                var ReturnCode = new SqlParameter("@ReturnCode", SqlDbType.NVarChar, 1000) { Direction = System.Data.ParameterDirection.Output };
                ReturnCode.Value = "";

                var rs = _context.Database.ExecuteSqlRaw(sql, Id, UserName, FullName, Department_id, JobTitle_id, Email, Phone, Avatar, IsActive, ReturnCode);
                if (ReturnCode.Value.ToString() == "success")
                {
                    return new { Title = "Cập nhật thành công", Error = false };
                }
                else
                {
                    return new { Title = "Cập nhật không thành công", Error = true };
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Json(new { Title = "Đã xảy ra lỗi: " + ex.Message, Error = true });
            }
        }

        [DynamicAuthorize(RequiredPermission = EAction.Delete)]
        [HttpPost("/ManagementUser/Delete")]
        public object Delete([FromBody] DataCustom request)
        {

            try
            {
                var record = _context.ManagementUser.FirstOrDefault(x => x.Id == request.Id);
                if (record != null)
                {
                    record.IsActive = request.IsActive;
                    _context.SaveChanges();
                    return Json(new { Title = "Xóa thành công", Error = false });
                }
                else
                {
                    return Json(new { Title = "Xóa không thành công: ", Error = true });
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Json(new { Title = "Đã xảy ra lỗi: " + ex.Message, Error = true });
            }
        }

       

        [DynamicAuthorize(RequiredPermission = EAction.Access)]
        [HttpPost("/ManagementUser/LoadDepartment")]
        public JsonResult LoadDepartment([FromBody] DataCustom request)
        {
            try
            {
                // Lấy dữ liệu từ bảng ManagementDepartment và số lượng nhân viên từ bảng ManagementUser
                var departmentData = _context.ManagementDepartment
                    .Where(x => x.Status == 1)
                    .Select(x => new
                    {
                        id = x.Id,
                        text = x.DepartmentName,
                        parentId = x.ParenId
                    }).ToList();

                var userCounts = _context.ManagementUser
                    .Where(x => x.IsActive == request.IsActive)
                    .GroupBy(x => x.DepartmentId)
                    .Select(g => new
                    {
                        DepartmentId = g.Key,
                        UserCount = g.Count()
                    }).ToDictionary(x => x.DepartmentId, x => x.UserCount);

                // Hàm đệ quy để tạo tree và tổng hợp số lượng nhân viên
                Func<int?, List<object>> buildTree = null;
                buildTree = parentId =>
                {
                    var nodes = departmentData
                        .Where(x => x.parentId == parentId)
                        .Select(x =>
                        {
                            // Lấy danh sách con
                            var children = buildTree(x.id);

                            // Tính tổng số lượng nhân viên của phòng ban hiện tại và các phòng ban con
                            var totalUserCount = (userCounts.ContainsKey(x.id) ? userCounts[x.id] : 0) +
                                                 children.Sum(c => (int)((dynamic)c).totalUserCount);

                            return new
                            {
                                id = x.id,
                                text = $"{x.text} ({totalUserCount})",
                                totalUserCount = totalUserCount, // Lưu tổng số lượng để tính tiếp
                                children = children
                            };
                        }).ToList<object>();

                    return nodes;
                };

                // Tạo danh sách tree với parentId = null (gốc)
                var tree = buildTree(null);

                return Json(tree);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        [DynamicAuthorize(RequiredPermission = EAction.Access)]
        [HttpPost("/ManagementUser/LoadController")]
        public JsonResult LoadController()
        {
            try
            {
                var resultList = _context.ManagementMenu
                    .Where(x => x.Status == 1)
                    .Select(x => new
                    {
                        id = x.Id.ToString(),
                        name = x.MenuName
                    }).ToList();

                return Json(new { success = true, data = resultList });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Json(new { success = false, data = (object)null });
            }
        }

        [HttpPost("/ManagementUser/getItemPermission")]
        public IActionResult getItemPermission([FromBody] DataCustom request)
        {
            var userlogin = Get_Info_User_Login(_context);
            try
            {
                string sql = "[dbo].[Management_user_Get_Permission] @user_id";
                var _user_id = new SqlParameter("@user_id", SqlDbType.Int); _user_id.Value = userlogin.Id;
                var data = _context.Management_user_Get_Permissions.FromSqlRaw(sql, _user_id).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                return (IActionResult)Logger.LogError(ex);
            }

        }

        [DynamicAuthorize(RequiredPermission = EAction.Edit)]
        [HttpPost("/ManagementUser/UpdatePermission")]
        public object UpdatePermission()
        {
            var data = Request.Form["UpdatePermission"];
            List<ManagementPermission> objlist = JsonConvert.DeserializeObject<List<ManagementPermission>>(data);
            try
            {
               
                string sql = "[dbo].[Management_user_UpdatePermission]" +
                    "@ListPermission," +
                    "@ReturnCode OUT";

                var _list_Permission = new SqlParameter("@ListPermission", SqlDbType.Structured);

                var ReturnCode = new SqlParameter("@ReturnCode", SqlDbType.NVarChar, 1000) { Direction = System.Data.ParameterDirection.Output };
                ReturnCode.Value = "";

                var _tem = new DataTable();
                _tem.Columns.Add("Id", typeof(int));
                _tem.Columns.Add("UserId", typeof(int));
                _tem.Columns.Add("MenuId", typeof(int));
                _tem.Columns.Add("FullControl", typeof(int));
                _tem.Columns.Add("Access", typeof(int));
                _tem.Columns.Add("View", typeof(int));
                _tem.Columns.Add("Edit", typeof(int));
                _tem.Columns.Add("Insert", typeof(int));
                _tem.Columns.Add("Delete", typeof(int));

                foreach (var item in objlist)
                {
                    _tem.Rows.Add(
                        item.Id,
                        item.UserId,
                        item.MenuId,
                        item.FullControl,
                        item.Access,
                        item.View,
                        item.Edit,
                        item.Insert,
                        item.Delete
                    );
                }
                _list_Permission.Value = _tem;
                _list_Permission.TypeName = "dbo.tmp_UpdatePermission";

                var rs = _context.Database.ExecuteSqlRaw(sql,
                    _list_Permission,
                    ReturnCode);
                if (ReturnCode.Value.ToString() == "success")
                {
                    return new { Title = "Cập nhật thành công", Error = false };
                }
                else
                {
                    return new { Title = "Cập nhật không thành công", Error = true };
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Json(new { Title = "Đã xảy ra lỗi: " + ex.Message, Error = true });
            }
        }
    }
}
