
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTPMQL_MVC.Migrations
{
    public partial class SyncProductModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AddProductFields đã đồng bộ các cột cần thiết.
            // Không cần thêm hoặc xóa cột lần nữa.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không thực hiện thao tác đảo ngược.
        }
    }
}
