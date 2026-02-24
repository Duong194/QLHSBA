using DACS_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DACS_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;


        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<IActionResult> Index(string searchEmail)
        {
            // Lưu giá trị tìm kiếm vào ViewBag để hiển thị trên form
            ViewBag.CurrentFilter = searchEmail;

            // Lấy danh sách người dùng từ database
            var users = _userManager.Users;

            // Nếu có tìm kiếm theo email
            if (!string.IsNullOrEmpty(searchEmail))
            {
                users = users.Where(u => u.Email.Contains(searchEmail));
            }

            // Chuyển thành danh sách
            var userList = await users.ToListAsync();

            // Tạo dictionary lưu vai trò của người dùng
            var userRoles = new Dictionary<string, string>();
            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "Không có";
            }

            ViewBag.UserRoles = userRoles;
            return View(userList);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            string currentRole = roles.FirstOrDefault();
            ViewBag.CurrentRole = currentRole;
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Thêm danh sách bệnh nhân
            ViewBag.MaBenhNhanList = new SelectList(_context.BenhNhans, "MaBenhNhan", "HoTen", user.MaBenhNhan);
            // Thêm danh sách mã nhân viên
     

            // Danh sách nhân viên theo vai trò
            if (currentRole == SD.Role_BacSi)
            {
                ViewBag.MaNhanVienList = new SelectList(
                    _context.NhanVienYTes.Where(nv => nv.VaiTro == "Bác sĩ"),
                    "MaNhanVien", "HoTen", user.MaNhanVien);
            }
            else if (currentRole == SD.Role_YTa)
            {
                ViewBag.MaNhanVienList = new SelectList(
                    _context.NhanVienYTes.Where(nv => nv.VaiTro == "Y tá"),
                    "MaNhanVien", "HoTen", user.MaNhanVien);
            }
            else
            {
                // Nếu admin hoặc chưa chọn vai trò thì load toàn bộ
                ViewBag.MaNhanVienList = new SelectList(_context.NhanVienYTes, "MaNhanVien", "HoTen", user.MaNhanVien);
            }


            return View(user);
        }
        [HttpGet]
        public JsonResult GetNhanVienByRole(string role)
        {
            IQueryable<NhanVienYTe> query = _context.NhanVienYTes;

            if (role == SD.Role_BacSi)
            {
                query = query.Where(nv => nv.VaiTro == "Bác sĩ");
            }
            else if (role == SD.Role_YTa)
            {
                query = query.Where(nv => nv.VaiTro == "Y tá");
            }

            var nhanVienList = query.Select(nv => new
            {
                MaNhanVien = nv.MaNhanVien,
                HoTen = nv.HoTen
            }).ToList();

            return Json(nhanVienList);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model, string newRole)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Address = model.Address;
            user.Age = model.Age;
            user.MaBenhNhan = model.MaBenhNhan;
            user.MaNhanVien = model.MaNhanVien;

            await _userManager.UpdateAsync(user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!string.IsNullOrEmpty(newRole))
            {
                await _userManager.AddToRoleAsync(user, newRole);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        // POST: Xoá user
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu user có role Admin
            if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
            {
                TempData["Error"] = "Không thể xóa tài khoản Admin!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Xóa người dùng thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa người dùng!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
