
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace DACS_web.Models
{

    public class Khoa
    {
        [Key]
        public string? MaKhoa { get; set; }
        public string TenKhoa { get; set; }
        public string MoTa { get; set; }

        public ICollection<NhanVienYTe>? NhanVienYTes { get; set; }

    }
}
