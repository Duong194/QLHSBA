using System.ComponentModel.DataAnnotations;

namespace DACS_web.Models
{
    public class BaiBao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; }

        [Required]
        public string NoiDung { get; set; }

        public string TacGia { get; set; }

        [Display(Name = "Ngày đăng")]
        [DataType(DataType.Date)]
        public DateTime NgayDang { get; set; }

        public string? HinhAnh { get; set; } // Đường dẫn ảnh

        public string DanhMuc { get; set; } // Ví dụ: "Sức khỏe", "Công nghệ"
    }

}
