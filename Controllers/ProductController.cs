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
        public IActionResult Index(string? keyword)
        {
            try
            {
                var lsProducts = _context.Products
                    .AsNoTracking()
                    .OrderByDescending(x => x.DateCreated);
                if (!String.IsNullOrEmpty(keyword))
                {
                     lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(x => x.ProductName.Contains(keyword))
                    .OrderByDescending(x => x.DateCreated);
                }
                List<Product> models = new List<Product>(lsProducts);
                return View(models);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("danhmuc/{Alias}", Name = ("ListProduct"))]
        public IActionResult List(string title)
        {
            try
            {
                var danhMuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Title == title);

                var lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == danhMuc.CatId)
                    .OrderByDescending(x => x.DateCreated);
                List<Product> models = new List<Product>(lsProducts);
                
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
                    .Where(x => x.CatId == product.CatId && x.ProductId != id)
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
