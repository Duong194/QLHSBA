using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace DACS_web.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
        public string? MaBenhNhan { get; set; }

        [ForeignKey("MaBenhNhan")]
        public BenhNhan? BenhNhan { get; set; }

        public string? MaNhanVien { get; set; }

        [ForeignKey("MaNhanVien")]
        public NhanVienYTe? NhanVien { get; set; }


    }
}
