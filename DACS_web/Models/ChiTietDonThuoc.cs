using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_web.Models
{
    public class ChiTietDonThuoc
    {
        public string MaDon { get; set; }
        public string MaThuoc { get; set; }
        public int SoLuong { get; set; }
        public string LieuDung { get; set; }
        public string ThoiGianDung { get; set; }
        public string GhiChu { get; set; }

        [ForeignKey("MaDon")]
        public DonThuoc DonThuoc { get; set; }

        [ForeignKey("MaThuoc")]
        public Thuoc Thuoc { get; set; }

    }
}
