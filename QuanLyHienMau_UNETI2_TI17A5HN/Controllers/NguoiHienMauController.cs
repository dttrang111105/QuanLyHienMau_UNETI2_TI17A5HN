using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    [Authorize]
    public class NguoiHienMauController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NguoiHienMauController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /NguoiHienMau
        public IActionResult Index()
        {
            return RedirectToAction(nameof(HoSoCaNhan));
        }

        // GET: /NguoiHienMau/HoSoCaNhan
        [HttpGet]
        public async Task<IActionResult> HoSoCaNhan()
        {
            ViewData["Title"] = "Hồ Sơ Người Hiến Máu & Chứng Nhận Điện Tử";

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int maTaiKhoan))
            {
                return Challenge();
            }

            var nguoiHienMau = await _context.NguoiHienMaus
                .Include(n => n.TaiKhoan)
                .Include(n => n.PhieuDangKyHienMaus)
                    .ThenInclude(p => p.DotHienMau!)
                        .ThenInclude(d => d.DonViToChuc)
                .Include(n => n.PhieuDangKyHienMaus)
                    .ThenInclude(p => p.LichKhamSangLocs)
                .Include(n => n.PhieuDangKyHienMaus)
                    .ThenInclude(p => p.KetQuaHienMau)
                .FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan);

            if (nguoiHienMau == null)
            {
                // Tự động tạo hồ sơ NguoiHienMau nếu chưa có
                var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
                if (taiKhoan == null)
                {
                    return NotFound("Không tìm thấy tài khoản người dùng.");
                }

                nguoiHienMau = new NguoiHienMau
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = taiKhoan.HoTen,
                    Email = taiKhoan.Email,
                    TrangThai = "Hoạt động"
                };
                _context.NguoiHienMaus.Add(nguoiHienMau);
                await _context.SaveChangesAsync();
            }

            return View(nguoiHienMau);
        }

        // POST: /NguoiHienMau/HoSoCaNhan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoSoCaNhan(NguoiHienMau model)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int maTaiKhoan))
            {
                return Challenge();
            }

            var nguoiHienMau = await _context.NguoiHienMaus
                .Include(n => n.TaiKhoan)
                .FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan);

            if (nguoiHienMau == null)
            {
                return NotFound("Không tìm thấy hồ sơ người hiến máu.");
            }

            if (string.IsNullOrWhiteSpace(model.HoTen))
            {
                ModelState.AddModelError("HoTen", "Họ tên không được để trống.");
            }

            if (model.CanNang.HasValue && (model.CanNang.Value < 30 || model.CanNang.Value > 200))
            {
                ModelState.AddModelError("CanNang", "Cân nặng phải từ 30kg đến 200kg.");
            }

            if (!ModelState.IsValid)
            {
                // Reload relations for view
                await _context.Entry(nguoiHienMau)
                    .Collection(n => n.PhieuDangKyHienMaus)
                    .Query()
                    .Include(p => p.DotHienMau!)
                        .ThenInclude(d => d.DonViToChuc)
                    .Include(p => p.LichKhamSangLocs)
                    .Include(p => p.KetQuaHienMau)
                    .LoadAsync();

                ViewData["Title"] = "Hồ Sơ Người Hiến Máu & Chứng Nhận Điện Tử";
                return View(nguoiHienMau);
            }

            // Cập nhật các trường được phép sửa
            nguoiHienMau.HoTen = model.HoTen.Trim();
            nguoiHienMau.NgaySinh = model.NgaySinh;
            nguoiHienMau.GioiTinh = model.GioiTinh;
            nguoiHienMau.SoDienThoai = model.SoDienThoai?.Trim();
            nguoiHienMau.Email = model.Email?.Trim();
            nguoiHienMau.DiaChi = model.DiaChi?.Trim();
            nguoiHienMau.NhomMau = model.NhomMau?.Trim();
            nguoiHienMau.NgheNghiep = model.NgheNghiep?.Trim();
            nguoiHienMau.CanNang = model.CanNang;
            nguoiHienMau.GhiChuSucKhoe = model.GhiChuSucKhoe?.Trim();

            // Đồng bộ thông tin tài khoản nếu có
            if (nguoiHienMau.TaiKhoan != null)
            {
                nguoiHienMau.TaiKhoan.HoTen = nguoiHienMau.HoTen;
                if (!string.IsNullOrEmpty(nguoiHienMau.Email))
                {
                    nguoiHienMau.TaiKhoan.Email = nguoiHienMau.Email;
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật hồ sơ thông tin người hiến máu thành công!";
            return RedirectToAction(nameof(HoSoCaNhan));
        }
    }
}
