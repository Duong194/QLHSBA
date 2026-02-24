using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BacSiKeDon",
                table: "DonThuocs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_DonThuocs_BacSiKeDon",
                table: "DonThuocs",
                column: "BacSiKeDon");

            migrationBuilder.AddForeignKey(
                name: "FK_DonThuocs_NhanVienYTes_BacSiKeDon",
                table: "DonThuocs",
                column: "BacSiKeDon",
                principalTable: "NhanVienYTes",
                principalColumn: "MaNhanVien",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonThuocs_NhanVienYTes_BacSiKeDon",
                table: "DonThuocs");

            migrationBuilder.DropIndex(
                name: "IX_DonThuocs_BacSiKeDon",
                table: "DonThuocs");

            migrationBuilder.AlterColumn<string>(
                name: "BacSiKeDon",
                table: "DonThuocs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
