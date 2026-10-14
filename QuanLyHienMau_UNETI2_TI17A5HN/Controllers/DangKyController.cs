// ============================================================================
// DỰ ÁN QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VITALIS - UNETI (TI17A5HN)
// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Phụ trách: Nhóm thực hiện Module 3 - Lớp DHTI17A5HN
// ============================================================================

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

            // 1. Kiểm tra trạng thái hoạt động của người hiến
            if (nguoiHienMau.TrangThai != "Hoạt động")
            {
                TempData["ErrorMessage"] = "Hồ sơ của bạn hiện đang ở trạng thái tạm khóa hoặc không hoạt động, không thể đăng ký tham gia đợt hiến máu.";
                return RedirectToAction("HoSoCaNhan", "NguoiHienMau");
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
                    .Where(d => d.TrangThai == "Đang mở đăng ký" && d.NgayBatDauDangKy <= DateTime.Now && d.HanDangKy >= DateTime.Now)
                    .OrderByDescending(d => d.NgayBatDauDangKy)
                    .FirstOrDefaultAsync();
            }

            if (dotHienMau == null)
            {
                TempData["ErrorMessage"] = "Hiện tại không tìm thấy đợt hiến máu nào phù hợp để đăng ký.";
                return RedirectToAction("Index", "DotHienMau");
            }

            // 2. Kiểm tra trạng thái đợt hiến
            if (dotHienMau.TrangThai != "Đang mở đăng ký")
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' đang ở trạng thái '{dotHienMau.TrangThai}', không tiếp nhận đăng ký mới.";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }

            // 3. Kiểm tra khoảng thời gian nhận đăng ký của đợt hiến
            var now = DateTime.Now;
            if (now < dotHienMau.NgayBatDauDangKy)
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' chưa đến thời gian mở nhận đăng ký (Mở từ {dotHienMau.NgayBatDauDangKy:dd/MM/yyyy}).";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }
            if (now > dotHienMau.HanDangKy)
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' đã kết thúc thời hạn nhận đăng ký vào ngày {dotHienMau.HanDangKy:dd/MM/yyyy}.";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }

            // 4. Kiểm tra xem người hiến đã có phiếu đang xử lý cho đợt này chưa
            var existingPhieu = await _context.PhieuDangKyHienMaus
                .FirstOrDefaultAsync(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien 
                                       && p.MaDot == dotHienMau.MaDot 
                                       && p.TrangThai != "Đã hủy" 
                                       && p.TrangThai != "Từ chối");

            if (existingPhieu != null)
            {
                TempData["ErrorMessage"] = "Bạn đã có phiếu đăng ký đang xử lý cho đợt hiến máu này rồi.";
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
                return NotFound("Không tìm thấy thông tin hồ sơ người hiến máu.");
            }

            // 1. Kiểm tra trạng thái hoạt động của người hiến
            if (nguoiHienMau.TrangThai != "Hoạt động")
            {
                TempData["ErrorMessage"] = "Hồ sơ của bạn hiện đang ở trạng thái tạm khóa hoặc không hoạt động, không thể thực hiện nộp phiếu.";
                return RedirectToAction("HoSoCaNhan", "NguoiHienMau");
            }

            var dotHienMau = await _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .FirstOrDefaultAsync(d => d.MaDot == maDot);

            if (dotHienMau == null)
            {
                TempData["ErrorMessage"] = "Đợt hiến máu không tồn tại.";
                return RedirectToAction("Index", "DotHienMau");
            }

            // 2. Kiểm tra trạng thái đợt hiến máu
            if (dotHienMau.TrangThai != "Đang mở đăng ký")
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' đang ở trạng thái '{dotHienMau.TrangThai}', không thể tiếp nhận đăng ký mới.";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }

            // 3. Kiểm tra thời hạn đăng ký tại Server
            var now = DateTime.Now;
            if (now < dotHienMau.NgayBatDauDangKy)
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' chưa đến ngày mở nhận đăng ký.";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }
            if (now > dotHienMau.HanDangKy)
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' đã kết thúc hạn nhận đăng ký vào ngày {dotHienMau.HanDangKy:dd/MM/yyyy}.";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }

            // 4. Server-side validation thông tin người đăng ký
            var finalHoTen = !string.IsNullOrWhiteSpace(hoTen) ? hoTen.Trim() : nguoiHienMau.HoTen;
            if (string.IsNullOrWhiteSpace(finalHoTen))
            {
                ViewBag.ErrorMessage = "Vui lòng cập nhật đầy đủ họ và tên trước khi nộp phiếu.";
                ViewBag.DotHienMau = dotHienMau;
                ViewBag.NguoiHienMau = nguoiHienMau;
                return View(dotHienMau);
            }

            var finalPhone = !string.IsNullOrWhiteSpace(soDienThoai) ? soDienThoai.Trim() : nguoiHienMau.SoDienThoai;
            if (string.IsNullOrWhiteSpace(finalPhone))
            {
                ViewBag.ErrorMessage = "Vui lòng cung cấp số điện thoại liên lạc để nhận xác nhận phiếu.";
                ViewBag.DotHienMau = dotHienMau;
                ViewBag.NguoiHienMau = nguoiHienMau;
                return View(dotHienMau);
            }

            // 5. Kiểm tra cân nặng tối thiểu
            var finalWeight = canNang ?? nguoiHienMau.CanNang;
            if (!finalWeight.HasValue || finalWeight.Value <= 0)
            {
                ViewBag.ErrorMessage = "Vui lòng nhập cân nặng hiện tại của bạn.";
                ViewBag.DotHienMau = dotHienMau;
                ViewBag.NguoiHienMau = nguoiHienMau;
                return View(dotHienMau);
            }

            if (finalWeight.Value < dotHienMau.CanNangToiThieu)
            {
                var weightErrMsg = $"Cân nặng của bạn ({finalWeight.Value}kg) chưa đạt mức tối thiểu quy định cho đợt này (yêu cầu ≥ {dotHienMau.CanNangToiThieu}kg).";
                ViewBag.ErrorMessage = weightErrMsg;
                TempData["ErrorMessage"] = weightErrMsg;
                ViewBag.DotHienMau = dotHienMau;
                ViewBag.NguoiHienMau = nguoiHienMau;
                return View(dotHienMau);
            }

            // 6. Kiểm tra chống trùng lặp phiếu đang xử lý cho cùng đợt
            var existingPhieu = await _context.PhieuDangKyHienMaus
                .FirstOrDefaultAsync(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien 
                                       && p.MaDot == maDot 
                                       && p.TrangThai != "Đã hủy" 
                                       && p.TrangThai != "Từ chối");

            if (existingPhieu != null)
            {
                TempData["ErrorMessage"] = "Bạn đã có phiếu đăng ký đang trong tiến trình xử lý cho đợt hiến máu này.";
                return RedirectToAction(nameof(TheoDoi), new { id = existingPhieu.MaPhieu });
            }

            // 7. Cập nhật thông tin nhanh cho hồ sơ người hiến
            nguoiHienMau.HoTen = finalHoTen;
            nguoiHienMau.SoDienThoai = finalPhone;
            if (!string.IsNullOrWhiteSpace(email)) nguoiHienMau.Email = email.Trim();
            if (!string.IsNullOrWhiteSpace(bloodGroup) && bloodGroup != "UNKNOWN")
            {
                nguoiHienMau.NhomMau = bloodGroup.Trim();
            }
            nguoiHienMau.CanNang = finalWeight.Value;

            // 8. Tạo phiếu đăng ký (Server xác định NgayNop, TrangThai khởi tạo là "Chờ xác nhận")
            var phieu = new PhieuDangKyHienMau
            {
                MaNguoiHien = nguoiHienMau.MaNguoiHien,
                MaDot = maDot,
                NgayNop = DateTime.Now,
                TrangThai = "Chờ xác nhận",
                LyDoHienMau = !string.IsNullOrEmpty(bloodVolume) ? $"Dung tích dự kiến: {bloodVolume}. {lyDoHienMau}".Trim() : lyDoHienMau,
                GhiChu = !string.IsNullOrEmpty(timeSlot) ? $"Khung giờ hẹn: {timeSlot}. {ghiChu}".Trim() : ghiChu,
                NgayXuLy = null,
                GhiChuXacNhan = null
            };

            _context.PhieuDangKyHienMaus.Add(phieu);
            await _context.SaveChangesAsync();

            // Lưu ý: Lịch khám sàng lọc và phân công bác sĩ sẽ do nhân viên y tế (Module 4) thực hiện khi duyệt phiếu,
            // Module 3 không tự tiện tạo lịch khám giả định trước khi phiếu được duyệt.

            TempData["SuccessMessage"] = $"Đăng ký hiến máu thành công! Mã phiếu của bạn là #DKHM-{phieu.MaPhieu:D5}. Vui lòng chờ ban tổ chức xác nhận.";
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
                    // Donor CHỈ ĐƯỢC XEM phiếu của chính mình
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
                    .Take(30)
                    .ToListAsync();
            }

            PhieuDangKyHienMau? selectedPhieu = null;

            // Tra cứu theo ID
            if (id.HasValue)
            {
                selectedPhieu = userPhieuList.FirstOrDefault(p => p.MaPhieu == id.Value);
                if (selectedPhieu == null)
                {
                    if (userRole == "Admin" || userRole == "Medical Staff")
                    {
                        selectedPhieu = await query.FirstOrDefaultAsync(p => p.MaPhieu == id.Value);
                    }
                    else
                    {
                        // Chặn IDOR: Người dùng Donor nhập ID phiếu của người khác
                        TempData["ErrorMessage"] = "Bạn không có quyền truy cập phiếu đăng ký này hoặc phiếu không tồn tại.";
                    }
                }
            }

            // Tra cứu theo mã phiếu (VD: DKHM-00001 hoặc 1)
            if (selectedPhieu == null && !string.IsNullOrEmpty(code))
            {
                var cleanCode = code.Replace("#", "").Replace("DKHM-", "").Replace("VTL-", "").Trim();
                if (int.TryParse(cleanCode, out int parsedId))
                {
                    selectedPhieu = userPhieuList.FirstOrDefault(p => p.MaPhieu == parsedId);
                    if (selectedPhieu == null && (userRole == "Admin" || userRole == "Medical Staff"))
                    {
                        selectedPhieu = await query.FirstOrDefaultAsync(p => p.MaPhieu == parsedId);
                    }
                }
            }

            // Mặc định hiển thị phiếu mới nhất trong danh sách của người dùng
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
                .Include(p => p.KetQuaHienMau)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null)
            {
                return NotFound("Không tìm thấy thông tin phiếu đăng ký.");
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Kiểm tra quyền sở hữu đối với Donor (Chặn IDOR)
            if (userRole == "Donor" && (nguoiHienMau == null || phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien))
            {
                return Forbid();
            }

            // Chỉ cho phép hủy ở các trạng thái được phép (Whitelist: "Chờ xác nhận", "Đã xác nhận")
            var trangThaiChoPhep = new[] { "Chờ xác nhận", "Đã xác nhận" };
            if (!trangThaiChoPhep.Contains(phieu.TrangThai))
            {
                if (phieu.TrangThai == "Đã hủy")
                {
                    TempData["ErrorMessage"] = "Phiếu đăng ký này đã được hủy trước đó.";
                }
                else if (phieu.TrangThai == "Từ chối")
                {
                    TempData["ErrorMessage"] = "Phiếu đăng ký đã bị từ chối tiếp nhận, không thể thực hiện thao tác hủy.";
                }
                else if (phieu.TrangThai == "Hiến máu thành công" || phieu.TrangThai == "Không đủ điều kiện hiến")
                {
                    TempData["ErrorMessage"] = "Phiếu đã có kết quả hiến máu cuối cùng, không thể hủy.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Không thể hủy phiếu đăng ký đang ở trạng thái '{phieu.TrangThai}'. Vui lòng liên hệ ban tổ chức để được hỗ trợ.";
                }
                return RedirectToAction(nameof(TheoDoi), new { id = phieu.MaPhieu });
            }

            // Cập nhật trạng thái phiếu sang "Đã hủy"
            phieu.TrangThai = "Đã hủy";
            phieu.NgayXuLy = DateTime.Now;
            phieu.GhiChuXacNhan = "Người hiến đã chủ động hủy phiếu đăng ký trên cổng thông tin.";

            // Cập nhật các lịch khám kèm theo (nếu chưa hoàn thành)
            foreach (var lich in phieu.LichKhamSangLocs.Where(l => l.TrangThai != "Đã hoàn thành"))
            {
                lich.TrangThai = "Đã hủy";
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã hủy thành công phiếu đăng ký #DKHM-{phieu.MaPhieu:D5}.";

            return RedirectToAction(nameof(TheoDoi), new { id = phieu.MaPhieu });
        }
    }
}
