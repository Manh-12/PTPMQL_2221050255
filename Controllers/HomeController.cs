using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DemoMvc.Models; // Đổi từ DemoMVC thành DemoMvc

namespace DemoMvc.Controllers; // Đổi từ DemoMVC thành DemoMvc

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
