using Microsoft.AspNetCore.Mvc;
using PTPMQL_MVC.Models; // Đồng bộ với tên dự án thực tế

namespace PTPMQL_MVC.Controllers
{
    public class ProductController : Controller
    {
        // Sử dụng một biến static để lưu thông tin sản phẩm tạm thời trên RAM
        private static Product _currentProduct = new Product
        {
            Name = "Ổ cắm điện Panasonic",
            Price = 150000,
            Category = "Thiết bị điện"
        };

        // GET: Product
        public IActionResult Index()
        {
            // Cấu hình dữ liệu truyền qua ViewBag và ViewData
            ViewBag.PageTitle = "Danh mục thiết bị điện";
            ViewBag.WelcomeMessage = "Chào mừng bạn đến với cửa hàng thiết bị điện";

            ViewData["StoreName"] = "Điện Việt Store";
            ViewData["Contact"] = "Hotline: 0917 333 000";

            // Truyền đối tượng sản phẩm hiện tại sang giao diện hiển thị
            return View(_currentProduct);
        }

        // POST: Product/Add
        [HttpPost]
        public IActionResult Add(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                // Cập nhật lại tên sản phẩm hiển thị trên màn hình
                _currentProduct.Name = name;
                _currentProduct.Price = 0; // Giá mặc định cho sản phẩm mới thêm nhanh
                _currentProduct.Category = "Thiết bị điện";

                // Sử dụng TempData lưu trạng thái thông báo
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                TempData["AddedProduct"] = name;
            }

            return RedirectToAction("Index");
        }
    }
}
