using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using QTComputer.Models;

namespace QTComputer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SalesController : Controller
    {
        private readonly DbComputerContext _context;
        public INotyfService _notyfService { get; }
        public SalesController(DbComputerContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(DateTime fromDate, DateTime toDate)
        {
            // Truy vấn dữ liệu từ CSDL
            var orders = _context.OrderDetails
                .Where(o => o.CreateDate >= fromDate && o.CreateDate <= toDate)
                .ToList();

            // Tính tổng số tiền bán được
            int totalSales = (int)orders.Sum(o => o.Price * o.Amount);

            // Truyền dữ liệu vào view để hiển thị
            ViewBag.FromDate = fromDate.ToShortDateString();
            ViewBag.ToDate = toDate.ToShortDateString();
            ViewBag.TotalSales = totalSales;

            return View();
        }
    }
}