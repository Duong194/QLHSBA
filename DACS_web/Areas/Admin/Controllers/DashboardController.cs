using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserName = user.FullName ?? user.UserName;

            // Xác định role
            var role = User.IsInRole(SD.Role_Admin) ? "Admin"
                      : User.IsInRole(SD.Role_BacSi) ? "BacSi"
                      : User.IsInRole(SD.Role_YTa) ? "YTa"
                      : "Unknown";

            ViewBag.Role = role;

            // Load dữ liệu ca trực dựa theo role
            if (role == "Admin")
            {
                // Admin: Xem tất cả ca trực hôm nay
                var caTrucsToday = await _context.CaTrucs
                    .Include(c => c.NhanVien)
                    .ThenInclude(nv => nv.Khoa)
                    .Where(c => c.NgayTruc.Date == DateTime.Today)
                    .OrderBy(c => c.GioBatDau)
                    .ToListAsync();

                ViewBag.CaTrucsToday = caTrucsToday;
            }
            else if (role == "BacSi" || role == "YTa")
            {
                // Bác sĩ/Y tá: Chỉ xem ca trực của bản thân
                var maNhanVien = user.MaNhanVien;

                if (!string.IsNullOrEmpty(maNhanVien))
                {
                    var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    var endOfWeek = startOfWeek.AddDays(7);

                    var myShifts = await _context.CaTrucs
                        .Where(c => c.MaNhanVien == maNhanVien
                                 && c.NgayTruc >= startOfWeek
                                 && c.NgayTruc < endOfWeek)
                        .OrderBy(c => c.NgayTruc)
                        .ThenBy(c => c.GioBatDau)
                        .ToListAsync();

                    ViewBag.MyShifts = myShifts;
                }
                else
                {
                    ViewBag.MyShifts = new List<CaTruc>();
                }
            }

            if (role == "BacSi" && !string.IsNullOrEmpty(user?.MaNhanVien))
            {
                var today = DateTime.Today;
                var todayAppointments = await _context.CuocHenKhams
                    .Include(c => c.BenhNhan)
                    .Include(c => c.CaTruc)
                    .Where(c => c.MaBacSiDuKien == user.MaNhanVien
                             && c.NgayGioHen.Date == today
                             && c.TrangThaiHen != "Đã hủy")
                    .OrderBy(c => c.GioKhamDuKien)
                    .ToListAsync();

                ViewBag.TodayAppointments = todayAppointments;

                // Thống kê cho stats cards
                ViewBag.TotalPatientsThisMonth = await _context.CuocHenKhams
                    .Where(c => c.MaBacSiDuKien == user.MaNhanVien
                             && c.NgayGioHen.Month == DateTime.Now.Month
                             && c.NgayGioHen.Year == DateTime.Now.Year)
                    .Select(c => c.MaBenhNhan)
                    .Distinct()
                    .CountAsync();

                ViewBag.TodayAppointmentsCount = todayAppointments.Count;

                ViewBag.WeekPrescriptions = 28; // TODO: Tính từ database thực tế
                ViewBag.TotalRecordsProcessed = 156; // TODO: Tính từ database thực tế
            }
            return View();
        }
    }
}