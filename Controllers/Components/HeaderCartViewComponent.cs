using Microsoft.AspNetCore.Mvc;
using QTComputer.Extension;
using QTComputer.ModelViews;

namespace QTComputer.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}