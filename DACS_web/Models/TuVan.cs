using System.ComponentModel.DataAnnotations;

namespace DACS_web.Models
{
    public class TuVan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string HoTen { get; set; }

        [Required]
        public string SoDienThoai { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public string? GhiChu { get; set; }
        public bool DaLienHe { get; set; } = false;

    }
}
