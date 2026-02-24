using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DACS_web.Models
{
    public class CaTruc
    {
        [Key]
        public int MaCaTruc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        public string MaNhanVien { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày trực")]
        [DataType(DataType.Date)]
        public DateTime NgayTruc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ca trực")]
        public string TenCa { get; set; } // "Sáng", "Chiều", "Tối", "Đêm"

        [Required]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        public TimeSpan GioKetThuc { get; set; }

        public string? GhiChu { get; set; }

        [ForeignKey("MaNhanVien")]
        public NhanVienYTe? NhanVien { get; set; }
    }
}
