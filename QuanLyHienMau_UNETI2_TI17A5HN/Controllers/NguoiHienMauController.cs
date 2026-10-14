// ============================================================================
// DỰ ÁN QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VITALIS - UNETI (TI17A5HN)
// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Phụ trách: Nhóm thực hiện Module 3 - Lớp DHTI17A5HN
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;
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
                // Tự động tạo hồ sơ NguoiHienMau nếu tài khoản chưa có (Auto-provisioning)
                var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
                if (taiKhoan == null)
                {
                    return NotFound("Không tìm thấy thông tin tài khoản người dùng.");
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
                return NotFound("Không tìm thấy hồ sơ người hiến máu của tài khoản hiện tại.");
            }

            // --- SERVER-SIDE VALIDATION ---
            // 1. Kiểm tra Họ và tên
            if (string.IsNullOrWhiteSpace(model.HoTen))
            {
                ModelState.AddModelError("HoTen", "Họ tên người hiến máu không được để trống.");
            }
            else if (model.HoTen.Trim().Length > 100)
            {
                ModelState.AddModelError("HoTen", "Họ tên không được vượt quá 100 ký tự.");
            }

            // 2. Kiểm tra Ngày sinh & Độ tuổi (18 - 60 tuổi theo quy định y tế)
            if (model.NgaySinh.HasValue)
            {
                var birthDate = model.NgaySinh.Value.Date;
                var today = DateTime.Today;

                if (birthDate > today)
                {
                    ModelState.AddModelError("NgaySinh", "Ngày sinh không hợp lệ (không được lớn hơn ngày hiện tại).");
                }
                else
                {
                    int age = today.Year - birthDate.Year;
                    if (birthDate > today.AddYears(-age)) age--;

                    if (age < 18 || age > 60)
                    {
                        ModelState.AddModelError("NgaySinh", $"Độ tuổi người hiến máu theo quy định phải từ 18 đến 60 tuổi (Hiện tại: {age} tuổi).");
                    }
                }
            }

            // 3. Kiểm tra Cân nặng
            if (!model.CanNang.HasValue)
            {
                ModelState.AddModelError("CanNang", "Vui lòng nhập cân nặng hiện tại (kg).");
            }
            else if (model.CanNang.Value < 30.0 || model.CanNang.Value > 200.0)
            {
                ModelState.AddModelError("CanNang", "Cân nặng phải từ 30kg đến 200kg.");
            }

            // 4. Kiểm tra Số điện thoại
            if (!string.IsNullOrWhiteSpace(model.SoDienThoai))
            {
                var phone = model.SoDienThoai.Trim();
                if (!Regex.IsMatch(phone, @"^0[0-9]{9,10}$"))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại không hợp lệ (phải gồm 10-11 chữ số và bắt đầu bằng số 0).");
                }
            }

            // 5. Kiểm tra Email
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var email = model.Email.Trim();
                if (!new EmailAddressAttribute().IsValid(email))
                {
                    ModelState.AddModelError("Email", "Địa chỉ email không đúng định dạng.");
                }
            }

            // Nếu phát hiện lỗi validation
            if (!ModelState.IsValid)
            {
                // Giữ lại các giá trị người dùng vừa nhập vào đối tượng render ra form
                nguoiHienMau.HoTen = model.HoTen ?? string.Empty;
                nguoiHienMau.NgaySinh = model.NgaySinh;
                nguoiHienMau.GioiTinh = model.GioiTinh;
                nguoiHienMau.SoDienThoai = model.SoDienThoai;
                nguoiHienMau.Email = model.Email;
                nguoiHienMau.DiaChi = model.DiaChi;
                nguoiHienMau.NhomMau = model.NhomMau;
                nguoiHienMau.NgheNghiep = model.NgheNghiep;
                nguoiHienMau.CanNang = model.CanNang;
                nguoiHienMau.GhiChuSucKhoe = model.GhiChuSucKhoe;

                // Tải lại các quan hệ phiếu để View không bị lỗi hiển thị
                await _context.Entry(nguoiHienMau)
                    .Collection(n => n.PhieuDangKyHienMaus)
                    .Query()
                    .Include(p => p.DotHienMau!)
                        .ThenInclude(d => d.DonViToChuc)
                    .Include(p => p.LichKhamSangLocs)
                    .Include(p => p.KetQuaHienMau)
                    .LoadAsync();

                ViewBag.OpenEditModal = true;
                TempData["ErrorMessage"] = "Cập nhật hồ sơ thất bại. Vui lòng kiểm tra các trường báo lỗi.";
                ViewData["Title"] = "Hồ Sơ Người Hiến Máu & Chứng Nhận Điện Tử";
                return View(nguoiHienMau);
            }

            // Cập nhật an toàn chỉ các trường được phép thay đổi (Whitelist)
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

            // Đồng bộ sang TaiKhoan nếu có
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
