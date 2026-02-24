
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DACS_web.Models
{

    public class HinhAnhYTe
    {
        [Key]
        public string? MaAnh { get; set; }
        public string MaHoSo { get; set; }
        public string LoaiHinh { get; set; }
        public string? FileURL { get; set; }
        public string MoTa { get; set; }

        [ForeignKey("MaHoSo")]
        public HoSoBenhAn? HoSoBenhAn { get; set; }
    }
}
