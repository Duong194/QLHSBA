using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "XetNghiemCLSs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "KetQua",
                table: "XetNghiemCLSs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "XetNghiemCLSs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "GioThucHien",
                table: "XetNghiemCLSs",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaBacSiChiDinh",
                table: "XetNghiemCLSs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_XetNghiemCLSs_MaBacSiChiDinh",
                table: "XetNghiemCLSs",
                column: "MaBacSiChiDinh");

            migrationBuilder.AddForeignKey(
                name: "FK_XetNghiemCLSs_NhanVienYTes_MaBacSiChiDinh",
                table: "XetNghiemCLSs",
                column: "MaBacSiChiDinh",
                principalTable: "NhanVienYTes",
                principalColumn: "MaNhanVien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_XetNghiemCLSs_NhanVienYTes_MaBacSiChiDinh",
                table: "XetNghiemCLSs");

            migrationBuilder.DropIndex(
                name: "IX_XetNghiemCLSs_MaBacSiChiDinh",
                table: "XetNghiemCLSs");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "XetNghiemCLSs");

            migrationBuilder.DropColumn(
                name: "GioThucHien",
                table: "XetNghiemCLSs");

            migrationBuilder.DropColumn(
                name: "MaBacSiChiDinh",
                table: "XetNghiemCLSs");

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "XetNghiemCLSs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KetQua",
                table: "XetNghiemCLSs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
