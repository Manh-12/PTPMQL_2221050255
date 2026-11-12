
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PTPMQL_MVC.Data;
using PTPMQL_MVC.Models;

namespace PTPMQL_MVC.Controllers
{
    public class StudentController : Controller
    {
        // ==================================================
        // 1. KẾT NỐI DATABASE
        // ==================================================

        private readonly ApplicationDbContext _db;

        public StudentController(ApplicationDbContext db)
        {
            _db = db;
        }


        // ==================================================
        // 2. HIỂN THỊ DANH SÁCH SINH VIÊN
        // URL: /Student/Index
        // ==================================================

        public async Task<IActionResult> Index(string? keyword, int page = 1, int pageSize = 10)
        {
            var students = _db.Students
                .Include(s => s.Major)
                    .ThenInclude(m => m!.Faculty)
                .Include(s => s.TrainingSystem)
                .AsQueryable();

            // Tìm kiếm theo mã sinh viên hoặc họ tên
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                students = students.Where(s =>
                    s.StudentId.Contains(keyword) ||
                    s.FullName.Contains(keyword));
            }

            // Sắp xếp theo mã sinh viên giảm dần
            students = students.OrderByDescending(s => s.StudentId);

            pageSize = pageSize is 10 or 20 or 50 ? pageSize : 10;
            var totalCount = await students.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            page = Math.Clamp(page, 1, totalPages);

            ViewBag.Keyword = keyword;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;

            return View(await students
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync());
        }


        // ==================================================
        // 3. NẠP DỮ LIỆU CHO DROPDOWN
        // ==================================================

        private async Task LoadOptions()
        {
            // Danh sách hệ đào tạo
            ViewBag.TrainingSystems = await _db.TrainingSystems
                .OrderBy(t => t.SystemCode)
                .ToListAsync();

            // Danh sách ngành học và khoa
            ViewBag.Majors = await _db.Majors
                .Include(m => m.Faculty)
                .OrderBy(m => m.MajorCode)
                .ToListAsync();
        }


        // ==================================================
        // 4. HIỂN THỊ FORM THÊM SINH VIÊN
        // URL: /Student/Create
        // ==================================================

        [HttpGet]
        public async Task<IActionResult> Create(bool modal = false)
        {
            await LoadOptions();

            var student = new Student
            {
                AdmissionYear = DateTime.Now.Year,
                SystemCode = "21",
                MajorCode = "05"
            };

            if (modal)
            {
                return PartialView("_CreateStudentForm", student);
            }

            return View(student);
        }


