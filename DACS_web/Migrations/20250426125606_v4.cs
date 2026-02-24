using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Khoas_MaKhoaTiepNhan",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "KhoaRaVien",
                table: "DotKhams");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "GioRaVien",
                table: "DotKhams",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaKhoaRaVien",
                table: "DotKhams",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_MaKhoaRaVien",
                table: "DotKhams",
                column: "MaKhoaRaVien");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Khoas_MaKhoaRaVien",
                table: "DotKhams",
                column: "MaKhoaRaVien",
                principalTable: "Khoas",
                principalColumn: "MaKhoa",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Khoas_MaKhoaTiepNhan",
                table: "DotKhams",
                column: "MaKhoaTiepNhan",
                principalTable: "Khoas",
                principalColumn: "MaKhoa",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Khoas_MaKhoaRaVien",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Khoas_MaKhoaTiepNhan",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_MaKhoaRaVien",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "GioRaVien",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "MaKhoaRaVien",
                table: "DotKhams");

            migrationBuilder.AddColumn<string>(
                name: "KhoaRaVien",
                table: "DotKhams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Khoas_MaKhoaTiepNhan",
                table: "DotKhams",
                column: "MaKhoaTiepNhan",
                principalTable: "Khoas",
                principalColumn: "MaKhoa",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
