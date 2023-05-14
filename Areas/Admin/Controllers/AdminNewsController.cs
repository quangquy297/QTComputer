using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QTComputer.Helper;
using QTComputer.Models;

namespace QTComputer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminNewsController : Controller
    {
        private readonly DbComputerContext _context;
        public INotyfService _notifyService { get; }
        public AdminNewsController(DbComputerContext context, INotyfService notifyService)
        {
            _notifyService = notifyService;
            _context = context;
        }

        // GET: Admin/AdminNews
        public IActionResult Index()
        {
            var lsNews = _context.News
                .AsNoTracking()
                .OrderBy(x => x.PostId);
            List<News> models = new List<News>(lsNews);

            return View(models);
        }

        // GET: Admin/AdminNews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: Admin/AdminNews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminNews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountId,Tags,CatId,Views")] News news, IFormFile? fThumb)
        {
            if (ModelState.IsValid)
            {
                news.Title = Utilities.ToTitleCase(news.Title);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(news.Title) + extension;
                    news.Thumb = await Utilities.UploadFile(fThumb, @"news", image.ToLower());
                }
                if (string.IsNullOrEmpty(news.Thumb)) news.Thumb = "default.png";
                news.CreatedDate = DateTime.Now;
                _context.Add(news);
                await _context.SaveChangesAsync();
                _notifyService.Success("THÊM MỚI THÀNH CÔNG");
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: Admin/AdminNews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: Admin/AdminNews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountId,Tags,CatId,Views")] News news, IFormFile? fThumb)
        {
            if (id != news.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    news.Title = Utilities.ToTitleCase(news.Title);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(news.Title) + extension;
                        news.Thumb = await Utilities.UploadFile(fThumb, @"news", image.ToLower());
                    }

                    _context.Update(news);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("CẬP NHẬT THÀNH CÔNG");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.PostId))
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
            return View(news);
        }

        // GET: Admin/AdminNews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (news == null)
            {
                return NotFound();
            }

            return PartialView("Delete",news);
        }

        // POST: Admin/AdminNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'DbComputerContext.News'  is null.");
            }
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }
            
            await _context.SaveChangesAsync();
            _notifyService.Success("XÓA THÀNH CÔNG");
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
          return (_context.News?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
