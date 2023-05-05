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
    public class AdminSuppliersController : Controller
    {
        private readonly DbComputerContext _context;
        public INotyfService _notifyService { get; }
        public AdminSuppliersController(DbComputerContext context, INotyfService notifyService)
        {
            _notifyService = notifyService;
            _context = context;
        }

        // GET: Admin/AdminSuppliers
        public IActionResult Index()
        {
            var lsSuppliers = _context.Suppliers
                .AsNoTracking()
                .OrderBy(x => x.SupplierId);
            List<Supplier> models = new List<Supplier>(lsSuppliers);
            return View(models);
        }

        // GET: Admin/AdminSuppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var Supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (Supplier == null)
            {
                return NotFound();
            }

            return View(Supplier);
        }

        // GET: Admin/AdminSuppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminSuppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,CompanyName,Address,Phone")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {

                _context.Add(supplier);
                await _context.SaveChangesAsync();
                _notifyService.Success("THÊM MỚI THÀNH CÔNG");
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Admin/AdminSuppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var Supplier = await _context.Suppliers.FindAsync(id);
            if (Supplier == null)
            {
                return NotFound();
            }
            return View(Supplier);
        }

        // POST: Admin/AdminSuppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierId,CompanyName,Address,Phone")] Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Chỉnh sửa thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierId))
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
            return View(supplier);
        }

        // GET: Admin/AdminSuppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return PartialView("Delete", supplier);
        }

        // POST: Admin/AdminSuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return (_context.Suppliers?.Any(e => e.SupplierId == id)).GetValueOrDefault();
        }
    }
}
