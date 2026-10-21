using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;

public sealed class ThongKeService(ApplicationDbContext db, Module5Clock clock)
{
    public async Task<DashboardViewModel> DashboardAsync()
    {
        var now = clock.Now;
        var model = new DashboardViewModel
        {
            ThoiDiemThongKe = now,
            TongDonViToChuc = await db.Set<DonViToChuc>().CountAsync(),
            TongDotHienMau = await db.Set<DotHienMau>().CountAsync(),
            SoDotDangMo = await db.Set<DotHienMau>().CountAsync(x => x.TrangThai == Module5TrangThai.DotHienMau.DangMoDangKy),
            TongNguoiHienMau = await db.Set<NguoiHienMau>().CountAsync(),
            TongPhieu = await db.Set<PhieuDangKyHienMau>().CountAsync(),
            SoPhieuChoXacNhan = await db.Set<PhieuDangKyHienMau>().CountAsync(x => x.TrangThai == Module5TrangThai.Phieu.ChoXacNhan),
            SoPhieuDaXacNhan = await db.Set<PhieuDangKyHienMau>().CountAsync(x => x.TrangThai == Module5TrangThai.Phieu.DaXacNhan),
            SoLichSapToi = await db.Set<LichKhamSangLoc>().CountAsync(x =>
                x.TrangThai == Module5TrangThai.LichKham.DaLenLich && x.ThoiGianBatDau >= now),
            SoPhieuThanhCong = await db.Set<PhieuDangKyHienMau>().CountAsync(x => x.TrangThai == Module5TrangThai.Phieu.HienMauThanhCong)
        };
        model.LichSapToi = await db.Set<LichKhamSangLoc>().AsNoTracking()
            .Include(x => x.PhieuDangKyHienMau).ThenInclude(x => x.NguoiHienMau)
            .Include(x => x.PhieuDangKyHienMau).ThenInclude(x => x.DotHienMau)
            .Where(x => x.TrangThai == Module5TrangThai.LichKham.DaLenLich && x.ThoiGianBatDau >= now)
            .OrderBy(x => x.ThoiGianBatDau).ThenBy(x => x.MaLichKhamSangLoc).Take(6).ToListAsync();
        model.CanhBaoChiTieu = await TheoDotQuery()
            .Where(x => x.ChiTieu > 0 && x.ThanhCong >= x.ChiTieu)
            .OrderBy(x => x.TenDot).ToListAsync();
        return model;
    }

    public async Task<ThongKeViewModel> TongHopAsync()
    {
        var model = new ThongKeViewModel { ThoiDiemThongKe = clock.Now };
        model.TheoDonVi = await db.Set<DonViToChuc>().AsNoTracking()
            .OrderBy(x => x.TenDonViToChuc)
            .Select(x => new ThongKeDonViRow
            {
                MaDonViToChuc = x.MaDonViToChuc,
                TenDonViToChuc = x.TenDonViToChuc,
                SoDot = x.DotHienMaus.Count(),
                SoThanhCong = x.DotHienMaus.SelectMany(d => d.PhieuDangKyHienMaus)
                    .Count(p => p.TrangThai == Module5TrangThai.Phieu.HienMauThanhCong)
            }).ToListAsync();
        model.TheoDot = await TheoDotQuery().OrderByDescending(x => x.TongPhieu)
            .ThenBy(x => x.TenDot).ToListAsync();
        var nhomTrangThai = await db.Set<PhieuDangKyHienMau>().AsNoTracking()
            .GroupBy(x => x.TrangThai)
            .Select(g => new ThongKeTrangThaiRow { TrangThai = g.Key, SoLuong = g.Count() }).ToListAsync();
        model.TheoTrangThai = Module5TrangThai.Phieu.TatCa.Select(s => new ThongKeTrangThaiRow
        {
            TrangThai = s, SoLuong = nhomTrangThai.FirstOrDefault(x => x.TrangThai == s)?.SoLuong ?? 0
        }).ToList();
        model.PhieuTheoThang = await db.Set<PhieuDangKyHienMau>().AsNoTracking()
            .GroupBy(x => new { x.NgayNop.Year, x.NgayNop.Month })
            .Select(g => new ThongKeThangRow { Nam = g.Key.Year, Thang = g.Key.Month, SoLuong = g.Count() })
            .OrderByDescending(x => x.Nam).ThenByDescending(x => x.Thang).ToListAsync();
        model.LichTheoThang = await db.Set<LichKhamSangLoc>().AsNoTracking()
            .GroupBy(x => new { x.ThoiGianBatDau.Year, x.ThoiGianBatDau.Month })
            .Select(g => new ThongKeThangRow { Nam = g.Key.Year, Thang = g.Key.Month, SoLuong = g.Count() })
            .OrderByDescending(x => x.Nam).ThenByDescending(x => x.Thang).ToListAsync();
        var max = model.TheoDot.Select(x => x.TongPhieu).DefaultIfEmpty(0).Max();
        model.DotNhieuPhieuNhat = model.TheoDot.Where(x => max > 0 && x.TongPhieu == max).ToList();
        model.TongDaKham = model.TheoDot.Sum(x => x.DaKham);
        model.TongThanhCong = model.TheoDot.Sum(x => x.ThanhCong);
        return model;
    }

    public IQueryable<ThongKeDotRow> TheoDotQuery() => db.Set<DotHienMau>().AsNoTracking()
        .Select(x => new ThongKeDotRow
        {
            MaDot = x.MaDot,
            TenDot = x.TenDot,
            TenDonViToChuc = x.DonViToChuc.TenDonViToChuc,
            ChiTieu = x.ChiTieuDonViMau,
            TongPhieu = x.PhieuDangKyHienMaus.Count(),
            DaXacNhan = x.PhieuDangKyHienMaus.Count(p => p.TrangThai == Module5TrangThai.Phieu.DaXacNhan),
            DaQuaXacNhan = x.PhieuDangKyHienMaus.Count(p => Module5TrangThai.Phieu.DaQuaXacNhan.Contains(p.TrangThai) ||
                (p.TrangThai == Module5TrangThai.Phieu.DaHuy && p.NgayXuLy.HasValue)),
            DaKham = x.PhieuDangKyHienMaus.Count(p => p.LichKhamSangLocs.Any(l => l.TrangThai == Module5TrangThai.LichKham.DaHoanThanh)),
            ThanhCong = x.PhieuDangKyHienMaus.Count(p => p.TrangThai == Module5TrangThai.Phieu.HienMauThanhCong)
        });
}
