using Microsoft.AspNetCore.Mvc;
using QTComputer.Extension;
using QTComputer.ModelViews;

namespace QTComputer.Controllers.Components
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            int soluongsanpham = 0;
            if (cart != null) soluongsanpham = cart.Count();
            return View(cart);
        }
    }
}
