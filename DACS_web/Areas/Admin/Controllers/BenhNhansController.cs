using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BenhNhansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public BenhNhansController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = SD.Role_BenhNhan)]
        public IActionResult NoInfo()
        {
            return View();
        }

        // Action đặc biệt cho bệnh nhân từ dropdown
        [Authorize(Roles = SD.Role_BenhNhan)]
        public async Task<IActionResult> BenhNhanDetails()
        {
            var user = await _userManager.GetUserAsync(User);

            // Kiểm tra xem user có mã bệnh nhân không
            if (string.IsNullOrEmpty(user.MaBenhNhan))
            {
                // Nếu chưa có mã bệnh nhân, chuyển tới trang NoInfo
                return RedirectToAction("NoInfo");
            }

            // Nếu có mã bệnh nhân, chuyển thẳng tới trang Details
            return RedirectToAction("Details", new { id = user.MaBenhNhan });
        }

        // GET: BenhNhans
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            IQueryable<BenhNhan> query = _context.BenhNhans;

            if (User.IsInRole(SD.Role_BenhNhan))
            {
                if (!string.IsNullOrEmpty(user.MaBenhNhan))
                {
                    query = query.Where(b => b.MaBenhNhan == user.MaBenhNhan);
                }
                else
                {
                    return View(new List<BenhNhan>()); // Không có thông tin bệnh nhân
                }
            }

            return View(await query.ToListAsync());
        }

        // GET: BenhNhans/Details/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var benhNhan = await _context.BenhNhans
                .Include(bn => bn.DotKhams) // 👉 Load thêm đợt khám
                    .ThenInclude(dk => dk.KhoaTiepNhan) // 👉 Nếu muốn lấy tên khoa
                .Include(bn => bn.DotKhams)
                    .ThenInclude(dk => dk.KhoaRaVien) // 👉 Load thêm khoa ra viện
                .FirstOrDefaultAsync(m => m.MaBenhNhan == id);

            if (benhNhan == null)
                return NotFound();

            // Nếu là bệnh nhân, kiểm tra quyền truy cập
            if (User.IsInRole(SD.Role_BenhNhan))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user.MaBenhNhan != id)
                {
                    return Forbid(); // Không được phép xem thông tin bệnh nhân khác
                }
            }

            return View(benhNhan);
        }

        // GET: BenhNhans/Create
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public IActionResult Create()
        {
            return View();
        }

        // POST: BenhNhans/Create
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BenhNhan benhNhan)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sinh mã tự động BNxxx
                    var maxMa = await _context.BenhNhans
                        .OrderByDescending(b => b.MaBenhNhan)
                        .Select(b => b.MaBenhNhan)
                        .FirstOrDefaultAsync();

                    int nextId = 1;
                    if (!string.IsNullOrEmpty(maxMa) && maxMa.Length >= 5)
                    {
                        int.TryParse(maxMa.Substring(2), out nextId);
                        nextId++;
                    }

                    benhNhan.MaBenhNhan = $"BN{nextId:D3}"; // Ví dụ: BN001, BN002

                    _context.BenhNhans.Add(benhNhan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi lưu database: {ex.Message}");
                }
            }
            return View(benhNhan);
        }

        // GET: BenhNhans/Edit/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var benhNhan = await _context.BenhNhans.FindAsync(id);
            if (benhNhan == null)
                return NotFound();

            return View(benhNhan);
        }

        // POST: BenhNhans/Edit/5
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, BenhNhan benhNhan)
        {
            if (id != benhNhan.MaBenhNhan)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(benhNhan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BenhNhanExists(benhNhan.MaBenhNhan))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(benhNhan);
        }

        // GET: BenhNhans/Delete/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var benhNhan = await _context.BenhNhans
                .FirstOrDefaultAsync(m => m.MaBenhNhan == id);
            if (benhNhan == null)
                return NotFound();

            return View(benhNhan);
        }

        // POST: BenhNhans/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var benhNhan = await _context.BenhNhans.FindAsync(id);
            if (benhNhan != null)
            {
                _context.BenhNhans.Remove(benhNhan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BenhNhanExists(string id)
        {
            return _context.BenhNhans.Any(e => e.MaBenhNhan == id);
        }
    }
}