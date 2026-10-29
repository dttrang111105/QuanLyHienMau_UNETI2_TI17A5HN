// Họ và tên: NGUYỄN THỊ PHƯƠNG THẢO
// Mã sinh viên: 23103100272
// Nội dung thực hiện: Quản lý đợt hiến máu,
// tìm kiếm, lọc, sắp xếp và phân trang.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    /// <summary>
    /// Controller Module 2 – Quản lý Đợt Hiến Máu.
    /// Sử dụng Repository Pattern: Controller không trực tiếp gọi DbContext,
    /// mà thông qua IDotHienMauRepository (Dependency Injection).
    /// </summary>
    public class DotHienMauController : Controller
    {
        private readonly IDotHienMauRepository _repo;
        private const int PageSize = 6;

        public DotHienMauController(IDotHienMauRepository repo)
        {
            _repo = repo;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  GET: /DotHienMau
        //  Danh sách + Tìm kiếm + Lọc + Sắp xếp + Phân trang
        //  Thứ tự xử lý: Truy vấn → Tìm kiếm → Lọc → Sắp xếp → Phân trang → Hiển thị
        // ════════════════════════════════════════════════════════════════════════
        public async Task<IActionResult> Index(
            string? search,
            string? status,
            int?    donViId,
            string? nhomMau,
            double? canNang,
            string? thoiHan,
            string? sort,
            int     page = 1)
        {
            // 1. Lấy truy vấn gốc (IQueryable, chưa thực thi SQL)
            var query = _repo.GetQueryable();

            // 2. Tìm kiếm theo tên đợt hoặc tên đơn vị tổ chức
            query = _repo.ApplySearch(query, search);

            // 3. Lọc theo nhiều điều kiện kết hợp
            query = _repo.ApplyFilterDonVi(query, donViId);
            query = _repo.ApplyFilterNhomMau(query, nhomMau);
            query = _repo.ApplyFilterTrangThai(query, status);
            query = _repo.ApplyFilterCanNang(query, canNang);
            query = _repo.ApplyFilterThoiHan(query, thoiHan);

            // 4. Sắp xếp
            query = _repo.ApplySort(query, sort);

            // 5. Đếm tổng số bản ghi (SELECT COUNT(*) — không tải toàn bộ data)
            var totalItems = await _repo.CountAsync(query);
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Đảm bảo page hợp lệ
            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            // 6. Lấy dữ liệu phân trang (Skip + Take trên EF Core LINQ)
            var result = await _repo.GetPagedAsync(query, page, PageSize);

            // 7. Load dropdown đơn vị tổ chức cho bộ lọc
            ViewBag.DonViList = await _repo.GetActiveDonViToChucsAsync();

            // 8. Giữ lại tham số tìm kiếm/lọc/sắp xếp khi chuyển trang
            ViewBag.CurrentSearch  = search;
            ViewBag.CurrentStatus  = status;
            ViewBag.CurrentDonViId = donViId;
            ViewBag.CurrentNhomMau = nhomMau;
            ViewBag.CurrentCanNang = canNang;
            ViewBag.CurrentThoiHan = thoiHan;
            ViewBag.CurrentSort    = sort;

            // 9. Thông tin phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = totalPages;
            ViewBag.TotalItems  = totalItems;

            return View(result);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  GET: /DotHienMau/Details/5
        // ════════════════════════════════════════════════════════════════════════
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var dotHienMau = await _repo.GetByIdAsync(id.Value);

            if (dotHienMau == null)
                return NotFound();

            return View(dotHienMau);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  GET: /DotHienMau/Create
        // ════════════════════════════════════════════════════════════════════════
        [Authorize(Roles = "Admin,Medical Staff")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Dropdown đơn vị tổ chức lấy từ DB qua Repository
            ViewBag.DonViList = await _repo.GetActiveDonViToChucsAsync();
            return View();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  POST: /DotHienMau/Create
        // ════════════════════════════════════════════════════════════════════════
        [Authorize(Roles = "Admin,Medical Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DotHienMau model)
        {
            // Trạng thái do hệ thống tự đặt khi tạo mới
            model.TrangThai = "Chưa mở";

            // Xóa lỗi binding cũ cho trường TrangThai (không nhập từ form)
            ModelState.Remove(nameof(model.TrangThai));

            // Validation nghiệp vụ: hạn đăng ký phải >= ngày bắt đầu
            if (model.HanDangKy < model.NgayBatDauDangKy)
            {
                ModelState.AddModelError(
                    nameof(model.HanDangKy),
                    "Hạn đăng ký phải sau hoặc bằng ngày bắt đầu đăng ký.");
            }

            if (ModelState.IsValid)
            {
                await _repo.AddAsync(model);
                await _repo.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm đợt hiến máu thành công.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi thì nạp lại dropdown
            ViewBag.DonViList = await _repo.GetActiveDonViToChucsAsync();
            return View(model);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  GET: /DotHienMau/Edit/5
        // ════════════════════════════════════════════════════════════════════════
        [Authorize(Roles = "Admin,Medical Staff")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dot = await _repo.GetByIdAsync(id);

            if (dot == null)
                return NotFound();

            ViewBag.DonViList = await _repo.GetActiveDonViToChucsAsync();
            return View(dot);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  POST: /DotHienMau/Edit/5
        // ════════════════════════════════════════════════════════════════════════
        [Authorize(Roles = "Admin,Medical Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DotHienMau model)
        {
            if (id != model.MaDot)
                return NotFound();

            // Validation nghiệp vụ
            if (model.HanDangKy < model.NgayBatDauDangKy)
            {
                ModelState.AddModelError(
                    nameof(model.HanDangKy),
                    "Hạn đăng ký phải sau hoặc bằng ngày bắt đầu đăng ký.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DonViList = await _repo.GetActiveDonViToChucsAsync();
                return View(model);
            }

            // Lấy entity đang được EF Core track để cập nhật an toàn
            var dot = await _repo.GetByIdAsync(id);

            if (dot == null)
                return NotFound();

            // Chỉ cập nhật các trường được phép; TrangThai giữ nguyên
            dot.TenDot              = model.TenDot;
            dot.MaDonViToChuc       = model.MaDonViToChuc;
            dot.ChiTieuDonViMau     = model.ChiTieuDonViMau;
            dot.NhomMauUuTien       = model.NhomMauUuTien;
            dot.CanNangToiThieu     = model.CanNangToiThieu;
            dot.NgayBatDauDangKy    = model.NgayBatDauDangKy;
            dot.HanDangKy           = model.HanDangKy;
            dot.MoTaDot             = model.MoTaDot;
            dot.DieuKienNguoiHien   = model.DieuKienNguoiHien;

            await _repo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật đợt hiến máu thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════════════════════════════════
        //  POST: /DotHienMau/ChangeStatus
        //  Cập nhật trạng thái đợt hiến máu theo luồng hợp lệ
        // ════════════════════════════════════════════════════════════════════════
        [Authorize(Roles = "Admin,Medical Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string trangThai)
        {
            var dot = await _repo.GetByIdAsync(id);

            if (dot == null)
                return NotFound();

            // Chỉ cho phép các trạng thái hợp lệ
            var validStates = new[] { "Chưa mở", "Đang mở đăng ký", "Tạm dừng", "Đã đóng" };

            if (!validStates.Contains(trangThai))
                return BadRequest();

            // Kiểm tra nghiệp vụ khi chuyển sang "Đang mở đăng ký"
            if (trangThai == "Đang mở đăng ký")
            {
                if (string.IsNullOrWhiteSpace(dot.TenDot)   ||
                    dot.ChiTieuDonViMau <= 0                 ||
                    dot.CanNangToiThieu < 0                  ||
                    dot.HanDangKy < dot.NgayBatDauDangKy)
                {
                    TempData["ErrorMessage"] =
                        "Dữ liệu chưa hợp lệ nên không thể mở đăng ký.";
                    return RedirectToAction(nameof(Index));
                }
            }

            dot.TrangThai = trangThai;
            await _repo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ════════════════════════════════════════════════════════════════════════
        //  POST: /DotHienMau/Delete/5
        //  Xóa đợt hiến máu — kiểm tra phiếu đăng ký trước khi xóa
        //  để không làm mất dữ liệu lịch sử
        // ════════════════════════════════════════════════════════════════════════
        [Authorize(Roles = "Admin,Medical Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var dot = await _repo.GetByIdAsync(id);

            if (dot == null)
                return NotFound();

            // Kiểm tra bằng LINQ Any() — không xóa nếu đã có phiếu đăng ký
            var hasPhieu = await _repo.HasPhieuDangKyAsync(id);

            if (hasPhieu)
            {
                TempData["ErrorMessage"] =
                    "Không thể xóa đợt hiến máu vì đã có phiếu đăng ký.";
                return RedirectToAction(nameof(Index));
            }

            await _repo.DeleteAsync(dot);
            await _repo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa đợt hiến máu thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
