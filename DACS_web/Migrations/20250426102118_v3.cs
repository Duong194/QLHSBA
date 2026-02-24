using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LyDoVaoVien",
                table: "DotKhams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrangThaiChuyenVien",
                table: "DotKhams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LyDoVaoVien",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "TrangThaiChuyenVien",
                table: "DotKhams");
        }
    }
}
