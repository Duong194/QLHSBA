using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CuocHenKhamsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CuocHenKhamsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CuocHenKhams
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BenhNhan + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Index(string? trangThaiFilter, DateTime? ngayFilter)
        {
            var user = await _userManager.GetUserAsync(User);
            var cuocHenKhams = _context.CuocHenKhams
                .Include(c => c.BenhNhan)
                .Include(c => c.BacSiDuKien)
                .Include(c => c.CaTruc)
                .AsQueryable();

            // Nếu là bác sĩ, chỉ thấy các cuộc hẹn có bác sĩ dự kiến là chính mình
            if (User.IsInRole(SD.Role_BacSi) && !string.IsNullOrEmpty(user?.MaNhanVien))
            {
                cuocHenKhams = cuocHenKhams.Where(c => c.MaBacSiDuKien == user.MaNhanVien);
            }

            // Nếu là bệnh nhân, chỉ thấy cuộc hẹn của chính mình
            if (User.IsInRole(SD.Role_BenhNhan) && !string.IsNullOrEmpty(user?.MaBenhNhan))
            {
                cuocHenKhams = cuocHenKhams.Where(c => c.MaBenhNhan == user.MaBenhNhan);
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangThaiFilter))
            {
                cuocHenKhams = cuocHenKhams.Where(c => c.TrangThaiHen == trangThaiFilter);
            }

            // Lọc theo ngày
            if (ngayFilter.HasValue)
            {
                cuocHenKhams = cuocHenKhams.Where(c => c.NgayGioHen.Date == ngayFilter.Value.Date);
            }

            ViewBag.TrangThaiList = new SelectList(new[] { "Chờ khám", "Đã xác nhận", "Đã khám", "Đã hủy" });
            ViewBag.TrangThaiFilter = trangThaiFilter ?? "";
            ViewBag.NgayFilter = ngayFilter?.ToString("yyyy-MM-dd") ?? "";

            return View(await cuocHenKhams.OrderByDescending(x => x.NgayGioHen).ToListAsync());
        }

        [Authorize(Roles = "Admin,BenhNhan,YTa")]
        public async Task<IActionResult> XemLichTrucBacSi(string? maBacSi, DateTime? tuNgay)
        {
            try
            {
                // Khởi tạo ViewBag mặc định để tránh lỗi View
                ViewBag.LichHenCount = new Dictionary<int, int>();
                ViewBag.BacSiList = new SelectList(new List<NhanVienYTe>(), "MaNhanVien", "HoTen");
                ViewBag.MaBacSi = maBacSi ?? "";
                ViewBag.TuNgay = tuNgay?.Date ?? DateTime.Today;

                var startDate = tuNgay?.Date ?? DateTime.Today;
                var endDate = startDate.AddDays(14);

                // BƯỚC 1: Lấy tất cả nhân viên (RAW - không filter)
                List<NhanVienYTe> allNhanVien;
                try
                {
                    allNhanVien = await _context.NhanVienYTes.ToListAsync();
                    Console.WriteLine($"✅ BƯỚC 1: Lấy được {allNhanVien.Count} nhân viên");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi bước 1 (Lấy nhân viên): {ex.Message}";
                    return View(new List<CaTruc>());
                }

                // BƯỚC 2: Filter bác sĩ trong C# (không dùng SQL)
                List<NhanVienYTe> bacSiList;
                try
                {
                    bacSiList = allNhanVien.Where(nv => nv.VaiTro == "Bác sĩ").ToList();
                    Console.WriteLine($"✅ BƯỚC 2: Có {bacSiList.Count} bác sĩ");

                    if (!bacSiList.Any())
                    {
                        TempData["ErrorMessage"] = "Không có bác sĩ nào trong hệ thống!";
                        return View(new List<CaTruc>());
                    }

                    ViewBag.BacSiList = new SelectList(bacSiList, "MaNhanVien", "HoTen");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi bước 2 (Filter bác sĩ): {ex.Message}";
                    return View(new List<CaTruc>());
                }

                // BƯỚC 3: Lấy tất cả ca trực (RAW - không filter)
                List<CaTruc> allCaTrucs;
                try
                {
                    allCaTrucs = await _context.CaTrucs.ToListAsync();
                    Console.WriteLine($"✅ BƯỚC 3: Lấy được {allCaTrucs.Count} ca trực");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi bước 3 (Lấy ca trực): {ex.Message}";
                    Console.WriteLine($"❌ BƯỚC 3 LỖI: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    return View(new List<CaTruc>());
                }

                // BƯỚC 4: Lấy tất cả khoa
                List<Khoa> allKhoa;
                try
                {
                    allKhoa = await _context.Khoas.ToListAsync();
                    Console.WriteLine($"✅ BƯỚC 4: Lấy được {allKhoa.Count} khoa");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi bước 4 (Lấy khoa): {ex.Message}";
                    return View(new List<CaTruc>());
                }

                // BƯỚC 5: Filter trong C# (không dùng SQL)
                List<CaTruc> caTrucs;
                try
                {
                    var maBacSiList = bacSiList.Select(b => b.MaNhanVien).ToList();

                    caTrucs = allCaTrucs
                        .Where(c => c.NgayTruc.Date >= startDate && c.NgayTruc.Date <= endDate)
                        .Where(c => string.IsNullOrEmpty(maBacSi)
                            ? maBacSiList.Contains(c.MaNhanVien)
                            : c.MaNhanVien == maBacSi)
                        .OrderBy(c => c.NgayTruc)
                        .ThenBy(c => c.GioBatDau)
                        .ToList();

                    Console.WriteLine($"✅ BƯỚC 5: Lọc được {caTrucs.Count} ca trực phù hợp");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi bước 5 (Filter ca trực): {ex.Message}";
                    return View(new List<CaTruc>());
                }

                // BƯỚC 6: Gán navigation properties trong C#
                try
                {
                    foreach (var ct in caTrucs)
                    {
                        ct.NhanVien = allNhanVien.FirstOrDefault(nv => nv.MaNhanVien == ct.MaNhanVien);

                        if (ct.NhanVien != null && !string.IsNullOrEmpty(ct.NhanVien.MaKhoa))
                        {
                            ct.NhanVien.Khoa = allKhoa.FirstOrDefault(k => k.MaKhoa == ct.NhanVien.MaKhoa);
                        }
                    }

                    Console.WriteLine($"✅ BƯỚC 6: Đã gán navigation properties");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi bước 6 (Gán relationships): {ex.Message}";
                    return View(new List<CaTruc>());
                }

                if (!caTrucs.Any())
                {
                    TempData["ErrorMessage"] = "Không có lịch trực nào trong khoảng thời gian này.";
                }

                Console.WriteLine($"✅ HOÀN THÀNH: Trả về {caTrucs.Count} ca trực");
                return View(caTrucs);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi tổng thể: {ex.Message}";
                Console.WriteLine($"❌ LỖI TỔNG: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                ViewBag.LichHenCount = new Dictionary<int, int>();
                ViewBag.BacSiList = new SelectList(new List<NhanVienYTe>(), "MaNhanVien", "HoTen");
                ViewBag.MaBacSi = "";
                ViewBag.TuNgay = DateTime.Today;

                return View(new List<CaTruc>());
            }
        }

        // GET: Đặt lịch khám theo ca trực (cho bệnh nhân)
        [Authorize(Roles = SD.Role_BenhNhan)]
        public async Task<IActionResult> DatLichTheoCaTruc(int maCaTruc)
        {
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(user?.MaBenhNhan))
            {
                TempData["ErrorMessage"] = "Bạn chưa được gán mã bệnh nhân!";
                return RedirectToAction("Index", "Home");
            }

            var caTruc = await _context.CaTrucs
                .Include(c => c.NhanVien)
                .ThenInclude(nv => nv.Khoa)
                .FirstOrDefaultAsync(c => c.MaCaTruc == maCaTruc);

            if (caTruc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy ca trực!";
                return RedirectToAction(nameof(XemLichTrucBacSi));
            }

            // Kiểm tra ca trực đã qua chưa
            var now = DateTime.Now;
            var caTrucDateTime = caTruc.NgayTruc.Date.Add(caTruc.GioBatDau);
            if (caTrucDateTime < now)
            {
                TempData["ErrorMessage"] = "Không thể đặt lịch cho ca trực đã qua!";
                return RedirectToAction(nameof(XemLichTrucBacSi));
            }

            // Đếm số lượng cuộc hẹn hiện tại
            var soLuongHen = await _context.CuocHenKhams
                .Where(c => c.MaCaTruc == maCaTruc && c.TrangThaiHen != "Đã hủy")
                .CountAsync();

            var cuocHenKham = new CuocHenKham
            {
                MaBenhNhan = user.MaBenhNhan,
                MaBacSiDuKien = caTruc.MaNhanVien,
                NgayGioHen = caTruc.NgayTruc,
                CaKham = caTruc.TenCa,
                MaCaTruc = maCaTruc,
                TrangThaiHen = "Chờ khám"
            };

            ViewBag.CaTruc = caTruc;
            ViewBag.SoLuongHen = soLuongHen;

            // ✨ TẠO DANH SÁCH GIỜ KHÁM HỢP LỆ (chỉ trong khung giờ làm việc)
            ViewBag.GioKhamGoiY = GenerateGioKhamTrongCa(caTruc, now);

            return View(cuocHenKham);
        }
        private List<TimeSpan> GenerateGioKhamTrongCa(CaTruc caTruc, DateTime now)
        {
            var gioGoiY = new List<TimeSpan>();
            var gioBatDau = caTruc.GioBatDau;
            var gioKetThuc = caTruc.GioKetThuc;

            // Nếu ca trực là hôm nay, chỉ cho phép đặt lịch từ giờ hiện tại + 30 phút
            if (caTruc.NgayTruc.Date == now.Date)
            {
                var gioToiThieu = now.TimeOfDay.Add(TimeSpan.FromMinutes(30));
                if (gioToiThieu > gioBatDau)
                {
                    gioBatDau = TimeSpan.FromMinutes(Math.Ceiling(gioToiThieu.TotalMinutes / 30) * 30);
                }
            }

            // Xử lý ca đêm (qua ngày mới)
            if (gioKetThuc < gioBatDau)
            {
                gioKetThuc = gioKetThuc.Add(TimeSpan.FromHours(24));
            }

            // Chia ca thành các khung giờ 30 phút
            var currentTime = gioBatDau;
            while (currentTime < gioKetThuc)
            {
                // Nếu là ca đêm và giờ > 24h, trừ đi 24h
                var displayTime = currentTime.TotalHours >= 24
                    ? currentTime.Subtract(TimeSpan.FromHours(24))
                    : currentTime;

                gioGoiY.Add(displayTime);
                currentTime = currentTime.Add(TimeSpan.FromMinutes(30));
            }

            return gioGoiY;
        }

        // POST: Đặt lịch khám theo ca trực (cho bệnh nhân)
        // POST: Đặt lịch khám theo ca trực (cho bệnh nhân)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_BenhNhan)]
        public async Task<IActionResult> DatLichTheoCaTruc(CuocHenKham cuocHenKham)
        {
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(user?.MaBenhNhan))
            {
                TempData["ErrorMessage"] = "Bạn chưa được gán mã bệnh nhân!";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra ca trực còn khả dụng
            var caTruc = await _context.CaTrucs.FindAsync(cuocHenKham.MaCaTruc);
            if (caTruc == null)
            {
                TempData["ErrorMessage"] = "Ca trực không tồn tại!";
                return RedirectToAction(nameof(XemLichTrucBacSi));
            }

            // Kiểm tra xem bệnh nhân đã đặt lịch trong ca này chưa
            var daDatLich = await _context.CuocHenKhams
                .AnyAsync(c => c.MaBenhNhan == user.MaBenhNhan
                            && c.MaCaTruc == cuocHenKham.MaCaTruc
                            && c.TrangThaiHen != "Đã hủy");

            if (daDatLich)
            {
                TempData["ErrorMessage"] = "Bạn đã đặt lịch khám trong ca trực này!";
                return RedirectToAction(nameof(XemLichTrucBacSi));
            }

            // Tự sinh mã cuộc hẹn
            var lastCuocHen = await _context.CuocHenKhams
                .OrderByDescending(c => c.MaCuocHen)
                .FirstOrDefaultAsync();

            int nextId = 1;
            if (lastCuocHen != null && int.TryParse(lastCuocHen.MaCuocHen?.Substring(2), out int lastId))
            {
                nextId = lastId + 1;
            }

            cuocHenKham.MaCuocHen = "CH" + nextId.ToString("D4");
            cuocHenKham.MaBenhNhan = user.MaBenhNhan;
            cuocHenKham.TrangThaiHen = "Chờ khám";
            cuocHenKham.MaBacSiDuKien = caTruc.MaNhanVien;
            cuocHenKham.CaKham = caTruc.TenCa;

            // ✨ QUAN TRỌNG: Kết hợp Ngày + Giờ khám dự kiến
            if (cuocHenKham.GioKhamDuKien.HasValue)
            {
                cuocHenKham.NgayGioHen = caTruc.NgayTruc.Date.Add(cuocHenKham.GioKhamDuKien.Value);
            }
            else
            {
                // Nếu không chọn giờ, dùng giờ bắt đầu ca
                cuocHenKham.NgayGioHen = caTruc.NgayTruc.Date.Add(caTruc.GioBatDau);
            }

            _context.CuocHenKhams.Add(cuocHenKham);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt lịch khám thành công! Vui lòng chờ xác nhận.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Tạo lịch khám cho bệnh nhân (bác sĩ tạo theo ca trực của mình)
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Create(int? maCaTruc)
        {
            var user = await _userManager.GetUserAsync(User);

            ViewData["MaBenhNhan"] = new SelectList(_context.BenhNhans.OrderBy(b => b.HoTen), "MaBenhNhan", "HoTen");

            // Nếu là bác sĩ, chỉ lấy ca trực của bác sĩ đó
            if (User.IsInRole(SD.Role_BacSi) && !string.IsNullOrEmpty(user?.MaNhanVien))
            {
                var caTrucs = await _context.CaTrucs
                    .Where(c => c.MaNhanVien == user.MaNhanVien && c.NgayTruc >= DateTime.Today)
                    .OrderBy(c => c.NgayTruc)
                    .ThenBy(c => c.GioBatDau)
                    .Select(c => new
                    {
                        c.MaCaTruc,
                        DisplayText = $"{c.NgayTruc:dd/MM/yyyy} - Ca {c.TenCa} ({c.GioBatDau:hh\\:mm} - {c.GioKetThuc:hh\\:mm})"
                    })
                    .ToListAsync();

                ViewData["MaCaTruc"] = new SelectList(caTrucs, "MaCaTruc", "DisplayText", maCaTruc);
                ViewData["MaBacSi"] = user.MaNhanVien;
            }
            else
            {
                ViewData["MaBacSiDuKien"] = new SelectList(
                    _context.NhanVienYTes.Where(nv => nv.VaiTro == "Bác sĩ").OrderBy(nv => nv.HoTen),
                    "MaNhanVien", "HoTen"
                );
            }

            var cuocHenKham = new CuocHenKham();
            if (maCaTruc.HasValue)
            {
                var caTruc = await _context.CaTrucs.FindAsync(maCaTruc.Value);
                if (caTruc != null)
                {
                    cuocHenKham.MaCaTruc = maCaTruc.Value;
                    cuocHenKham.NgayGioHen = caTruc.NgayTruc;
                    cuocHenKham.CaKham = caTruc.TenCa;
                    cuocHenKham.MaBacSiDuKien = caTruc.MaNhanVien;
                }
            }

            return View(cuocHenKham);
        }

        // POST: CuocHenKhams/Create
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CuocHenKham cuocHenKham)
        {
            if (ModelState.IsValid)
            {
                // Tự sinh Mã Cuộc Hẹn
                var lastCuocHen = await _context.CuocHenKhams
                    .OrderByDescending(c => c.MaCuocHen)
                    .FirstOrDefaultAsync();

                int nextId = 1;
                if (lastCuocHen != null && int.TryParse(lastCuocHen.MaCuocHen?.Substring(2), out int lastId))
                {
                    nextId = lastId + 1;
                }

                cuocHenKham.MaCuocHen = "CH" + nextId.ToString("D4");

                // Nếu có chọn ca trực, lấy thông tin từ ca trực
                if (cuocHenKham.MaCaTruc.HasValue)
                {
                    var caTruc = await _context.CaTrucs.FindAsync(cuocHenKham.MaCaTruc.Value);
                    if (caTruc != null)
                    {
                        cuocHenKham.NgayGioHen = caTruc.NgayTruc;
                        cuocHenKham.CaKham = caTruc.TenCa;
                        cuocHenKham.MaBacSiDuKien = caTruc.MaNhanVien;
                    }
                }

                cuocHenKham.TrangThaiHen = "Đã xác nhận"; // Admin/Bác sĩ tạo thì xác nhận luôn

                _context.Add(cuocHenKham);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo lịch khám thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaBenhNhan"] = new SelectList(_context.BenhNhans, "MaBenhNhan", "HoTen", cuocHenKham.MaBenhNhan);
            ViewData["MaBacSiDuKien"] = new SelectList(
                _context.NhanVienYTes.Where(nv => nv.VaiTro == "Bác sĩ"),
                "MaNhanVien", "HoTen", cuocHenKham.MaBacSiDuKien
            );
            return View(cuocHenKham);
        }

        // GET: Lịch khám của bác sĩ trong các ca trực
        // GET: Lịch khám của bác sĩ trong các ca trực
        // GET: Lịch khám của bác sĩ trong các ca trực
        [Authorize(Roles = SD.Role_BacSi)]
        public async Task<IActionResult> LichKhamTheoCaTruc(DateTime? tuNgay)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                Console.WriteLine($"🔍 USER: {user?.UserName}, MaNhanVien: {user?.MaNhanVien}");

                if (string.IsNullOrEmpty(user?.MaNhanVien))
                {
                    TempData["ErrorMessage"] = "Bạn chưa được gán mã nhân viên!";
                    return RedirectToAction("Index", "Dashboard");
                }

                var startDate = tuNgay ?? DateTime.Today;
                var endDate = startDate.AddDays(7);

                Console.WriteLine($"🔍 Tìm ca trực từ {startDate:yyyy-MM-dd} đến {endDate:yyyy-MM-dd}");

                // ✅ BƯỚC 1: Load TẤT CẢ dữ liệu vào memory trước
                var allCaTrucs = await _context.CaTrucs.AsNoTracking().ToListAsync();
                var allCuocHens = await _context.CuocHenKhams.AsNoTracking().ToListAsync();
                var allBenhNhans = await _context.BenhNhans.AsNoTracking().ToListAsync();

                Console.WriteLine($"🔍 Đã load: {allCaTrucs.Count} ca trực, {allCuocHens.Count} cuộc hẹn, {allBenhNhans.Count} bệnh nhân");

                // ✅ BƯỚC 2: Filter trong C# (KHÔNG dùng SQL)
                var caTrucs = allCaTrucs
                    .Where(c => c.MaNhanVien == user.MaNhanVien
                             && c.NgayTruc >= startDate
                             && c.NgayTruc < endDate)
                    .OrderBy(c => c.NgayTruc)
                    .ThenBy(c => c.GioBatDau)
                    .ToList();

                Console.WriteLine($"🔍 Ca trực của bác sĩ {user.MaNhanVien}: {caTrucs.Count}");
                foreach (var ct in caTrucs)
                {
                    Console.WriteLine($"   - Ca {ct.MaCaTruc}: {ct.NgayTruc:yyyy-MM-dd} - {ct.TenCa}");
                }

                // ✅ BƯỚC 3: Lấy danh sách MaCaTruc
                var maCaTrucs = caTrucs.Select(c => c.MaCaTruc).ToList();

                // ✅ BƯỚC 4: Filter cuộc hẹn trong C# (KHÔNG dùng Contains trong SQL)
                var cuocHenKhams = allCuocHens
                    .Where(c => c.MaCaTruc.HasValue
                             && maCaTrucs.Contains(c.MaCaTruc.Value)
                             && c.TrangThaiHen != "Đã hủy")
                    .OrderBy(c => c.NgayGioHen)
                    .ThenBy(c => c.GioKhamDuKien)
                    .ToList();

                Console.WriteLine($"🔍 Cuộc hẹn trong các ca trực: {cuocHenKhams.Count}");
                foreach (var ch in cuocHenKhams)
                {
                    Console.WriteLine($"   - Hẹn {ch.MaCuocHen}: MaCaTruc={ch.MaCaTruc}, BN={ch.MaBenhNhan}, Trạng thái={ch.TrangThaiHen}");
                }

                // ✅ BƯỚC 5: Gán navigation properties trong C#
                foreach (var cuocHen in cuocHenKhams)
                {
                    cuocHen.BenhNhan = allBenhNhans.FirstOrDefault(b => b.MaBenhNhan == cuocHen.MaBenhNhan);
                    cuocHen.CaTruc = caTrucs.FirstOrDefault(c => c.MaCaTruc == cuocHen.MaCaTruc);
                }

                ViewBag.CaTrucs = caTrucs;
                ViewBag.TuNgay = startDate;

                Console.WriteLine($"✅ Trả về View với {cuocHenKhams.Count} cuộc hẹn");

                return View(cuocHenKhams);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                ViewBag.CaTrucs = new List<CaTruc>();
                ViewBag.TuNgay = DateTime.Today;

                return View(new List<CuocHenKham>());
            }
        }

        // GET: CuocHenKhams/Edit/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var cuocHenKham = await _context.CuocHenKhams
                .Include(c => c.CaTruc)
                .FirstOrDefaultAsync(c => c.MaCuocHen == id);

            if (cuocHenKham == null) return NotFound();

            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == cuocHenKham.MaBenhNhan);
            ViewBag.TenBenhNhan = benhNhan?.HoTen;

            ViewData["MaBacSiDuKien"] = new SelectList(
                _context.NhanVienYTes.Where(nv => nv.VaiTro == "Bác sĩ"),
                "MaNhanVien", "HoTen", cuocHenKham.MaBacSiDuKien
            );

            ViewData["TrangThaiList"] = new SelectList(new[] { "Chờ khám", "Đã xác nhận", "Đã khám", "Đã hủy" }, cuocHenKham.TrangThaiHen);

            return View(cuocHenKham);
        }

        // POST: CuocHenKhams/Edit/5
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CuocHenKham cuocHenKham)
        {
            if (id != cuocHenKham.MaCuocHen) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var cuocHenHienTai = await _context.CuocHenKhams
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.MaCuocHen == id);

                    if (cuocHenHienTai == null) return NotFound();

                    cuocHenKham.MaBenhNhan = cuocHenHienTai.MaBenhNhan;
                    _context.Update(cuocHenKham);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật lịch khám thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuocHenKhamExists(cuocHenKham.MaCuocHen))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["TrangThaiList"] = new SelectList(new[] { "Chờ khám", "Đã xác nhận", "Đã khám", "Đã hủy" }, cuocHenKham.TrangThaiHen);
            return View(cuocHenKham);
        }

        // GET: CuocHenKhams/Delete/5
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var cuocHenKham = await _context.CuocHenKhams
                .Include(c => c.BenhNhan)
                .Include(c => c.BacSiDuKien)
                .Include(c => c.CaTruc)
                .FirstOrDefaultAsync(m => m.MaCuocHen == id);

            if (cuocHenKham == null) return NotFound();

            return View(cuocHenKham);
        }

        // POST: CuocHenKhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_BacSi + "," + SD.Role_YTa)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cuocHenKham = await _context.CuocHenKhams.FindAsync(id);
            if (cuocHenKham != null)
            {
                _context.CuocHenKhams.Remove(cuocHenKham);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa lịch khám thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Hủy lịch khám (cho bệnh nhân)
        [HttpPost]
        [Authorize(Roles = SD.Role_BenhNhan)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyLichKham(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var cuocHenKham = await _context.CuocHenKhams.FindAsync(id);

            if (cuocHenKham == null) return NotFound();

            // Kiểm tra quyền hủy
            if (cuocHenKham.MaBenhNhan != user?.MaBenhNhan)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền hủy lịch khám này!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra thời gian hủy (ví dụ: chỉ được hủy trước 2 giờ)
            var thoiGianKham = cuocHenKham.NgayGioHen.Date.Add(cuocHenKham.GioKhamDuKien ?? TimeSpan.Zero);
            if (thoiGianKham.Subtract(DateTime.Now).TotalHours < 2)
            {
                TempData["ErrorMessage"] = "Chỉ được hủy lịch trước ít nhất 2 giờ!";
                return RedirectToAction(nameof(Index));
            }

            cuocHenKham.TrangThaiHen = "Đã hủy";
            _context.Update(cuocHenKham);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã hủy lịch khám thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool CuocHenKhamExists(string id)
        {
            return _context.CuocHenKhams.Any(e => e.MaCuocHen == id);
        }

        // Helper method để tạo gợi ý giờ khám
        private List<TimeSpan> GenerateGioKhamGoiY(CaTruc caTruc, int soLuongHenHienTai)
        {
            var gioGoiY = new List<TimeSpan>();
            var gioBatDau = caTruc.GioBatDau;
            var gioKetThuc = caTruc.GioKetThuc;

            // Chia ca thành các khung giờ 30 phút
            var currentTime = gioBatDau;
            while (currentTime < gioKetThuc)
            {
                gioGoiY.Add(currentTime);
                currentTime = currentTime.Add(TimeSpan.FromMinutes(30));
            }

            return gioGoiY;
        }

        // POST: Cập nhật trạng thái đã khám
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_BacSi)]
        public async Task<IActionResult> CapNhatDaKham(string id)
        {
            var cuocHenKham = await _context.CuocHenKhams.FindAsync(id);

            if (cuocHenKham == null)
                return NotFound();

            // Kiểm tra quyền (chỉ bác sĩ được phân công mới được cập nhật)
            var user = await _userManager.GetUserAsync(User);
            if (cuocHenKham.MaBacSiDuKien != user?.MaNhanVien)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền cập nhật cuộc hẹn này!";
                return RedirectToAction(nameof(LichKhamTheoCaTruc));
            }

            cuocHenKham.TrangThaiHen = "Đã khám";
            _context.Update(cuocHenKham);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã cập nhật trạng thái khám thành công!";
            return RedirectToAction(nameof(LichKhamTheoCaTruc));
        }
    }
}