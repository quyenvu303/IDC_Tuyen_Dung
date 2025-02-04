using TuyenDungCoreApp.Controllers;
using TuyenDungCoreApp.Filters;
using TuyenDungCoreApp.Models;
using TuyenDungCoreApp.Services;
using TuyenDungCoreApp.Utils;
using TuyenDungModel.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using static GearMarketBackend.Areas.Admin.Controllers.ManagementUserController;

namespace GearMarketBackend.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ManagementMenuController : BaseController
    {
        private readonly AdminDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ManagementMenuController> _logger;

        public ManagementMenuController(
            ILogger<ManagementMenuController> logger,
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
        [HttpPost("/ManagementMenu/GetUserPermissions")]
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
        [HttpPost("/ManagementMenu/JTable")]
        public JsonResult JTable()
        {
            try
            {
                var resultList = _context.ManagementMenu//.Where(x => x.Status == 1)
                    .Select(x => new { 
                        x.Id, 
                        x.ParentId,
                        x.MenuName,
                        x.Areas,
                        x.Controller,
                        x.Action,
                        x.Status,
                        x.Icon
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
        [HttpPost("/ManagementMenu/LoadMenu")]
        public JsonResult LoadMenu()
        {
            try
            {
                string sql = "[dbo].[Management_Menu_GetListMenu]";
                var data = _context.ManagementMenu_LoadMenu.FromSqlRaw(sql).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Json(new { success = false, data = (object)null });
            }
        }

        [DynamicAuthorize(RequiredPermission = EAction.Insert)]
        [HttpPost("/ManagementMenu/Insert")]
        public object Insert()
        {
            var data = Request.Form["submit"];
            ManagementMenu obj = JsonConvert.DeserializeObject<ManagementMenu>(data);
            try
            {
                string sql = "[dbo].[Management_Menu_Insert]" +
                    "@ParentId," +
                    "@MenuName," +
                    "@Areas," +
                    "@Controller," +
                    "@Icon," +
                    "@SortOrder," +
                    "@Status," +
                    "@ReturnCode OUT";

                var _ParentId = new SqlParameter("@ParentId", SqlDbType.Int);
                _ParentId.Value = CheckNullParameterStore(_ParentId, obj.ParentId);

                var _MenuName = new SqlParameter("@MenuName", SqlDbType.NVarChar);
                _MenuName.Value = CheckNullParameterStore(_MenuName, obj.MenuName);

                var _Areas = new SqlParameter("@Areas", SqlDbType.NVarChar);
                _Areas.Value = CheckNullParameterStore(_Areas, obj.Areas);

                var _Controller = new SqlParameter("@Controller", SqlDbType.NVarChar);
                _Controller.Value = CheckNullParameterStore(_Controller, obj.Controller);

                var _Icon = new SqlParameter("@Icon", SqlDbType.NVarChar, 100);
                _Icon.Value = CheckNullParameterStore(_Icon, obj.Icon);

                var _SortOrder = new SqlParameter("@SortOrder", SqlDbType.NVarChar, 100);
                _SortOrder.Value = CheckNullParameterStore(_SortOrder, obj.SortOrder);

                var _Status = new SqlParameter("@Status", SqlDbType.Int);
                _Status.Value = CheckNullParameterStore(_Status, obj.Status);

                var ReturnCode = new SqlParameter("@ReturnCode", SqlDbType.NVarChar, 1000) { Direction = System.Data.ParameterDirection.Output };
                ReturnCode.Value = "";

                var rs = _context.Database.ExecuteSqlRaw(sql, 
                    _ParentId,
                    _MenuName,
                    _Areas,
                    _Controller,
                    _Icon,
                    _SortOrder,
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

        [HttpPost("/ManagementMenu/GetItem")]
        public IActionResult GetItem([FromBody] DataCustom request)
        {
            try
            {
                var data = _context.ManagementMenu.Where(x => x.Id == request.Id).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                return (IActionResult)Logger.LogError(ex);
            }

        }

        [DynamicAuthorize(RequiredPermission = EAction.Edit)]
        [HttpPost("/ManagementMenu/Update")]
        public object Update()
        {
            var data = Request.Form["submit"];
            ManagementMenu obj = JsonConvert.DeserializeObject<ManagementMenu>(data);
            try
            {
                string sql = "[dbo].[Management_Menu_Update]" +
                    "@Id," +
                    "@ParentId," +
                    "@MenuName," +
                    "@Areas," +
                    "@Controller," +
                    "@Icon," +
                    "@SortOrder," +
                    "@Status," +
                    "@ReturnCode OUT";

                var _Id = new SqlParameter("@Id", SqlDbType.Int);
                _Id.Value = CheckNullParameterStore(_Id, obj.Id);

                var _ParentId = new SqlParameter("@ParentId", SqlDbType.Int);
                _ParentId.Value = CheckNullParameterStore(_ParentId, obj.ParentId);

                var _MenuName = new SqlParameter("@MenuName", SqlDbType.NVarChar);
                _MenuName.Value = CheckNullParameterStore(_MenuName, obj.MenuName);

                var _Areas = new SqlParameter("@Areas", SqlDbType.NVarChar);
                _Areas.Value = CheckNullParameterStore(_Areas, obj.Areas);

                var _Controller = new SqlParameter("@Controller", SqlDbType.NVarChar);
                _Controller.Value = CheckNullParameterStore(_Controller, obj.Controller);

                var _Icon = new SqlParameter("@Icon", SqlDbType.NVarChar, 100);
                _Icon.Value = CheckNullParameterStore(_Icon, obj.Icon);

                var _SortOrder = new SqlParameter("@SortOrder", SqlDbType.NVarChar, 100);
                _SortOrder.Value = CheckNullParameterStore(_SortOrder, obj.SortOrder);

                var _Status = new SqlParameter("@Status", SqlDbType.Int);
                _Status.Value = CheckNullParameterStore(_Status, obj.Status);

                var ReturnCode = new SqlParameter("@ReturnCode", SqlDbType.NVarChar, 1000) { Direction = System.Data.ParameterDirection.Output };
                ReturnCode.Value = "";

                var rs = _context.Database.ExecuteSqlRaw(sql,
                    _Id,
                    _ParentId,
                    _MenuName,
                    _Areas,
                    _Controller,
                    _Icon,
                    _SortOrder,
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
        [HttpPost("/ManagementMenu/Delete")]
        public object Delete([FromBody] DataCustom request)
        {

            try
            {
                var record = _context.ManagementMenu.FirstOrDefault(x => x.Id == request.Id);
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
