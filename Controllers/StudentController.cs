using Microsoft.AspNetCore.Mvc;
using DemoMVC.Models;

namespace DemoMVC.Controllers
{
    public class StudentController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Message = "Trang quản lý sinh viên";
            return View();
        }

        // Hiển thị Form nhập liệu
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Nhận dữ liệu từ Form gửi lên dạng Model Student và truyền sang trang Result
        [HttpPost]
        public IActionResult Create(Student student)
        {
            return View("Result", student);
        }
    }
} // <-- Thêm dấu đóng ngoặc nhọn này để sửa dứt điểm lỗi CS1513
