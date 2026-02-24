using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DACS_web.ViewModels
{
    public class DonThuocViewModel
    {
        [Required]
        public string MaHoSo { get; set; }

        [Required]
        [Display(Name = "Bác sĩ kê đơn")]
        public string BacSiKeDon { get; set; }

        [Display(Name = "Hướng dẫn sử dụng")]
        public string HuongDanSuDung { get; set; }

        [Display(Name = "Ngày kê")]
        public DateTime NgayKe { get; set; } = DateTime.Now;

        [Display(Name = "Danh sách thuốc")]
        public List<ChiTietDonThuocVM> ChiTietThuocs { get; set; } = new();
    }

    public class ChiTietDonThuocVM
    {
        [Required]
        [Display(Name = "Mã thuốc")]
        public string MaThuoc { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Display(Name = "Liều dùng")]
        public string LieuDung { get; set; }

        [Display(Name = "Thời gian dùng")]
        public string ThoiGianDung { get; set; }

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }
    }
}
