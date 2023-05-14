using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QTComputer.Helper;
using QTComputer.Models;
using X.PagedList;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QTComputer.Controllers
{
    public class BlogController : Controller
    {
        private readonly DbComputerContext _context;
        public BlogController(DbComputerContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        [Route("tin-tuc", Name = ("Blog"))]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page ?? 1;
            var lsNews = _context.News
                .AsNoTracking()
                .OrderBy(x => x.PostId);
            var models = lsNews.ToPagedList(pageNumber, 10);
            return View(models);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [Route("/tin-tuc/{id}", Name = "TinChiTiet")]
        public IActionResult Details(int id)
        {
            var news = _context.News.AsNoTracking().SingleOrDefault(x => x.PostId == id);
            if (news == null)
            {
                return RedirectToAction("Index");
            }
            return View(news);
        }
    }
}