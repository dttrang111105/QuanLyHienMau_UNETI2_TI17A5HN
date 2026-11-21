// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Quản lí đơn vị tổ chức hiến máu
// Thêm sửa xóa đơn vị, quản lí đơn vị tổ chức

using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Models.ViewModels;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    [Authorize(Roles = "Admin,Medical Staff")]
    public class AdminController : Controller
    {
        // Ho va ten: NGUYEN THI PHUONG THAO
        // Ma sinh vien: 23103100272
        // Noi dung: Quan ly dot hien mau (Admin panel)
        private readonly IDotHienMauRepository _dotRepo;
        private readonly QuanLyHienMau_UNETI2_TI17A5HN.Repositories.IPhieuDangKyRepository _phieuRepository;
        private readonly IDonViToChucRepository _donViToChucRepository;
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly INguoiHienMauRepository _nguoiHienMauRepo;
        private readonly KhamSangLocService _khamSangLocService;
        private readonly PasswordHasher<TaiKhoan> _passwordHasher;

        private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_.-]+$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly HashSet<string> AllowedRoles = new(StringComparer.Ordinal) { "Admin", "Medical Staff", "Donor" };
        private static readonly HashSet<string> AllowedAccountStatuses = new(StringComparer.Ordinal) { "Hoạt động", "Bị khóa" };
        private static readonly HashSet<string> AllowedDonViStatuses = new(StringComparer.Ordinal) { "Hoạt động", "Tạm dừng" };

        public AdminController(
            IDotHienMauRepository dotRepo,
            QuanLyHienMau_UNETI2_TI17A5HN.Repositories.IPhieuDangKyRepository phieuRepository,
            IDonViToChucRepository donViToChucRepository,
            ITaiKhoanRepository taiKhoanRepository,
            INguoiHienMauRepository nguoiHienMauRepo,
            KhamSangLocService khamSangLocService)
        {
            _dotRepo = dotRepo;
            _phieuRepository = phieuRepository;
            _donViToChucRepository = donViToChucRepository;
            _taiKhoanRepository = taiKhoanRepository;
            _nguoiHienMauRepo = nguoiHienMauRepo;
            _khamSangLocService = khamSangLocService;
            _passwordHasher = new PasswordHasher<TaiKhoan>();
        }

        private static bool TryParseDateTime(string? input, out DateTime result)
        {
            result = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(input)) return false;
            string[] formats = new[] { "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "yyyy-MM-dd", "M/d/yyyy h:mm:ss tt", "d/M/yyyy h:mm:ss tt" };
            if (DateTime.TryParseExact(input.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) return true;
            if (DateTime.TryParse(input.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) return true;
            if (DateTime.TryParse(input.Trim(), new CultureInfo("vi-VN"), DateTimeStyles.None, out result)) return true;
            if (DateTime.TryParse(input.Trim(), out result)) return true;
            return false;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Dashboard()
        {
            // 8+ KPI Macro từ database SQL Server qua Repository
            var tongNguoiHien = await _nguoiHienMauRepo.GetQueryableWithDetails().CountAsync();
            var tongPhieu = await _phieuRepository.CountPhieuAsync();
            var phieuChoXacNhan = await _phieuRepository.CountPhieuAsync("Chờ xác nhận");
            var phieuDaXacNhan = await _phieuRepository.CountPhieuAsync("Đã xác nhận");
            var phieuDaKham = await _phieuRepository.CountPhieuAsync("Đã khám sàng lọc");
            var hienMauThanhCong = await _phieuRepository.CountPhieuAsync("Hiến máu thành công");
            var khongDuDieuKien = await _phieuRepository.CountPhieuAsync("Không đủ điều kiện hiến");
            var dotDangMo = await _dotRepo.CountAsync(_dotRepo.ApplyFilterTrangThai(_dotRepo.GetQueryable(), "Đang mở đăng ký"));
            var donViStats = await _donViToChucRepository.GetDonViStatsAsync();
            var tongDonVi = donViStats.TotalCount;
            var tongLichKham = await _phieuRepository.CountLichKhamAsync();

            ViewBag.TongNguoiHien = tongNguoiHien;
            ViewBag.TongPhieu = tongPhieu;
            ViewBag.PhieuChoXacNhan = phieuChoXacNhan;
            ViewBag.PhieuDaXacNhan = phieuDaXacNhan;
            ViewBag.PhieuDaKham = phieuDaKham;
            ViewBag.HienMauThanhCong = hienMauThanhCong;
            ViewBag.KhongDuDieuKien = khongDuDieuKien;
            ViewBag.DotDangMo = dotDangMo;
            ViewBag.TongDonVi = tongDonVi;
            ViewBag.TongLichKham = tongLichKham;

            // Thống kê theo nhóm máu (LINQ thực tế từ NguoiHienMauRepository)
            var bloodGroupStats = await _nguoiHienMauRepo.GetQueryableWithDetails()
                .GroupBy(n => n.NhomMau ?? "Chưa xác định")
                .Select(g => new { NhomMau = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            ViewBag.BloodGroupStats = bloodGroupStats;

            // Thống kê theo từng Đợt hiến máu
            var dotList = await _phieuRepository.GetRecentDotWithStatsAsync(8);
            ViewBag.DotList = dotList;

            // Danh sách các phiếu mới nhất cần xử lý
            var recentTickets = await _phieuRepository.GetRecentPhieusAsync(6);
            ViewBag.RecentTickets = recentTickets;

            return View();
        }

        // ==========================================
        // MODULE 4: QUẢN LÝ PHIẾU ĐĂNG KÝ HIẾN MÁU
        // ==========================================

        // GET: /Admin/PhieuDangKyHienMau
        public async Task<IActionResult> PhieuDangKyHienMau(
            string? tenNguoiHien,
            string? tenDot,
            int? maDonViToChuc,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sapXep = "ngayNop",
            string? chieuSapXep = "desc",
            int page = 1)
        {
            var result = await _phieuRepository.GetPageAsync(new PhieuDangKyFilter(
                tenNguoiHien, tenDot, maDonViToChuc, trangThai, tuNgay, denNgay,
                sapXep, chieuSapXep, page, 10));

            ViewBag.TotalCount = result.Counts.Tong;
            ViewBag.ChoXacNhanCount = result.Counts.ChoXacNhan;
            ViewBag.DaXacNhanCount = result.Counts.DaXacNhan;
            ViewBag.ChoKhamCount = result.Counts.ChoKham;
            ViewBag.DaKhamCount = result.Counts.DaKham;
            ViewBag.TuChoiCount = result.Counts.TuChoi;
            ViewBag.DaHuyCount = result.Counts.DaHuy;
            ViewBag.CurrentTenNguoiHien = tenNguoiHien;
            ViewBag.CurrentTenDot = tenDot;
            ViewBag.CurrentMaDonViToChuc = maDonViToChuc;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.CurrentTuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.CurrentDenNgay = denNgay?.ToString("yyyy-MM-dd");
            ViewBag.CurrentSapXep = result.SapXep;
            ViewBag.CurrentChieuSapXep = result.ChieuSapXep;
            ViewBag.CurrentPage = result.Page;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.DonViToChucs = result.DonViToChucs;
            ViewBag.DotDatChiTieu = await _phieuRepository.GetDotDatChiTieuAsync();
            return View(result.Items);
        }

        // GET: /Admin/ChiTietPhieuDangKy/5
        public async Task<IActionResult> ChiTietPhieuDangKy(int id)
        {
            var details = await _phieuRepository.GetDetailsAsync(id);
            if (details is null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin phiếu đăng ký.";
                return RedirectToAction(nameof(PhieuDangKyHienMau));
            }
            ViewBag.LichSuPhieuKhac = details.LichSu;
            ViewBag.SoPhieuThanhCongDot = details.SoPhieuThanhCongDot;
            return View(details.Phieu);
        }

        // POST: /Admin/XacNhanPhieu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanPhieu(int id, string? ghiChu, string? returnUrl)
        {
            await using var transaction = await _phieuRepository.BeginTransactionAsync();
            var phieu = await _phieuRepository.GetForUpdateAsync(id);
            if (phieu is null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký.";
                await transaction.RollbackAsync();
                return RedirectToAction(nameof(PhieuDangKyHienMau));
            }

            string? error = null;
            if (phieu.TrangThai == "Đã hủy")
                error = "Phiếu đã bị hủy, không thể xác nhận.";
            else if (phieu.TrangThai != "Chờ xác nhận")
                error = $"Chỉ phiếu Chờ xác nhận mới được duyệt. Trạng thái hiện tại: '{phieu.TrangThai}'.";
            else if (phieu.DotHienMau is null)
                error = "Không tìm thấy đợt hiến máu liên quan đến phiếu.";
            else if (phieu.NguoiHienMau is null || phieu.NguoiHienMau.TrangThai != "Hoạt động")
                error = "Người hiến máu không tồn tại hoặc hiện không hoạt động.";
            else if (phieu.KetQuaHienMau is not null)
                error = "Phiếu đã có kết quả xác nhận trước đó, không thể duyệt lại.";
            else if (phieu.NgayXuLy.HasValue || !string.IsNullOrWhiteSpace(phieu.GhiChuXacNhan))
                error = "Phiếu đã có thông tin xử lý trước đó, vui lòng kiểm tra lịch sử.";
            else if (string.IsNullOrWhiteSpace(ghiChu))
                error = "Vui lòng nhập ghi chú xác nhận trước khi duyệt phiếu.";
            else if (ghiChu.Trim().Length > 500)
                error = "Ghi chú xác nhận không được vượt quá 500 ký tự.";

            if (error is not null)
            {
                TempData["ErrorMessage"] = error;
                await transaction.RollbackAsync();
            }
            else
            {
                phieu.TrangThai = "Đã xác nhận";
                phieu.NgayXuLy = DateTime.Now;
                phieu.GhiChuXacNhan = ghiChu!.Trim();
                await _phieuRepository.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = $"Đã xác nhận phiếu #DKHM-{phieu.MaPhieu:D5}. Nhân viên y tế chịu trách nhiệm đánh giá toàn bộ điều kiện hiến máu.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction(nameof(ChiTietPhieuDangKy), new { id });
        }

        // POST: /Admin/TuChoiPhieu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoiPhieu(int id, string? lyDoTuChoi, string? returnUrl)
        {
            var phieu = await _phieuRepository.GetForUpdateAsync(id);
            if (phieu is null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký.";
                return RedirectToAction(nameof(PhieuDangKyHienMau));
            }

            if (phieu.TrangThai != "Chờ xác nhận")
                TempData["ErrorMessage"] = $"Chỉ phiếu Chờ xác nhận mới được từ chối. Trạng thái hiện tại: '{phieu.TrangThai}'.";
            else if (string.IsNullOrWhiteSpace(lyDoTuChoi))
                TempData["ErrorMessage"] = "Vui lòng nhập lý do từ chối phiếu đăng ký.";
            else if (lyDoTuChoi.Trim().Length > 500)
                TempData["ErrorMessage"] = "Lý do từ chối không được vượt quá 500 ký tự.";
            else
            {
                phieu.TrangThai = "Từ chối";
                phieu.NgayXuLy = DateTime.Now;
                phieu.GhiChuXacNhan = lyDoTuChoi.Trim();
                await _phieuRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã từ chối phiếu #DKHM-{phieu.MaPhieu:D5}. Lý do: {lyDoTuChoi.Trim()}.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction(nameof(ChiTietPhieuDangKy), new { id });
        }

        // POST: /Admin/HuyPhieu - chỉ hủy phiếu chưa có kết quả cuối cùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyPhieu(int id, string? ghiChu, string? returnUrl)
        {
            var phieu = await _phieuRepository.GetForUpdateAsync(id);
            if (phieu is null)
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký.";
            else if (phieu.TrangThai == "Đã hủy")
                TempData["ErrorMessage"] = "Phiếu đã được hủy trước đó.";
            else if (phieu.TrangThai == "Từ chối" || phieu.TrangThai == "Hiến máu thành công" || phieu.TrangThai == "Không đủ điều kiện hiến" || phieu.KetQuaHienMau is not null)
                TempData["ErrorMessage"] = "Không thể hủy phiếu đã bị từ chối hoặc đã có kết quả cuối cùng.";
            else if (ghiChu?.Trim().Length > 500)
                TempData["ErrorMessage"] = "Ghi chú hủy không được vượt quá 500 ký tự.";
            else
            {
                phieu.TrangThai = "Đã hủy";
                phieu.NgayXuLy = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(ghiChu))
                    phieu.GhiChuXacNhan = string.IsNullOrWhiteSpace(phieu.GhiChuXacNhan)
                        ? $"[Hủy phiếu] {ghiChu.Trim()}"
                        : $"{phieu.GhiChuXacNhan} [Hủy phiếu] {ghiChu.Trim()}";
                foreach (var lich in phieu.LichKhamSangLocs.Where(l => l.TrangThai == "Đã lên lịch"))
                {
                    lich.TrangThai = "Đã hủy";
                    var ghiChuLich = "Phiếu đăng ký đã hủy";
                    lich.GhiChu = string.IsNullOrWhiteSpace(lich.GhiChu) ? ghiChuLich : $"{lich.GhiChu} [{ghiChuLich}]";
                }
                await _phieuRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã hủy phiếu #DKHM-{phieu.MaPhieu:D5}; các lịch khám chưa hoàn thành đã được hủy.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction(nameof(ChiTietPhieuDangKy), new { id });
        }

        // POST: /Admin/ChuyenChoKham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChuyenChoKham(int id, string? returnUrl)
        {
            var phieu = await _phieuRepository.GetForUpdateAsync(id);
            if (phieu is null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký.";
                return RedirectToAction(nameof(PhieuDangKyHienMau));
            }

            if (phieu.TrangThai != "Đã xác nhận")
                TempData["ErrorMessage"] = $"Chỉ phiếu Đã xác nhận mới được chuyển sang Chờ khám sàng lọc. Trạng thái hiện tại: '{phieu.TrangThai}'.";
            else if (phieu.KetQuaHienMau is not null)
                TempData["ErrorMessage"] = "Phiếu đã có kết quả cuối cùng, không thể chuyển sang Chờ khám sàng lọc.";
            else if (!phieu.LichKhamSangLocs.Any(l => l.TrangThai == "Đã lên lịch"))
                TempData["ErrorMessage"] = "Không thể chuyển sang 'Chờ khám sàng lọc' khi phiếu chưa có lịch khám sàng lọc đang hiệu lực. Vui lòng tạo lịch khám trước.";
            else
            {
                phieu.TrangThai = "Chờ khám sàng lọc";
                phieu.NgayXuLy = DateTime.Now;
                await _phieuRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã chuyển phiếu #DKHM-{phieu.MaPhieu:D5} sang trạng thái 'Chờ khám sàng lọc'.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction(nameof(ChiTietPhieuDangKy), new { id });
        }
        // ==========================================
        // MODULE 4: QUẢN LÝ LỊCH KHÁM SÀNG LỌC
        // ==========================================

        // GET: /Admin/LichKhamSangLoc
        public async Task<IActionResult> LichKhamSangLoc(string? search, string? trangThai, DateTime? ngay, int? maDot, int page = 1)
        {
            int pageSize = 12;
            if (page < 1) page = 1;

            var query = _phieuRepository.GetLichKhamQueryableWithDetails();

            // Thống kê nhanh
            ViewBag.TotalLich = await _phieuRepository.CountLichKhamAsync();
            ViewBag.DaLenLichCount = await _phieuRepository.CountLichKhamAsync("Đã lên lịch");
            ViewBag.DaHoanThanhCount = await _phieuRepository.CountLichKhamAsync("Đã hoàn thành");
            ViewBag.DaHuyCount = await _phieuRepository.CountLichKhamAsync("Đã hủy");

            // Bộ lọc từ khóa
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(l =>
                    (l.PhieuDangKyHienMau != null && l.PhieuDangKyHienMau.NguoiHienMau != null && l.PhieuDangKyHienMau.NguoiHienMau.HoTen.ToLower().Contains(searchLower)) ||
                    (l.PhieuDangKyHienMau != null && l.PhieuDangKyHienMau.NguoiHienMau != null && l.PhieuDangKyHienMau.NguoiHienMau.SoDienThoai != null && l.PhieuDangKyHienMau.NguoiHienMau.SoDienThoai.Contains(searchLower)) ||
                    (l.BacSiSangLoc != null && l.BacSiSangLoc.ToLower().Contains(searchLower)) ||
                    (l.DiaDiemHoacLienKet != null && l.DiaDiemHoacLienKet.ToLower().Contains(searchLower)) ||
                    l.MaPhieu.ToString().Contains(searchLower) ||
                    l.MaLichKhamSangLoc.ToString().Contains(searchLower));
            }

            // Bộ lọc trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "all")
            {
                query = query.Where(l => l.TrangThai == trangThai);
            }

            // Bộ lọc ngày
            if (ngay.HasValue)
            {
                var date = ngay.Value.Date;
                var nextDate = date.AddDays(1);
                query = query.Where(l => l.ThoiGianBatDau >= date && l.ThoiGianBatDau < nextDate);
            }

            // Bộ lọc đợt
            if (maDot.HasValue && maDot.Value > 0)
            {
                query = query.Where(l => l.PhieuDangKyHienMau != null && l.PhieuDangKyHienMau.MaDot == maDot.Value);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(l => l.ThoiGianBatDau)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.CurrentNgay = ngay?.ToString("yyyy-MM-dd");
            ViewBag.CurrentMaDot = maDot;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.DotHienMaus = await _phieuRepository.GetAllDotHienMausOrderedAsync();

            // Danh sách phiếu đang ở trạng thái hợp lệ để tạo lịch khám mới nếu cần (chưa có lịch đang hiệu lực)
            ViewBag.PhieuSanSangTaoLich = await _phieuRepository.GetPhieuSanSangTaoLichAsync();

            return View(items);
        }

        // POST: /Admin/TaoLichKham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoLichKham(
            int maPhieu,
            string? thoiGianBatDau,
            string? thoiGianKetThuc,
            string? hinhThucTiepNhan,
            string? diaDiem,
            string? bacSi,
            string? ghiChu,
            string? returnUrl)
        {
            var phieu = await _phieuRepository.GetForUpdateAsync(maPhieu);

            if (phieu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký để tạo lịch khám.";
                return RedirectToAction(nameof(LichKhamSangLoc));
            }

            if (!TryParseDateTime(thoiGianBatDau, out DateTime dtBatDau) || !TryParseDateTime(thoiGianKetThuc, out DateTime dtKetThuc))
            {
                TempData["ErrorMessage"] = "Định dạng thời gian khám không hợp lệ.";
            }
            else
            {
                // Nếu phiếu đang ở Chờ khám sàng lọc mà chưa có lịch hiệu lực (dữ liệu cũ), khôi phục về Đã xác nhận trước khi lập lịch
                if (phieu.TrangThai == "Chờ khám sàng lọc" && !phieu.LichKhamSangLocs.Any(l => l.TrangThai == "Đã lên lịch"))
                {
                    await _khamSangLocService.KhoiPhucPhieuChoKhamAsync(maPhieu);
                }

                var model = new LichKhamFormViewModel
                {
                    MaPhieu = maPhieu,
                    ThoiGianBatDau = dtBatDau,
                    ThoiGianKetThuc = dtKetThuc,
                    HinhThucTiepNhan = !string.IsNullOrWhiteSpace(hinhThucTiepNhan) ? hinhThucTiepNhan.Trim() : "Trực tiếp tại điểm hiến",
                    DiaDiemHoacLienKet = !string.IsNullOrWhiteSpace(diaDiem) ? diaDiem.Trim() : (phieu.DotHienMau?.DonViToChuc?.TenDonViToChuc ?? "Điểm hiến máu UNETI"),
                    BacSiSangLoc = !string.IsNullOrWhiteSpace(bacSi) ? bacSi.Trim() : "BS. CKI Trần Mai Linh",
                    GhiChu = ghiChu
                };

                var result = await _khamSangLocService.LuuLichAsync(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Đã tạo lịch khám sàng lọc mới cho phiếu #DKHM-{phieu.MaPhieu:D5} thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(LichKhamSangLoc));
        }

        // POST: /Admin/SuaLichKham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaLichKham(
            int maLich,
            string? thoiGianBatDau,
            string? thoiGianKetThuc,
            string? hinhThucTiepNhan,
            string? diaDiem,
            string? bacSi,
            string? ghiChu,
            string? returnUrl)
        {
            var lich = await _phieuRepository.GetLichKhamForUpdateAsync(maLich);

            if (lich == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch khám sàng lọc.";
                return RedirectToAction(nameof(LichKhamSangLoc));
            }

            if (!TryParseDateTime(thoiGianBatDau, out DateTime dtBatDau) || !TryParseDateTime(thoiGianKetThuc, out DateTime dtKetThuc))
            {
                TempData["ErrorMessage"] = "Định dạng thời gian khám không hợp lệ.";
            }
            else
            {
                var model = new LichKhamFormViewModel
                {
                    MaLichKhamSangLoc = maLich,
                    MaPhieu = lich.MaPhieu,
                    ThoiGianBatDau = dtBatDau,
                    ThoiGianKetThuc = dtKetThuc,
                    HinhThucTiepNhan = !string.IsNullOrWhiteSpace(hinhThucTiepNhan) ? hinhThucTiepNhan.Trim() : (lich.HinhThucTiepNhan ?? "Trực tiếp tại điểm hiến"),
                    DiaDiemHoacLienKet = !string.IsNullOrWhiteSpace(diaDiem) ? diaDiem.Trim() : (lich.DiaDiemHoacLienKet ?? "Điểm hiến máu UNETI"),
                    BacSiSangLoc = !string.IsNullOrWhiteSpace(bacSi) ? bacSi.Trim() : (lich.BacSiSangLoc ?? "BS. CKI Trần Mai Linh"),
                    GhiChu = ghiChu
                };

                var result = await _khamSangLocService.LuuLichAsync(model, maLich);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Đã cập nhật thông tin lịch khám #{lich.MaLichKhamSangLoc} thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(LichKhamSangLoc));
        }

        // POST: /Admin/HuyLichKham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyLichKham(int maLich, string? ghiChu, string? returnUrl)
        {
            var result = await _khamSangLocService.HuyLichAsync(maLich, ghiChu);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(LichKhamSangLoc));
        }

        // POST: /Admin/HoanThanhKham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoanThanhKham(int maLich, string? returnUrl)
        {
            var result = await _khamSangLocService.HoanThanhLichAsync(maLich);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(LichKhamSangLoc));
        }

        public IActionResult HoSoBenhSu()
        {
            return View();
        }


        public IActionResult KhoLuuTruMau()
        {
            return View();
        }

        public IActionResult GiamSatTuiMau()
        {
            return View();
        }

        public IActionResult TieuHuy()
        {
            return View();
        }

        // GET: /Admin/NguoiHienMau
        // Module 3: Quản lý người hiến máu & phân loại nhóm máu hiếm (Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> NguoiHienMau(
            string? search, 
            string? nhomMau, 
            string? rhGroup, 
            string? status, 
            string? tier, 
            int page = 1, 
            int pageSize = 10)
        {
            // 1. Tính toán các KPI Macro tổng quan từ database qua INguoiHienMauRepository
            var allDonorsQuery = _nguoiHienMauRepo.GetQueryableWithDetails();
            var tongNguoiHien = await allDonorsQuery.CountAsync();
            var tongMauHiem = await allDonorsQuery.CountAsync(n => n.NhomMau != null && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));
            var oAm = await allDonorsQuery.CountAsync(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("O") && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));
            var aAm = await allDonorsQuery.CountAsync(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("A") && !n.NhomMau.ToUpper().Contains("AB") && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));
            var bAm = await allDonorsQuery.CountAsync(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("B") && !n.NhomMau.ToUpper().Contains("AB") && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));
            var abAm = await allDonorsQuery.CountAsync(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("AB") && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));

            // Tính toán KPI người đủ điều kiện và tích cực theo chuẩn quy tắc 84 ngày của Module 3
            // Chỉ tính các lần hiến máu có kết quả thực sự là "Hiến máu thành công" và lấy ngày từ KetQuaHienMau.NgayCapNhat
            var allDonorsListForKpi = await allDonorsQuery.AsNoTracking().ToListAsync();
            var today = DateTime.Today;
            var duDieuKienCount = allDonorsListForKpi.Count(d =>
                (d.TrangThai == "Hoạt động" || string.IsNullOrEmpty(d.TrangThai)) &&
                DangKyController.EvaluateRecentDonationEligibility(d.PhieuDangKyHienMaus, today).IsEligible);
            var hienTichCucCount = allDonorsListForKpi.Count(d =>
                DangKyController.EvaluateRecentDonationEligibility(d.PhieuDangKyHienMaus, today).TotalSuccessfulDonations >= 5);

            // 3. Áp dụng các bộ lọc
            var query = allDonorsQuery;

            // Lọc từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(n => n.HoTen.ToLower().Contains(searchLower) ||
                                         (n.Email != null && n.Email.ToLower().Contains(searchLower)) ||
                                         (n.SoDienThoai != null && n.SoDienThoai.Contains(searchLower)) ||
                                         (n.DiaChi != null && n.DiaChi.ToLower().Contains(searchLower)) ||
                                         (n.TaiKhoan != null && n.TaiKhoan.TenDangNhap.ToLower().Contains(searchLower)) ||
                                         n.MaNguoiHien.ToString().Contains(searchLower));
            }

            // Lọc theo nhóm máu ABO (bao quát cả tiền tố "Nhóm ...")
            if (!string.IsNullOrWhiteSpace(nhomMau) && nhomMau != "all")
            {
                var upperNm = nhomMau.Trim().ToUpper();
                if (upperNm == "AB")
                {
                    query = query.Where(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("AB"));
                }
                else if (upperNm == "A")
                {
                    query = query.Where(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("A") && !n.NhomMau.ToUpper().Contains("AB"));
                }
                else if (upperNm == "B")
                {
                    query = query.Where(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("B") && !n.NhomMau.ToUpper().Contains("AB"));
                }
                else if (upperNm == "O")
                {
                    query = query.Where(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("O"));
                }
            }

            // Lọc theo Rh và loại đặc biệt
            if (!string.IsNullOrWhiteSpace(rhGroup) && rhGroup != "all")
            {
                if (rhGroup == "rh_am")
                {
                    query = query.Where(n => n.NhomMau != null && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));
                }
                else if (rhGroup == "rh_duong")
                {
                    query = query.Where(n => n.NhomMau != null && (n.NhomMau.Contains("+") || n.NhomMau.Contains("Rh(+)") || n.NhomMau.Contains("Rh+") || n.NhomMau.ToLower().Contains("dương")));
                }
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                if (status == "hoatdong")
                {
                    query = query.Where(n => n.TrangThai == "Hoạt động" || n.TrangThai == null);
                }
                else if (status == "tamhoan")
                {
                    query = query.Where(n => n.TrangThai != null && n.TrangThai != "Hoạt động");
                }
            }

            // Lấy toàn bộ danh sách phù hợp để xử lý tier hoặc phân trang
            var donorList = await query.OrderByDescending(n => n.MaNguoiHien).ToListAsync();

            // Lọc theo phân hạng cống hiến nếu có (chỉ tính các lần "Hiến máu thành công")
            if (!string.IsNullOrWhiteSpace(tier) && tier != "all")
            {
                if (tier == "diamond")
                {
                    donorList = donorList.Where(d => d.PhieuDangKyHienMaus.Count(DangKyController.IsConfirmedSuccessfulDonation) >= 20).ToList();
                }
                else if (tier == "gold")
                {
                    donorList = donorList.Where(d => {
                        var c = d.PhieuDangKyHienMaus.Count(DangKyController.IsConfirmedSuccessfulDonation);
                        return c >= 10 && c < 20;
                    }).ToList();
                }
                else if (tier == "silver")
                {
                    donorList = donorList.Where(d => {
                        var c = d.PhieuDangKyHienMaus.Count(DangKyController.IsConfirmedSuccessfulDonation);
                        return c >= 5 && c < 10;
                    }).ToList();
                }
                else if (tier == "bronze")
                {
                    donorList = donorList.Where(d => {
                        var c = d.PhieuDangKyHienMaus.Count(DangKyController.IsConfirmedSuccessfulDonation);
                        return c >= 1 && c < 5;
                    }).ToList();
                }
            }

            // Lọc theo điều kiện sẵn sàng hiến ngay nếu status == "dudieukien"
            if (status == "dudieukien")
            {
                donorList = donorList.Where(d =>
                    (d.TrangThai == "Hoạt động" || string.IsNullOrEmpty(d.TrangThai)) &&
                    DangKyController.EvaluateRecentDonationEligibility(d.PhieuDangKyHienMaus, today).IsEligible).ToList();
            }

            // 4. Phân trang
            pageSize = pageSize <= 0 ? 10 : pageSize;
            var totalItems = donorList.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var pagedDonors = donorList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // 5. Gán dữ liệu cho View
            ViewBag.TongNguoiHien = tongNguoiHien;
            ViewBag.TongMauHiem = tongMauHiem;
            ViewBag.OAm = oAm;
            ViewBag.AAm = aAm;
            ViewBag.BAm = bAm;
            ViewBag.ABAm = abAm;
            ViewBag.DuDieuKienCount = duDieuKienCount;
            ViewBag.HienTichCucCount = hienTichCucCount;

            ViewBag.TotalItems = totalItems;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentNhomMau = nhomMau;
            ViewBag.CurrentRhGroup = rhGroup;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentTier = tier;

            return View(pagedDonors);
        }

        // POST: /Admin/KhoaMoKhoaNguoiHien
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KhoaMoKhoaNguoiHien(int id, string? returnUrl)
        {
            var donor = await _nguoiHienMauRepo.GetByIdWithTaiKhoanAsync(id);

            if (donor == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ người hiến máu để thao tác.";
            }
            else
            {
                var isCurrentlyActive = donor.TrangThai == "Hoạt động" || string.IsNullOrEmpty(donor.TrangThai);
                var newStatus = isCurrentlyActive ? "Bị khóa" : "Hoạt động";
                donor.TrangThai = newStatus;
                if (donor.TaiKhoan != null)
                {
                    donor.TaiKhoan.TrangThai = newStatus;
                }

                await _nguoiHienMauRepo.SaveChangesAsync();
                var actionName = newStatus == "Bị khóa" ? "Khóa" : "Mở khóa";
                TempData["SuccessMessage"] = $"Đã {actionName} hồ sơ người hiến máu '{donor.HoTen}' thành công.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(NguoiHienMau));
        }

        // GET: /Admin/XuatDanhSachNguoiHienCsv
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XuatDanhSachNguoiHienCsv()
        {
            var donors = await _nguoiHienMauRepo.GetQueryableWithDetails()
                .OrderByDescending(n => n.MaNguoiHien)
                .ToListAsync();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Mã người hiến,Họ và tên,Ngày sinh,Giới tính,Nhóm máu,Cân nặng (kg),Số điện thoại,Email,Địa chỉ,Số lần hiến thành công,Trạng thái");
            foreach (var d in donors)
            {
                var soLan = d.PhieuDangKyHienMaus.Count(DangKyController.IsConfirmedSuccessfulDonation);
                sb.AppendLine($"{d.MaNguoiHien},\"{d.HoTen.Replace("\"", "\"\"")}\",\"{(d.NgaySinh?.ToString("dd/MM/yyyy") ?? "")}\",\"{d.GioiTinh ?? ""}\",\"{d.NhomMau ?? ""}\",\"{d.CanNang?.ToString() ?? ""}\",\"{d.SoDienThoai ?? ""}\",\"{d.Email ?? ""}\",\"{(d.DiaChi ?? "").Replace("\"", "\"\"")}\",{soLan},\"{d.TrangThai ?? "Hoạt động"}\"");
            }

            var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return File(bytes, "text/csv; charset=utf-8", $"DanhSachNguoiHienMau_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }

        // GET: /Admin/DonViToChuc
        public async Task<IActionResult> DonViToChuc(string? search, string? status)
        {
            var donViList = await _donViToChucRepository.GetAllWithDotHienMauAsync(search, status);
            var stats = await _donViToChucRepository.GetDonViStatsAsync();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
            ViewBag.TotalDonViCount = stats.TotalCount;
            ViewBag.ActiveDonViCount = stats.ActiveCount;
            ViewBag.InactiveDonViCount = stats.InactiveCount;
            ViewBag.TotalDotCount = stats.TotalDotCount;

            return View(donViList);
        }

        // POST: /Admin/CreateDonViToChuc
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDonViToChuc([Bind("TenDonViToChuc,MoTa,EmailLienHe,TrangThai")] DonViToChuc donVi)
        {
            if (string.IsNullOrWhiteSpace(donVi.TenDonViToChuc))
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức không được để trống.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            var tenTrimmed = donVi.TenDonViToChuc.Trim();
            if (tenTrimmed.Length < 3 || tenTrimmed.Length > 200)
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức phải có độ dài từ 3 đến 200 ký tự.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            string? cleanEmail = null;
            if (!string.IsNullOrWhiteSpace(donVi.EmailLienHe))
            {
                cleanEmail = donVi.EmailLienHe.Trim();
                if (cleanEmail.Length > 100 || !EmailRegex.IsMatch(cleanEmail))
                {
                    TempData["ErrorMessage"] = "Địa chỉ email liên hệ không đúng định dạng.";
                    return RedirectToAction(nameof(DonViToChuc));
                }
            }

            if (!string.IsNullOrWhiteSpace(donVi.MoTa) && donVi.MoTa.Trim().Length > 500)
            {
                TempData["ErrorMessage"] = "Mô tả đơn vị tổ chức không được vượt quá 500 ký tự.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            if (await _donViToChucRepository.IsTenDonViExistsAsync(tenTrimmed))
            {
                TempData["ErrorMessage"] = $"Tên đơn vị \"{tenTrimmed}\" đã tồn tại. Vui lòng chọn tên khác.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            donVi.TenDonViToChuc = tenTrimmed;
            donVi.EmailLienHe = cleanEmail;
            donVi.MoTa = donVi.MoTa?.Trim();
            donVi.TrangThai = (!string.IsNullOrWhiteSpace(donVi.TrangThai) && AllowedDonViStatuses.Contains(donVi.TrangThai.Trim()))
                ? donVi.TrangThai.Trim()
                : "Hoạt động";

            await _donViToChucRepository.AddAsync(donVi);
            await _donViToChucRepository.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thêm đơn vị tổ chức thành công!";

            return RedirectToAction(nameof(DonViToChuc));
        }

        // POST: /Admin/EditDonViToChuc
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditDonViToChuc(int id, [Bind("MaDonViToChuc,TenDonViToChuc,MoTa,EmailLienHe,TrangThai")] DonViToChuc donVi)
        {
            if (id != donVi.MaDonViToChuc)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(donVi.TenDonViToChuc))
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức không được để trống.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            var tenTrimmed = donVi.TenDonViToChuc.Trim();
            if (tenTrimmed.Length < 3 || tenTrimmed.Length > 200)
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức phải có độ dài từ 3 đến 200 ký tự.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            string? cleanEmail = null;
            if (!string.IsNullOrWhiteSpace(donVi.EmailLienHe))
            {
                cleanEmail = donVi.EmailLienHe.Trim();
                if (cleanEmail.Length > 100 || !EmailRegex.IsMatch(cleanEmail))
                {
                    TempData["ErrorMessage"] = "Địa chỉ email liên hệ không đúng định dạng.";
                    return RedirectToAction(nameof(DonViToChuc));
                }
            }

            if (!string.IsNullOrWhiteSpace(donVi.MoTa) && donVi.MoTa.Trim().Length > 500)
            {
                TempData["ErrorMessage"] = "Mô tả đơn vị tổ chức không được vượt quá 500 ký tự.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            if (await _donViToChucRepository.IsTenDonViExistsAsync(tenTrimmed, donVi.MaDonViToChuc))
            {
                TempData["ErrorMessage"] = $"Tên đơn vị \"{tenTrimmed}\" đã tồn tại. Vui lòng chọn tên khác.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            var existing = await _donViToChucRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.TenDonViToChuc = tenTrimmed;
            existing.MoTa = donVi.MoTa?.Trim();
            existing.EmailLienHe = cleanEmail;
            existing.TrangThai = (!string.IsNullOrWhiteSpace(donVi.TrangThai) && AllowedDonViStatuses.Contains(donVi.TrangThai.Trim()))
                ? donVi.TrangThai.Trim()
                : "Hoạt động";

            _donViToChucRepository.Update(existing);
            await _donViToChucRepository.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật đơn vị tổ chức thành công!";

            return RedirectToAction(nameof(DonViToChuc));
        }

        // GET: /Admin/CheckDuplicateDonViToChuc (AJAX API kiểm tra trùng tên)
        [HttpGet]
        public async Task<IActionResult> CheckDuplicateDonViToChuc(string ten, int? excludeId)
        {
            if (string.IsNullOrWhiteSpace(ten))
            {
                return Json(new { isDuplicate = false });
            }

            var isDuplicate = await _donViToChucRepository.IsTenDonViExistsAsync(ten.Trim(), excludeId);
            return Json(new { isDuplicate });
        }

        // POST: /Admin/DeleteDonViToChuc
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDonViToChuc(int id)
        {
            var donVi = await _donViToChucRepository.GetByIdAsync(id);
            if (donVi == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn vị tổ chức cần xóa.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            if (await _donViToChucRepository.HasDotHienMauAsync(id))
            {
                TempData["ErrorMessage"] = $"Không thể xóa đơn vị \"{donVi.TenDonViToChuc}\" vì đang có đợt hiến máu liên kết. Việc xóa sẽ làm các đợt hiến máu này không còn hợp lệ.";
            }
            else
            {
                _donViToChucRepository.Delete(donVi);
                await _donViToChucRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa đơn vị tổ chức thành công!";
            }

            return RedirectToAction(nameof(DonViToChuc));
        }

        // ==========================================
        // MODULE 5: QUẢN LÝ KẾT QUẢ HIẾN MÁU
        // ==========================================

        // GET: /Admin/KetQuaHienMau
        public async Task<IActionResult> KetQuaHienMau(string? search, string? ketQua, int? maDot, int page = 1)
        {
            int pageSize = 10;
            if (page < 1) page = 1;

            var query = _phieuRepository.GetKetQuaQueryableWithDetails();

            // Thống kê nhanh toàn bộ kết quả
            ViewBag.TotalCount = await _phieuRepository.CountKetQuaAsync();
            ViewBag.SuccessCount = await _phieuRepository.CountKetQuaAsync("Hiến máu thành công");
            ViewBag.RejectCount = await _phieuRepository.CountKetQuaAsync("Không đủ điều kiện hiến");

            // Danh sách phiếu "Đã khám sàng lọc" chưa có kết quả (để nhân viên y tế nhập kết quả)
            ViewBag.PendingResults = await _phieuRepository.GetPendingResultsAsync(10);

            // Bộ lọc tìm kiếm
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(k =>
                    (k.PhieuDangKyHienMau != null && k.PhieuDangKyHienMau.NguoiHienMau != null && k.PhieuDangKyHienMau.NguoiHienMau.HoTen.ToLower().Contains(searchLower)) ||
                    (k.PhieuDangKyHienMau != null && k.PhieuDangKyHienMau.DotHienMau != null && k.PhieuDangKyHienMau.DotHienMau.TenDot.ToLower().Contains(searchLower)) ||
                    k.MaPhieu.ToString().Contains(searchLower) ||
                    k.MaKetQuaHienMau.ToString().Contains(searchLower) ||
                    (k.NhanXet != null && k.NhanXet.ToLower().Contains(searchLower)));
            }

            if (!string.IsNullOrWhiteSpace(ketQua) && ketQua != "all")
            {
                query = query.Where(k => k.KetQua == ketQua);
            }

            if (maDot.HasValue && maDot.Value > 0)
            {
                query = query.Where(k => k.PhieuDangKyHienMau != null && k.PhieuDangKyHienMau.MaDot == maDot.Value);
            }

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentKetQua = ketQua;
            ViewBag.CurrentMaDot = maDot;
            ViewBag.DotList = await _phieuRepository.GetAllDotHienMausOrderedAsync();

            int totalFiltered = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalFiltered = totalFiltered;

            var items = await query
                .OrderByDescending(k => k.NgayCapNhat ?? DateTime.MinValue)
                .ThenByDescending(k => k.MaKetQuaHienMau)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(items);
        }

        // GET: /Admin/KetQuaXetNghiem (Alias / redirect)
        public IActionResult KetQuaXetNghiem()
        {
            return RedirectToAction(nameof(KetQuaHienMau));
        }

        // GET: /Admin/ChiTietKetQua/{id}
        public async Task<IActionResult> ChiTietKetQua(int id)
        {
            var ketQua = await _phieuRepository.GetKetQuaDetailsByIdAsync(id);

            if (ketQua == null)
            {
                return NotFound("Không tìm thấy kết quả hiến máu.");
            }

            return View(ketQua);
        }

        // POST: /Admin/TaoKetQua
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoKetQua(int maPhieu, string ketQua, double? diemDanhGia, string? nhanXet, string? returnUrl)
        {
            var phieu = await _phieuRepository.GetForUpdateAsync(maPhieu);
            if (phieu is null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký để nhập kết quả.";
                return RedirectToAction(nameof(KetQuaHienMau));
            }

            if (phieu.TrangThai != "Đã khám sàng lọc")
                TempData["ErrorMessage"] = $"Chỉ phiếu Đã khám sàng lọc mới được ghi kết quả. Trạng thái hiện tại: '{phieu.TrangThai}'.";
            else if (!await _phieuRepository.HasLichKhamHoanThanhAsync(maPhieu))
                TempData["ErrorMessage"] = "Chỉ phiếu có lịch khám sàng lọc đã hoàn thành mới được ghi nhận kết quả.";
            else if (phieu.KetQuaHienMau is not null)
                TempData["ErrorMessage"] = "Phiếu này đã có kết quả. Vui lòng dùng chức năng cập nhật kết quả.";
            else if (string.IsNullOrWhiteSpace(ketQua) || (ketQua != "Hiến máu thành công" && ketQua != "Không đủ điều kiện hiến"))
                TempData["ErrorMessage"] = "Kết quả không hợp lệ. Chỉ chấp nhận 'Hiến máu thành công' hoặc 'Không đủ điều kiện hiến'.";
            else if (diemDanhGia.HasValue && (diemDanhGia.Value < 0 || diemDanhGia.Value > 10))
                TempData["ErrorMessage"] = "Điểm đánh giá phải nằm trong khoảng từ 0 đến 10.";
            else
            {
                await using var transaction = await _phieuRepository.BeginTransactionAsync();
                try
                {
                    var ketQuaMoi = new KetQuaHienMau
                    {
                        MaPhieu = maPhieu,
                        KetQua = ketQua.Trim(),
                        DiemDanhGia = diemDanhGia,
                        NhanXet = nhanXet?.Trim(),
                        NgayCapNhat = DateTime.Now
                    };
                    _phieuRepository.AddKetQua(ketQuaMoi);
                    phieu.TrangThai = ketQua.Trim();
                    phieu.NgayXuLy = DateTime.Now;
                    await _phieuRepository.SaveChangesAsync();

                    var successful = await _phieuRepository.CountHienMauThanhCongAsync(phieu.MaDot);
                    var quotaWarning = phieu.DotHienMau is not null && phieu.DotHienMau.ChiTieuDonViMau > 0 && successful >= phieu.DotHienMau.ChiTieuDonViMau
                        ? $" Đợt '{phieu.DotHienMau.TenDot}' đã đạt chỉ tiêu ({successful}/{phieu.DotHienMau.ChiTieuDonViMau}). Nhân viên y tế có thể xem xét đóng đợt; các phiếu hiện có được giữ nguyên."
                        : string.Empty;
                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = $"Đã ghi nhận kết quả '{ketQua.Trim()}' cho phiếu #DKHM-{phieu.MaPhieu:D5}." + quotaWarning;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Không thể lưu kết quả do dữ liệu vừa thay đổi. Vui lòng tải lại và kiểm tra trạng thái phiếu.";
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction(nameof(KetQuaHienMau));
        }
        // POST: /Admin/SuaKetQua
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaKetQua(int id, string ketQua, double? diemDanhGia, string? nhanXet, string? returnUrl)
        {
            var ketQuaRecord = await _phieuRepository.GetKetQuaForUpdateAsync(id);
            if (ketQuaRecord is null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bản ghi kết quả hiến máu.";
                return RedirectToAction(nameof(KetQuaHienMau));
            }

            var phieu = ketQuaRecord.PhieuDangKyHienMau;
            if (phieu is null || phieu.TrangThai != ketQuaRecord.KetQua ||
                (phieu.TrangThai != "Hiến máu thành công" && phieu.TrangThai != "Không đủ điều kiện hiến"))
                TempData["ErrorMessage"] = "Không thể sửa kết quả khi trạng thái phiếu không khớp với kết quả cuối cùng đã ghi nhận.";
            else if (string.IsNullOrWhiteSpace(ketQua) || (ketQua != "Hiến máu thành công" && ketQua != "Không đủ điều kiện hiến"))
                TempData["ErrorMessage"] = "Kết quả không hợp lệ. Chỉ chấp nhận 'Hiến máu thành công' hoặc 'Không đủ điều kiện hiến'.";
            else if (diemDanhGia.HasValue && (diemDanhGia.Value < 0 || diemDanhGia.Value > 10))
                TempData["ErrorMessage"] = "Điểm đánh giá phải nằm trong khoảng từ 0 đến 10.";
            else
            {
                await using var transaction = await _phieuRepository.BeginTransactionAsync();
                try
                {
                    ketQuaRecord.KetQua = ketQua.Trim();
                    ketQuaRecord.DiemDanhGia = diemDanhGia;
                    ketQuaRecord.NhanXet = nhanXet?.Trim();
                    ketQuaRecord.NgayCapNhat = DateTime.Now;
                    phieu!.TrangThai = ketQua.Trim();
                    phieu.NgayXuLy = DateTime.Now;
                    await _phieuRepository.SaveChangesAsync();

                    var successful = await _phieuRepository.CountHienMauThanhCongAsync(phieu.MaDot);
                    var quotaWarning = phieu.DotHienMau is not null && phieu.DotHienMau.ChiTieuDonViMau > 0 && successful >= phieu.DotHienMau.ChiTieuDonViMau
                        ? $" Đợt '{phieu.DotHienMau.TenDot}' đã đạt chỉ tiêu ({successful}/{phieu.DotHienMau.ChiTieuDonViMau}). Nhân viên y tế có thể xem xét đóng đợt; các phiếu hiện có được giữ nguyên."
                        : string.Empty;
                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = $"Đã cập nhật kết quả hiến máu #{ketQuaRecord.MaKetQuaHienMau}." + quotaWarning;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Không thể cập nhật kết quả do dữ liệu vừa thay đổi. Vui lòng tải lại và kiểm tra trạng thái phiếu.";
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction(nameof(ChiTietKetQua), new { id = ketQuaRecord.MaKetQuaHienMau });
        }
        public IActionResult BaoCao()
        {
            return RedirectToAction("Index", "ThongKe");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> NhanSu(string? search, string? vaiTro, string? trangThai)
        {
            var accounts = await _taiKhoanRepository.GetAllWithProfileAsync(search, vaiTro, trangThai);
            var stats = await _taiKhoanRepository.GetAccountStatsAsync();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentVaiTro = vaiTro;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.TotalAccounts = stats.TotalCount;
            ViewBag.AdminCount = stats.AdminCount;
            ViewBag.MedicalStaffCount = stats.MedicalStaffCount;
            ViewBag.DonorCount = stats.DonorCount;
            ViewBag.ActiveCount = stats.ActiveCount;
            ViewBag.LockedCount = stats.LockedCount;

            return View(accounts);
        }

        // POST: /Admin/KhoaMoKhoaTaiKhoan
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KhoaMoKhoaTaiKhoan(int id, string? returnUrl)
        {
            var account = await _taiKhoanRepository.GetByIdAsync(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản để thao tác.";
            }
            else
            {
                // Ngăn chặn Quản trị viên tự khóa tài khoản đang đăng nhập của chính mình
                var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(currentUserIdStr, out int currentUserId) && currentUserId == id && account.TrangThai == "Hoạt động")
                {
                    TempData["ErrorMessage"] = "Không thể tự khóa tài khoản Quản trị viên đang đăng nhập của chính bạn.";
                }
                else
                {
                    var newStatus = account.TrangThai == "Hoạt động" ? "Bị khóa" : "Hoạt động";
                    await _taiKhoanRepository.UpdateAccountStatusAsync(id, newStatus);
                    var actionName = newStatus == "Bị khóa" ? "Khóa" : "Mở khóa";
                    TempData["SuccessMessage"] = $"Đã {actionName} thành công tài khoản '{account.TenDangNhap}'.";
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(NhanSu));
        }

        // POST: /Admin/TaoTaiKhoan
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoTaiKhoan(CreateAccountAdminViewModel model)
        {
            // 1. Kiểm tra các trường thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(model.TenDangNhap) ||
                string.IsNullOrWhiteSpace(model.MatKhau) ||
                string.IsNullOrWhiteSpace(model.HoTen))
            {
                SetModalError("Vui lòng điền đầy đủ các thông tin bắt buộc (*).", model);
                return RedirectToAction(nameof(NhanSu));
            }

            var cleanUsername = model.TenDangNhap.Trim();
            var cleanHoTen = model.HoTen.Trim();

            // 2. Kiểm tra độ dài và định dạng tên đăng nhập
            if (cleanUsername.Length < 3 || cleanUsername.Length > 50)
            {
                SetModalError("Tên đăng nhập phải có độ dài từ 3 đến 50 ký tự.", model);
                return RedirectToAction(nameof(NhanSu));
            }

            if (!UsernameRegex.IsMatch(cleanUsername))
            {
                SetModalError("Tên đăng nhập chỉ được chứa chữ cái, chữ số, dấu gạch dưới (_), gạch ngang (-) và dấu chấm (.), không chứa khoảng trắng.", model);
                return RedirectToAction(nameof(NhanSu));
            }

            // 3. Kiểm tra độ dài mật khẩu khởi tạo
            if (model.MatKhau.Length < 6 || model.MatKhau.Length > 100)
            {
                SetModalError("Mật khẩu khởi tạo phải có độ dài từ 6 đến 100 ký tự.", model);
                return RedirectToAction(nameof(NhanSu));
            }

            // 4. Kiểm tra độ dài họ tên
            if (cleanHoTen.Length < 2 || cleanHoTen.Length > 100)
            {
                SetModalError("Họ và tên phải có độ dài từ 2 đến 100 ký tự.", model);
                return RedirectToAction(nameof(NhanSu));
            }

            // 5. Kiểm tra định dạng email nếu có nhập
            string? cleanEmail = null;
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                cleanEmail = model.Email.Trim();
                if (cleanEmail.Length > 100 || !EmailRegex.IsMatch(cleanEmail))
                {
                    SetModalError("Địa chỉ email không đúng định dạng (Ví dụ: nguoidung@domain.com).", model);
                    return RedirectToAction(nameof(NhanSu));
                }
            }

            // 6. Kiểm tra trùng tên đăng nhập qua Repository
            if (await _taiKhoanRepository.IsTenDangNhapExistsAsync(cleanUsername))
            {
                SetModalError($"Tên đăng nhập '{cleanUsername}' đã được sử dụng. Vui lòng chọn tên khác.", model);
                return RedirectToAction(nameof(NhanSu));
            }

            // 7. Kiểm tra trùng email qua Repository nếu có nhập email
            if (!string.IsNullOrWhiteSpace(cleanEmail) && await _taiKhoanRepository.IsEmailExistsAsync(cleanEmail))
            {
                SetModalError("Địa chỉ email này đã được sử dụng cho tài khoản khác.", model);
                return RedirectToAction(nameof(NhanSu));
            }

            // 8. Chuẩn hóa Vai trò & Trạng thái hợp lệ
            var role = (!string.IsNullOrWhiteSpace(model.VaiTro) && AllowedRoles.Contains(model.VaiTro.Trim()))
                ? model.VaiTro.Trim()
                : "Medical Staff";
            var status = (!string.IsNullOrWhiteSpace(model.TrangThai) && AllowedAccountStatuses.Contains(model.TrangThai.Trim()))
                ? model.TrangThai.Trim()
                : "Hoạt động";

            var account = new TaiKhoan
            {
                TenDangNhap = cleanUsername,
                HoTen = cleanHoTen,
                Email = cleanEmail,
                VaiTro = role,
                TrangThai = status
            };

            account.MatKhau = _passwordHasher.HashPassword(account, model.MatKhau);

            if (account.VaiTro == "Donor")
            {
                var donorProfile = new NguoiHienMau
                {
                    HoTen = account.HoTen,
                    Email = account.Email,
                    TrangThai = account.TrangThai
                };
                await _taiKhoanRepository.RegisterDonorAccountAsync(account, donorProfile);
            }
            else
            {
                await _taiKhoanRepository.AddAsync(account);
                await _taiKhoanRepository.SaveChangesAsync();
            }

            var tenVaiTro = account.VaiTro switch
            {
                "Admin" => "Quản trị viên",
                "Medical Staff" => "Nhân viên y tế",
                _ => "Người hiến máu"
            };
            TempData["SuccessMessage"] = $"Đã tạo mới tài khoản '{account.TenDangNhap}' ({tenVaiTro}) thành công!";
            return RedirectToAction(nameof(NhanSu));
        }

        private void SetModalError(string errorMsg, CreateAccountAdminViewModel model)
        {
            TempData["CreateAccountError"] = errorMsg;
            TempData["ErrorMessage"] = errorMsg;
            TempData["CreateTenDangNhap"] = model.TenDangNhap;
            TempData["CreateHoTen"] = model.HoTen;
            TempData["CreateEmail"] = model.Email;
            TempData["CreateVaiTro"] = model.VaiTro;
            TempData["CreateTrangThai"] = model.TrangThai;
            TempData["OpenCreateModal"] = true;
        }

        // POST: /Admin/DoiMatKhau
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(int id, string matKhauMoi)
        {
            if (string.IsNullOrWhiteSpace(matKhauMoi) || matKhauMoi.Length < 6 || matKhauMoi.Length > 100)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới phải có độ dài từ 6 đến 100 ký tự và không chỉ chứa khoảng trắng.";
                return RedirectToAction(nameof(NhanSu));
            }

            var account = await _taiKhoanRepository.GetByIdAsync(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(NhanSu));
            }

            var hashed = _passwordHasher.HashPassword(account, matKhauMoi);
            await _taiKhoanRepository.ChangePasswordAsync(id, hashed);

            TempData["SuccessMessage"] = $"Đã đổi mật khẩu cho tài khoản '{account.TenDangNhap}' thành công.";
            return RedirectToAction(nameof(NhanSu));
        }

        // POST: /Admin/CapNhatVaiTro
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatVaiTro(int id, string vaiTroMoi, string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(vaiTroMoi) || !AllowedRoles.Contains(vaiTroMoi.Trim()))
            {
                TempData["ErrorMessage"] = "Vai trò phân quyền không hợp lệ.";
                return RedirectToAction(nameof(NhanSu));
            }

            var cleanRole = vaiTroMoi.Trim();
            var account = await _taiKhoanRepository.GetByIdAsync(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản cần phân quyền.";
                return RedirectToAction(nameof(NhanSu));
            }

            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(currentUserIdStr, out int currentUserId) && currentUserId == id && cleanRole != "Admin")
            {
                TempData["ErrorMessage"] = "Không thể tự hạ quyền Quản trị viên của tài khoản đang đăng nhập.";
                return RedirectToAction(nameof(NhanSu));
            }

            await _taiKhoanRepository.UpdateAccountRoleAsync(id, cleanRole);
            var tenVaiTro = cleanRole switch
            {
                "Admin" => "Quản trị viên",
                "Medical Staff" => "Nhân viên y tế",
                _ => "Người hiến máu"
            };
            TempData["SuccessMessage"] = $"Đã cập nhật phân quyền tài khoản '{account.TenDangNhap}' sang vai trò '{tenVaiTro}' thành công.";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(NhanSu));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RBAC()
        {
            return View();
        }

        public IActionResult KhoLanhIoT()
        {
            return View();
        }

        // ==============================================================
        // Ho va ten: NGUYEN THI PHUONG THAO
        // Ma sinh vien: 23103100272
        // Noi dung thuc hien: Quan ly dot hien mau, tim kiem,
        //   loc, sap xep va phan trang (Admin Panel - Module 2)
        // ==============================================================

        // GET: /Admin/DotHienMau
        public async Task<IActionResult> DotHienMau(
            string? search, string? status, int? donViId,
            string? nhomMau, double? canNang, string? thoiHan,
            string? sort, int page = 1)
        {
            int pageSize = 10;

            var query = _dotRepo.GetQueryable();
            query = _dotRepo.ApplySearch(query, search);
            query = _dotRepo.ApplyFilterTrangThai(query, status);
            query = _dotRepo.ApplyFilterDonVi(query, donViId);
            query = _dotRepo.ApplyFilterNhomMau(query, nhomMau);
            query = _dotRepo.ApplyFilterCanNang(query, canNang);
            query = _dotRepo.ApplyFilterThoiHan(query, thoiHan);
            query = _dotRepo.ApplySort(query, sort);

            int totalCount = await _dotRepo.CountAsync(query);
            page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize)));
            var items = await _dotRepo.GetPagedAsync(query, page, pageSize);

            // Metric cards
            var all = _dotRepo.GetQueryable();
            ViewBag.TotalCount   = await _dotRepo.CountAsync(all);
            ViewBag.ChuaMoCount  = await _dotRepo.CountAsync(all.Where(d => d.TrangThai == "Chưa mở"));
            ViewBag.DangMoCount  = await _dotRepo.CountAsync(all.Where(d => d.TrangThai == "Đang mở đăng ký"));
            var today = DateTime.Today;
            ViewBag.DangNhanCount = await _dotRepo.CountAsync(all.Where(d => d.TrangThai == "Đang mở đăng ký" && d.NgayBatDauDangKy.Date <= today && d.HanDangKy.Date >= today));
            ViewBag.TamDungCount = await _dotRepo.CountAsync(all.Where(d => d.TrangThai == "Tạm dừng"));
            ViewBag.DaDongCount  = await _dotRepo.CountAsync(all.Where(d => d.TrangThai == "Đã đóng"));

            // Paging info
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalItems  = totalCount;

            // Filter preservation
            ViewBag.Search  = search;
            ViewBag.Status  = status;
            ViewBag.DonViId = donViId;
            ViewBag.NhomMau = nhomMau;
            ViewBag.CanNang = canNang;
            ViewBag.ThoiHan = thoiHan;
            ViewBag.Sort    = sort;

            ViewBag.DonViList = await _dotRepo.GetActiveDonViToChucsAsync();

            return View(items);
        }

        // POST: /Admin/AdminCreateDot
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Medical Staff")]
        public async Task<IActionResult> AdminCreateDot(DotHienMau model)
        {
            if (!(await _dotRepo.GetActiveDonViToChucsAsync()).Any(d => d.MaDonViToChuc == model.MaDonViToChuc))
                ModelState.AddModelError(nameof(model.MaDonViToChuc), "Vui lòng chọn đơn vị tổ chức đang hoạt động.");
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(DotHienMau));
            }
            try
            {
                model.TrangThai = "Chưa mở";
                await _dotRepo.AddAsync(model);
                await _dotRepo.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm đợt hiến máu thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi thêm: " + ex.Message;
            }
            return RedirectToAction(nameof(DotHienMau));
        }

        // POST: /Admin/AdminEditDot
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Medical Staff")]
        public async Task<IActionResult> AdminEditDot(int id, DotHienMau model)
        {
            if (!(await _dotRepo.GetActiveDonViToChucsAsync()).Any(d => d.MaDonViToChuc == model.MaDonViToChuc))
                ModelState.AddModelError(nameof(model.MaDonViToChuc), "Vui lòng chọn đơn vị tổ chức đang hoạt động.");
            if (id != model.MaDot)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(DotHienMau));
            }
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(DotHienMau));
            }
            try
            {
                var existing = await _dotRepo.GetByIdAsync(id);
                if (existing == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đợt hiến máu.";
                    return RedirectToAction(nameof(DotHienMau));
                }
                existing.TenDot            = model.TenDot;
                existing.MaDonViToChuc     = model.MaDonViToChuc;
                existing.ChiTieuDonViMau   = model.ChiTieuDonViMau;
                existing.NhomMauUuTien     = model.NhomMauUuTien;
                existing.CanNangToiThieu   = model.CanNangToiThieu;
                existing.NgayBatDauDangKy  = model.NgayBatDauDangKy;
                existing.HanDangKy         = model.HanDangKy;
                existing.MoTaDot           = model.MoTaDot;
                existing.DieuKienNguoiHien = model.DieuKienNguoiHien;

                await _dotRepo.UpdateAsync(existing);
                await _dotRepo.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật đợt hiến máu thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật: " + ex.Message;
            }
            return RedirectToAction(nameof(DotHienMau));
        }

        // POST: /Admin/AdminChangeStatusDot
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Medical Staff")]
        public async Task<IActionResult> AdminChangeStatusDot(int id, string trangThai)
        {
            try
            {
                var dot = await _dotRepo.GetByIdAsync(id);
                if (dot == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đợt hiến máu.";
                    return RedirectToAction(nameof(DotHienMau));
                }
                dot.TrangThai = trangThai;
                var errors = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(dot,
                    new System.ComponentModel.DataAnnotations.ValidationContext(dot), errors, true)
                    || !(await _dotRepo.GetActiveDonViToChucsAsync()).Any(d => d.MaDonViToChuc == dot.MaDonViToChuc))
                {
                    TempData["ErrorMessage"] = "Dữ liệu đợt hoặc đơn vị tổ chức chưa hợp lệ. " + string.Join(" ", errors.Select(e => e.ErrorMessage));
                    return RedirectToAction(nameof(DotHienMau));
                }
                await _dotRepo.UpdateAsync(dot);
                await _dotRepo.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật trạng thái: " + ex.Message;
            }
            return RedirectToAction(nameof(DotHienMau));
        }

        // POST: /Admin/AdminDeleteDot
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Medical Staff")]
        public async Task<IActionResult> AdminDeleteDot(int id)
        {
            try
            {
                var dot = await _dotRepo.GetByIdAsync(id);
                if (dot == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đợt hiến máu.";
                    return RedirectToAction(nameof(DotHienMau));
                }
                bool hasPhieu = await _dotRepo.HasPhieuDangKyAsync(id);
                if (hasPhieu)
                {
                    TempData["ErrorMessage"] = "Không thể xóa đợt này vì đã có phiếu đăng ký liên quan.";
                    return RedirectToAction(nameof(DotHienMau));
                }
                await _dotRepo.DeleteAsync(dot);
                await _dotRepo.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa đợt hiến máu thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa: " + ex.Message;
            }
            return RedirectToAction(nameof(DotHienMau));
        }
    }
}
