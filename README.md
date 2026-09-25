Định tuyến trong MVC
Định tuyến (Routing) là cơ chế giúp ASP.NET Core MVC xác định URL của người dùng sẽ được xử lý bởi Controller và Action nào
MVC sẽ gọi bộ điều khiển (Controller) và các hành động bên trong (Action) thông qua URL
Logic định tuyến MVC sử dụng dạng: /Controller/Action/Parameters VD: https://localhost:5001/Home/Index trong đó: Home là Controller, Index là Action
Định tuyến được cấu hình trong file Program.cs: app.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
ViewBag, ViewData, TempData
ViewBag, ViewData và TempData đều được dùng để truyền dữ liệu giữa Controller và View, nhưng phạm vi lưu trữ và thời gian tồn tại của chúng khác nhau

ViewBag là 1 đối tượng động dùng để truyền dữ liệu từ Controller sang View VD: Controller: public IActionResult Index() { ViewBag.Name = "Nguyễn Văn A"; ViewBag.Age = 20;

return View(); } View:

@ViewBag.Name
Tuổi: @ViewBag.Age

- ViewData là một dictionary có kiểu: ViewDataDictionary VD: Controller: public IActionResult Index() { ViewData["Name"] = "Nguyễn Văn A"; ViewData["Age"] = 20;
return View();
} View:

@ViewData["Name"]
Tuổi: @ViewData["Age"]

hoặc: @((string)ViewData["Name"]) - TempData dùng để truyền dữ liệu từ request này sang request tiếp theo, thường được dùng sau khi: Redict, Submit Form, thông báo thành công hoặc thất bại VD: Controller: public IActionResult Save() { TempData["Message"] = "Lưu dữ liệu thành công";
return RedirectToAction("Index");
}

public IActionResult Index() { return View(); } View:

@TempData["Message"]
=> Kết quả trả về: Lưu dữ liệu thành công Sau khi đọc 1 lần:
@TempData["Message"]
sẽ bị xóa, nếu muốn giữ lại: TempData.Keep(); hoặc TempData.Keep("Message");