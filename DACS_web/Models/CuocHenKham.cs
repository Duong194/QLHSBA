using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_web.Models
{
    public class CuocHenKham
    {
        [Key]
        public string? MaCuocHen { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân")]
        public string MaBenhNhan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày khám")]
        [DataType(DataType.Date)]
        public DateTime NgayGioHen { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        public string MaBacSiDuKien { get; set; }

        // Thêm trường ca trực để liên kết với lịch trực
        public int? MaCaTruc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ca khám")]
        public string? CaKham { get; set; } // "Sáng", "Chiều", "Tối", "Đêm"

        public TimeSpan? GioKhamDuKien { get; set; } // Giờ cụ thể trong ca

        [Required]
        public string TrangThaiHen { get; set; } // "Chờ khám", "Đã xác nhận", "Đã khám", "Đã hủy"

        public string? LyDoKham { get; set; }

        public string? GhiChu { get; set; }

        [ForeignKey("MaBenhNhan")]
        public BenhNhan? BenhNhan { get; set; }

        [ForeignKey("MaBacSiDuKien")]
        public NhanVienYTe? BacSiDuKien { get; set; }

        [ForeignKey("MaCaTruc")]
        public CaTruc? CaTruc { get; set; }

        public ICollection<DotKham>? DotKhams { get; set; }
    }
}