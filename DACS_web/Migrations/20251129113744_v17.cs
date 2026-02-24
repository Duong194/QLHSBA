using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaKham",
                table: "CuocHenKhams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "CuocHenKhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "GioKhamDuKien",
                table: "CuocHenKhams",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LyDoKham",
                table: "CuocHenKhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaCaTruc",
                table: "CuocHenKhams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuocHenKhams_MaCaTruc",
                table: "CuocHenKhams",
                column: "MaCaTruc");

            migrationBuilder.AddForeignKey(
                name: "FK_CuocHenKhams_CaTrucs_MaCaTruc",
                table: "CuocHenKhams",
                column: "MaCaTruc",
                principalTable: "CaTrucs",
                principalColumn: "MaCaTruc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuocHenKhams_CaTrucs_MaCaTruc",
                table: "CuocHenKhams");

            migrationBuilder.DropIndex(
                name: "IX_CuocHenKhams_MaCaTruc",
                table: "CuocHenKhams");

            migrationBuilder.DropColumn(
                name: "CaKham",
                table: "CuocHenKhams");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "CuocHenKhams");

            migrationBuilder.DropColumn(
                name: "GioKhamDuKien",
                table: "CuocHenKhams");

            migrationBuilder.DropColumn(
                name: "LyDoKham",
                table: "CuocHenKhams");

            migrationBuilder.DropColumn(
                name: "MaCaTruc",
                table: "CuocHenKhams");
        }
    }
}
