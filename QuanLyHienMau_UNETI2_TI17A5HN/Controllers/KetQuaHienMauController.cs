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
public class KetQuaHienMauController(ApplicationDbContext db, Module5Service service) : Controller
{
    private const int PageSize = 10;
    private TaiKhoan TaiKhoan => (TaiKhoan)HttpContext.Items[typeof(TaiKhoan)]!;
    private bool LaNhanVien => Module5TrangThai.VaiTro.LaNhanVien(TaiKhoan.VaiTro);

    [HttpGet, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> Index(string? search, string? ketQua, int? maDot, int page = 1) =>
        View("Index", await DanhSachAsync(search, ketQua, maDot, page, false));

    [HttpGet]
    public async Task<IActionResult> CuaToi(string? search, string? ketQua, int? maDot, int page = 1) =>
        View("Index", await DanhSachAsync(search, ketQua, maDot, page, true));

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var query = DuLieuDayDu();
        if (!LaNhanVien)
            query = query.Where(x => x.PhieuDangKyHienMau.NguoiHienMau.MaTaiKhoan == TaiKhoan.MaTaiKhoan);
        var ketQua = await query.SingleOrDefaultAsync(x => x.MaKetQuaHienMau == id);
        if (ketQua is null) return NotFound();
        ViewData["LaNhanVien"] = LaNhanVien;
        return View(ketQua);
    }

    [HttpGet, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> Create(int? maPhieu)
    {
        var model = new KetQuaFormViewModel { MaPhieu = maPhieu ?? 0 };
        await NapLuaChonAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Module5Authorize(ChiNhanVien = true)]
    public async Task<IActionResult> Create(KetQuaFormViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await service.GhiKetQuaAsync(model);
            if (result.Success)
            {
                TempData["Module5Success"] = result.Message;
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            ModelState.AddModelError("", result.Message);
        }
        await NapLuaChonAsync(model);
        return View(model);
    }

    private IQueryable<KetQuaHienMau> DuLieuDayDu() => db.Set<KetQuaHienMau>().AsNoTracking()
        .Include(x => x.PhieuDangKyHienMau).ThenInclude(x => x.NguoiHienMau)
        .Include(x => x.PhieuDangKyHienMau).ThenInclude(x => x.DotHienMau).ThenInclude(x => x.DonViToChuc);

    private async Task<KetQuaListViewModel> DanhSachAsync(string? search, string? ketQua, int? maDot, int page, bool chiCuaToi)
    {
        var query = DuLieuDayDu();
        var phieus = db.Set<PhieuDangKyHienMau>().AsNoTracking();
        if (chiCuaToi)
        {
            query = query.Where(x => x.PhieuDangKyHienMau.NguoiHienMau.MaTaiKhoan == TaiKhoan.MaTaiKhoan);
            phieus = phieus.Where(x => x.NguoiHienMau.MaTaiKhoan == TaiKhoan.MaTaiKhoan);
        }
        var model = new KetQuaListViewModel
        {
            Search = search?.Trim(), KetQua = ketQua, MaDot = maDot, ChiCuaToi = chiCuaToi,
            SoThanhCong = await query.CountAsync(x => x.KetQua == Module5TrangThai.Phieu.HienMauThanhCong),
            SoKhongDuDieuKien = await query.CountAsync(x => x.KetQua == Module5TrangThai.Phieu.KhongDuDieuKienHien),
            SoChoKetQua = await phieus.CountAsync(x => x.TrangThai == Module5TrangThai.Phieu.DaKhamSangLoc &&
                x.KetQuaHienMau == null && x.LichKhamSangLocs.Any(l => l.TrangThai == Module5TrangThai.LichKham.DaHoanThanh))
        };
        if (!string.IsNullOrWhiteSpace(model.Search))
        {
            var tuKhoa = model.Search;
            var coMa = int.TryParse(tuKhoa.TrimStart('#'), out var ma);
            query = query.Where(x => x.PhieuDangKyHienMau.NguoiHienMau.HoTen.Contains(tuKhoa) ||
                x.PhieuDangKyHienMau.DotHienMau.TenDot.Contains(tuKhoa) ||
                (coMa && (x.MaPhieu == ma || x.MaKetQuaHienMau == ma)));
        }
        if (!string.IsNullOrWhiteSpace(ketQua)) query = query.Where(x => x.KetQua == ketQua);
        if (maDot.HasValue) query = query.Where(x => x.PhieuDangKyHienMau.MaDot == maDot);
        model.TotalCount = await query.CountAsync();
        model.TotalPages = (int)Math.Ceiling(model.TotalCount / (double)PageSize);
        model.Page = Math.Clamp(page, 1, Math.Max(1, model.TotalPages));
        model.Items = await query.OrderByDescending(x => x.NgayCapNhat).ThenByDescending(x => x.MaKetQuaHienMau)
            .Skip((model.Page - 1) * PageSize).Take(PageSize).ToListAsync();
        var dots = db.Set<DotHienMau>().AsNoTracking();
        if (chiCuaToi) dots = dots.Where(x => x.PhieuDangKyHienMaus.Any(p => p.NguoiHienMau.MaTaiKhoan == TaiKhoan.MaTaiKhoan));
        model.DotOptions = await dots.OrderBy(x => x.TenDot)
            .Select(x => new SelectListItem(x.TenDot, x.MaDot.ToString())).ToListAsync();
        return model;
    }

    private async Task NapLuaChonAsync(KetQuaFormViewModel model)
    {
        var query = db.Set<PhieuDangKyHienMau>().AsNoTracking().Where(x =>
            x.TrangThai == Module5TrangThai.Phieu.DaKhamSangLoc && x.KetQuaHienMau == null &&
            x.LichKhamSangLocs.Any(l => l.TrangThai == Module5TrangThai.LichKham.DaHoanThanh));
        var rows = await query.OrderBy(x => x.NguoiHienMau.HoTen)
            .Select(x => new { x.MaPhieu, x.NguoiHienMau.HoTen, x.DotHienMau.TenDot }).ToListAsync();
        model.PhieuOptions = rows.Select(x => new SelectListItem($"#{x.MaPhieu} · {x.HoTen} · {x.TenDot}", x.MaPhieu.ToString())).ToList();
        model.Phieu = await query.Include(x => x.NguoiHienMau)
            .Include(x => x.DotHienMau).ThenInclude(x => x.DonViToChuc)
            .Include(x => x.LichKhamSangLocs)
            .SingleOrDefaultAsync(x => x.MaPhieu == model.MaPhieu);
    }
}
