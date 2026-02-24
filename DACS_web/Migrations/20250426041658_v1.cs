using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS_web.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BenhNhans",
                columns: table => new
                {
                    MaBenhNhan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CMND = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgheNghiep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DanToc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuocTich = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoiTuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BHYT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TienSuDiUng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TienSuBenhBanThan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TienSuBenhGiaDinh = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhans", x => x.MaBenhNhan);
                });

            migrationBuilder.CreateTable(
                name: "Khoas",
                columns: table => new
                {
                    MaKhoa = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Khoas", x => x.MaKhoa);
                });

            migrationBuilder.CreateTable(
                name: "Thuocs",
                columns: table => new
                {
                    MaThuoc = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenThuoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChiDinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TacDungPhu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TuongTacThuoc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thuocs", x => x.MaThuoc);
                });

            migrationBuilder.CreateTable(
                name: "CuocHenKhams",
                columns: table => new
                {
                    MaCuocHen = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayGioHen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BacSiDuKien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiHen = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuocHenKhams", x => x.MaCuocHen);
                    table.ForeignKey(
                        name: "FK_CuocHenKhams_BenhNhans_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhans",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhanVienYTes",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhoa = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienYTes", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NhanVienYTes_Khoas_MaKhoa",
                        column: x => x.MaKhoa,
                        principalTable: "Khoas",
                        principalColumn: "MaKhoa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DotKhams",
                columns: table => new
                {
                    MaDotKham = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayVaoVien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioVaoVien = table.Column<TimeSpan>(type: "time", nullable: false),
                    MaKhoaTiepNhan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HinhThucTiepNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayRaVien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KhoaRaVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongSoNgayDieuTri = table.Column<int>(type: "int", nullable: false),
                    MaCuocHen = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhams", x => x.MaDotKham);
                    table.ForeignKey(
                        name: "FK_DotKhams_BenhNhans_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhans",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DotKhams_CuocHenKhams_MaCuocHen",
                        column: x => x.MaCuocHen,
                        principalTable: "CuocHenKhams",
                        principalColumn: "MaCuocHen");
                    table.ForeignKey(
                        name: "FK_DotKhams_Khoas_MaKhoaTiepNhan",
                        column: x => x.MaKhoaTiepNhan,
                        principalTable: "Khoas",
                        principalColumn: "MaKhoa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoBenhAns",
                columns: table => new
                {
                    MaHoSo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiRaVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TienLuongGan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TienLuongXa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KetQuaRaVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChanDoanRaVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaBacSiLapBenhAn = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaBacSiDieuTri = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoBenhAns", x => x.MaHoSo);
                    table.ForeignKey(
                        name: "FK_HoSoBenhAns_BenhNhans_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhans",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoBenhAns_NhanVienYTes_MaBacSiDieuTri",
                        column: x => x.MaBacSiDieuTri,
                        principalTable: "NhanVienYTes",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoSoBenhAns_NhanVienYTes_MaBacSiLapBenhAn",
                        column: x => x.MaBacSiLapBenhAn,
                        principalTable: "NhanVienYTes",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChanDoans",
                columns: table => new
                {
                    MaChanDoan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoaiChanDoan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaICD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChanDoans", x => x.MaChanDoan);
                    table.ForeignKey(
                        name: "FK_ChanDoans_HoSoBenhAns_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAns",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonThuocs",
                columns: table => new
                {
                    MaDon = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayKe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BacSiKeDon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HuongDanSuDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonThuocs", x => x.MaDon);
                    table.ForeignKey(
                        name: "FK_DonThuocs_HoSoBenhAns_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAns",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaKhamMats",
                columns: table => new
                {
                    MaKQ = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ViTriMat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThiLucKhongKinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThiLucCoKinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NhanAp = table.Column<float>(type: "real", nullable: false),
                    TongTrangMat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTaBoPhanMat = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaKhamMats", x => x.MaKQ);
                    table.ForeignKey(
                        name: "FK_KetQuaKhamMats_HoSoBenhAns_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAns",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XetNghiemCLSs",
                columns: table => new
                {
                    MaXetNghiem = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaHoSo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoaiXetNghiem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayThucHien = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XetNghiemCLSs", x => x.MaXetNghiem);
                    table.ForeignKey(
                        name: "FK_XetNghiemCLSs_HoSoBenhAns_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAns",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonThuocs",
                columns: table => new
                {
                    MaDon = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaThuoc = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    LieuDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonThuocs", x => new { x.MaDon, x.MaThuoc });
                    table.ForeignKey(
                        name: "FK_ChiTietDonThuocs_DonThuocs_MaDon",
                        column: x => x.MaDon,
                        principalTable: "DonThuocs",
                        principalColumn: "MaDon",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonThuocs_Thuocs_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuocs",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChanDoans_MaHoSo",
                table: "ChanDoans",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonThuocs_MaThuoc",
                table: "ChiTietDonThuocs",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_CuocHenKhams_MaBenhNhan",
                table: "CuocHenKhams",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DonThuocs_MaHoSo",
                table: "DonThuocs",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_MaBenhNhan",
                table: "DotKhams",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_MaCuocHen",
                table: "DotKhams",
                column: "MaCuocHen");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_MaKhoaTiepNhan",
                table: "DotKhams",
                column: "MaKhoaTiepNhan");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhAns_MaBacSiDieuTri",
                table: "HoSoBenhAns",
                column: "MaBacSiDieuTri");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhAns_MaBacSiLapBenhAn",
                table: "HoSoBenhAns",
                column: "MaBacSiLapBenhAn");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhAns_MaBenhNhan",
                table: "HoSoBenhAns",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaKhamMats_MaHoSo",
                table: "KetQuaKhamMats",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVienYTes_MaKhoa",
                table: "NhanVienYTes",
                column: "MaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_XetNghiemCLSs_MaHoSo",
                table: "XetNghiemCLSs",
                column: "MaHoSo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChanDoans");

            migrationBuilder.DropTable(
                name: "ChiTietDonThuocs");

            migrationBuilder.DropTable(
                name: "DotKhams");

            migrationBuilder.DropTable(
                name: "KetQuaKhamMats");

            migrationBuilder.DropTable(
                name: "XetNghiemCLSs");

            migrationBuilder.DropTable(
                name: "DonThuocs");

            migrationBuilder.DropTable(
                name: "Thuocs");

            migrationBuilder.DropTable(
                name: "CuocHenKhams");

            migrationBuilder.DropTable(
                name: "HoSoBenhAns");

            migrationBuilder.DropTable(
                name: "BenhNhans");

            migrationBuilder.DropTable(
                name: "NhanVienYTes");

            migrationBuilder.DropTable(
                name: "Khoas");
        }
    }
}
