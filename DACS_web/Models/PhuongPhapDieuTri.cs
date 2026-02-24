
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DACS_web.Models
{

    public class PhuongPhapDieuTri
    {
        [Key]
        public string? MaPhuongPhap { get; set; }
        public string MaHoSo { get; set; }
        public string NoiKhoa { get; set; }
        public string NgoaiKhoa { get; set; }
        public string MoTaChiTiet { get; set; }

        [ForeignKey("MaHoSo")]
        public HoSoBenhAn? HoSoBenhAn { get; set; }
    }
}
