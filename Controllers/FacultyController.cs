using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTPMQL_MVC.Data;
using PTPMQL_MVC.Models;
namespace PTPMQL_MVC.Controllers;
public class FacultyController : Controller
{
    private readonly ApplicationDbContext _db;
    public FacultyController(ApplicationDbContext db) => _db=db;
    public async Task<IActionResult> Index() => View(await _db.Faculties.Include(f=>f.Majors).OrderBy(f=>f.FacultyName).ToListAsync());
    [HttpGet] public IActionResult Create() => View();
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Create(Faculty model)
    {
        if(!ModelState.IsValid) return View(model);
        if(await _db.Faculties.AnyAsync(f=>f.FacultyId==model.FacultyId)){ModelState.AddModelError("FacultyId","Mã khoa đã tồn tại.");return View(model);}
        _db.Faculties.Add(model); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
}
