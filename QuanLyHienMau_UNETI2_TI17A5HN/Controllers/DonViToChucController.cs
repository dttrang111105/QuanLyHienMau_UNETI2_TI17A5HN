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

        public DonViToChucController(IDonViToChucRepository donViRepository)
        {
            _donViRepository = donViRepository;
        }

        // GET: /DonViToChuc
        public async Task<IActionResult> Index(string? search, string? status)
        {
            var donViList = await _donViRepository.GetAllWithDotHienMauAsync(search, status);
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
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
            return View(donVi);
        }

        // POST: /DonViToChuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("TenDonViToChuc,MoTa,EmailLienHe,TrangThai")] DonViToChuc donVi)
        {
            // 1. Kiểm tra tên bắt buộc
            if (string.IsNullOrWhiteSpace(donVi.TenDonViToChuc))
            {
                TempData["ErrorMessage"] = "Tên đơn vị tổ chức không được để trống.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Kiểm tra định dạng email nếu có
            if (!string.IsNullOrWhiteSpace(donVi.EmailLienHe))
            {
                if (!EmailRegex.IsMatch(donVi.EmailLienHe.Trim()))
                {
                    TempData["ErrorMessage"] = "Địa chỉ email liên hệ không đúng định dạng.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // 3. Kiểm tra trùng tên đơn vị thông qua Repository
            var tenTrimmed = donVi.TenDonViToChuc.Trim();
            var isDuplicate = await _donViRepository.IsTenDonViExistsAsync(tenTrimmed);
            if (isDuplicate)
            {
                TempData["ErrorMessage"] = $"Tên đơn vị \"{tenTrimmed}\" đã tồn tại. Vui lòng chọn tên khác.";
                return RedirectToAction(nameof(Index));
            }

            donVi.TenDonViToChuc = tenTrimmed;
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

            if (!string.IsNullOrWhiteSpace(donVi.EmailLienHe))
            {
                if (!EmailRegex.IsMatch(donVi.EmailLienHe.Trim()))
                {
                    TempData["ErrorMessage"] = "Địa chỉ email liên hệ không đúng định dạng.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var tenTrimmed = donVi.TenDonViToChuc.Trim();
            var isDuplicate = await _donViRepository.IsTenDonViExistsAsync(tenTrimmed, donVi.MaDonViToChuc);
            if (isDuplicate)
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
            existing.MoTa = donVi.MoTa;
            existing.EmailLienHe = donVi.EmailLienHe?.Trim();
            existing.TrangThai = donVi.TrangThai;

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
            var hasDot = await _donViRepository.HasDotHienMauAsync(id);
            var donVi = await _donViRepository.GetByIdAsync(id);

            if (donVi != null)
            {
                if (hasDot)
                {
                    TempData["ErrorMessage"] = $"Không thể xóa đơn vị \"{donVi.TenDonViToChuc}\" vì đang có đợt hiến máu liên kết. Không được xóa đơn vị nếu làm các đợt hiến máu không còn hợp lệ.";
                }
                else
                {
                    _donViRepository.Delete(donVi);
                    await _donViRepository.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã xóa đơn vị tổ chức thành công!";
                }
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
