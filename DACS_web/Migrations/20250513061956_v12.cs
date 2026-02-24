using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaNhanVien",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MaNhanVien",
                table: "AspNetUsers",
                column: "MaNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_NhanVienYTes_MaNhanVien",
                table: "AspNetUsers",
                column: "MaNhanVien",
                principalTable: "NhanVienYTes",
                principalColumn: "MaNhanVien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_NhanVienYTes_MaNhanVien",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MaNhanVien",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaNhanVien",
                table: "AspNetUsers");
        }
    }
}
