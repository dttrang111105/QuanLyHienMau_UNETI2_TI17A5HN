// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Module 1 - Controller Quản lý Đơn vị tổ chức
// Triển khai Repository Pattern: Controller xử lý Request/Response, Repository làm việc với dữ liệu

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers
{
    [Authorize(Roles = "Admin,Medical Staff")]
    public class DonViToChucController : Controller
    {
        private readonly IDonViToChucRepository _donViRepository;
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly HashSet<string> AllowedStatuses = new(StringComparer.Ordinal) { "Hoạt động", "Tạm dừng" };

        public DonViToChucController(IDonViToChucRepository donViRepository)
        {
            _donViRepository = donViRepository;
        }

        // GET: /DonViToChuc
        public async Task<IActionResult> Index(string? search, string? status)
        {
            var donViList = await _donViRepository.GetAllWithDotHienMauAsync(search, status);
            var stats = await _donViRepository.GetDonViStatsAsync();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
            ViewBag.TotalDonViCount = stats.TotalCount;
            ViewBag.ActiveDonViCount = stats.ActiveCount;
            ViewBag.InactiveDonViCount = stats.InactiveCount;
            ViewBag.TotalDotCount = stats.TotalDotCount;

            return View("~/Views/Admin/DonViToChuc.cshtml", donViList);
        }

        // GET: /DonViToChuc/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var donVi = await _donViRepository.GetByIdWithDotHienMauAsync(id);
            if (donVi == null)
            {
                return NotFound("Không tìm thấy đơn vị tổ chức.");
            }

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new
                {
                    donVi.MaDonViToChuc,
                    donVi.TenDonViToChuc,
                    donVi.MoTa,
                    donVi.EmailLienHe,
                    donVi.TrangThai,
                    SoDotHienMau = donVi.DotHienMaus?.Count ?? 0
                });
            }

            return RedirectToAction("Index", "DotHienMau", new { donViId = id });
        }

        // POST: /DonViToChuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("TenDonViToChuc,MoTa,EmailLienHe,TrangThai")] DonViToChuc donVi)
        {
            if (string.IsNullOrWhiteSpace(donVi.TenDonViToChuc))
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức không được để trống.";
                return RedirectToAction(nameof(Index));
            }

            var tenTrimmed = donVi.TenDonViToChuc.Trim();
            if (tenTrimmed.Length > 200)
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức không được vượt quá 200 ký tự.";
                return RedirectToAction(nameof(Index));
            }

            string? cleanEmail = null;
            if (!string.IsNullOrWhiteSpace(donVi.EmailLienHe))
            {
                cleanEmail = donVi.EmailLienHe.Trim();
                if (cleanEmail.Length > 100 || !EmailRegex.IsMatch(cleanEmail))
                {
                    TempData["ErrorMessage"] = "Địa chỉ email liên hệ không đúng định dạng.";
                    return RedirectToAction(nameof(Index));
                }
            }

            if (!string.IsNullOrWhiteSpace(donVi.MoTa) && donVi.MoTa.Trim().Length > 500)
            {
                TempData["ErrorMessage"] = "Mô tả đơn vị tổ chức không được vượt quá 500 ký tự.";
                return RedirectToAction(nameof(Index));
            }

            if (await _donViRepository.IsTenDonViExistsAsync(tenTrimmed))
            {
                TempData["ErrorMessage"] = $"Tên đơn vị \"{tenTrimmed}\" đã tồn tại. Vui lòng chọn tên khác.";
                return RedirectToAction(nameof(Index));
            }

            donVi.TenDonViToChuc = tenTrimmed;
            donVi.EmailLienHe = cleanEmail;
            donVi.MoTa = donVi.MoTa?.Trim();
            donVi.TrangThai = (!string.IsNullOrWhiteSpace(donVi.TrangThai) && AllowedStatuses.Contains(donVi.TrangThai.Trim()))
                ? donVi.TrangThai.Trim()
                : "Hoạt động";

            await _donViRepository.AddAsync(donVi);
            await _donViRepository.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thêm mới đơn vị tổ chức thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /DonViToChuc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("MaDonViToChuc,TenDonViToChuc,MoTa,EmailLienHe,TrangThai")] DonViToChuc donVi)
        {
            if (id != donVi.MaDonViToChuc)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(donVi.TenDonViToChuc))
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức không được để trống.";
                return RedirectToAction(nameof(Index));
            }

            var tenTrimmed = donVi.TenDonViToChuc.Trim();
            if (tenTrimmed.Length > 200)
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức không được vượt quá 200 ký tự.";
                return RedirectToAction(nameof(Index));
            }

            string? cleanEmail = null;
            if (!string.IsNullOrWhiteSpace(donVi.EmailLienHe))
            {
                cleanEmail = donVi.EmailLienHe.Trim();
                if (cleanEmail.Length > 100 || !EmailRegex.IsMatch(cleanEmail))
                {
                    TempData["ErrorMessage"] = "Địa chỉ email liên hệ không đúng định dạng.";
                    return RedirectToAction(nameof(Index));
                }
            }

            if (!string.IsNullOrWhiteSpace(donVi.MoTa) && donVi.MoTa.Trim().Length > 500)
            {
                TempData["ErrorMessage"] = "Mô tả đơn vị tổ chức không được vượt quá 500 ký tự.";
                return RedirectToAction(nameof(Index));
            }

            if (await _donViRepository.IsTenDonViExistsAsync(tenTrimmed, donVi.MaDonViToChuc))
            {
                TempData["ErrorMessage"] = $"Tên đơn vị \"{tenTrimmed}\" đã tồn tại. Vui lòng chọn tên khác.";
                return RedirectToAction(nameof(Index));
            }

            var existing = await _donViRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.TenDonViToChuc = tenTrimmed;
            existing.MoTa = donVi.MoTa?.Trim();
            existing.EmailLienHe = cleanEmail;
            existing.TrangThai = (!string.IsNullOrWhiteSpace(donVi.TrangThai) && AllowedStatuses.Contains(donVi.TrangThai.Trim()))
                ? donVi.TrangThai.Trim()
                : "Hoạt động";

            _donViRepository.Update(existing);
            await _donViRepository.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật đơn vị tổ chức thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /DonViToChuc/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var donVi = await _donViRepository.GetByIdAsync(id);
            if (donVi == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn vị tổ chức cần xóa.";
                return RedirectToAction(nameof(Index));
            }

            if (await _donViRepository.HasDotHienMauAsync(id))
            {
                TempData["ErrorMessage"] = $"Không thể xóa đơn vị \"{donVi.TenDonViToChuc}\" vì đang có đợt hiến máu liên kết. Việc xóa sẽ làm các đợt hiến máu này không còn hợp lệ.";
            }
            else
            {
                _donViRepository.Delete(donVi);
                await _donViRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa đơn vị tổ chức thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /DonViToChuc/CheckDuplicate
        [HttpGet]
        public async Task<IActionResult> CheckDuplicate(string ten, int? excludeId)
        {
            if (string.IsNullOrWhiteSpace(ten))
            {
                return Json(new { isDuplicate = false });
            }

            var isDuplicate = await _donViRepository.IsTenDonViExistsAsync(ten.Trim(), excludeId);
            return Json(new { isDuplicate });
        }
    }
}
