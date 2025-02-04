using Microsoft.AspNetCore.Mvc;

namespace TuyenDungWebsite.Areas.IDC.Controllers
{
    [Area("IDC")]
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
