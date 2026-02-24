
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DACS_web.Models
{

    public class KetQuaKhamMat
    {
        [Key]
        public string? MaKQ { get; set; }
        public string MaHoSo { get; set; }
        public string? ViTriMat { get; set; }
        public string? ThiLucKhongKinh { get; set; }
        public string? ThiLucCoKinh { get; set; }
        public float? NhanAp { get; set; }
        public string? TongTrangMat { get; set; }
        public string? MoTaBoPhanMat { get; set; }

        [ForeignKey("MaHoSo")]
        public HoSoBenhAn? HoSoBenhAn { get; set; }
    }
}
