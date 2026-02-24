using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DACS_web.Models
{
    public class BenhNhan
    {
        [Key]
        public string? MaBenhNhan { get; set; }

        public string HoTen { get; set; }
        [DataType(DataType.Date)]

        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string CMND { get; set; }
        public string SoDienThoai { get; set; }
        public string NgheNghiep { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string DiaChi { get; set; }
        public string DoiTuong { get; set; }
        public string BHYT { get; set; }
        public string TienSuDiUng { get; set; }
        public string TienSuBenhBanThan { get; set; }
        public string TienSuBenhGiaDinh { get; set; }

        public ICollection<HoSoBenhAn>? HoSoBenhAns { get; set; }
        public ICollection<CuocHenKham>? CuocHenKhams { get; set; }
        public ICollection<DotKham>? DotKhams { get; set; }
    }
}
