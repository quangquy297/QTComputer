using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using QTComputer.Helper;
using QTComputer.Models;

namespace QTComputer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly DbComputerContext _context;
        public INotyfService _notifyService { get; }

        public AdminProductsController(DbComputerContext context, INotyfService notifyService)
        {
            _notifyService = notifyService;
            _context = context;
        }

        // GET: Admin/AdminProducts
        public IActionResult Index(/*int CatID = 0*/)
        {
            List<Product> lsProducts = new List<Product>();

            //if(CatID != 0)
            //{
            //    lsProducts = _context.Products
            //    .AsNoTracking()
            //    .Where(x=>x.CatId==CatID)
            //    .Include(x => x.Cat)
            //    .OrderByDescending(x => x.ProductId).ToList();
            //}
            //else
            //{
                lsProducts = _context.Products
                .AsNoTracking()
                .Include(x => x.Cat)
                .Include(x => x.Supplier)
                .OrderByDescending(x => x.ProductId).ToList();
            //}
            List<Product> models = new List<Product>(lsProducts.AsQueryable());
            //ViewBag.CurrentCateID = CatID;
            //ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);

            return View(models);
        }
        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["NhaCungCap"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Thumb,DateCreated,DateModified,Title,UnitsInStock,SupplierId")] Product product, Microsoft.AspNetCore.Http.IFormFile? fThumb)
        {
            if (ModelState.IsValid)
            {
                product.ProductName = Utilities.ToTitleCase(product.ProductName);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.ProductName) + extension;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                product.DateModified = DateTime.Now;
                product.DateCreated = DateTime.Now;

                _context.Add(product);
                await _context.SaveChangesAsync();
                _notifyService.Success("THÊM MỚI THÀNH CÔNG");
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            ViewData["NhaCungCap"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");

            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Thumb,DateCreated,DateModified,Title,UnitsInStock,SupplierId")] Product product, Microsoft.AspNetCore.Http.IFormFile? fThumb)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.ProductName = Utilities.ToTitleCase(product.ProductName);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(product.ProductName) + extension;
                        product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                    product.DateModified = DateTime.Now;

                    _context.Update(product);
                    _notifyService.Success("CẬP NHẬT THÀNH CÔNG");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return PartialView("Delete", product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _notifyService.Success("XÓA THÀNH CÔNG");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}