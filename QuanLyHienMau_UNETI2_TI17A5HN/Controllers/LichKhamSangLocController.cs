using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Filters;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;

[Module5Authorize]
public class LichKhamSangLocController(ApplicationDbContext db, Module5Service service, Module5Clock clock) : Controller
{
    private const int PageSize = 10;
    private TaiKhoan TaiKhoan => (TaiKhoan)HttpContext.Items[typeof(TaiKhoan)]!;
    private bool LaNhanVien => Module5TrangThai.VaiTro.LaNhanVien(TaiKhoan.VaiTro);

    [HttpGet, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> Index(string? search, string? trangThai, int? maDot, DateTime? ngay, int page = 1) =>
        View("Index", await DanhSachAsync(search, trangThai, maDot, ngay, page, false));

    [HttpGet]
    public async Task<IActionResult> CuaToi(string? search, string? trangThai, int? maDot, DateTime? ngay, int page = 1) =>
        View("Index", await DanhSachAsync(search, trangThai, maDot, ngay, page, true));

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var lich = await GioiHanQuyen(DuLieuDayDu()).SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);
        if (lich is null) return NotFound();
        ViewData["LaNhanVien"] = LaNhanVien;
        return View(lich);
    }

    [HttpGet, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> Create(int? maPhieu)
    {
        var batDau = clock.Now.AddHours(1);
        var model = new LichKhamFormViewModel
        {
            MaPhieu = maPhieu ?? 0,
            ThoiGianBatDau = new DateTime(batDau.Year, batDau.Month, batDau.Day, batDau.Hour, batDau.Minute, 0),
            ThoiGianKetThuc = new DateTime(batDau.Year, batDau.Month, batDau.Day, batDau.Hour, batDau.Minute, 0).AddMinutes(30)
        };
        await NapLuaChonAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> Create(LichKhamFormViewModel model)
    {
        model.MaLichKhamSangLoc = null;
        if (ModelState.IsValid)
        {
            var result = await service.LuuLichAsync(model);
            if (result.Success)
            {
                TempData["Module5Success"] = result.Message;
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            ModelState.AddModelError("", result.Message);
        }
        await NapLuaChonAsync(model);
        return View("Form", model);
    }

    [HttpGet, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> Edit(int id)
    {
        var lich = await db.Set<LichKhamSangLoc>().AsNoTracking().Include(x => x.PhieuDangKyHienMau)
            .SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);
        if (lich is null) return NotFound();
        if (lich.TrangThai != Module5TrangThai.LichKham.DaLenLich ||
            lich.PhieuDangKyHienMau.TrangThai != Module5TrangThai.Phieu.ChoKhamSangLoc)
        {
            TempData["Module5Error"] = "Lịch không còn đủ điều kiện chỉnh sửa.";
            return RedirectToAction(nameof(Details), new { id });
        }
        var model = new LichKhamFormViewModel
        {
            MaLichKhamSangLoc = id, MaPhieu = lich.MaPhieu,
            ThoiGianBatDau = lich.ThoiGianBatDau, ThoiGianKetThuc = lich.ThoiGianKetThuc,
            HinhThucTiepNhan = lich.HinhThucTiepNhan, DiaDiemHoacLienKet = lich.DiaDiemHoacLienKet,
            BacSiSangLoc = lich.BacSiSangLoc, GhiChu = lich.GhiChu
        };
        await NapLuaChonAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> Edit(int id, LichKhamFormViewModel model)
    {
        model.MaLichKhamSangLoc = id;
        if (ModelState.IsValid)
        {
            var result = await service.LuuLichAsync(model, id);
            if (result.Success)
            {
                TempData["Module5Success"] = result.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            ModelState.AddModelError("", result.Message);
        }
        await NapLuaChonAsync(model);
        return View("Form", model);
    }

    [HttpPost, ValidateAntiForgeryToken, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> Huy(int id)
    {
        var result = await service.HuyLichAsync(id);
        TempData[result.Success ? "Module5Success" : "Module5Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> HoanThanh(int id)
    {
        var result = await service.HoanThanhLichAsync(id);
        TempData[result.Success ? "Module5Success" : "Module5Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    private IQueryable<LichKhamSangLoc> DuLieuDayDu() => db.Set<LichKhamSangLoc>().AsNoTracking()
        .Include(x => x.PhieuDangKyHienMau).ThenInclude(x => x.NguoiHienMau)
        .Include(x => x.PhieuDangKyHienMau).ThenInclude(x => x.DotHienMau).ThenInclude(x => x.DonViToChuc)
        .Include(x => x.PhieuDangKyHienMau).ThenInclude(x => x.KetQuaHienMau);

    private IQueryable<LichKhamSangLoc> GioiHanQuyen(IQueryable<LichKhamSangLoc> query) => LaNhanVien
        ? query : query.Where(x => x.PhieuDangKyHienMau.NguoiHienMau.MaTaiKhoan == TaiKhoan.MaTaiKhoan);

    private async Task<LichKhamListViewModel> DanhSachAsync(string? search, string? trangThai,
        int? maDot, DateTime? ngay, int page, bool chiCuaToi)
    {
        var query = DuLieuDayDu();
        if (chiCuaToi)
            query = query.Where(x => x.PhieuDangKyHienMau.NguoiHienMau.MaTaiKhoan == TaiKhoan.MaTaiKhoan);
        var now = clock.Now;
        var homNay = now.Date;
        var ngayMai = homNay.AddDays(1);
        var model = new LichKhamListViewModel
        {
            Search = search?.Trim(), TrangThai = trangThai, MaDot = maDot, Ngay = ngay, ChiCuaToi = chiCuaToi,
            SoLichHomNay = await query.CountAsync(x => x.TrangThai != Module5TrangThai.LichKham.DaHuy &&
                x.ThoiGianBatDau >= homNay && x.ThoiGianBatDau < ngayMai),
            SoLichSapToi = await query.CountAsync(x => x.TrangThai == Module5TrangThai.LichKham.DaLenLich && x.ThoiGianBatDau >= now),
            SoLichHoanThanh = await query.CountAsync(x => x.TrangThai == Module5TrangThai.LichKham.DaHoanThanh)
        };
        if (!string.IsNullOrWhiteSpace(model.Search))
        {
            var tuKhoa = model.Search;
            var coMa = int.TryParse(tuKhoa.TrimStart('#'), out var ma);
            query = query.Where(x => x.PhieuDangKyHienMau.NguoiHienMau.HoTen.Contains(tuKhoa) ||
                x.PhieuDangKyHienMau.DotHienMau.TenDot.Contains(tuKhoa) || x.BacSiSangLoc.Contains(tuKhoa) ||
                (coMa && (x.MaPhieu == ma || x.MaLichKhamSangLoc == ma)));
        }
        if (!string.IsNullOrWhiteSpace(trangThai)) query = query.Where(x => x.TrangThai == trangThai);
        if (maDot.HasValue) query = query.Where(x => x.PhieuDangKyHienMau.MaDot == maDot);
        if (ngay.HasValue)
        {
            var tuNgay = ngay.Value.Date;
            var denNgay = tuNgay.AddDays(1);
            query = query.Where(x => x.ThoiGianBatDau >= tuNgay && x.ThoiGianBatDau < denNgay);
        }
        model.TotalCount = await query.CountAsync();
        model.TotalPages = (int)Math.Ceiling(model.TotalCount / (double)PageSize);
        model.Page = Math.Clamp(page, 1, Math.Max(1, model.TotalPages));
        model.Items = await query.OrderByDescending(x => x.ThoiGianBatDau).ThenByDescending(x => x.MaLichKhamSangLoc)
            .Skip((model.Page - 1) * PageSize).Take(PageSize).ToListAsync();
        var dots = db.Set<DotHienMau>().AsNoTracking();
        if (chiCuaToi) dots = dots.Where(x => x.PhieuDangKyHienMaus.Any(p => p.NguoiHienMau.MaTaiKhoan == TaiKhoan.MaTaiKhoan));
        model.DotOptions = await dots.OrderBy(x => x.TenDot)
            .Select(x => new SelectListItem(x.TenDot, x.MaDot.ToString())).ToListAsync();
        return model;
    }

    private async Task NapLuaChonAsync(LichKhamFormViewModel model)
    {
        var phieus = db.Set<PhieuDangKyHienMau>().AsNoTracking().Where(x => model.MaLichKhamSangLoc.HasValue
            ? x.MaPhieu == model.MaPhieu
            : x.TrangThai == Module5TrangThai.Phieu.DaXacNhan && x.NguoiHienMau.TrangThai &&
                x.KetQuaHienMau == null && !x.LichKhamSangLocs.Any(l => l.TrangThai != Module5TrangThai.LichKham.DaHuy));
        var rows = await phieus.OrderBy(x => x.NguoiHienMau.HoTen)
            .Select(x => new { x.MaPhieu, x.NguoiHienMau.HoTen, x.DotHienMau.TenDot }).ToListAsync();
        model.PhieuOptions = rows.Select(x => new SelectListItem($"#{x.MaPhieu} · {x.HoTen} · {x.TenDot}", x.MaPhieu.ToString())).ToList();
        var bacSiDaCo = db.Set<LichKhamSangLoc>().AsNoTracking().Select(x => x.BacSiSangLoc);
        var nhanVien = db.Set<TaiKhoan>().AsNoTracking().Where(x => x.TrangThai &&
            (x.VaiTro == Module5TrangThai.VaiTro.NhanVienYTe || x.VaiTro == "NhanVienYTe" || x.VaiTro == "NhanVien"))
            .Select(x => x.HoTen);
        model.BacSiOptions = await bacSiDaCo.Union(nhanVien).OrderBy(x => x).ToListAsync();
    }
}
