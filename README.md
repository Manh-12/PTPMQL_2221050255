Đinh tuyến trong MVC
Định tuyến (Routing) là cơ chế giúp ASP.NET Core MVC xác định URL của người dùng sẽ được xử lý bởi Controller và Action nào
MVC sẽ gọi bộ điều khiển (Controller) và các hành động bên trong (Action) thông qua URL
Logic định tuyến MVC sử dụng dạng: /Controller/Action/Parameters VD: https://localhost:5001/Home/Index trong đó: Home là Controller, Index là Action
Định tuyến được cấu hình trong file Program.cs: app.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");