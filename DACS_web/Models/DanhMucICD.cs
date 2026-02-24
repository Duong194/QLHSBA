
using System.ComponentModel.DataAnnotations;
namespace DACS_web.Models
{

    public class DanhMucICD
    {
        [Key]
        public string MaICD { get; set; }
        public string TenBenh { get; set; }
    }
}