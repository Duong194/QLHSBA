
using System.ComponentModel.DataAnnotations;
namespace DACS_web.Models
{

    public class Thuoc
    {
        [Key]
        public string? MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string DonViTinh { get; set; }
        public string ChiDinh { get; set; }
        public decimal DonGia { get; set; }
        public string TacDungPhu { get; set; }
        public string TuongTacThuoc { get; set; }
    }
}
