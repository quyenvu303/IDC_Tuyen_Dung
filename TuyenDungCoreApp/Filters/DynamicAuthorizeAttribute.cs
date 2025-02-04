using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using TuyenDungCoreApp.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TuyenDungModel.DataTable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;

namespace TuyenDungCoreApp.Filters
{
    public class DynamicAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string RequiredPermission { get; set; } // Thuộc tính cho phép gán quyền cụ thể

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var user = context.HttpContext.User;

                // Kiểm tra nếu người dùng chưa đăng nhập
                if (!user.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                        { "Controller", "Login" },
                        { "Action", "Index" }
                    });
                    return;
                }

                // Lấy AdminDbContext từ RequestServices
                var dbContext = context.HttpContext.RequestServices.GetService(typeof(AdminDbContext)) as AdminDbContext;
                if (dbContext == null)
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                        { "Controller", "Login" },
                        { "Action", "Index" }
                    });
                    return;
                }

                // Lấy controller từ route
                var controller = context.RouteData.Values["controller"]?.ToString();

                // Lấy UserId từ Claims
                var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userIdClaim == null)
                {
                    context.Result = new ForbidResult();
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                        { "Controller", "Login" },
                        { "Action", "Index" }
                    });
                    return;
                }

                var user_name = userIdClaim.Value;
                if (string.IsNullOrEmpty(user_name) && string.IsNullOrEmpty(controller))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                        { "Controller", "Login" },
                        { "Action", "Index" }
                    });
                    return;
                }
                // Lấy quyền từ cơ sở dữ liệu
                var permissions = Get_Permission_User_Login(dbContext, user_name, controller);
                if (permissions != null && permissions.Count > 0)
                {
                    // Kiểm tra quyền cụ thể dựa trên RequiredPermission
                    var hasPermission = RequiredPermission switch
                    {
                        "FullControl" => permissions.Any(p => p.FullControl == 1),
                        "Access" => permissions.Any(p => p.Access == 1),
                        "View" => permissions.Any(p => p.View == 1),
                        "Edit" => permissions.Any(p => p.Edit == 1),
                        "Insert" => permissions.Any(p => p.Insert == 1),
                        "Delete" => permissions.Any(p => p.Delete == 1),
                        _ => false
                    };

                    if (!hasPermission)
                    {
                        context.Result = new ForbidResult();
                    }
                }
                else
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                        { "Controller", "Login" },
                        { "Action", "Index" }
                    });
                    return;
                }
                
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        private List<ManagementPermission> Get_Permission_User_Login(AdminDbContext dbContext, string user_name, string controller)
        {
            try
            {
                string sql = "[dbo].[Get_Permission_User_Login] @User_name,@Controller";
                var _user_name = new SqlParameter("@User_name", SqlDbType.NVarChar) { Value = user_name };
                var _Controller = new SqlParameter("@Controller", SqlDbType.NVarChar, 500) { Value = controller };

                var results = dbContext.ManagementPermission.FromSqlRaw(sql, _user_name, _Controller).AsEnumerable().ToList();
                return results;
            }
            catch (Exception ex)
            {

                return new List<ManagementPermission>();
            }
        }
    }

}
