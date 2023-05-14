using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QTComputer.Helper;
using QTComputer.Models;
using QTComputer.ModelView;
using System.Linq;
using X.PagedList;

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
        public IActionResult Index(string? keyword, int? page, string sortOrder, int? categoryId, int? supplierId)
        {
            try
            {
                var pageNumber = page ?? 1;
                var lsProducts = _context.Products.AsNoTracking();
                if (!String.IsNullOrEmpty(keyword))
                {
                    lsProducts = lsProducts.Where(x => x.ProductName.Contains(keyword));
                }
                if (categoryId != null)
                {
                    lsProducts = lsProducts.Where(p => p.CatId == categoryId);
                }
                if (supplierId != null)
                {
                    lsProducts = lsProducts.Where(p => p.SupplierId == supplierId);
                    ViewBag.Suppliers = _context.Suppliers.ToList();
                }
                lsProducts = SortProducts(lsProducts, sortOrder);
                var totalProducts = lsProducts.Count();
                ViewBag.TotalProducts = totalProducts;
                var categories = _context.Categories.ToList();
                var selectedCategories = new List<int>(); // danh sách các danh mục đã chọn ban đầu là rỗng
                if (categoryId != null)
                {
                    selectedCategories.Add((int)categoryId); // thêm danh mục hiện tại vào danh sách các danh mục đã chọn
                }
                var models = new Tuple<CategoryViewModel, IPagedList<Product>>(
                    new CategoryViewModel
                    {
                        Categories = categories,
                        SelectedCategories = selectedCategories
                    },
                    lsProducts.ToPagedList(pageNumber, 9)
                );

                return View(models);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        private IQueryable<Product> SortProducts(IQueryable<Product> products, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "date_asc":
                    products = products.OrderBy(p => p.DateCreated);
                    break;
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderByDescending(p => p.DateCreated);
                    break;
            }
            return products;
        }

        [Route("danhmuc/{id}", Name = ("ListProduct"))]
        public IActionResult List(int id, int? page, string? keyword, string sortOrder, int? categoryId, int? supplierId)
        {
            //try
            //{
            //    var pageNumber = page ?? 1;
            //    var danhMuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.CatId == id);

            //    var lsProducts = _context.Products
            //        .AsNoTracking()
            //        .Where(x => x.CatId == danhMuc.CatId)
            //        .OrderByDescending(x => x.DateCreated);
            //    var models = lsProducts.ToPagedList(pageNumber, 10); // lấy 10 sản phẩm trên 1 trang
            //    ViewBag.CurrentCat = danhMuc;
            //    return View(models);
            //}
            //catch
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            try
            {
                var pageNumber = page ?? 1;
                var lsProducts = _context.Products.AsNoTracking();
                if (!String.IsNullOrEmpty(keyword))
                {
                    lsProducts = lsProducts.Where(x => x.ProductName.Contains(keyword));
                }
                if (categoryId != null)
                {
                    lsProducts = lsProducts.Where(p => p.CatId == categoryId);
                }
                if (supplierId != null)
                {
                    lsProducts = lsProducts.Where(p => p.SupplierId == supplierId);
                    ViewBag.Suppliers = _context.Suppliers.ToList();
                }
                lsProducts = SortProducts(lsProducts, sortOrder);
                var totalProducts = lsProducts.Count();
                ViewBag.TotalProducts = totalProducts;
                var categories = _context.Categories.ToList();
                var selectedCategories = new List<int>(); // danh sách các danh mục đã chọn ban đầu là rỗng
                if (categoryId != null)
                {
                    selectedCategories.Add((int)categoryId); // thêm danh mục hiện tại vào danh sách các danh mục đã chọn
                }
                var models = new Tuple<CategoryViewModel, IPagedList<Product>>(
                    new CategoryViewModel
                    {
                        Categories = categories,
                        SelectedCategories = selectedCategories
                    },
                    lsProducts.ToPagedList(pageNumber, 9)
                );

                return View(models);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("/san-pham/{id}", Name = "ProductDetails")]
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
