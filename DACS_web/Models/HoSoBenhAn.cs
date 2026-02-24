using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DACS_web.Models
{
    public class HoSoBenhAn
    {
        [Key]
        public string? MaHoSo { get; set; }

        [Required]
        public string MaBenhNhan { get; set; }

        public string? MaDotKham { get; set; }

        public DateTime NgayTao { get; set; }
        public string? TrangThaiRaVien { get; set; }
        public string? TienLuongGan { get; set; }
        public string? TienLuongXa { get; set; }
        public string? KetQuaRaVien { get; set; }
        public string? ChanDoanRaVien { get; set; }

        [Required]
        public string MaBacSiLapBenhAn { get; set; }

        [Required]
        public string MaBacSiDieuTri { get; set; }

        // Foreign Keys
        [ForeignKey("MaBenhNhan")]
        public BenhNhan? BenhNhan { get; set; }

        [ForeignKey("MaDotKham")]
        public DotKham? DotKham { get; set; }

        [ForeignKey("MaBacSiLapBenhAn")]
        public NhanVienYTe? BacSiLapBenhAn { get; set; }

        [ForeignKey("MaBacSiDieuTri")]
        public NhanVienYTe? BacSiDieuTri { get; set; }

        // Collections
        public ICollection<ChanDoan>? ChanDoans { get; set; }
        public ICollection<KetQuaKhamMat>? KetQuaKhamMats { get; set; }
        public ICollection<PhuongPhapDieuTri>? PhuongPhapDieuTris { get; set; }
        public ICollection<HinhAnhYTe>? HinhAnhYTes { get; set; }
        public ICollection<DonThuoc>? DonThuocs { get; set; }
        public ICollection<XetNghiemCLS>? XetNghiemCLSs { get; set; } // ✅ THÊM
    }
}