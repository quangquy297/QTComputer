using Microsoft.AspNetCore.Mvc;

namespace QTComputer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AdminLogin", "AdminLogin");
            }
            return View();
        }
    }
}
