using Microsoft.AspNetCore.Mvc;
using PTPMQL_MVC.Models;

namespace PTPMQL_MVC.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "Trang quản lý sinh viên";

            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Student student)
        {
            return View("Result", student);
        }
    }
}