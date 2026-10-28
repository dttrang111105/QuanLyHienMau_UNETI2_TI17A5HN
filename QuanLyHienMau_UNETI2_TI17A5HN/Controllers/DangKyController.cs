// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Phụ trách: Nhóm thực hiện Module 3 - Lớp DHTI17A5HN

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
    public class DangKyController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Quy định khoảng cách tối thiểu giữa 2 lần hiến máu toàn phần thành công: 84 ngày (12 tuần / ~3 tháng theo chuẩn Bộ Y Tế)
        public const int SoNgayChoToiThieuGiuaHaiLanHien = 84;

        // Khóa đồng bộ theo từng tài khoản/người hiến để ngăn chặn nhiều request nộp phiếu đồng thời (Race Condition)
        private static readonly ConcurrentDictionary<int, SemaphoreSlim> _submissionLocks = new();

        public sealed record RecentDonationEligibilityResult(
            bool IsEligible,
            int TotalSuccessfulDonations,
            PhieuDangKyHienMau? LastSuccessfulPhieu,
            DateTime? LastActualDonationDate,
            DateTime? NextEligibleDate,
            int? DaysSinceLastDonation,
            int RemainingCooldownDays,
            bool HasMissingActualDonationDate,
            string? ErrorMessage);

        /// <summary>
        /// Kiểm tra một phiếu đăng ký có phải là lần hiến máu thành công thực tế hay không.
        /// Phiếu bị hủy, bị từ chối, không đủ điều kiện hiến hoặc đang chờ xử lý/khám sàng lọc KHÔNG được tính là đã hiến máu thành công.
        /// </summary>
        public static bool IsConfirmedSuccessfulDonation(PhieuDangKyHienMau? phieu)
        {
            if (phieu == null) return false;

            var trangThai = phieu.TrangThai?.Trim();
            if (string.Equals(trangThai, "Đã hủy", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trangThai, "Từ chối", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trangThai, "Không đủ điều kiện hiến", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (phieu.KetQuaHienMau != null)
            {
                return string.Equals(phieu.KetQuaHienMau.KetQua?.Trim(), "Hiến máu thành công", StringComparison.OrdinalIgnoreCase);
            }

            return string.Equals(trangThai, "Hiến máu thành công", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Lấy ngày hoàn tất hiến máu thực tế từ bản ghi kết quả hiến máu (KetQuaHienMau.NgayCapNhat).
        /// Tuyệt đối KHÔNG lấy NgayNop (ngày tạo phiếu đăng ký) hoặc NgayXuLy (ngày duyệt/xử lý phiếu) thay cho ngày hiến máu thực tế.
        /// </summary>
        public static DateTime? GetActualDonationDate(PhieuDangKyHienMau? phieu)
        {
            if (!IsConfirmedSuccessfulDonation(phieu)) return null;

            if (phieu!.KetQuaHienMau != null &&
                string.Equals(phieu.KetQuaHienMau.KetQua?.Trim(), "Hiến máu thành công", StringComparison.OrdinalIgnoreCase) &&
                phieu.KetQuaHienMau.NgayCapNhat.HasValue)
            {
                return phieu.KetQuaHienMau.NgayCapNhat.Value;
            }

            return null;
        }

        /// <summary>
        /// Đánh giá điều kiện thời gian chờ kể từ lần hiến máu thành công gần nhất của người hiến máu.
        /// </summary>
        public static RecentDonationEligibilityResult EvaluateRecentDonationEligibility(
            IEnumerable<PhieuDangKyHienMau>? lichSuPhieu,
            DateTime referenceDate)
        {
            var refDay = referenceDate.Date;
            var successfulDonations = (lichSuPhieu ?? Enumerable.Empty<PhieuDangKyHienMau>())
                .Where(IsConfirmedSuccessfulDonation)
                .ToList();

            // Trường hợp 1: Chưa từng hiến máu thành công (kể cả khi có phiếu bị hủy, bị từ chối hoặc đang chờ xử lý)
            if (successfulDonations.Count == 0)
            {
                return new RecentDonationEligibilityResult(
                    IsEligible: true,
                    TotalSuccessfulDonations: 0,
                    LastSuccessfulPhieu: null,
                    LastActualDonationDate: null,
                    NextEligibleDate: null,
                    DaysSinceLastDonation: null,
                    RemainingCooldownDays: 0,
                    HasMissingActualDonationDate: false,
                    ErrorMessage: null);
            }

            var datedDonations = successfulDonations
                .Select(p => new { Phieu = p, ActualDate = GetActualDonationDate(p) })
                .Where(x => x.ActualDate.HasValue)
                .OrderByDescending(x => x.ActualDate!.Value)
                .ThenByDescending(x => x.Phieu.MaPhieu)
                .ToList();

            var undatedDonations = successfulDonations
                .Where(p => !GetActualDonationDate(p).HasValue)
                .OrderByDescending(p => p.MaPhieu)
                .ToList();

            var latestDated = datedDonations.FirstOrDefault();

            // Trường hợp 2: Có lần hiến máu thành công kèm ngày hiến thực tế, kiểm tra khoảng cách 84 ngày từ lần gần nhất
            if (latestDated != null)
            {
                var lastActualDate = latestDated.ActualDate!.Value.Date;
                var nextEligibleDate = lastActualDate.AddDays(SoNgayChoToiThieuGiuaHaiLanHien);
                var daysElapsed = Math.Max(0, (int)(refDay - lastActualDate).TotalDays);
                var remainingDays = Math.Max(0, (int)(nextEligibleDate - refDay).TotalDays);

                if (refDay < nextEligibleDate)
                {
                    return new RecentDonationEligibilityResult(
                        IsEligible: false,
                        TotalSuccessfulDonations: successfulDonations.Count,
                        LastSuccessfulPhieu: latestDated.Phieu,
                        LastActualDonationDate: latestDated.ActualDate,
                        NextEligibleDate: nextEligibleDate,
                        DaysSinceLastDonation: daysElapsed,
                        RemainingCooldownDays: remainingDays,
                        HasMissingActualDonationDate: undatedDonations.Count > 0,
                        ErrorMessage: $"Bạn chưa đủ thời gian chờ tối thiểu ({SoNgayChoToiThieuGiuaHaiLanHien} ngày) kể từ lần hiến máu thành công gần nhất vào ngày {lastActualDate:dd/MM/yyyy} (Mã phiếu #DKHM-{latestDated.Phieu.MaPhieu:D5}). Ngày đủ điều kiện đăng ký hiến máu lại là từ ngày {nextEligibleDate:dd/MM/yyyy} (còn {remainingDays} ngày).");
                }
            }

            // Trường hợp 3: Có bản ghi xác nhận "Hiến máu thành công" nhưng thiếu ngày hoàn tất hiến máu thực tế trong dữ liệu
            // Không tự ý lấy NgayNop hoặc NgayXuLy làm ngày hiến; chặn đăng ký an toàn và yêu cầu cập nhật ngày hiến thực tế.
            if (undatedDonations.Count > 0)
            {
                var missingPhieu = undatedDonations.First();
                return new RecentDonationEligibilityResult(
                    IsEligible: false,
                    TotalSuccessfulDonations: successfulDonations.Count,
                    LastSuccessfulPhieu: missingPhieu,
                    LastActualDonationDate: null,
                    NextEligibleDate: null,
                    DaysSinceLastDonation: null,
                    RemainingCooldownDays: SoNgayChoToiThieuGiuaHaiLanHien,
                    HasMissingActualDonationDate: true,
                    ErrorMessage: $"Hồ sơ ghi nhận bạn đã có lần hiến máu thành công (Mã phiếu #DKHM-{missingPhieu.MaPhieu:D5}) nhưng chưa có ngày hoàn tất hiến máu thực tế trong cơ sở dữ liệu (KetQuaHienMau.NgayCapNhat bị trống). Hệ thống không lấy ngày tạo phiếu ({missingPhieu.NgayNop:dd/MM/yyyy}) làm ngày hiến thực tế; vui lòng liên hệ nhân viên y tế để cập nhật ngày hiến máu trước khi đăng ký lại.");
            }

            // Trường hợp 4: Đã đủ thời gian chờ >= 84 ngày kể từ lần hiến máu thành công gần nhất
            var validLatest = latestDated!;
            var validLastDate = validLatest.ActualDate!.Value.Date;
            var validNextDate = validLastDate.AddDays(SoNgayChoToiThieuGiuaHaiLanHien);
            var validDaysElapsed = Math.Max(0, (int)(refDay - validLastDate).TotalDays);

            return new RecentDonationEligibilityResult(
                IsEligible: true,
                TotalSuccessfulDonations: successfulDonations.Count,
                LastSuccessfulPhieu: validLatest.Phieu,
                LastActualDonationDate: validLatest.ActualDate,
                NextEligibleDate: validNextDate,
                DaysSinceLastDonation: validDaysElapsed,
                RemainingCooldownDays: 0,
                HasMissingActualDonationDate: false,
                ErrorMessage: null);
        }

        public DangKyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /DangKy
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(TheoDoi));
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

            var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
            if (taiKhoan == null || taiKhoan.TrangThai != "Hoạt động")
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn hiện không tồn tại hoặc đã bị khóa.";
                return RedirectToAction("Login", "Account");
            }

            var nguoiHienMau = await _context.NguoiHienMaus
                .Include(n => n.TaiKhoan)
                .FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan);

            if (nguoiHienMau == null)
            {
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
                var today = DateTime.Today;
                dotHienMau = await _context.DotHienMaus
                    .Include(d => d.DonViToChuc)
                    .Where(d => d.TrangThai == "Đang mở đăng ký" && d.NgayBatDauDangKy <= today && d.HanDangKy >= today)
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
            if (now.Date < dotHienMau.NgayBatDauDangKy.Date)
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' chưa đến thời gian mở nhận đăng ký (Mở từ {dotHienMau.NgayBatDauDangKy:dd/MM/yyyy}).";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }
            if (now.Date > dotHienMau.HanDangKy.Date)
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' đã kết thúc thời hạn nhận đăng ký vào ngày {dotHienMau.HanDangKy:dd/MM/yyyy}.";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }

            // 4. Kiểm tra điều kiện thời gian chờ kể từ lần hiến máu thành công gần nhất của chính người dùng đang đăng nhập
            var lichSuPhieu = await _context.PhieuDangKyHienMaus
                .Include(p => p.KetQuaHienMau)
                .Include(p => p.DotHienMau)
                .Where(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien)
                .ToListAsync();

            var recentEligibility = EvaluateRecentDonationEligibility(lichSuPhieu, DateTime.Today);
            ViewBag.RecentDonationEligibility = recentEligibility;
            if (!recentEligibility.IsEligible)
            {
                ViewBag.IsCooldownBlocked = true;
                ViewBag.ErrorMessage = recentEligibility.ErrorMessage;
                TempData["ErrorMessage"] = recentEligibility.ErrorMessage;
            }

            // 5. Kiểm tra xem người hiến đã có phiếu đang xử lý cho đợt này chưa
            var existingPhieu = lichSuPhieu
                .FirstOrDefault(p => p.MaDot == dotHienMau.MaDot
                                  && p.TrangThai != "Đã hủy"
                                  && p.TrangThai != "Từ chối"
                                  && p.TrangThai != "Không đủ điều kiện hiến"
                                  && !IsConfirmedSuccessfulDonation(p));

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
            DateTime? ngaySinh,
            string? gioiTinh,
            string? soDienThoai,
            string? email,
            string? ngheNghiep,
            string? lyDoHienMau,
            string? ghiChu,
            string? health_q1 = null,
            string? health_q2 = null)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int maTaiKhoan))
            {
                return Challenge();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
            if (taiKhoan == null || taiKhoan.TrangThai != "Hoạt động")
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn hiện không tồn tại hoặc đã bị khóa.";
                return RedirectToAction("Login", "Account");
            }

            var nguoiHienMau = await _context.NguoiHienMaus
                .Include(n => n.TaiKhoan)
                .FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan);

            if (nguoiHienMau == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có hồ sơ người hiến máu. Vui lòng cập nhật thông tin hồ sơ cá nhân trước khi nộp phiếu đăng ký.";
                return RedirectToAction("HoSoCaNhan", "NguoiHienMau");
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
            if (now.Date < dotHienMau.NgayBatDauDangKy.Date)
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' chưa đến ngày mở nhận đăng ký.";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }
            if (now.Date > dotHienMau.HanDangKy.Date)
            {
                TempData["ErrorMessage"] = $"Đợt hiến máu '{dotHienMau.TenDot}' đã kết thúc hạn nhận đăng ký vào ngày {dotHienMau.HanDangKy:dd/MM/yyyy}.";
                return RedirectToAction("Details", "DotHienMau", new { id = dotHienMau.MaDot });
            }

            // Hàm cục bộ trả lại View khi dữ liệu không hợp lệ (giữ lại thông tin người dùng đã nhập trên form mà không lưu DB)
            IActionResult ReturnInvalidForm(string errorMsg)
            {
                if (!string.IsNullOrWhiteSpace(hoTen)) nguoiHienMau.HoTen = hoTen.Trim();
                if (ngaySinh.HasValue) nguoiHienMau.NgaySinh = ngaySinh;
                if (!string.IsNullOrWhiteSpace(gioiTinh)) nguoiHienMau.GioiTinh = gioiTinh.Trim();
                if (!string.IsNullOrWhiteSpace(soDienThoai)) nguoiHienMau.SoDienThoai = soDienThoai.Trim();
                if (!string.IsNullOrWhiteSpace(email)) nguoiHienMau.Email = email.Trim();
                if (!string.IsNullOrWhiteSpace(ngheNghiep)) nguoiHienMau.NgheNghiep = ngheNghiep.Trim();
                if (canNang.HasValue) nguoiHienMau.CanNang = canNang;

                ViewBag.ErrorMessage = errorMsg;
                TempData["ErrorMessage"] = errorMsg;
                ViewBag.DotHienMau = dotHienMau;
                ViewBag.NguoiHienMau = nguoiHienMau;
                ViewData["Title"] = $"Nộp Phiếu Đăng Ký Hiến Máu – {dotHienMau.TenDot}";
                return View(dotHienMau);
            }

            // 4. Kiểm tra điều kiện thời gian chờ kể từ lần hiến máu thành công gần nhất của chính người dùng đang đăng nhập (chặn POST trực tiếp)
            var lichSuPhieuPreCheck = await _context.PhieuDangKyHienMaus
                .Include(p => p.KetQuaHienMau)
                .Include(p => p.DotHienMau)
                .Where(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien)
                .ToListAsync();

            var preCheckEligibility = EvaluateRecentDonationEligibility(lichSuPhieuPreCheck, DateTime.Today);
            ViewBag.RecentDonationEligibility = preCheckEligibility;
            if (!preCheckEligibility.IsEligible)
            {
                ViewBag.IsCooldownBlocked = true;
                return ReturnInvalidForm(preCheckEligibility.ErrorMessage!);
            }

            // 5. Server-side validation thông tin bắt buộc và định dạng của hồ sơ người đăng ký
            var finalHoTen = !string.IsNullOrWhiteSpace(hoTen) ? hoTen.Trim() : nguoiHienMau.HoTen?.Trim();
            if (string.IsNullOrWhiteSpace(finalHoTen))
            {
                return ReturnInvalidForm("Vui lòng cập nhật đầy đủ họ và tên trước khi nộp phiếu.");
            }
            if (finalHoTen.Length > 100)
            {
                return ReturnInvalidForm("Họ và tên không được vượt quá 100 ký tự.");
            }

            var finalNgaySinh = ngaySinh ?? nguoiHienMau.NgaySinh;
            if (!finalNgaySinh.HasValue)
            {
                return ReturnInvalidForm("Vui lòng cung cấp ngày sinh để kiểm tra điều kiện độ tuổi hiến máu (18 - 60 tuổi).");
            }
            else
            {
                var birthDate = finalNgaySinh.Value.Date;
                var today = DateTime.Today;
                if (birthDate > today)
                {
                    return ReturnInvalidForm("Ngày sinh không hợp lệ (không được lớn hơn ngày hiện tại).");
                }

                int age = today.Year - birthDate.Year;
                if (birthDate > today.AddYears(-age)) age--;
                if (age < 18 || age > 60)
                {
                    return ReturnInvalidForm($"Độ tuổi người hiến máu theo quy định phải từ 18 đến 60 tuổi (Hiện tại: {age} tuổi).");
                }
            }

            var finalGioiTinh = !string.IsNullOrWhiteSpace(gioiTinh) ? gioiTinh.Trim() : nguoiHienMau.GioiTinh?.Trim();
            if (!string.IsNullOrEmpty(finalGioiTinh) && finalGioiTinh.Length > 10)
            {
                return ReturnInvalidForm("Giới tính không được vượt quá 10 ký tự.");
            }

            var finalPhone = !string.IsNullOrWhiteSpace(soDienThoai) ? soDienThoai.Trim() : nguoiHienMau.SoDienThoai?.Trim();
            if (string.IsNullOrWhiteSpace(finalPhone))
            {
                return ReturnInvalidForm("Vui lòng cung cấp số điện thoại liên lạc để nhận xác nhận phiếu.");
            }
            if (!Regex.IsMatch(finalPhone, @"^0[0-9]{9,10}$"))
            {
                return ReturnInvalidForm("Số điện thoại không hợp lệ (phải gồm 10-11 chữ số và bắt đầu bằng số 0).");
            }

            var finalEmail = !string.IsNullOrWhiteSpace(email) ? email.Trim() : nguoiHienMau.Email?.Trim();
            if (!string.IsNullOrWhiteSpace(finalEmail))
            {
                if (finalEmail.Length > 100 || !new EmailAddressAttribute().IsValid(finalEmail))
                {
                    return ReturnInvalidForm("Địa chỉ email không đúng định dạng hoặc vượt quá 100 ký tự.");
                }

                bool emailExistsInOtherAccount = await _context.TaiKhoans
                    .AnyAsync(t => t.MaTaiKhoan != maTaiKhoan && t.Email != null && t.Email.ToLower() == finalEmail.ToLower());
                if (emailExistsInOtherAccount)
                {
                    return ReturnInvalidForm("Địa chỉ email này đã được sử dụng bởi một tài khoản khác trong hệ thống.");
                }
            }

            var finalNgheNghiep = !string.IsNullOrWhiteSpace(ngheNghiep) ? ngheNghiep.Trim() : nguoiHienMau.NgheNghiep?.Trim();
            if (!string.IsNullOrEmpty(finalNgheNghiep) && finalNgheNghiep.Length > 100)
            {
                return ReturnInvalidForm("Thông tin nghề nghiệp / đơn vị không được vượt quá 100 ký tự.");
            }

            // 5. Kiểm tra cân nặng hợp lệ và đạt mức tối thiểu của đợt hiến máu
            var finalWeight = canNang ?? nguoiHienMau.CanNang;
            if (!finalWeight.HasValue || finalWeight.Value <= 0)
            {
                return ReturnInvalidForm("Vui lòng nhập cân nặng hiện tại của bạn.");
            }
            if (finalWeight.Value < 30.0 || finalWeight.Value > 200.0)
            {
                return ReturnInvalidForm("Cân nặng không hợp lệ (phải nằm trong khoảng từ 30kg đến 200kg).");
            }
            if (finalWeight.Value < dotHienMau.CanNangToiThieu)
            {
                return ReturnInvalidForm($"Cân nặng của bạn ({finalWeight.Value}kg) chưa đạt mức tối thiểu quy định cho đợt này (yêu cầu ≥ {dotHienMau.CanNangToiThieu}kg).");
            }

            if (string.Equals(health_q1, "yes", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(health_q2, "yes", StringComparison.OrdinalIgnoreCase))
            {
                return ReturnInvalidForm("Kết quả tự sàng lọc y tế cho thấy bạn cần tạm hoãn hiến máu để đảm bảo an toàn sức khỏe.");
            }

            var formattedLyDo = !string.IsNullOrEmpty(bloodVolume) ? $"Dung tích dự kiến: {bloodVolume}. {lyDoHienMau}".Trim() : lyDoHienMau?.Trim();
            if (!string.IsNullOrEmpty(formattedLyDo) && formattedLyDo.Length > 500)
            {
                return ReturnInvalidForm("Lý do hiến máu không được vượt quá 500 ký tự.");
            }

            var formattedGhiChu = !string.IsNullOrEmpty(timeSlot) ? $"Khung giờ hẹn: {timeSlot}. {ghiChu}".Trim() : ghiChu?.Trim();
            if (!string.IsNullOrEmpty(formattedGhiChu) && formattedGhiChu.Length > 500)
            {
                return ReturnInvalidForm("Ghi chú không được vượt quá 500 ký tự.");
            }

            // 6. Kiểm tra điều kiện thời gian chờ & chống trùng lặp phiếu đang xử lý (Bảo vệ đồng thời bằng SemaphoreSlim + Serializable Transaction)
            var userLock = _submissionLocks.GetOrAdd(nguoiHienMau.MaNguoiHien, _ => new SemaphoreSlim(1, 1));
            await userLock.WaitAsync();
            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                var lichSuPhieuInTx = await _context.PhieuDangKyHienMaus
                    .Include(p => p.KetQuaHienMau)
                    .Include(p => p.DotHienMau)
                    .Where(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien)
                    .ToListAsync();

                var txEligibility = EvaluateRecentDonationEligibility(lichSuPhieuInTx, DateTime.Today);
                if (!txEligibility.IsEligible)
                {
                    await transaction.RollbackAsync();
                    ViewBag.RecentDonationEligibility = txEligibility;
                    ViewBag.IsCooldownBlocked = true;
                    return ReturnInvalidForm(txEligibility.ErrorMessage!);
                }

                var existingPhieu = lichSuPhieuInTx
                    .FirstOrDefault(p => p.MaDot == maDot
                                      && p.TrangThai != "Đã hủy"
                                      && p.TrangThai != "Từ chối"
                                      && p.TrangThai != "Không đủ điều kiện hiến"
                                      && !IsConfirmedSuccessfulDonation(p));

                if (existingPhieu != null)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Bạn đã có phiếu đăng ký đang trong tiến trình xử lý cho đợt hiến máu này.";
                    return RedirectToAction(nameof(TheoDoi), new { id = existingPhieu.MaPhieu });
                }

                // 7. Cập nhật thông tin hồ sơ người hiến đã qua kiểm tra hợp lệ
                nguoiHienMau.HoTen = finalHoTen;
                if (finalNgaySinh.HasValue) nguoiHienMau.NgaySinh = finalNgaySinh;
                if (!string.IsNullOrWhiteSpace(finalGioiTinh)) nguoiHienMau.GioiTinh = finalGioiTinh;
                nguoiHienMau.SoDienThoai = finalPhone;
                if (!string.IsNullOrWhiteSpace(finalEmail)) nguoiHienMau.Email = finalEmail;
                if (!string.IsNullOrWhiteSpace(finalNgheNghiep)) nguoiHienMau.NgheNghiep = finalNgheNghiep;
                if (!string.IsNullOrWhiteSpace(bloodGroup) && bloodGroup != "UNKNOWN")
                {
                    var cleanBloodGroup = bloodGroup.Trim();
                    if (cleanBloodGroup.Length <= 20)
                    {
                        nguoiHienMau.NhomMau = cleanBloodGroup;
                    }
                }
                nguoiHienMau.CanNang = finalWeight.Value;

                if (nguoiHienMau.TaiKhoan != null)
                {
                    nguoiHienMau.TaiKhoan.HoTen = nguoiHienMau.HoTen;
                    if (!string.IsNullOrWhiteSpace(nguoiHienMau.Email))
                    {
                        nguoiHienMau.TaiKhoan.Email = nguoiHienMau.Email;
                    }
                }

                // 8. Tạo phiếu đăng ký (Server xác định NgayNop, TrangThai khởi tạo là "Chờ xác nhận")
                var phieu = new PhieuDangKyHienMau
                {
                    MaNguoiHien = nguoiHienMau.MaNguoiHien,
                    MaDot = maDot,
                    NgayNop = DateTime.Now,
                    TrangThai = "Chờ xác nhận",
                    LyDoHienMau = formattedLyDo,
                    GhiChu = formattedGhiChu,
                    NgayXuLy = null,
                    GhiChuXacNhan = null
                };

                _context.PhieuDangKyHienMaus.Add(phieu);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"Đăng ký hiến máu thành công! Mã phiếu của bạn là #DKHM-{phieu.MaPhieu:D5}. Vui lòng chờ ban tổ chức xác nhận.";
                return RedirectToAction(nameof(TheoDoi), new { id = phieu.MaPhieu });
            }
            finally
            {
                userLock.Release();
            }
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
            bool isStaffOrAdmin = userRole == "Admin" || userRole == "Medical Staff" || userRole == "NhanVienYTe" || userRole == "Nhân viên y tế";

            if (!isStaffOrAdmin)
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
            bool explicitLookupFailed = false;

            // Tra cứu theo ID
            if (id.HasValue)
            {
                selectedPhieu = userPhieuList.FirstOrDefault(p => p.MaPhieu == id.Value);
                if (selectedPhieu == null)
                {
                    if (isStaffOrAdmin)
                    {
                        selectedPhieu = await query.FirstOrDefaultAsync(p => p.MaPhieu == id.Value);
                    }

                    if (selectedPhieu == null)
                    {
                        explicitLookupFailed = true;
                        TempData["ErrorMessage"] = "Bạn không có quyền truy cập phiếu đăng ký này hoặc phiếu không tồn tại.";
                    }
                }
            }
            // Tra cứu theo mã phiếu (VD: DKHM-00001 hoặc 1)
            else if (!string.IsNullOrWhiteSpace(code))
            {
                var cleanCode = code.Replace("#", "").Replace("DKHM-", "").Replace("VTL-", "").Trim();
                if (int.TryParse(cleanCode, out int parsedId))
                {
                    selectedPhieu = userPhieuList.FirstOrDefault(p => p.MaPhieu == parsedId);
                    if (selectedPhieu == null && isStaffOrAdmin)
                    {
                        selectedPhieu = await query.FirstOrDefaultAsync(p => p.MaPhieu == parsedId);
                    }
                }

                if (selectedPhieu == null)
                {
                    explicitLookupFailed = true;
                    TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký phù hợp hoặc bạn không có quyền truy cập.";
                }
            }

            // Chỉ mặc định hiển thị phiếu mới nhất khi người dùng không truyền id/code cụ thể bị từ chối
            if (selectedPhieu == null && !explicitLookupFailed)
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

            var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
            if (taiKhoan == null || taiKhoan.TrangThai != "Hoạt động")
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn hiện không tồn tại hoặc đã bị khóa.";
                return RedirectToAction("Login", "Account");
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

            // Chỉ chủ sở hữu phiếu mới được phép hủy phiếu đăng ký trên cổng người hiến máu (Chặn IDOR)
            if (nguoiHienMau == null || phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien)
            {
                return Forbid();
            }

            // Chỉ cho phép hủy ở các trạng thái được phép ("Chờ xác nhận", "Đã xác nhận")
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

            // Không cho phép hủy nếu đã có kết quả hiến máu hoặc đã tồn tại lịch khám sàng lọc đang hoạt động
            if (phieu.KetQuaHienMau != null)
            {
                TempData["ErrorMessage"] = "Phiếu đã có kết quả hiến máu cuối cùng, không thể hủy.";
                return RedirectToAction(nameof(TheoDoi), new { id = phieu.MaPhieu });
            }

            if (phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy"))
            {
                TempData["ErrorMessage"] = "Phiếu đăng ký đã được xếp lịch khám sàng lọc, không thể tự hủy trực tuyến. Vui lòng liên hệ ban tổ chức hoặc nhân viên y tế để được hỗ trợ.";
                return RedirectToAction(nameof(TheoDoi), new { id = phieu.MaPhieu });
            }

            // Cập nhật trạng thái phiếu sang "Đã hủy"
            phieu.TrangThai = "Đã hủy";
            phieu.NgayXuLy = DateTime.Now;
            phieu.GhiChuXacNhan = "Người hiến đã chủ động hủy phiếu đăng ký trên cổng thông tin.";

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã hủy thành công phiếu đăng ký #DKHM-{phieu.MaPhieu:D5}.";

            return RedirectToAction(nameof(TheoDoi), new { id = phieu.MaPhieu });
        }
    }
}
