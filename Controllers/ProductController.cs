using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using QTComputer.Helper;
using QTComputer.Models;

namespace QTComputer.Controllers
{
    public class ProductController : Controller
    {
        private readonly DbComputerContext _context;

        public ProductController(DbComputerContext context)
        {
            _context = context;
        }
        [Route("/san-pham", Name = "Product")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = Utilities.PAGE_SIZE;
                var lsProducts = _context.Products
                    .AsNoTracking()
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(lsProducts, pageNumber, pageSize);

                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("danhmuc/{Alias}", Name = ("ListProduct"))]
        public IActionResult List(string alias, int page = 1)
        {
            try
            {
                var pageSize = 10;
                var danhMuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == alias);

                var lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == danhMuc.CatId)
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(lsProducts, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = danhMuc;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }


        }
        [Route("/{Alias}-{id}", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    RedirectToAction("Index");
                }

                var lsProduct = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == product.CatId && x.ProductId != id && x.Active == true)
                    .Take(4)
                    .OrderByDescending(x => x.DateCreated)
                    .ToList();
                ViewBag.SanPham = lsProduct;

                return View(product);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }


        }
    }
}
