using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuocHenKhams_CaTrucs_MaCaTruc",
                table: "CuocHenKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_CuocHenKhams_NhanVienYTes_MaBacSiDuKien",
                table: "CuocHenKhams");

            migrationBuilder.AddForeignKey(
                name: "FK_CuocHenKhams_CaTrucs_MaCaTruc",
                table: "CuocHenKhams",
                column: "MaCaTruc",
                principalTable: "CaTrucs",
                principalColumn: "MaCaTruc",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CuocHenKhams_NhanVienYTes_MaBacSiDuKien",
                table: "CuocHenKhams",
                column: "MaBacSiDuKien",
                principalTable: "NhanVienYTes",
                principalColumn: "MaNhanVien",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuocHenKhams_CaTrucs_MaCaTruc",
                table: "CuocHenKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_CuocHenKhams_NhanVienYTes_MaBacSiDuKien",
                table: "CuocHenKhams");

            migrationBuilder.AddForeignKey(
                name: "FK_CuocHenKhams_CaTrucs_MaCaTruc",
                table: "CuocHenKhams",
                column: "MaCaTruc",
                principalTable: "CaTrucs",
                principalColumn: "MaCaTruc");

            migrationBuilder.AddForeignKey(
                name: "FK_CuocHenKhams_NhanVienYTes_MaBacSiDuKien",
                table: "CuocHenKhams",
                column: "MaBacSiDuKien",
                principalTable: "NhanVienYTes",
                principalColumn: "MaNhanVien",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
