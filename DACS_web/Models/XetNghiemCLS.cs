using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_web.Models
{
    public class XetNghiemCLS
    {
        [Key]
        public string? MaXetNghiem { get; set; }  // ✅ Có dấu ? vì sẽ được gán trong Controller

        [Required(ErrorMessage = "Vui lòng chọn hồ sơ bệnh án")]
        [Display(Name = "Hồ sơ bệnh án")]
        public string MaHoSo { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại xét nghiệm")]
        [Display(Name = "Loại xét nghiệm")]
        public string LoaiXetNghiem { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Kết quả")]
        public string? KetQua { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày thực hiện")]
        [Display(Name = "Ngày thực hiện")]
        [DataType(DataType.Date)]
        public DateTime NgayThucHien { get; set; } = DateTime.Now;

        [Display(Name = "Giờ thực hiện")]
        [DataType(DataType.Time)]
        public TimeSpan? GioThucHien { get; set; }

        [Display(Name = "Bác sĩ chỉ định")]
        public string? MaBacSiChiDinh { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // Foreign Keys
        [ForeignKey("MaHoSo")]
        public HoSoBenhAn? HoSoBenhAn { get; set; }

        [ForeignKey("MaBacSiChiDinh")]
        public NhanVienYTe? BacSiChiDinh { get; set; }
    }
}