using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DACS_web.Models
{
    public class ChanDoan
    {
        [Key]
        public string MaChanDoan { get; set; }

        [Required]
        public string MaHoSo { get; set; }

        [Required]
        public string MaICD { get; set; }

        public string LoaiChanDoan { get; set; }
        public string MoTa { get; set; }
        public string ViTri { get; set; }

        [ForeignKey("MaHoSo")]
        [ValidateNever]
        public HoSoBenhAn? HoSoBenhAn { get; set; }

        [ForeignKey("MaICD")]
        [ValidateNever]
        public DanhMucICD? DanhMucICD { get; set; }
    }
}
