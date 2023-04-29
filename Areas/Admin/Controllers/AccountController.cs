using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QTComputer.Areas.Admin.Models;
using QTComputer.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QTComputer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly DbComputerContext _context;
        public INotyfService _notyfService { get; }
        public AccountController(DbComputerContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        [Route("admin-login", Name = "AdminLogin")]
        public IActionResult AdminLogin(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID != null) return RedirectToAction("Index", "Home", new { Area = "Admin" });
            
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("admin-login", Name = "AdminLogin")]
        public async Task<IActionResult> AdminLogin(LoginVM model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    Account acc = _context.Accounts
                        .AsNoTracking()
                    .Include(p => p.Role)
                    .SingleOrDefault(x => x.Email.Trim() == model.UserName);

                    if (acc == null)
                    {
                        _notyfService.Success("Thông tin đăng nhập chưa chính xác");
                    }
                    string pass = (model.Password.Trim());
                    // + kh.Salt.Trim()
                    if (acc.Password.Trim() != pass)
                    {
                        _notyfService.Success("Thông tin đăng nhập chưa chính xác");
                        return View(model);
                    }
                    //đăng nhập thành công

                    //ghi nhận thời gian đăng nhập
                    acc.LastLogin = DateTime.Now;
                    _context.Update(acc);
                    await _context.SaveChangesAsync();


                    var taikhoanID = HttpContext.Session.GetString("AccountId");
                    //identity
                    //luuw seccion
                    HttpContext.Session.SetString("AccountId", acc.AccountId.ToString());

                    //identity
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, acc.FullName),
                        new Claim(ClaimTypes.Email, acc.Email),
                        new Claim("AccountId", acc.AccountId.ToString()),
                        new Claim("RoleId", acc.RoleId.ToString()),
                        new Claim(ClaimTypes.Role, acc.Role.RoleName)
                    };
                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);



                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
            }
            catch
            {
                return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            }
            return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
        }
        [Route("admin-logout", Name = "AdminLogout")]
        public IActionResult AdminLogout()
        {
            try
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
                return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            }
            catch
            {
                return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            }
        }

    }
}