using GearMarketBackend.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using TuyenDungCoreApp.Controllers;
using TuyenDungCoreApp.Filters;
using TuyenDungCoreApp.Models;
using TuyenDungCoreApp.Services;
using TuyenDungCoreApp.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using TuyenDungModel.DataTable;
using static GearMarketBackend.Areas.Admin.Controllers.ManagementUserController;

namespace TuyenDungCoreApp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ManagementDepartmentController : BaseController
    {
        private readonly AdminDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ManagementDepartmentController> _logger;

        public ManagementDepartmentController(
            ILogger<ManagementDepartmentController> logger,
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

        [DynamicAuthorize(RequiredPermission = EAction.Access)]
        [HttpPost("/ManagementDepartment/GetUserPermissions")]
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

        [DynamicAuthorize(RequiredPermission = EAction.View)]
        [HttpPost("/ManagementDepartment/JTable")]
        public JsonResult JTable()
        {
            try
            {
                var resultList = _context.ManagementDepartment
                    .Select(x => new {
                        x.Id,
                        x.ParenId,
                        x.DepartmentCode,
                        x.DepartmentName,
                        x.Status
                    })
                    .ToList();
                return Json(new { success = true, data = resultList });
            }
            catch (Exception ex)
            {
                return (JsonResult)Logger.LogError(ex);
            }
        }

        [DynamicAuthorize(RequiredPermission = EAction.Access)]
        [HttpPost("/ManagementDepartment/LoadDepartment")]
        public JsonResult LoadDepartment()
        {
            try
            {
                string sql = "[dbo].[Management_Department_GetListDepartment]";
                var data = _context.ManagementDepartment_LoadDepartment.FromSqlRaw(sql).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Json(new { success = false, data = (object)null });
            }
        }

        [DynamicAuthorize(RequiredPermission = EAction.Insert)]
        [HttpPost("/ManagementDepartment/Insert")]
        public object Insert()
        {
            var data = Request.Form["submit"];
            ManagementDepartment obj = JsonConvert.DeserializeObject<ManagementDepartment>(data);
            try
            {
                string sql = "[dbo].[Management_Department_Insert]" +
                    "@ParentId," +
                    "@Department_Code," +
                    "@Department_Name," +
                    "@Status," +
                    "@ReturnCode OUT";

                var _ParenId = new SqlParameter("@ParentId", SqlDbType.Int);
                _ParenId.Value = CheckNullParameterStore(_ParenId, obj.ParenId);

                var _Department_Code = new SqlParameter("@Department_Code", SqlDbType.NVarChar);
                _Department_Code.Value = CheckNullParameterStore(_Department_Code, obj.DepartmentCode);

                var _DepartmentName = new SqlParameter("@Department_Name", SqlDbType.NVarChar);
                _DepartmentName.Value = CheckNullParameterStore(_DepartmentName, obj.DepartmentName);

                var _Status = new SqlParameter("@Status", SqlDbType.Int);
                _Status.Value = CheckNullParameterStore(_Status, obj.Status);

                var ReturnCode = new SqlParameter("@ReturnCode", SqlDbType.NVarChar, 1000) { Direction = System.Data.ParameterDirection.Output };
                ReturnCode.Value = "";

                var rs = _context.Database.ExecuteSqlRaw(sql,
                    _ParenId,
                    _Department_Code,
                    _DepartmentName,
                    _Status,
                    ReturnCode);
                if (ReturnCode.Value.ToString() == "success")
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
        
        [HttpPost("/ManagementDepartment/GetItem")]
        public IActionResult GetItem([FromBody] DataCustom request)
        {
            try
            {
                var data = _context.ManagementDepartment.Where(x => x.Id == request.Id).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                return (IActionResult)Logger.LogError(ex);
            }
        }
        
        [DynamicAuthorize(RequiredPermission = EAction.Edit)]
        [HttpPost("/ManagementDepartment/Update")]
        public object Update()
        {
            var data = Request.Form["submit"];
            ManagementDepartment obj = JsonConvert.DeserializeObject<ManagementDepartment>(data);
            try
            {
                string sql = "[dbo].[Management_Department_Update]" +
                    "@Id," +
                    "@ParentId," +
                    "@Department_Code," +
                    "@Department_Name," +
                    "@Status," +
                    "@ReturnCode OUT";

                var _Id = new SqlParameter("@Id", SqlDbType.Int);
                _Id.Value = CheckNullParameterStore(_Id, obj.Id);

                var _ParenId = new SqlParameter("@ParentId", SqlDbType.Int);
                _ParenId.Value = CheckNullParameterStore(_ParenId, obj.ParenId);

                var _Department_Code = new SqlParameter("@Department_Code", SqlDbType.NVarChar);
                _Department_Code.Value = CheckNullParameterStore(_Department_Code, obj.DepartmentCode);

                var _DepartmentName = new SqlParameter("@Department_Name", SqlDbType.NVarChar);
                _DepartmentName.Value = CheckNullParameterStore(_DepartmentName, obj.DepartmentName);

                var _Status = new SqlParameter("@Status", SqlDbType.Int);
                _Status.Value = CheckNullParameterStore(_Status, obj.Status);

                var ReturnCode = new SqlParameter("@ReturnCode", SqlDbType.NVarChar, 1000) { Direction = System.Data.ParameterDirection.Output };
                ReturnCode.Value = "";

                var rs = _context.Database.ExecuteSqlRaw(sql,
                    _Id,
                    _ParenId,
                    _Department_Code,
                    _DepartmentName,
                    _Status,
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

        [DynamicAuthorize(RequiredPermission = EAction.Delete)]
        [HttpPost("/ManagementDepartment/Delete")]
        public object Delete([FromBody] DataCustom request)
        {

            try
            {
                var record = _context.ManagementDepartment.FirstOrDefault(x => x.Id == request.Id);
                if (record != null)
                {
                    record.Status = request.Status;
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
    }
}
