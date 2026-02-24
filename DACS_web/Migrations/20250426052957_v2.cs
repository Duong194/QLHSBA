using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MaICD",
                table: "ChanDoans",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "DanhMucICDs",
                columns: table => new
                {
                    MaICD = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenBenh = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucICDs", x => x.MaICD);
                });

            migrationBuilder.CreateTable(
                name: "HinhAnhYTes",
                columns: table => new
                {
                    MaAnh = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoaiHinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhYTes", x => x.MaAnh);
                    table.ForeignKey(
                        name: "FK_HinhAnhYTes_HoSoBenhAns_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAns",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhuongPhapDieuTris",
                columns: table => new
                {
                    MaPhuongPhap = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoiKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgoaiKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongPhapDieuTris", x => x.MaPhuongPhap);
                    table.ForeignKey(
                        name: "FK_PhuongPhapDieuTris_HoSoBenhAns_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAns",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChanDoans_MaICD",
                table: "ChanDoans",
                column: "MaICD");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhYTes_MaHoSo",
                table: "HinhAnhYTes",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongPhapDieuTris_MaHoSo",
                table: "PhuongPhapDieuTris",
                column: "MaHoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_ChanDoans_DanhMucICDs_MaICD",
                table: "ChanDoans",
                column: "MaICD",
                principalTable: "DanhMucICDs",
                principalColumn: "MaICD",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChanDoans_DanhMucICDs_MaICD",
                table: "ChanDoans");

            migrationBuilder.DropTable(
                name: "DanhMucICDs");

            migrationBuilder.DropTable(
                name: "HinhAnhYTes");

            migrationBuilder.DropTable(
                name: "PhuongPhapDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_ChanDoans_MaICD",
                table: "ChanDoans");

            migrationBuilder.AlterColumn<string>(
                name: "MaICD",
                table: "ChanDoans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
