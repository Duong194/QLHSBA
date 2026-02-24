using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BacSiDuKien",
                table: "CuocHenKhams");

            migrationBuilder.AddColumn<string>(
                name: "MaBacSiDuKien",
                table: "CuocHenKhams",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CuocHenKhams_MaBacSiDuKien",
                table: "CuocHenKhams",
                column: "MaBacSiDuKien");

            migrationBuilder.AddForeignKey(
                name: "FK_CuocHenKhams_NhanVienYTes_MaBacSiDuKien",
                table: "CuocHenKhams",
                column: "MaBacSiDuKien",
                principalTable: "NhanVienYTes",
                principalColumn: "MaNhanVien",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuocHenKhams_NhanVienYTes_MaBacSiDuKien",
                table: "CuocHenKhams");

            migrationBuilder.DropIndex(
                name: "IX_CuocHenKhams_MaBacSiDuKien",
                table: "CuocHenKhams");

            migrationBuilder.DropColumn(
                name: "MaBacSiDuKien",
                table: "CuocHenKhams");

            migrationBuilder.AddColumn<string>(
                name: "BacSiDuKien",
                table: "CuocHenKhams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
