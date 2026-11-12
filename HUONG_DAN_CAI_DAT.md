# Hướng dẫn chạy chức năng Khoa - Ngành - Sinh viên

## 1. Công nghệ
Dự án hiện tại dùng ASP.NET Core MVC .NET 10 và SQLite (`ptpmql.db`), không phải PHP/MySQL. Phần mới được viết theo đúng cấu trúc hiện tại.

## 2. Các quan hệ khóa ngoại
- `Major.FacultyId` -> `Faculty.FacultyId` (một khoa có nhiều ngành).
- `Student.MajorCode` -> `Major.MajorCode` (một ngành có nhiều sinh viên).
- `Student.SystemCode` -> `TrainingSystem.SystemCode` (một hệ đào tạo có nhiều sinh viên).

Các khóa ngoại dùng `DeleteBehavior.Restrict` để tránh xóa khoa/ngành/hệ đang được tham chiếu.

## 3. Tạo migration và cập nhật database
Mở Terminal tại thư mục `PTPMQL_MVC`, chạy:

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add AddFacultyMajorStudentRelations
dotnet ef database update
dotnet run
```

Nếu máy đã cài `dotnet-ef`, bỏ qua lệnh cài tool. Migration sẽ tạo bảng `Faculties`, `Majors`, `TrainingSystems`, `Students` và nạp dữ liệu danh mục mẫu bằng `HasData`.

**Lưu ý:** Sao lưu `ptpmql.db` trước khi cập nhật. Nếu đây chỉ là database thử nghiệm và migration cũ gây xung đột, đóng ứng dụng, đổi tên/xóa `ptpmql.db` rồi chạy `dotnet ef database update` để tạo DB mới. Không xóa DB nếu đang có dữ liệu cần giữ.

## 4. Chức năng đã thêm
- Danh sách khoa và thêm khoa.
- Danh sách sinh viên, tìm theo mã/tên.
- Thêm sinh viên bằng danh sách chọn hệ đào tạo và ngành; khoa được suy ra từ ngành.
- Sửa họ tên/địa chỉ và xóa sinh viên.
- Tự sinh mã theo cấu trúc `YY + mã hệ (2 số) + mã ngành (2 số) + số thứ tự (4 số)`.

Ví dụ: năm nhập học 2022, hệ `21`, ngành `05`:
`2221050255`, tiếp theo `2221050256`.
Mỗi tổ hợp năm + hệ + ngành có dãy số riêng. Mã bắt đầu ở `0255` theo ví dụ đề bài; nếu muốn bắt đầu `0001`, đổi `DefaultIfEmpty(254)` thành `DefaultIfEmpty(0)` trong `StudentController`.

## 5. Dữ liệu khoa/ngành
Danh sách khoa/ngành được nạp mẫu trong `Data/ApplicationDbContext.cs`. Tên khoa/ngành là dữ liệu danh mục tham khảo để phục vụ bài tập; hãy đối chiếu danh mục đào tạo chính thức của trường và cập nhật nếu giảng viên yêu cầu đúng tên/mã hiện hành.
