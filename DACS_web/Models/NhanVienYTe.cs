using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_web.Models
{
    public class NhanVienYTe
    {
        [Key]
        public string? MaNhanVien { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public string VaiTro { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khoa")]
        public string MaKhoa { get; set; }

        // Thêm trường hình ảnh
        public string? HinhAnh { get; set; } // Lưu đường dẫn file

        // Thêm trường học vị (dành cho bác sĩ)
        public string? HocVi { get; set; } // ThS, TS, PGS.TS, GS.TS, BS, BS.CKII

        // Thêm trường chuyên khoa (dành cho bác sĩ)
        public string? ChuyenKhoa { get; set; }

        // Thêm trường kinh nghiệm
        public int? NamKinhNghiem { get; set; }

        [ForeignKey("MaKhoa")]
        public Khoa? Khoa { get; set; }

        // Quan hệ với ca trực
        public ICollection<CaTruc>? CaTrucs { get; set; }
    }
}