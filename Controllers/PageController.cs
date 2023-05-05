using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QTComputer.Models;

namespace QTComputer.Controllers
{
    public class PageController : Controller
    {
        private readonly DbComputerContext _context;

        public PageController(DbComputerContext context)
        {
            _context = context;
        }

        [Route("/page/{id}", Name = "PageDetails")]
        public IActionResult Details(int? id)
        {
            if (id==null) return RedirectToAction("Index", "Home");

            var page = _context.Pages.AsNoTracking().SingleOrDefault(x => x.PageId == id);
            if (page == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(page);
        }

    }
}
