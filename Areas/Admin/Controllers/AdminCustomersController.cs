using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using QTComputer.Helper;
using QTComputer.Models;

namespace QTComputer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCustomersController : Controller
    {
        private readonly DbComputerContext _context;
        public INotyfService _notifyService { get; }

        public AdminCustomersController(DbComputerContext context, INotyfService notifyService)
        {
            _notifyService = notifyService;

            _context = context;
        }

        // GET: Admin/AdminCustomers
        public IActionResult Index(int? page)
        {
            var lsCustomers = _context.Customers
                .AsNoTracking()
                .OrderBy(x => x.CreateDate);
            List<Customer> models = new List<Customer>(lsCustomers);
            return View(models);
        }

        // GET: Admin/AdminCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/AdminCustomers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FullName,Birthday,Avatar,Address,Email,Phone,CreateDate,Password,LastLogin,Active")] Customer customer, IFormFile? fAvatar)
        {
            if (ModelState.IsValid)
            {
                if (fAvatar != null)
                {
                    string extension = Path.GetExtension(fAvatar.FileName);
                    string imageName = Utilities.SEOUrl(customer.FullName) + extension;
                    customer.Avatar = await Utilities.UploadFile(fAvatar, @"customer", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(customer.Avatar)) customer.Avatar = "default.jpg";
                _context.Add(customer);
                await _context.SaveChangesAsync();
                _notifyService.Success("THÊM MỚI THÀNH CÔNG");
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Admin/AdminCustomers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/AdminCustomers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FullName,Birthday,Avatar,Address,Email,Phone,CreateDate,Password,LastLogin,Active")] Customer customer, IFormFile? fAvatar)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (fAvatar != null)
                    {
                        string extension = Path.GetExtension(fAvatar.FileName);
                        string imageName = Utilities.SEOUrl(customer.FullName) + extension;
                        customer.Avatar = await Utilities.UploadFile(fAvatar, @"customer", imageName.ToLower());
                    }
                    if (string.IsNullOrEmpty(customer.Avatar)) customer.Avatar = "default.jpg";

                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Chỉnh sửa thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Admin/AdminCustomers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return PartialView("Delete",customer);
        }

        // POST: Admin/AdminCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'DbComputerContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
