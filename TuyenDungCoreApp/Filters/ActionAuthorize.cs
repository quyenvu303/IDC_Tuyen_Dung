using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TuyenDungCoreApp.Filters
{
    public class ActionAuthorize : ActionFilterAttribute, IActionFilter
    {
        public List<string> Actions { get; set; }
        public string Resource { get; set; }
        

    }
    public static partial class ExtensionMethods
    {
        public static bool IsAjaxRequest(this Microsoft.AspNetCore.Http.HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }
    }
}
