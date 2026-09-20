using Microsoft.AspNetCore.Mvc;
using PTPMQL_MVC.Models;

namespace PTPMQL_MVC.Controllers
{
    public class StudentController : Controller
    {
        // Tạo một danh sách tĩnh (static) lưu trong bộ nhớ RAM để giữ dữ liệu sinh viên
        private static List<Student> _studentList = new List<Student>();

        // GET: Student
        public IActionResult Index()
        {
            ViewBag.Message = "Trang quản lý sinh viên";
            
            // Truyền danh sách sinh viên hiện có sang trang danh sách (Index.cshtml)
            return View(_studentList);
        }

        // GET: Student/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (student != null)
            {
                // Thêm sinh viên mới vừa nhập từ form vào danh sách tạm thời
                _studentList.Add(student);
            }

            // Trả về trực tiếp trang Result.cshtml để hiển thị thông tin sinh viên vừa nhập
            return View("Result", student);
        }
    }
}
