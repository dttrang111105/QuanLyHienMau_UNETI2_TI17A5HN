using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyHienMau_UNETI2_TI17A5HN.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

public sealed class LichKhamSangLocRepository(ApplicationDbContext db, DongHoHeThong clock)
{
    private const int PageSize = 10;

    public Task<LichKhamSangLoc?> ChiTietAsync(int id, int? maTaiKhoan) =>
        DuLieuDayDu(maTaiKhoan).SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);

    public Task<LichKhamSangLoc?> LichDeSuaAsync(int id) => db.LichKhamSangLocs.AsNoTracking()
        .Include(x => x.PhieuDangKyHienMau)
        .SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);

    public Task<LichKhamSangLoc?> TimLichDeGhiAsync(int id) => db.LichKhamSangLocs
        .Include(x => x.PhieuDangKyHienMau)
        .SingleOrDefaultAsync(x => x.MaLichKhamSangLoc == id);

    public Task<PhieuDangKyHienMau?> TimPhieuDeGhiAsync(int maPhieu) =>
        db.PhieuDangKyHienMaus.SingleOrDefaultAsync(x => x.MaPhieu == maPhieu);

    public Task<PhieuDangKyHienMau?> TimPhieuLapLichAsync(int maPhieu) => db.PhieuDangKyHienMaus
        .Include(x => x.NguoiHienMau)
        .SingleOrDefaultAsync(x => x.MaPhieu == maPhieu);

    public Task<bool> CoLichHieuLucAsync(int maPhieu, int? boQuaMaLich = null) =>
        LichHieuLuc(boQuaMaLich).AnyAsync(x => x.MaPhieu == maPhieu);

    public Task<bool> CoLichHoanThanhAsync(int maPhieu) =>
        db.LichKhamSangLocs.AnyAsync(x => x.MaPhieu == maPhieu &&
            x.TrangThai == TrangThaiHienMau.LichKham.DaHoanThanh);

    public Task<bool> CoLichTrungNguoiHienAsync(int maNguoiHien, DateTime batDau, DateTime ketThuc, int? boQuaMaLich) =>
        LichGiaoNhau(batDau, ketThuc, boQuaMaLich).AnyAsync(x =>
            x.PhieuDangKyHienMau != null && x.PhieuDangKyHienMau.MaNguoiHien == maNguoiHien);

    public Task<bool> CoLichTrungBacSiAsync(string bacSi, DateTime batDau, DateTime ketThuc, int? boQuaMaLich)
    {
        var tenSoSanh = bacSi.ToUpperInvariant();
        return LichGiaoNhau(batDau, ketThuc, boQuaMaLich).AnyAsync(x =>
            x.BacSiSangLoc != null && x.BacSiSangLoc.ToUpper() == tenSoSanh);
    }

    public void ThemLich(LichKhamSangLoc lich) => db.LichKhamSangLocs.Add(lich);

    public Task<int> LuuThayDoiAsync() => db.SaveChangesAsync();

    public Task<IDbContextTransaction> BatDauGiaoDichAsync() =>
        db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

    private IQueryable<LichKhamSangLoc> LichHieuLuc(int? boQuaMaLich) => db.LichKhamSangLocs
        .Where(x => x.TrangThai != TrangThaiHienMau.LichKham.DaHuy &&
            (!boQuaMaLich.HasValue || x.MaLichKhamSangLoc != boQuaMaLich));

    private IQueryable<LichKhamSangLoc> LichGiaoNhau(DateTime batDau, DateTime ketThuc, int? boQuaMaLich) =>
        LichHieuLuc(boQuaMaLich).Where(x => x.ThoiGianBatDau < ketThuc && batDau < x.ThoiGianKetThuc);

    private IQueryable<LichKhamSangLoc> DuLieuDayDu(int? maTaiKhoan) => db.Set<LichKhamSangLoc>().AsNoTracking()
        .Where(x => !maTaiKhoan.HasValue || (x.PhieuDangKyHienMau != null &&
            x.PhieuDangKyHienMau.NguoiHienMau != null && x.PhieuDangKyHienMau.NguoiHienMau.MaTaiKhoan == maTaiKhoan))
        .Include(x => x.PhieuDangKyHienMau!).ThenInclude(x => x!.NguoiHienMau)
        .Include(x => x.PhieuDangKyHienMau!).ThenInclude(x => x.DotHienMau!).ThenInclude(x => x.DonViToChuc)
        .Include(x => x.PhieuDangKyHienMau!).ThenInclude(x => x!.KetQuaHienMau);

    public async Task<LichKhamListViewModel> DanhSachAsync(string? search, string? trangThai,
        int? maDot, DateTime? ngay, int page, int? maTaiKhoan = null)
    {
        var query = DuLieuDayDu(maTaiKhoan);
        var now = clock.Now;
        var homNay = now.Date;
        var ngayMai = homNay.AddDays(1);
        var model = new LichKhamListViewModel
        {
            Search = search?.Trim(), TrangThai = trangThai, MaDot = maDot, Ngay = ngay, ChiCuaToi = maTaiKhoan.HasValue,
            SoLichHomNay = await query.CountAsync(x => x.TrangThai != TrangThaiHienMau.LichKham.DaHuy &&
                x.ThoiGianBatDau >= homNay && x.ThoiGianBatDau < ngayMai),
            SoLichSapToi = await query.CountAsync(x => x.TrangThai == TrangThaiHienMau.LichKham.DaLenLich && x.ThoiGianBatDau >= now),
            SoLichHoanThanh = await query.CountAsync(x => x.TrangThai == TrangThaiHienMau.LichKham.DaHoanThanh)
        };
        if (!string.IsNullOrWhiteSpace(model.Search))
        {
            var tuKhoa = model.Search;
            var coMa = int.TryParse(tuKhoa.TrimStart('#'), out var ma);
            query = query.Where(x => (x.PhieuDangKyHienMau != null && x.PhieuDangKyHienMau!.NguoiHienMau != null && x.PhieuDangKyHienMau!.NguoiHienMau.HoTen.Contains(tuKhoa)) ||
                (x.PhieuDangKyHienMau != null && x.PhieuDangKyHienMau!.DotHienMau != null && x.PhieuDangKyHienMau!.DotHienMau.TenDot.Contains(tuKhoa)) ||
                (x.BacSiSangLoc != null && x.BacSiSangLoc.Contains(tuKhoa)) ||
                (coMa && (x.MaPhieu == ma || x.MaLichKhamSangLoc == ma)));
        }
        if (!string.IsNullOrWhiteSpace(trangThai)) query = query.Where(x => x.TrangThai == trangThai);
        if (maDot.HasValue) query = query.Where(x => x.PhieuDangKyHienMau != null && x.PhieuDangKyHienMau!.MaDot == maDot);
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
        if (maTaiKhoan.HasValue) dots = dots.Where(x => x.PhieuDangKyHienMaus.Any(p => p.NguoiHienMau != null && p.NguoiHienMau.MaTaiKhoan == maTaiKhoan));
        model.DotOptions = await dots.OrderBy(x => x.TenDot)
            .Select(x => new SelectListItem(x.TenDot, x.MaDot.ToString())).ToListAsync();
        return model;
    }

    public async Task NapLuaChonAsync(LichKhamFormViewModel model)
    {
        var phieus = db.Set<PhieuDangKyHienMau>().AsNoTracking().Where(x => model.MaLichKhamSangLoc.HasValue
            ? x.MaPhieu == model.MaPhieu
            : x.TrangThai == TrangThaiHienMau.Phieu.DaXacNhan && x.NguoiHienMau != null && (x.NguoiHienMau.TrangThai!.ToString() == "True" || x.NguoiHienMau.TrangThai!.ToString() == TrangThaiHienMau.HoatDong) &&
                x.KetQuaHienMau == null && !x.LichKhamSangLocs.Any(l => l.TrangThai != TrangThaiHienMau.LichKham.DaHuy));
        var rows = await phieus.OrderBy(x => x.NguoiHienMau!.HoTen)
            .Select(x => new { x.MaPhieu, x.NguoiHienMau!.HoTen, x.DotHienMau!.TenDot }).ToListAsync();
        model.PhieuOptions = rows.Select(x => new SelectListItem($"#{x.MaPhieu} · {x.HoTen} · {x.TenDot}", x.MaPhieu.ToString())).ToList();
        var bacSiDaCo = db.Set<LichKhamSangLoc>().AsNoTracking().Where(x => x.BacSiSangLoc != null).Select(x => x.BacSiSangLoc!);
        var nhanVien = db.Set<TaiKhoan>().AsNoTracking().Where(x => (x.TrangThai!.ToString() == "True" || x.TrangThai!.ToString() == TrangThaiHienMau.HoatDong) &&
            (x.VaiTro == TrangThaiHienMau.VaiTro.NhanVienYTe || x.VaiTro == "NhanVienYTe" || x.VaiTro == "NhanVien" || x.VaiTro == "Medical Staff"))
            .Select(x => x.HoTen);
        model.BacSiOptions = await bacSiDaCo.Union(nhanVien).OrderBy(x => x).ToListAsync();
    }
}
