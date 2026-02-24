
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DACS_web.Models
{

    public class DonThuoc
    {
        [Key]
        public string? MaDon { get; set; }
        public string MaHoSo { get; set; }
        public DateTime NgayKe { get; set; }
        public string BacSiKeDon { get; set; }
        public string HuongDanSuDung { get; set; }
        public decimal TongTien { get; set; }

        [ForeignKey("MaHoSo")]
        public HoSoBenhAn HoSoBenhAn { get; set; }
        [ForeignKey("BacSiKeDon")]
        public NhanVienYTe NhanVienYTe { get; set; }
        public List<ChiTietDonThuoc> ChiTietDonThuocs { get; set; }
    }
}
