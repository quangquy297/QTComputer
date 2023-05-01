using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QTComputer.Models;

namespace QTComputer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly DbComputerContext _context;

        public SearchController(DbComputerContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                ls = _context.Products.AsNoTracking()
                                  .Include(a => a.Cat)
                                  .OrderByDescending(x => x.ProductName)
                                  .Take(10)
                                  .ToList();

                return PartialView("ListProductsSearchPartial", ls);
            }
            else
            {
                ls = _context.Products.AsNoTracking()
                                  .Include(a => a.Cat)
                                  .Where(x => x.ProductName.Contains(keyword))
                                  .OrderByDescending(x => x.ProductName)
                                  .Take(10)
                                  .ToList();
                if (ls == null)
                {
                    return PartialView("ListProductsSearchPartial", null);
                }
                else
                {
                    return PartialView("ListProductsSearchPartial", ls);
                }
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> SearchProduct(string searchString)
        {
            List<Product> ls = new List<Product>();
            if (_context.Products == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }

            var products = from p in _context.Products
                         select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Title!.Contains(searchString));
            }
            return View(await products.ToListAsync());

        }
    }
}
