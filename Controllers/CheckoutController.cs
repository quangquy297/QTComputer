﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QTComputer.Extension;
using QTComputer.Helper;
using QTComputer.Models;
using QTComputer.ModelViews;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QTComputer.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DbComputerContext _context;
        public INotyfService _notyfService { get; }
        public CheckoutController(DbComputerContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        [Route("checkout", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
            }
            ViewBag.GioHang = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            //Lay ra gio hang de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;

                khachhang.Address = muaHang.Address;
                _context.Update(khachhang);
                _context.SaveChanges();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // kiểm tra số lượng tồn trước khi tạo đơn hàng
                    foreach (var item in cart)
                    {
                        if (!CheckProductStock(item.product.ProductId, item.amount))
                        {
                            // Hiển thị thông báo lỗi nếu số lượng tồn không đủ
                            _notyfService.Error($"Sản phẩm {item.product.ProductName} không đủ số lượng tồn");
                            ViewBag.GioHang = cart;
                            return View(model);
                        }
                    }

                    //Khoi tao don hang
                    Order donhang = new Order();
                    donhang.CustomerId = model.CustomerId;
                    donhang.Address = model.Address;


                    donhang.OrderDate = DateTime.Now;
                    donhang.TransactStatusId = 1;//Don hang moi
                    donhang.Deleted = false;
                    donhang.Paid = false;
                    donhang.Note = Utilities.StripHTML(model.Note);
                    donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
                    _context.Add(donhang);
                    _context.SaveChanges();
                    //tao danh sach don hang

                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = donhang.OrderId;
                        orderDetail.ProductId = item.product.ProductId;
                        orderDetail.Amount = item.amount;
                        orderDetail.TotalMoney = donhang.TotalMoney;
                        orderDetail.Price = item.product.Price;
                        orderDetail.CreateDate = DateTime.Now;
                        _context.Add(orderDetail);

                        var sanpham = _context.Products.FirstOrDefault(p => p.ProductId == item.product.ProductId);
                        if (sanpham != null)
                        {
                            sanpham.UnitsInStock -= item.amount;
                            _context.Products.Update(sanpham);
                        }
                    }
                    
                    _context.SaveChanges();
                    //clear gio hang
                    HttpContext.Session.Remove("GioHang");
                    //Xuat thong bao
                    _notyfService.Success("Đặt hàng thành công");
                    //cap nhat thong tin khach hang
                    return RedirectToAction("Success");


                }
            }
            catch
            {
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewBag.GioHang = cart;
            return View(model);
        }
        private bool CheckProductStock(int productId, int amount)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null && product.UnitsInStock >= amount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("dat-hang-thanh-cong", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong" });
                }
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders
                    .Where(x => x.CustomerId == Convert.ToInt32(taikhoanID))
                    .OrderByDescending(x => x.OrderDate)
                    .FirstOrDefault();
                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FullName = khachhang.FullName;
                successVM.DonHangID = donhang.OrderId;
                successVM.Phone = khachhang.Phone;
                successVM.Address = khachhang.Address;
                return View(successVM);
            }
            catch
            {
                return View();
            }
        }
    }
}