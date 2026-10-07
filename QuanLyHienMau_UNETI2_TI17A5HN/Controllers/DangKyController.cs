using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    [Authorize]
    public class DangKyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DangKyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /DangKy/Create?maDot=1
        [HttpGet]
        public async Task<IActionResult> Create(int? maDot)
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
                var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
                if (taiKhoan == null)
                {
                    return NotFound("Không tìm thấy thông tin tài khoản.");
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

            DotHienMau? dotHienMau = null;
            if (maDot.HasValue)
            {
                dotHienMau = await _context.DotHienMaus
                    .Include(d => d.DonViToChuc)
                    .FirstOrDefaultAsync(d => d.MaDot == maDot.Value);
            }
            else
            {
                dotHienMau = await _context.DotHienMaus
                    .Include(d => d.DonViToChuc)
                    .Where(d => d.TrangThai == "Đang mở đăng ký")
                    .OrderByDescending(d => d.NgayBatDauDangKy)
                    .FirstOrDefaultAsync();
            }

            if (dotHienMau == null)
            {
                TempData["ErrorMessage"] = "Hiện tại không có đợt hiến máu nào đang mở đăng ký.";
                return RedirectToAction("Index", "DotHienMau");
            }

            if (dotHienMau.TrangThai != "Đang mở đăng ký")
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' đang ở trạng thái '{dotHienMau.TrangThai}', không thể tiếp nhận đăng ký mới.";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }

            // Kiểm tra xem người hiến đã đăng ký đợt này chưa
            var existingPhieu = await _context.PhieuDangKyHienMaus
                .FirstOrDefaultAsync(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien && p.MaDot == dotHienMau.MaDot && p.TrangThai != "Đã hủy");

            if (existingPhieu != null)
            {
                TempData["ErrorMessage"] = "Bạn đã có phiếu đăng ký cho đợt hiến máu này rồi.";
                return RedirectToAction(nameof(TheoDoi), new { id = existingPhieu.MaPhieu });
            }

            ViewBag.DotHienMau = dotHienMau;
            ViewBag.NguoiHienMau = nguoiHienMau;
            ViewData["Title"] = $"Nộp Phiếu Đăng Ký Hiến Máu – {dotHienMau.TenDot}";

            return View(dotHienMau);
        }

        // POST: /DangKy/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int maDot,
            string? timeSlot,
            string? bloodVolume,
            string? bloodGroup,
            double? canNang,
            string? hoTen,
            string? soDienThoai,
            string? email,
            string? lyDoHienMau,
            string? ghiChu)
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
                return NotFound("Không tìm thấy thông tin người hiến máu.");
            }

            var dotHienMau = await _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .FirstOrDefaultAsync(d => d.MaDot == maDot);

            if (dotHienMau == null || dotHienMau.TrangThai != "Đang mở đăng ký")
            {
                TempData["ErrorMessage"] = "Đợt hiến máu không tồn tại hoặc đã đóng đăng ký.";
                return RedirectToAction("Index", "DotHienMau");
            }

            // Kiểm tra cân nặng tối thiểu
            if (canNang.HasValue && canNang.Value < dotHienMau.CanNangToiThieu)
            {
                var weightErrMsg = $"Cân nặng của bạn ({canNang.Value}kg) chưa đạt mức tối thiểu quy định cho đợt này (≥ {dotHienMau.CanNangToiThieu}kg).";
                ViewBag.ErrorMessage = weightErrMsg;
                TempData["ErrorMessage"] = weightErrMsg;
                ViewBag.DotHienMau = dotHienMau;
                ViewBag.NguoiHienMau = nguoiHienMau;
                return View(dotHienMau);
            }

            // Kiểm tra trùng lặp phiếu
            var existingPhieu = await _context.PhieuDangKyHienMaus
                .FirstOrDefaultAsync(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien && p.MaDot == maDot && p.TrangThai != "Đã hủy");

            if (existingPhieu != null)
            {
                TempData["ErrorMessage"] = "Bạn đã có phiếu đăng ký đang xử lý cho đợt hiến máu này.";
                return RedirectToAction(nameof(TheoDoi), new { id = existingPhieu.MaPhieu });
            }

            // Cập nhật thông tin nhanh cho hồ sơ người hiến
            if (!string.IsNullOrWhiteSpace(hoTen)) nguoiHienMau.HoTen = hoTen.Trim();
            if (!string.IsNullOrWhiteSpace(soDienThoai)) nguoiHienMau.SoDienThoai = soDienThoai.Trim();
            if (!string.IsNullOrWhiteSpace(email)) nguoiHienMau.Email = email.Trim();
            if (!string.IsNullOrWhiteSpace(bloodGroup) && bloodGroup != "UNKNOWN") nguoiHienMau.NhomMau = bloodGroup.Trim();
            if (canNang.HasValue) nguoiHienMau.CanNang = canNang.Value;

            // Tạo phiếu đăng ký
            var phieu = new PhieuDangKyHienMau
            {
                MaNguoiHien = nguoiHienMau.MaNguoiHien,
                MaDot = maDot,
                NgayNop = DateTime.Now,
                TrangThai = "Chờ xác nhận",
                LyDoHienMau = !string.IsNullOrEmpty(bloodVolume) ? $"Dung tích dự kiến: {bloodVolume}. {lyDoHienMau}".Trim() : lyDoHienMau,
                GhiChu = !string.IsNullOrEmpty(timeSlot) ? $"Khung giờ hẹn: {timeSlot}. {ghiChu}".Trim() : ghiChu
            };
            _context.PhieuDangKyHienMaus.Add(phieu);
            await _context.SaveChangesAsync();

            // Tạo lịch khám sàng lọc tự động
            var lichKham = new LichKhamSangLoc
            {
                MaPhieu = phieu.MaPhieu,
                ThoiGianBatDau = dotHienMau.NgayBatDauDangKy.Date.AddHours(7).AddMinutes(30),
                ThoiGianKetThuc = dotHienMau.NgayBatDauDangKy.Date.AddHours(11).AddMinutes(30),
                HinhThucTiepNhan = "Trực tiếp tại điểm hiến",
                DiaDiemHoacLienKet = dotHienMau.DonViToChuc?.TenDonViToChuc ?? "Điểm hiến máu UNETI",
                BacSiSangLoc = "BS. CKI Trần Mai Linh",
                GhiChu = timeSlot ?? "Ca 1: 07:30 – 09:30",
                TrangThai = "Đã lên lịch"
            };
            _context.LichKhamSangLocs.Add(lichKham);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đăng ký hiến máu thành công! Mã phiếu của bạn là #DKHM-{phieu.MaPhieu:D5}.";
            return RedirectToAction(nameof(TheoDoi), new { id = phieu.MaPhieu });
        }

        // GET: /DangKy/TheoDoi?id=1&code=...
        [HttpGet]
        public async Task<IActionResult> TheoDoi(int? id, string? code)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int maTaiKhoan))
            {
                return Challenge();
            }

            var nguoiHienMau = await _context.NguoiHienMaus
                .FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan);

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IQueryable<PhieuDangKyHienMau> query = _context.PhieuDangKyHienMaus
                .Include(p => p.DotHienMau!)
                    .ThenInclude(d => d.DonViToChuc)
                .Include(p => p.NguoiHienMau)
                .Include(p => p.LichKhamSangLocs)
                .Include(p => p.KetQuaHienMau);

            List<PhieuDangKyHienMau> userPhieuList = new List<PhieuDangKyHienMau>();

            if (userRole == "Donor" || nguoiHienMau != null)
            {
                if (nguoiHienMau != null)
                {
                    userPhieuList = await query
                        .Where(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien)
                        .OrderByDescending(p => p.NgayNop)
                        .ToListAsync();
                }
            }
            else
            {
                // Admin / Medical Staff
                userPhieuList = await query
                    .OrderByDescending(p => p.NgayNop)
                    .Take(20)
                    .ToListAsync();
            }

            PhieuDangKyHienMau? selectedPhieu = null;

            if (id.HasValue)
            {
                selectedPhieu = userPhieuList.FirstOrDefault(p => p.MaPhieu == id.Value);
                if (selectedPhieu == null && (userRole == "Admin" || userRole == "Medical Staff"))
                {
                    selectedPhieu = await query.FirstOrDefaultAsync(p => p.MaPhieu == id.Value);
                }
            }

            if (selectedPhieu == null && !string.IsNullOrEmpty(code))
            {
                // Code dạng DKHM-00001 hoặc 1
                var cleanCode = code.Replace("#", "").Replace("DKHM-", "").Replace("VTL-", "").Trim();
                if (int.TryParse(cleanCode, out int parsedId))
                {
                    selectedPhieu = userPhieuList.FirstOrDefault(p => p.MaPhieu == parsedId);
                }
            }

            if (selectedPhieu == null)
            {
                selectedPhieu = userPhieuList.FirstOrDefault();
            }

            ViewBag.DanhSachPhieu = userPhieuList;
            ViewData["Title"] = selectedPhieu != null
                ? $"Theo Dõi Phiếu Đăng Ký #DKHM-{selectedPhieu.MaPhieu:D5}"
                : "Theo Dõi Trạng Thái Phiếu Đăng Ký Hiến Máu";

            return View(selectedPhieu);
        }

        // POST: /DangKy/Huy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int maTaiKhoan))
            {
                return Challenge();
            }

            var nguoiHienMau = await _context.NguoiHienMaus
                .FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan);

            var phieu = await _context.PhieuDangKyHienMaus
                .Include(p => p.LichKhamSangLocs)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null)
            {
                return NotFound("Không tìm thấy phiếu đăng ký.");
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Kiểm tra quyền sở hữu đối với Donor
            if (userRole == "Donor" && (nguoiHienMau == null || phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien))
            {
                return Forbid();
            }

            // Chỉ cho phép hủy khi phiếu đang ở trạng thái 'Chờ xác nhận' hoặc 'Đã xác nhận'
            if (phieu.TrangThai == "Hiến máu thành công" || phieu.TrangThai == "Đã khám sàng lọc")
            {
                TempData["ErrorMessage"] = $"Không thể hủy phiếu đã ở trạng thái '{phieu.TrangThai}'.";
                return RedirectToAction(nameof(TheoDoi), new { id = phieu.MaPhieu });
            }

            phieu.TrangThai = "Đã hủy";
            phieu.NgayXuLy = DateTime.Now;
            phieu.GhiChuXacNhan = "Người hiến đã chủ động hủy phiếu đăng ký.";

            foreach (var lich in phieu.LichKhamSangLocs)
            {
                lich.TrangThai = "Đã hủy";
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã hủy thành công phiếu đăng ký #DKHM-{phieu.MaPhieu:D5}.";

            return RedirectToAction(nameof(TheoDoi), new { id = phieu.MaPhieu });
        }
    }
}