        // ==================================================
        // 5. XỬ LÝ THÊM SINH VIÊN
        // URL: POST /Student/Create
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, string? keyword)
        {
            // Mã sinh viên được tự động tạo
            ModelState.Remove(nameof(Student.StudentId));

            // Kiểm tra dữ liệu đầu vào
            if (!ModelState.IsValid)
            {
                await LoadOptions();
                if (IsAjaxRequest())
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return PartialView("_CreateStudentForm", student);
                }

                return View(student);
            }

            // Kiểm tra năm tuyển sinh
            if (student.AdmissionYear < 2000 ||
                student.AdmissionYear > 2099)
            {
                ModelState.AddModelError(
                    nameof(student.AdmissionYear),
                    "Năm tuyển sinh phải từ 2000 đến 2099."
                );

                await LoadOptions();
                if (IsAjaxRequest())
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return PartialView("_CreateStudentForm", student);
                }

                return View(student);
            }

            // Kiểm tra hệ đào tạo có tồn tại không
            bool systemExists = await _db.TrainingSystems
                .AnyAsync(t => t.SystemCode == student.SystemCode);

            if (!systemExists)
            {
                ModelState.AddModelError(
                    nameof(student.SystemCode),
                    "Hệ đào tạo không tồn tại."
                );

                await LoadOptions();
                if (IsAjaxRequest())
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return PartialView("_CreateStudentForm", student);
                }

                return View(student);
            }

            // Kiểm tra ngành học có tồn tại không
            bool majorExists = await _db.Majors
                .AnyAsync(m => m.MajorCode == student.MajorCode);

            if (!majorExists)
            {
                ModelState.AddModelError(
                    nameof(student.MajorCode),
                    "Ngành học không tồn tại."
                );

                await LoadOptions();
                if (IsAjaxRequest())
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return PartialView("_CreateStudentForm", student);
                }

                return View(student);
            }


            // ==================================================
            // 5.1. TẠO TIỀN TỐ MÃ SINH VIÊN
            //
            // Cấu trúc:
            // YY + SystemCode + MajorCode + XXXX
            //
            // Ví dụ:
            // 22 + 21 + 05 + 0255
            // Kết quả: 2221050255
            // ==================================================

            string prefix =
                (student.AdmissionYear % 100).ToString("D2") +
                student.SystemCode +
                student.MajorCode;


            // ==================================================
            // 5.2. TÌM SỐ THỨ TỰ LỚN NHẤT ĐÃ SỬ DỤNG
            // ==================================================

            var existingIds = await _db.Students
                .Where(s => s.StudentId.StartsWith(prefix))
                .Select(s => s.StudentId)
                .ToListAsync();

            // Bắt đầu từ 0255 để khớp mã ví dụ
            int maxNumber = 254;

            foreach (string existingId in existingIds)
            {
                // Mã sinh viên phải có đúng 10 ký tự
                if (existingId.Length != 10)
                {
                    continue;
                }

                // Lấy 4 ký tự cuối làm số thứ tự
                string numberPart = existingId.Substring(6, 4);

                if (int.TryParse(numberPart, out int number))
                {
                    if (number > maxNumber)
                    {
                        maxNumber = number;
                    }
                }
            }


            // ==================================================
            // 5.3. TẠO MÃ SINH VIÊN MỚI
            // ==================================================

            int nextNumber = maxNumber + 1;

            if (nextNumber > 9999)
            {
                ModelState.AddModelError(
                    "",
                    "Đã hết số thứ tự sinh viên cho năm, hệ và ngành này."
                );

                await LoadOptions();
                if (IsAjaxRequest())
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return PartialView("_CreateStudentForm", student);
                }

                return View(student);
            }

            student.StudentId = prefix + nextNumber.ToString("D4");


            // ==================================================
            // 5.4. KIỂM TRA MÃ SINH VIÊN TRÙNG
            // ==================================================

            bool idExists = await _db.Students
                .AnyAsync(s => s.StudentId == student.StudentId);

            if (idExists)
            {
                ModelState.AddModelError(
                    "",
                    "Mã sinh viên đã tồn tại. Vui lòng thử lại."
                );

                await LoadOptions();
                if (IsAjaxRequest())
                {
                    Response.StatusCode = StatusCodes.Status409Conflict;
                    return PartialView("_CreateStudentForm", student);
                }

                return View(student);
            }


            // ==================================================
            // 5.5. LƯU SINH VIÊN VÀO DATABASE
            // ==================================================

            try
            {
                _db.Students.Add(student);

                await _db.SaveChangesAsync();

                if (IsAjaxRequest())
                {
                    var matchingStudents = _db.Students.AsQueryable();
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        keyword = keyword.Trim();
                        matchingStudents = matchingStudents.Where(s =>
                            s.StudentId.Contains(keyword) ||
                            s.FullName.Contains(keyword));
                    }

                    return Json(new
                    {
                        success = true,
                        message = "Thêm sinh viên thành công!",
                        totalCount = await matchingStudents.CountAsync(),
                        matchesSearch = string.IsNullOrWhiteSpace(keyword) ||
                            student.StudentId.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            student.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase),
                        student = new
                        {
                            student.StudentId,
                            student.FullName,
                            student.Address,
                            majorName = await _db.Majors
                                .Where(m => m.MajorCode == student.MajorCode)
                                .Select(m => m.MajorName)
                                .FirstAsync(),
                            facultyName = await _db.Majors
                                .Where(m => m.MajorCode == student.MajorCode)
                                .Select(m => m.Faculty!.FacultyName)
                                .FirstAsync(),
                            trainingSystemName = await _db.TrainingSystems
                                .Where(t => t.SystemCode == student.SystemCode)
                                .Select(t => t.SystemName)
                                .FirstAsync()
                        }
                    });
                }

                // Lưu thông báo thành công
                TempData["Success"] = "Thêm sinh viên thành công!";

                // Trả về trang Result.cshtml
                return View("Result", student);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    "",
                    "Không thể lưu sinh viên. Vui lòng kiểm tra dữ liệu và thử lại."
                );

                await LoadOptions();
                if (IsAjaxRequest())
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return PartialView("_CreateStudentForm", student);
                }

                return View(student);
            }
        }

        private bool IsAjaxRequest()
        {
            return string.Equals(
                Request.Headers.XRequestedWith,
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase);
        }


        // ==================================================
        // 6. HIỂN THỊ FORM SỬA SINH VIÊN
        // URL: /Student/Edit/{id}
        // ==================================================

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var student = await _db.Students
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            await LoadOptions();

            return View(student);
        }


        // ==================================================
        // 7. XỬ LÝ CẬP NHẬT SINH VIÊN
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            // Mã sinh viên không được thay đổi
            ModelState.Remove(nameof(Student.StudentId));

            // Hệ đào tạo và ngành học không sửa trong chức năng này
            ModelState.Remove(nameof(Student.SystemCode));
            ModelState.Remove(nameof(Student.MajorCode));

            if (!ModelState.IsValid)
            {
                await LoadOptions();
                return View(student);
            }

            var studentInDb = await _db.Students
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (studentInDb == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin sinh viên
            studentInDb.FullName = student.FullName;
            studentInDb.Address = student.Address;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Cập nhật sinh viên thành công!";

            return RedirectToAction(nameof(Index));
        }


        // ==================================================
        // 8. XÓA SINH VIÊN
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var student = await _db.Students
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            _db.Students.Remove(student);

            await _db.SaveChangesAsync();

            TempData["Success"] = "Xóa sinh viên thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}
