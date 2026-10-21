using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;
[Authorize(Roles = "Admin,NhanVienYTe")]
[Microsoft.AspNetCore.Mvc.ServiceFilter(typeof(QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure.Module5DatabaseExceptionFilter))]
public class LichKhamSangLocController(ApplicationDbContext db, Module5Service service, Module5Pages pages) : Controller
{
    public IActionResult Index(string? q, string? trangThai, DateTime? tuNgay, int page = 1, string view = "calendar", DateTime? ngay = null) => RedirectToAction("LichKhamSangLoc", "Admin", new { q, trangThai, tuNgay, page, view, ngay });
    [HttpGet]
    public IActionResult Create() => RedirectToAction("LichKhamSangLoc", "Admin", new { newSlot = true });

    [HttpGet]
    public IActionResult Edit(int id) => RedirectToAction("LichKhamSangLoc", "Admin", new { editId = id });

    [HttpGet]
    public async Task<IActionResult> Export(string? q, string? trangThai, int? maDot, string? bacSi, DateTime? ngay)
    {
        var query = db.LichKhamSangLocs.AsNoTracking().Include(x => x.Phieu).ThenInclude(x => x.NguoiHien).Include(x => x.Phieu).ThenInclude(x => x.Dot).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.BacSiSangLoc.Contains(q) || x.Phieu.NguoiHien.HoTen.Contains(q) || x.Phieu.Dot.TenDot.Contains(q));
        if (!string.IsNullOrWhiteSpace(trangThai)) query = query.Where(x => x.TrangThai == trangThai);
        if (maDot.HasValue) query = query.Where(x => x.Phieu.MaDot == maDot);
        if (!string.IsNullOrWhiteSpace(bacSi)) { var key = bacSi.Trim().Normalize(System.Text.NormalizationForm.FormC).ToUpperInvariant(); query = query.Where(x => x.BacSiChuanHoa == key); }
        if (ngay.HasValue) { var date = ngay.Value.Date; var end = date.AddDays(1); query = query.Where(x => x.ThoiGianBatDau < end && x.ThoiGianKetThuc > date); }
        var data = await query.OrderBy(x => x.ThoiGianBatDau).ToListAsync();
        var rows = new List<string[]> { new[] { "Mã lịch", "Mã phiếu", "Người hiến", "Đợt", "Bắt đầu", "Kết thúc", "Bác sĩ", "Địa điểm", "Trạng thái" } };
        rows.AddRange(data.Select(x => new[] { x.MaLichKhamSangLoc.ToString(), x.MaPhieu.ToString(), x.Phieu.NguoiHien.HoTen, x.Phieu.Dot.TenDot, x.ThoiGianBatDau.ToString("dd/MM/yyyy HH:mm"), x.ThoiGianKetThuc.ToString("dd/MM/yyyy HH:mm"), x.BacSiSangLoc, x.DiaDiemHoacLienKet, x.TrangThai }));
        return File(QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure.Module5Csv.Encode(rows), "text/csv; charset=utf-8", "LichKhamSangLoc.csv");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(LichFormVm vm)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var error = await service.LuuLich(vm);
                if (error == null)
                {
                    TempData["Ok"] = "Đã lưu lịch và cập nhật trạng thái phiếu.";
                    return RedirectToAction("LichKhamSangLoc", "Admin", new { ngay = vm.ThoiGianBatDau, view = "calendar" });
                }

                ModelState.AddModelError("", error);
            }
            catch (Exception ex)when (ex is DbUpdateException || ex is System.Data.Common.DbException)
            {
                db.ChangeTracker.Clear();
                ModelState.AddModelError("", "Dữ liệu vừa thay đổi hoặc lịch bị trùng. Hãy tải lại và kiểm tra.");
            }
        }

        return await pages.Lich(this, ngay: vm.ThoiGianBatDau, newSlot: true, form: vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string? returnUrl)
    {
        var e = await service.DoiTrangThaiLich(id, false);
        TempData[e == null ? "Ok" : "Error"] = e ?? "Đã hủy lịch. Phiếu trở về Đã xác nhận để có thể lập lịch mới.";
        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id, string? returnUrl)
    {
        var e = await service.DoiTrangThaiLich(id, true);
        TempData[e == null ? "Ok" : "Error"] = e ?? "Đã hoàn thành khám và cập nhật phiếu Đã khám sàng lọc.";
        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(Index));
    }
}
