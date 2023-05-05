using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using QTComputer.Helper;
using QTComputer.Models;

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
        public IActionResult Index()
        {
            var lsNews = _context.News
                .AsNoTracking()
                .OrderBy(x => x.PostId);
            List<News> models = new List<News>(lsNews);
            return View(models);

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