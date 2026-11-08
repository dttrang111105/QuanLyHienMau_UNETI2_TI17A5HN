using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

public sealed class KetQuaHienMauRepository(ApplicationDbContext db)
{
    private const int PageSize = 10;

    public Task<KetQuaHienMau?> ChiTietAsync(int id, int? maTaiKhoan) =>
        DuLieuDayDu(maTaiKhoan).SingleOrDefaultAsync(x => x.MaKetQuaHienMau == id);

    public Task<PhieuDangKyHienMau?> TimPhieuGhiKetQuaAsync(int maPhieu) => db.PhieuDangKyHienMaus
        .Include(x => x.DotHienMau)
        .SingleOrDefaultAsync(x => x.MaPhieu == maPhieu);

    public Task<KetQuaHienMau?> TimKetQuaCuaPhieuAsync(int maPhieu) =>
        db.KetQuaHienMaus.SingleOrDefaultAsync(x => x.MaPhieu == maPhieu);

    public Task<bool> CoKetQuaAsync(int maPhieu) => db.KetQuaHienMaus.AnyAsync(x => x.MaPhieu == maPhieu);

    public Task<int> DemPhieuThanhCongAsync(int maDot) =>
        db.PhieuDangKyHienMaus.CountAsync(x => x.MaDot == maDot &&
            x.TrangThai == TrangThaiHienMau.Phieu.HienMauThanhCong);

    public void ThemKetQua(KetQuaHienMau ketQua) => db.KetQuaHienMaus.Add(ketQua);

    private IQueryable<KetQuaHienMau> DuLieuDayDu(int? maTaiKhoan) => db.Set<KetQuaHienMau>().AsNoTracking()
        .Where(x => !maTaiKhoan.HasValue || x.PhieuDangKyHienMau!.NguoiHienMau!.MaTaiKhoan == maTaiKhoan)
        .Include(x => x.PhieuDangKyHienMau).ThenInclude(x => x!.NguoiHienMau)
        .Include(x => x.PhieuDangKyHienMau).ThenInclude(x => x!.DotHienMau).ThenInclude(x => x!.DonViToChuc);

    public async Task<KetQuaListViewModel> DanhSachAsync(string? search, string? ketQua, int? maDot, int page, int? maTaiKhoan = null)
    {
        var query = DuLieuDayDu(maTaiKhoan);
        var phieus = db.Set<PhieuDangKyHienMau>().AsNoTracking();
        if (maTaiKhoan.HasValue)
            phieus = phieus.Where(x => x.NguoiHienMau!.MaTaiKhoan == maTaiKhoan);
        var model = new KetQuaListViewModel
        {
            Search = search?.Trim(), KetQua = ketQua, MaDot = maDot, ChiCuaToi = maTaiKhoan.HasValue,
            SoThanhCong = await query.CountAsync(x => x.KetQua == TrangThaiHienMau.Phieu.HienMauThanhCong),
            SoKhongDuDieuKien = await query.CountAsync(x => x.KetQua == TrangThaiHienMau.Phieu.KhongDuDieuKienHien),
            SoChoKetQua = await phieus.CountAsync(x => x.TrangThai == TrangThaiHienMau.Phieu.DaKhamSangLoc &&
                x.KetQuaHienMau == null && x.LichKhamSangLocs.Any(l => l.TrangThai == TrangThaiHienMau.LichKham.DaHoanThanh))
        };
        if (!string.IsNullOrWhiteSpace(model.Search))
        {
            var tuKhoa = model.Search;
            var coMa = int.TryParse(tuKhoa.TrimStart('#'), out var ma);
            query = query.Where(x => x.PhieuDangKyHienMau!.NguoiHienMau!.HoTen.Contains(tuKhoa) ||
                x.PhieuDangKyHienMau!.DotHienMau!.TenDot.Contains(tuKhoa) ||
                (coMa && (x.MaPhieu == ma || x.MaKetQuaHienMau == ma)));
        }
        if (!string.IsNullOrWhiteSpace(ketQua)) query = query.Where(x => x.KetQua == ketQua);
        if (maDot.HasValue) query = query.Where(x => x.PhieuDangKyHienMau!.MaDot == maDot);
        model.TotalCount = await query.CountAsync();
        model.TotalPages = (int)Math.Ceiling(model.TotalCount / (double)PageSize);
        model.Page = Math.Clamp(page, 1, Math.Max(1, model.TotalPages));
        model.Items = await query.OrderByDescending(x => x.NgayCapNhat).ThenByDescending(x => x.MaKetQuaHienMau)
            .Skip((model.Page - 1) * PageSize).Take(PageSize).ToListAsync();
        var dots = db.Set<DotHienMau>().AsNoTracking();
        if (maTaiKhoan.HasValue) dots = dots.Where(x => x.PhieuDangKyHienMaus.Any(p => p.NguoiHienMau!.MaTaiKhoan == maTaiKhoan));
        model.DotOptions = await dots.OrderBy(x => x.TenDot)
            .Select(x => new SelectListItem(x.TenDot, x.MaDot.ToString())).ToListAsync();
        return model;
    }

    public async Task NapLuaChonAsync(KetQuaFormViewModel model)
    {
        var query = db.Set<PhieuDangKyHienMau>().AsNoTracking().Where(x =>
            x.TrangThai == TrangThaiHienMau.Phieu.DaKhamSangLoc && x.KetQuaHienMau == null &&
            x.LichKhamSangLocs.Any(l => l.TrangThai == TrangThaiHienMau.LichKham.DaHoanThanh));
        var rows = await query.OrderBy(x => x.NguoiHienMau!.HoTen)
            .Select(x => new { x.MaPhieu, x.NguoiHienMau!.HoTen, x.DotHienMau!.TenDot }).ToListAsync();
        model.PhieuOptions = rows.Select(x => new SelectListItem($"#{x.MaPhieu} · {x.HoTen} · {x.TenDot}", x.MaPhieu.ToString())).ToList();
        model.Phieu = await query.Include(x => x.NguoiHienMau)
            .Include(x => x.DotHienMau).ThenInclude(x => x!.DonViToChuc)
            .Include(x => x.LichKhamSangLocs)
            .SingleOrDefaultAsync(x => x.MaPhieu == model.MaPhieu);
    }
}
