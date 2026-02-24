using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChuyenKhoa",
                table: "NhanVienYTes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "NhanVienYTes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HocVi",
                table: "NhanVienYTes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NamKinhNghiem",
                table: "NhanVienYTes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CaTrucs",
                columns: table => new
                {
                    MaCaTruc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayTruc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenCa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GioBatDau = table.Column<TimeSpan>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeSpan>(type: "time", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaTrucs", x => x.MaCaTruc);
                    table.ForeignKey(
                        name: "FK_CaTrucs_NhanVienYTes_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVienYTes",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaTrucs_MaNhanVien",
                table: "CaTrucs",
                column: "MaNhanVien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaTrucs");

            migrationBuilder.DropColumn(
                name: "ChuyenKhoa",
                table: "NhanVienYTes");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "NhanVienYTes");

            migrationBuilder.DropColumn(
                name: "HocVi",
                table: "NhanVienYTes");

            migrationBuilder.DropColumn(
                name: "NamKinhNghiem",
                table: "NhanVienYTes");
        }
    }
}
