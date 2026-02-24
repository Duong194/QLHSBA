using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ThongTinBenhNhansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongTinBenhNhansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ThongTinBenhNhans
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Index(string maBenhNhan)
        {
            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return NotFound();
            }

            var benhNhan = await _context.BenhNhans.FindAsync(maBenhNhan);
            if (benhNhan == null)
            {
                return NotFound();
            }

            var hoSoBenhAns = await _context.HoSoBenhAns
                .Where(h => h.MaBenhNhan == maBenhNhan)
                .Include(h => h.BacSiLapBenhAn)
                .Include(h => h.BacSiDieuTri)
                .ToListAsync();

            var viewModel = new ThongTinBenhNhan
            {
                BenhNhan = benhNhan,
                HoSoBenhAns = hoSoBenhAns
            };

            return View(viewModel);
        }
    }
}
