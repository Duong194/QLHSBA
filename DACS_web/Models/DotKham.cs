using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_web.Models
{
    public class DotKham
    {
        [Key]
        public string? MaDotKham { get; set; }

        public string MaBenhNhan { get; set; }
        public string LyDoVaoVien { get; set; }
        public DateTime NgayVaoVien { get; set; }
        public TimeSpan GioVaoVien { get; set; }

        [Required]
        public string MaKhoaTiepNhan { get; set; }
        public string HinhThucTiepNhan { get; set; }

        public DateTime? NgayRaVien { get; set; }
        public TimeSpan? GioRaVien { get; set; }
        public string? MaKhoaRaVien { get; set; }
        public int TongSoNgayDieuTri { get; set; }

        // Foreign Keys
        [ForeignKey("MaBenhNhan")]
        public BenhNhan? BenhNhan { get; set; }

        [ForeignKey("MaKhoaTiepNhan")]
        public Khoa? KhoaTiepNhan { get; set; }

        [ForeignKey("MaKhoaRaVien")]
        public Khoa? KhoaRaVien { get; set; }

        public string? MaCuocHen { get; set; }
        public CuocHenKham? CuocHenKham { get; set; }

        // ✅ THÊM: Collection hồ sơ bệnh án
        public ICollection<HoSoBenhAn>? HoSoBenhAns { get; set; }
    }
}