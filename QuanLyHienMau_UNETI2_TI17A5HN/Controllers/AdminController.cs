// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Quản lí đơn vị tổ chức hiến máu
// Thêm sửa xóa đơn vị, quản lí đơn vị tổ chức 

using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    [Authorize(Roles = "Admin,Medical Staff")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
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
            // 8+ KPI Macro từ database SQL Server
            var tongNguoiHien = await _context.NguoiHienMaus.CountAsync();
            var tongPhieu = await _context.PhieuDangKyHienMaus.CountAsync();
            var phieuChoXacNhan = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Chờ xác nhận");
            var phieuDaXacNhan = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Đã xác nhận");
            var phieuDaKham = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Đã khám sàng lọc");
            var hienMauThanhCong = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Hiến máu thành công");
            var khongDuDieuKien = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Không đủ điều kiện hiến");
            var dotDangMo = await _context.DotHienMaus.CountAsync(d => d.TrangThai == "Đang mở đăng ký");
            var tongDonVi = await _context.DonViToChucs.CountAsync();
            var tongLichKham = await _context.LichKhamSangLocs.CountAsync();

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

            // Thống kê theo nhóm máu (LINQ thực tế từ NguoiHienMau)
            var bloodGroupStats = await _context.NguoiHienMaus
                .GroupBy(n => n.NhomMau ?? "Chưa xác định")
                .Select(g => new { NhomMau = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            ViewBag.BloodGroupStats = bloodGroupStats;

            // Thống kê theo từng Đợt hiến máu (Bảng dữ liệu thật)
            var dotList = await _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .Include(d => d.PhieuDangKyHienMaus)
                .OrderByDescending(d => d.NgayBatDauDangKy)
                .Take(8)
                .ToListAsync();

            ViewBag.DotList = dotList;

            // Danh sách các phiếu mới nhất cần xử lý
            var recentTickets = await _context.PhieuDangKyHienMaus
                .Include(p => p.NguoiHienMau)
                .Include(p => p.DotHienMau)
                .OrderByDescending(p => p.NgayNop)
                .Take(6)
                .ToListAsync();

            ViewBag.RecentTickets = recentTickets;

            return View();
        }

        // ==========================================
        // MODULE 4: QUẢN LÝ PHIẾU ĐĂNG KÝ HIẾN MÁU
        // ==========================================

        // GET: /Admin/PhieuDangKyHienMau
        public async Task<IActionResult> PhieuDangKyHienMau(string? search, string? trangThai, int? maDot, int page = 1)
        {
            int pageSize = 10;
            if (page < 1) page = 1;

            var query = _context.PhieuDangKyHienMaus
                .Include(p => p.NguoiHienMau)
                .Include(p => p.DotHienMau!)
                    .ThenInclude(d => d.DonViToChuc)
                .Include(p => p.LichKhamSangLocs)
                .Include(p => p.KetQuaHienMau)
                .AsQueryable();

            // Thống kê nhanh
            ViewBag.TotalCount = await _context.PhieuDangKyHienMaus.CountAsync();
            ViewBag.ChoXacNhanCount = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Chờ xác nhận");
            ViewBag.DaXacNhanCount = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Đã xác nhận");
            ViewBag.ChoKhamCount = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Chờ khám sàng lọc");
            ViewBag.DaKhamCount = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Đã khám sàng lọc");
            ViewBag.TuChoiCount = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Từ chối");
            ViewBag.DaHuyCount = await _context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Đã hủy");

            // Bộ lọc từ khóa
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(p =>
                    (p.NguoiHienMau != null && p.NguoiHienMau.HoTen.ToLower().Contains(searchLower)) ||
                    (p.NguoiHienMau != null && p.NguoiHienMau.SoDienThoai != null && p.NguoiHienMau.SoDienThoai.Contains(searchLower)) ||
                    (p.NguoiHienMau != null && p.NguoiHienMau.Email != null && p.NguoiHienMau.Email.ToLower().Contains(searchLower)) ||
                    (p.DotHienMau != null && p.DotHienMau.TenDot.ToLower().Contains(searchLower)) ||
                    p.MaPhieu.ToString().Contains(searchLower));
            }

            // Bộ lọc trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "all")
            {
                query = query.Where(p => p.TrangThai == trangThai);
            }

            // Bộ lọc đợt hiến máu
            if (maDot.HasValue && maDot.Value > 0)
            {
                query = query.Where(p => p.MaDot == maDot.Value);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(p => p.NgayNop)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.CurrentMaDot = maDot;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.DotHienMaus = await _context.DotHienMaus.OrderByDescending(d => d.NgayBatDauDangKy).ToListAsync();

            return View(items);
        }

        // GET: /Admin/ChiTietPhieuDangKy/5
        public async Task<IActionResult> ChiTietPhieuDangKy(int id)
        {
            var phieu = await _context.PhieuDangKyHienMaus
                .Include(p => p.NguoiHienMau!)
                    .ThenInclude(n => n.TaiKhoan)
                .Include(p => p.DotHienMau!)
                    .ThenInclude(d => d.DonViToChuc)
                .Include(p => p.LichKhamSangLocs)
                .Include(p => p.KetQuaHienMau)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin phiếu đăng ký.";
                return RedirectToAction(nameof(PhieuDangKyHienMau));
            }

            // Lấy lịch sử các lần hiến trước của người này
            ViewBag.LichSuPhieuKhac = await _context.PhieuDangKyHienMaus
                .Include(p => p.DotHienMau)
                .Where(p => p.MaNguoiHien == phieu.MaNguoiHien && p.MaPhieu != phieu.MaPhieu)
                .OrderByDescending(p => p.NgayNop)
                .ToListAsync();

            return View(phieu);
        }

        // POST: /Admin/XacNhanPhieu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanPhieu(int id, string? ghiChu, string? returnUrl)
        {
            var phieu = await _context.PhieuDangKyHienMaus
                .Include(p => p.LichKhamSangLocs)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký.";
                return RedirectToAction(nameof(PhieuDangKyHienMau));
            }

            if (phieu.TrangThai != "Chờ xác nhận")
            {
                TempData["ErrorMessage"] = $"Không thể xác nhận phiếu đang ở trạng thái '{phieu.TrangThai}'. Chỉ có thể duyệt phiếu ở trạng thái 'Chờ xác nhận'.";
            }
            else
            {
                phieu.TrangThai = "Đã xác nhận";
                phieu.NgayXuLy = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(ghiChu))
                {
                    phieu.GhiChuXacNhan = ghiChu.Trim();
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã duyệt và xác nhận thành công phiếu đăng ký #DKHM-{phieu.MaPhieu:D5}.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(ChiTietPhieuDangKy), new { id = phieu.MaPhieu });
        }

        // POST: /Admin/TuChoiPhieu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoiPhieu(int id, string lyDoTuChoi, string? returnUrl)
        {
            var phieu = await _context.PhieuDangKyHienMaus
                .Include(p => p.LichKhamSangLocs)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký.";
                return RedirectToAction(nameof(PhieuDangKyHienMau));
            }

            if (string.IsNullOrWhiteSpace(lyDoTuChoi))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập lý do từ chối phiếu đăng ký.";
            }
            else if (phieu.TrangThai != "Chờ xác nhận")
            {
                TempData["ErrorMessage"] = $"Không thể từ chối phiếu đang ở trạng thái '{phieu.TrangThai}'. Chỉ có thể từ chối phiếu ở trạng thái 'Chờ xác nhận'.";
            }
            else
            {
                phieu.TrangThai = "Từ chối";
                phieu.NgayXuLy = DateTime.Now;
                phieu.GhiChuXacNhan = lyDoTuChoi.Trim();

                // Hủy các lịch khám sàng lọc đang chờ
                foreach (var lich in phieu.LichKhamSangLocs.Where(l => l.TrangThai != "Đã hoàn thành"))
                {
                    lich.TrangThai = "Đã hủy";
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã từ chối phiếu #DKHM-{phieu.MaPhieu:D5}. Lý do: {lyDoTuChoi}.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(ChiTietPhieuDangKy), new { id = phieu.MaPhieu });
        }

        // POST: /Admin/ChuyenChoKham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChuyenChoKham(int id, string? returnUrl)
        {
            var phieu = await _context.PhieuDangKyHienMaus
                .Include(p => p.LichKhamSangLocs)
                .FirstOrDefaultAsync(p => p.MaPhieu == id);

            if (phieu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký.";
                return RedirectToAction(nameof(PhieuDangKyHienMau));
            }

            if (phieu.TrangThai != "Đã xác nhận")
            {
                TempData["ErrorMessage"] = $"Không thể chuyển ca khám cho phiếu đang ở trạng thái '{phieu.TrangThai}'. Phiếu cần ở trạng thái 'Đã xác nhận'.";
            }
            else
            {
                phieu.TrangThai = "Chờ khám sàng lọc";
                phieu.NgayXuLy = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã chuyển phiếu #DKHM-{phieu.MaPhieu:D5} sang trạng thái 'Chờ khám sàng lọc'.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(ChiTietPhieuDangKy), new { id = phieu.MaPhieu });
        }

        // ==========================================
        // MODULE 4: QUẢN LÝ LỊCH KHÁM SÀNG LỌC
        // ==========================================

        // GET: /Admin/LichKhamSangLoc
        public async Task<IActionResult> LichKhamSangLoc(string? search, string? trangThai, DateTime? ngay, int? maDot, int page = 1)
        {
            int pageSize = 12;
            if (page < 1) page = 1;

            var query = _context.LichKhamSangLocs
                .Include(l => l.PhieuDangKyHienMau!)
                    .ThenInclude(p => p.NguoiHienMau)
                .Include(l => l.PhieuDangKyHienMau!)
                    .ThenInclude(p => p.DotHienMau!)
                        .ThenInclude(d => d.DonViToChuc)
                .AsQueryable();

            // Thống kê nhanh
            ViewBag.TotalLich = await _context.LichKhamSangLocs.CountAsync();
            ViewBag.DaLenLichCount = await _context.LichKhamSangLocs.CountAsync(l => l.TrangThai == "Đã lên lịch");
            ViewBag.DaHoanThanhCount = await _context.LichKhamSangLocs.CountAsync(l => l.TrangThai == "Đã hoàn thành");
            ViewBag.DaHuyCount = await _context.LichKhamSangLocs.CountAsync(l => l.TrangThai == "Đã hủy");

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
            ViewBag.DotHienMaus = await _context.DotHienMaus.OrderByDescending(d => d.NgayBatDauDangKy).ToListAsync();

            // Danh sách phiếu đang ở trạng thái hợp lệ để tạo lịch khám mới nếu cần
            ViewBag.PhieuSanSangTaoLich = await _context.PhieuDangKyHienMaus
                .Include(p => p.NguoiHienMau)
                .Include(p => p.DotHienMau)
                .Where(p => p.TrangThai == "Đã xác nhận" || p.TrangThai == "Chờ khám sàng lọc")
                .OrderByDescending(p => p.NgayNop)
                .Take(50)
                .ToListAsync();

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
            var phieu = await _context.PhieuDangKyHienMaus
                .Include(p => p.DotHienMau)
                .Include(p => p.LichKhamSangLocs)
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);

            if (phieu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký để tạo lịch khám.";
                return RedirectToAction(nameof(LichKhamSangLoc));
            }

            if (!TryParseDateTime(thoiGianBatDau, out DateTime dtBatDau) || !TryParseDateTime(thoiGianKetThuc, out DateTime dtKetThuc))
            {
                TempData["ErrorMessage"] = "Định dạng thời gian khám không hợp lệ.";
            }
            else if (phieu.TrangThai != "Đã xác nhận" && phieu.TrangThai != "Chờ khám sàng lọc")
            {
                TempData["ErrorMessage"] = $"Không thể thiết lập lịch khám cho phiếu đang ở trạng thái '{phieu.TrangThai}'. Phiếu phải ở trạng thái 'Đã xác nhận' hoặc 'Chờ khám sàng lọc'.";
            }
            else if (dtBatDau >= dtKetThuc)
            {
                TempData["ErrorMessage"] = "Thời gian bắt đầu ca khám phải trước thời gian kết thúc.";
            }
            else
            {
                var lichMoi = new LichKhamSangLoc
                {
                    MaPhieu = maPhieu,
                    ThoiGianBatDau = dtBatDau,
                    ThoiGianKetThuc = dtKetThuc,
                    HinhThucTiepNhan = !string.IsNullOrWhiteSpace(hinhThucTiepNhan) ? hinhThucTiepNhan.Trim() : "Trực tiếp tại điểm hiến",
                    DiaDiemHoacLienKet = !string.IsNullOrWhiteSpace(diaDiem) ? diaDiem.Trim() : (phieu.DotHienMau?.DonViToChuc?.TenDonViToChuc ?? "Điểm hiến máu UNETI"),
                    BacSiSangLoc = !string.IsNullOrWhiteSpace(bacSi) ? bacSi.Trim() : "BS. CKI Trần Mai Linh",
                    GhiChu = ghiChu,
                    TrangThai = "Đã lên lịch"
                };

                _context.LichKhamSangLocs.Add(lichMoi);

                if (phieu.TrangThai == "Đã xác nhận")
                {
                    phieu.TrangThai = "Chờ khám sàng lọc";
                    phieu.NgayXuLy = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã tạo lịch khám sàng lọc mới cho phiếu #DKHM-{phieu.MaPhieu:D5} thành công!";
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
            var lich = await _context.LichKhamSangLocs
                .Include(l => l.PhieuDangKyHienMau)
                .FirstOrDefaultAsync(l => l.MaLichKhamSangLoc == maLich);

            if (lich == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch khám sàng lọc.";
                return RedirectToAction(nameof(LichKhamSangLoc));
            }

            if (!TryParseDateTime(thoiGianBatDau, out DateTime dtBatDau) || !TryParseDateTime(thoiGianKetThuc, out DateTime dtKetThuc))
            {
                TempData["ErrorMessage"] = "Định dạng thời gian khám không hợp lệ.";
            }
            else if (lich.TrangThai != "Đã lên lịch")
            {
                TempData["ErrorMessage"] = $"Không thể chỉnh sửa lịch khám đang ở trạng thái '{lich.TrangThai}'.";
            }
            else if (dtBatDau >= dtKetThuc)
            {
                TempData["ErrorMessage"] = "Thời gian bắt đầu ca khám phải trước thời gian kết thúc.";
            }
            else
            {
                lich.ThoiGianBatDau = dtBatDau;
                lich.ThoiGianKetThuc = dtKetThuc;
                if (!string.IsNullOrWhiteSpace(hinhThucTiepNhan)) lich.HinhThucTiepNhan = hinhThucTiepNhan.Trim();
                if (!string.IsNullOrWhiteSpace(diaDiem)) lich.DiaDiemHoacLienKet = diaDiem.Trim();
                if (!string.IsNullOrWhiteSpace(bacSi)) lich.BacSiSangLoc = bacSi.Trim();
                lich.GhiChu = ghiChu;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật thông tin lịch khám #{lich.MaLichKhamSangLoc} thành công.";
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
            var lich = await _context.LichKhamSangLocs
                .Include(l => l.PhieuDangKyHienMau)
                .FirstOrDefaultAsync(l => l.MaLichKhamSangLoc == maLich);

            if (lich == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch khám sàng lọc.";
                return RedirectToAction(nameof(LichKhamSangLoc));
            }

            if (lich.TrangThai != "Đã lên lịch")
            {
                TempData["ErrorMessage"] = $"Không thể hủy lịch khám đang ở trạng thái '{lich.TrangThai}'.";
            }
            else
            {
                lich.TrangThai = "Đã hủy";
                if (!string.IsNullOrWhiteSpace(ghiChu))
                {
                    lich.GhiChu = string.IsNullOrEmpty(lich.GhiChu) ? $"[Đã hủy: {ghiChu.Trim()}]" : $"{lich.GhiChu} [Đã hủy: {ghiChu.Trim()}]";
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã hủy lịch khám #{lich.MaLichKhamSangLoc} thành công.";
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
            var lich = await _context.LichKhamSangLocs
                .Include(l => l.PhieuDangKyHienMau)
                .FirstOrDefaultAsync(l => l.MaLichKhamSangLoc == maLich);

            if (lich == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch khám sàng lọc.";
                return RedirectToAction(nameof(LichKhamSangLoc));
            }

            if (lich.TrangThai != "Đã lên lịch")
            {
                TempData["ErrorMessage"] = $"Chỉ có thể xác nhận hoàn thành cho lịch khám đang ở trạng thái 'Đã lên lịch'.";
            }
            else
            {
                lich.TrangThai = "Đã hoàn thành";
                if (lich.PhieuDangKyHienMau != null)
                {
                    lich.PhieuDangKyHienMau.TrangThai = "Đã khám sàng lọc";
                    lich.PhieuDangKyHienMau.NgayXuLy = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã hoàn thành ca khám #{lich.MaLichKhamSangLoc}. Trạng thái phiếu đăng ký đã chuyển sang 'Đã khám sàng lọc'.";
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
        public async Task<IActionResult> NguoiHienMau(
            string? search, 
            string? nhomMau, 
            string? rhGroup, 
            string? status, 
            string? tier, 
            int page = 1, 
            int pageSize = 10)
        {
            // 1. Tính toán các KPI Macro tổng quan từ database (bao quát mọi định dạng lưu trữ thực tế)
            var tongNguoiHien = await _context.NguoiHienMaus.CountAsync();
            var tongMauHiem = await _context.NguoiHienMaus.CountAsync(n => n.NhomMau != null && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));
            var oAm = await _context.NguoiHienMaus.CountAsync(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("O") && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));
            var aAm = await _context.NguoiHienMaus.CountAsync(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("A") && !n.NhomMau.ToUpper().Contains("AB") && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));
            var bAm = await _context.NguoiHienMaus.CountAsync(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("B") && !n.NhomMau.ToUpper().Contains("AB") && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));
            var abAm = await _context.NguoiHienMaus.CountAsync(n => n.NhomMau != null && n.NhomMau.ToUpper().Contains("AB") && (n.NhomMau.Contains("-") || n.NhomMau.Contains("Rh(-)") || n.NhomMau.Contains("Rh-") || n.NhomMau.ToLower().Contains("âm")));

            // 2. Query cơ sở với đầy đủ Navigation Properties
            var allDonorsQuery = _context.NguoiHienMaus
                .Include(n => n.TaiKhoan)
                .Include(n => n.PhieuDangKyHienMaus)
                    .ThenInclude(p => p.KetQuaHienMau)
                .AsQueryable();

            // Tính toán KPI người đủ điều kiện và tích cực từ dữ liệu đã nạp
            // Người đủ điều kiện: Hoạt động và (chưa từng hiến hoặc lần hiến thành công gần nhất >= 84 ngày)
            // Người hiến tích cực: Số lần hiến thành công >= 5
            var allDonorsForKpi = await allDonorsQuery.Select(n => new
            {
                n.MaNguoiHien,
                n.TrangThai,
                SoLanHien = n.PhieuDangKyHienMaus.Count(p => p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null),
                LanHienCuoi = n.PhieuDangKyHienMaus
                    .Where(p => p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null)
                    .OrderByDescending(p => p.NgayXuLy ?? p.NgayNop)
                    .Select(p => p.NgayXuLy ?? p.NgayNop)
                    .FirstOrDefault()
            }).ToListAsync();

            var now = DateTime.Now;
            var duDieuKienCount = allDonorsForKpi.Count(d => (d.TrangThai == "Hoạt động" || string.IsNullOrEmpty(d.TrangThai)) && (d.LanHienCuoi == default || (now - d.LanHienCuoi).TotalDays >= 84));
            var hienTichCucCount = allDonorsForKpi.Count(d => d.SoLanHien >= 5);

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

            // Lọc theo phân hạng cống hiến nếu có
            if (!string.IsNullOrWhiteSpace(tier) && tier != "all")
            {
                if (tier == "diamond")
                {
                    donorList = donorList.Where(d => d.PhieuDangKyHienMaus.Count(p => p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null) >= 20).ToList();
                }
                else if (tier == "gold")
                {
                    donorList = donorList.Where(d => {
                        var c = d.PhieuDangKyHienMaus.Count(p => p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null);
                        return c >= 10 && c < 20;
                    }).ToList();
                }
                else if (tier == "silver")
                {
                    donorList = donorList.Where(d => {
                        var c = d.PhieuDangKyHienMaus.Count(p => p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null);
                        return c >= 5 && c < 10;
                    }).ToList();
                }
                else if (tier == "bronze")
                {
                    donorList = donorList.Where(d => {
                        var c = d.PhieuDangKyHienMaus.Count(p => p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null);
                        return c >= 1 && c < 5;
                    }).ToList();
                }
            }

            // Lọc theo điều kiện sẵn sàng hiến ngay nếu status == "dudieukien"
            if (status == "dudieukien")
            {
                donorList = donorList.Where(d => {
                    var lastSuccess = d.PhieuDangKyHienMaus
                        .Where(p => p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null)
                        .OrderByDescending(p => p.NgayXuLy ?? p.NgayNop)
                        .Select(p => p.NgayXuLy ?? p.NgayNop)
                        .FirstOrDefault();
                    return (d.TrangThai == "Hoạt động" || string.IsNullOrEmpty(d.TrangThai)) &&
                           (lastSuccess == default || (now - lastSuccess).TotalDays >= 84);
                }).ToList();
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

        // GET: /Admin/DonViToChuc
        public async Task<IActionResult> DonViToChuc(string? search, string? status)
        {
            var query = _context.DonViToChucs
                .Include(d => d.DotHienMaus)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(d => d.TenDonViToChuc.ToLower().Contains(searchLower) ||
                                         (d.EmailLienHe != null && d.EmailLienHe.ToLower().Contains(searchLower)) ||
                                         (d.MoTa != null && d.MoTa.ToLower().Contains(searchLower)));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                query = query.Where(d => d.TrangThai == status);
            }

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;

            var donViList = await query.OrderByDescending(d => d.MaDonViToChuc).ToListAsync();
            return View(donViList);
        }

        // POST: /Admin/CreateDonViToChuc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDonViToChuc([Bind("TenDonViToChuc,MoTa,EmailLienHe,TrangThai")] DonViToChuc donVi)
        {
            // Kiểm tra tên bắt buộc
            if (string.IsNullOrWhiteSpace(donVi.TenDonViToChuc))
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức không được để trống.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            // Kiểm tra định dạng email nếu có nhập
            if (!string.IsNullOrWhiteSpace(donVi.EmailLienHe))
            {
                var emailPattern = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailPattern.IsMatch(donVi.EmailLienHe.Trim()))
                {
                    TempData["ErrorMessage"] = "Địa chỉ email liên hệ không đúng định dạng.";
                    return RedirectToAction(nameof(DonViToChuc));
                }
            }

            // Kiểm tra trùng tên đơn vị tổ chức
            var tenTrimmed = donVi.TenDonViToChuc.Trim();
            var isDuplicate = await _context.DonViToChucs
                .AnyAsync(d => d.TenDonViToChuc.ToLower() == tenTrimmed.ToLower());
            if (isDuplicate)
            {
                TempData["ErrorMessage"] = $"Tên đơn vị \"{tenTrimmed}\" đã tồn tại. Vui lòng chọn tên khác.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            donVi.TenDonViToChuc = tenTrimmed;

            if (ModelState.IsValid)
            {
                _context.DonViToChucs.Add(donVi);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm đơn vị tổ chức thành công!";
            }
            return RedirectToAction(nameof(DonViToChuc));
        }

        // POST: /Admin/EditDonViToChuc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDonViToChuc(int id, [Bind("MaDonViToChuc,TenDonViToChuc,MoTa,EmailLienHe,TrangThai")] DonViToChuc donVi)
        {
            if (id != donVi.MaDonViToChuc)
            {
                return NotFound();
            }

            // Kiểm tra tên bắt buộc
            if (string.IsNullOrWhiteSpace(donVi.TenDonViToChuc))
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức không được để trống.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            // Kiểm tra định dạng email nếu có nhập
            if (!string.IsNullOrWhiteSpace(donVi.EmailLienHe))
            {
                var emailPattern = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailPattern.IsMatch(donVi.EmailLienHe.Trim()))
                {
                    TempData["ErrorMessage"] = "Địa chỉ email liên hệ không đúng định dạng.";
                    return RedirectToAction(nameof(DonViToChuc));
                }
            }

            // Kiểm tra trùng tên đơn vị tổ chức (loại trừ chính nó)
            var tenTrimmed = donVi.TenDonViToChuc.Trim();
            var isDuplicate = await _context.DonViToChucs
                .AnyAsync(d => d.TenDonViToChuc.ToLower() == tenTrimmed.ToLower()
                            && d.MaDonViToChuc != donVi.MaDonViToChuc);
            if (isDuplicate)
            {
                TempData["ErrorMessage"] = $"Tên đơn vị \"{tenTrimmed}\" đã tồn tại. Vui lòng chọn tên khác.";
                return RedirectToAction(nameof(DonViToChuc));
            }

            donVi.TenDonViToChuc = tenTrimmed;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donVi);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật đơn vị tổ chức thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DonViToChucs.Any(e => e.MaDonViToChuc == donVi.MaDonViToChuc))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
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

            var tenLower = ten.Trim().ToLower();
            bool isDuplicate;

            if (excludeId.HasValue)
            {
                isDuplicate = await _context.DonViToChucs
                    .AnyAsync(d => d.TenDonViToChuc.ToLower() == tenLower && d.MaDonViToChuc != excludeId.Value);
            }
            else
            {
                isDuplicate = await _context.DonViToChucs
                    .AnyAsync(d => d.TenDonViToChuc.ToLower() == tenLower);
            }

            return Json(new { isDuplicate });
        }

        // POST: /Admin/DeleteDonViToChuc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDonViToChuc(int id)
        {
            var donVi = await _context.DonViToChucs
                .Include(d => d.DotHienMaus)
                .FirstOrDefaultAsync(d => d.MaDonViToChuc == id);

            if (donVi != null)
            {
                if (donVi.DotHienMaus.Any())
                {
                    TempData["ErrorMessage"] = $"Không thể xóa đơn vị \"{donVi.TenDonViToChuc}\" vì đang có {donVi.DotHienMaus.Count} đợt hiến máu liên kết. Việc xóa sẽ làm các đợt hiến máu này không còn hợp lệ.";
                }
                else
                {
                    _context.DonViToChucs.Remove(donVi);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã xóa đơn vị tổ chức thành công!";
                }
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

            var query = _context.KetQuaHienMaus
                .Include(k => k.PhieuDangKyHienMau!)
                    .ThenInclude(p => p.NguoiHienMau)
                .Include(k => k.PhieuDangKyHienMau!)
                    .ThenInclude(p => p.DotHienMau!)
                        .ThenInclude(d => d.DonViToChuc)
                .Include(k => k.PhieuDangKyHienMau!)
                    .ThenInclude(p => p.LichKhamSangLocs)
                .AsQueryable();

            // Thống kê nhanh toàn bộ kết quả
            ViewBag.TotalCount = await _context.KetQuaHienMaus.CountAsync();
            ViewBag.SuccessCount = await _context.KetQuaHienMaus.CountAsync(k => k.KetQua == "Hiến máu thành công");
            ViewBag.RejectCount = await _context.KetQuaHienMaus.CountAsync(k => k.KetQua == "Không đủ điều kiện hiến");

            // Danh sách phiếu "Đã khám sàng lọc" chưa có kết quả (để nhân viên y tế nhập kết quả)
            ViewBag.PendingResults = await _context.PhieuDangKyHienMaus
                .Include(p => p.NguoiHienMau)
                .Include(p => p.DotHienMau)
                .Where(p => p.TrangThai == "Đã khám sàng lọc" && p.KetQuaHienMau == null)
                .OrderByDescending(p => p.NgayXuLy ?? p.NgayNop)
                .Take(10)
                .ToListAsync();

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
            ViewBag.DotList = await _context.DotHienMaus.OrderByDescending(d => d.NgayBatDauDangKy).ToListAsync();

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
            var ketQua = await _context.KetQuaHienMaus
                .Include(k => k.PhieuDangKyHienMau!)
                    .ThenInclude(p => p.NguoiHienMau!)
                        .ThenInclude(n => n.TaiKhoan)
                .Include(k => k.PhieuDangKyHienMau!)
                    .ThenInclude(p => p.DotHienMau!)
                        .ThenInclude(d => d.DonViToChuc)
                .Include(k => k.PhieuDangKyHienMau!)
                    .ThenInclude(p => p.LichKhamSangLocs)
                .FirstOrDefaultAsync(k => k.MaKetQuaHienMau == id);

            if (ketQua == null)
            {
                return NotFound("Không tìm thấy kết quả hiến máu.");
            }

            return View(ketQua);
        }

        // POST: /Admin/TaoKetQua
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoKetQua(
            int maPhieu,
            string ketQua,
            double? diemDanhGia,
            string? nhanXet,
            string? returnUrl)
        {
            var phieu = await _context.PhieuDangKyHienMaus
                .Include(p => p.KetQuaHienMau)
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);

            if (phieu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký để nhập kết quả.";
                return RedirectToAction(nameof(KetQuaHienMau));
            }

            if (phieu.TrangThai != "Đã khám sàng lọc")
            {
                TempData["ErrorMessage"] = $"Chỉ có thể nhập kết quả cho phiếu đang ở trạng thái 'Đã khám sàng lọc'. Phiếu hiện tại đang ở trạng thái '{phieu.TrangThai}'.";
            }
            else if (phieu.KetQuaHienMau != null)
            {
                TempData["ErrorMessage"] = "Phiếu này đã có kết quả hiến máu. Vui lòng sử dụng chức năng cập nhật kết quả.";
            }
            else if (string.IsNullOrWhiteSpace(ketQua) || (ketQua != "Hiến máu thành công" && ketQua != "Không đủ điều kiện hiến"))
            {
                TempData["ErrorMessage"] = "Kết quả hiến máu không hợp lệ. Chỉ chấp nhận 'Hiến máu thành công' hoặc 'Không đủ điều kiện hiến'.";
            }
            else if (diemDanhGia.HasValue && (diemDanhGia.Value < 0.0 || diemDanhGia.Value > 10.0))
            {
                TempData["ErrorMessage"] = "Điểm đánh giá phải nằm trong khoảng từ 0 đến 10.";
            }
            else
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
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

                    _context.KetQuaHienMaus.Add(ketQuaMoi);

                    phieu.TrangThai = ketQua.Trim();
                    phieu.NgayXuLy = DateTime.Now;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"Đã ghi nhận kết quả hiến máu '{ketQua.Trim()}' cho phiếu #DKHM-{phieu.MaPhieu:D5} thành công!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Lỗi khi lưu kết quả hiến máu: {ex.Message}";
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(KetQuaHienMau));
        }

        // POST: /Admin/SuaKetQua
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaKetQua(
            int id,
            string ketQua,
            double? diemDanhGia,
            string? nhanXet,
            string? returnUrl)
        {
            var ketQuaRecord = await _context.KetQuaHienMaus
                .Include(k => k.PhieuDangKyHienMau)
                .FirstOrDefaultAsync(k => k.MaKetQuaHienMau == id);

            if (ketQuaRecord == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bản ghi kết quả hiến máu.";
                return RedirectToAction(nameof(KetQuaHienMau));
            }

            if (ketQuaRecord.PhieuDangKyHienMau != null &&
                (ketQuaRecord.PhieuDangKyHienMau.TrangThai == "Từ chối" || ketQuaRecord.PhieuDangKyHienMau.TrangThai == "Đã hủy"))
            {
                TempData["ErrorMessage"] = $"Không thể chỉnh sửa kết quả của phiếu đã ở trạng thái '{ketQuaRecord.PhieuDangKyHienMau.TrangThai}'.";
            }
            else if (string.IsNullOrWhiteSpace(ketQua) || (ketQua != "Hiến máu thành công" && ketQua != "Không đủ điều kiện hiến"))
            {
                TempData["ErrorMessage"] = "Kết quả hiến máu không hợp lệ. Chỉ chấp nhận 'Hiến máu thành công' hoặc 'Không đủ điều kiện hiến'.";
            }
            else if (diemDanhGia.HasValue && (diemDanhGia.Value < 0.0 || diemDanhGia.Value > 10.0))
            {
                TempData["ErrorMessage"] = "Điểm đánh giá phải nằm trong khoảng từ 0 đến 10.";
            }
            else
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    ketQuaRecord.KetQua = ketQua.Trim();
                    ketQuaRecord.DiemDanhGia = diemDanhGia;
                    ketQuaRecord.NhanXet = nhanXet?.Trim();
                    ketQuaRecord.NgayCapNhat = DateTime.Now;

                    if (ketQuaRecord.PhieuDangKyHienMau != null)
                    {
                        ketQuaRecord.PhieuDangKyHienMau.TrangThai = ketQua.Trim();
                        ketQuaRecord.PhieuDangKyHienMau.NgayXuLy = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"Đã cập nhật kết quả hiến máu #{ketQuaRecord.MaKetQuaHienMau} thành công!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật kết quả: {ex.Message}";
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(ChiTietKetQua), new { id = ketQuaRecord.MaKetQuaHienMau });
        }

        public IActionResult BaoCao()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult NhanSu()
        {
            return View();
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
    }
}
