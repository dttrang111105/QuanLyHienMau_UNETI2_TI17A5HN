using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

public sealed class EfPhieuDangKyRepository(ApplicationDbContext context) : IPhieuDangKyRepository
{
    public async Task<PhieuDangKyPage> GetPageAsync(PhieuDangKyFilter filter, CancellationToken cancellationToken = default)
    {
        const int pageSizeDefault = 10;
        var pageSize = filter.PageSize > 0 ? filter.PageSize : pageSizeDefault;
        var page = Math.Max(1, filter.Page);
        var query = context.PhieuDangKyHienMaus
            .Include(p => p.NguoiHienMau)
            .Include(p => p.DotHienMau!).ThenInclude(d => d.DonViToChuc)
            .Include(p => p.LichKhamSangLocs)
            .Include(p => p.KetQuaHienMau)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.TenNguoiHien))
        {
            var name = filter.TenNguoiHien.Trim();
            query = query.Where(p =>
                (p.NguoiHienMau != null && (p.NguoiHienMau.HoTen.Contains(name) ||
                    (p.NguoiHienMau.SoDienThoai != null && p.NguoiHienMau.SoDienThoai.Contains(name)) ||
                    (p.NguoiHienMau.Email != null && p.NguoiHienMau.Email.Contains(name)))) ||
                p.MaPhieu.ToString().Contains(name));
        }
        if (!string.IsNullOrWhiteSpace(filter.TenDot))
        {
            var campaign = filter.TenDot.Trim();
            query = query.Where(p => p.DotHienMau != null && p.DotHienMau.TenDot.Contains(campaign));
        }
        if (!string.IsNullOrWhiteSpace(filter.TrangThai) && filter.TrangThai != "all")
            query = query.Where(p => p.TrangThai == filter.TrangThai);
        if (filter.MaDonViToChuc is > 0)
            query = query.Where(p => p.DotHienMau != null && p.DotHienMau.MaDonViToChuc == filter.MaDonViToChuc.Value);
        if (filter.TuNgay.HasValue)
        {
            var start = filter.TuNgay.Value.Date;
            query = query.Where(p => p.NgayNop >= start);
        }
        if (filter.DenNgay.HasValue)
        {
            var endExclusive = filter.DenNgay.Value.Date.AddDays(1);
            query = query.Where(p => p.NgayNop < endExclusive);
        }

        var total = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        page = Math.Min(page, totalPages);
        var sort = filter.SapXep == "tenNguoiHien" ? "tenNguoiHien" : "ngayNop";
        var direction = string.Equals(filter.ChieuSapXep, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";
        var descending = direction == "desc";
        query = (sort, descending) switch
        {
            ("tenNguoiHien", false) => query.OrderBy(p => p.NguoiHienMau!.HoTen).ThenByDescending(p => p.NgayNop),
            ("tenNguoiHien", true) => query.OrderByDescending(p => p.NguoiHienMau!.HoTen).ThenByDescending(p => p.NgayNop),
            (_, false) => query.OrderBy(p => p.NgayNop),
            _ => query.OrderByDescending(p => p.NgayNop)
        };

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var counts = new PhieuTrangThaiCounts(
            await context.PhieuDangKyHienMaus.CountAsync(cancellationToken),
            await context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Chờ xác nhận", cancellationToken),
            await context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Đã xác nhận", cancellationToken),
            await context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Chờ khám sàng lọc", cancellationToken),
            await context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Đã khám sàng lọc", cancellationToken),
            await context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Từ chối", cancellationToken),
            await context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == "Đã hủy", cancellationToken));
        var units = await context.DonViToChucs.AsNoTracking().OrderBy(x => x.TenDonViToChuc).ToListAsync(cancellationToken);
        return new(items, units, counts, total, totalPages, page, sort, direction);
    }

    public async Task<PhieuDangKyDetails?> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var phieu = await context.PhieuDangKyHienMaus
            .Include(p => p.NguoiHienMau!).ThenInclude(n => n.TaiKhoan)
            .Include(p => p.DotHienMau!).ThenInclude(d => d.DonViToChuc)
            .Include(p => p.LichKhamSangLocs)
            .Include(p => p.KetQuaHienMau)
            .FirstOrDefaultAsync(p => p.MaPhieu == id, cancellationToken);
        if (phieu is null) return null;
        var history = await context.PhieuDangKyHienMaus.AsNoTracking()
            .Include(p => p.DotHienMau)
            .Where(p => p.MaNguoiHien == phieu.MaNguoiHien && p.MaPhieu != phieu.MaPhieu)
            .OrderByDescending(p => p.NgayNop)
            .ToListAsync(cancellationToken);
        var successful = await CountHienMauThanhCongAsync(phieu.MaDot, cancellationToken);
        return new(phieu, history, successful);
    }

    public Task<PhieuDangKyHienMau?> GetForUpdateAsync(int id, CancellationToken cancellationToken = default) =>
        context.PhieuDangKyHienMaus
            .Include(p => p.NguoiHienMau)
            .Include(p => p.DotHienMau)
            .Include(p => p.KetQuaHienMau)
            .Include(p => p.LichKhamSangLocs)
            .FirstOrDefaultAsync(p => p.MaPhieu == id, cancellationToken);

    public Task<KetQuaHienMau?> GetKetQuaForUpdateAsync(int id, CancellationToken cancellationToken = default) =>
        context.KetQuaHienMaus.Include(k => k.PhieuDangKyHienMau).ThenInclude(p => p!.DotHienMau)
            .FirstOrDefaultAsync(k => k.MaKetQuaHienMau == id, cancellationToken);

    public Task<LichKhamSangLoc?> GetLichKhamForUpdateAsync(int id, CancellationToken cancellationToken = default) =>
        context.LichKhamSangLocs.Include(l => l.PhieuDangKyHienMau)
            .FirstOrDefaultAsync(l => l.MaLichKhamSangLoc == id, cancellationToken);

    public void AddLichKham(LichKhamSangLoc lichKham) => context.LichKhamSangLocs.Add(lichKham);

    public Task<bool> HasLichKhamHoanThanhAsync(int maPhieu, CancellationToken cancellationToken = default) =>
        context.LichKhamSangLocs.AnyAsync(l => l.MaPhieu == maPhieu && l.TrangThai == "Đã hoàn thành", cancellationToken);

    public Task<int> CountHienMauThanhCongAsync(int maDot, CancellationToken cancellationToken = default) =>
        context.PhieuDangKyHienMaus.CountAsync(p => p.MaDot == maDot && p.TrangThai == "Hiến máu thành công", cancellationToken);

    public void AddKetQua(KetQuaHienMau ketQua) => context.KetQuaHienMaus.Add(ketQua);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);

    public async Task<IPhieuRepositoryTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable, CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        return new EfPhieuRepositoryTransaction(transaction);
    }

    public async Task<List<DotQuotaAlert>> GetDotDatChiTieuAsync(CancellationToken cancellationToken = default)
    {
        var raw = await context.DotHienMaus.AsNoTracking()
            .Where(d => d.ChiTieuDonViMau > 0)
            .Select(d => new
            {
                d.MaDot,
                d.TenDot,
                ChiTieu = d.ChiTieuDonViMau,
                DaHien = d.PhieuDangKyHienMaus.Count(p => p.TrangThai == "Hiến máu thành công")
            })
            .Where(d => d.DaHien >= d.ChiTieu)
            .OrderBy(d => d.TenDot)
            .ToListAsync(cancellationToken);

        return raw.Select(d => new DotQuotaAlert(d.MaDot, d.TenDot, d.ChiTieu, d.DaHien)).ToList();
    }

    public Task<List<DotHienMau>> GetAllDotHienMausOrderedAsync(CancellationToken cancellationToken = default) =>
        context.DotHienMaus.OrderByDescending(d => d.NgayBatDauDangKy).ToListAsync(cancellationToken);

    public Task<List<PhieuDangKyHienMau>> GetPhieuSanSangTaoLichAsync(CancellationToken cancellationToken = default) =>
        context.PhieuDangKyHienMaus
            .Include(p => p.NguoiHienMau)
            .Include(p => p.DotHienMau)
            .Include(p => p.LichKhamSangLocs)
            .Where(p => (p.TrangThai == "Đã xác nhận" || p.TrangThai == "Chờ khám sàng lọc") &&
                        !p.LichKhamSangLocs.Any(l => l.TrangThai == "Đã lên lịch"))
            .OrderByDescending(p => p.NgayNop)
            .Take(50)
            .ToListAsync(cancellationToken);

    public Task<List<PhieuDangKyHienMau>> GetPendingResultsAsync(int take = 10, CancellationToken cancellationToken = default) =>
        context.PhieuDangKyHienMaus
            .Include(p => p.NguoiHienMau)
            .Include(p => p.DotHienMau)
            .Where(p => p.TrangThai == "Đã khám sàng lọc" && p.KetQuaHienMau == null)
            .OrderByDescending(p => p.NgayXuLy ?? p.NgayNop)
            .Take(take)
            .ToListAsync(cancellationToken);

    public Task<List<PhieuDangKyHienMau>> GetRecentPhieusAsync(int take = 6, CancellationToken cancellationToken = default) =>
        context.PhieuDangKyHienMaus
            .Include(p => p.NguoiHienMau)
            .Include(p => p.DotHienMau)
            .OrderByDescending(p => p.NgayNop)
            .Take(take)
            .ToListAsync(cancellationToken);

    public Task<List<DotHienMau>> GetRecentDotWithStatsAsync(int take = 8, CancellationToken cancellationToken = default) =>
        context.DotHienMaus
            .Include(d => d.DonViToChuc)
            .Include(d => d.PhieuDangKyHienMaus)
            .OrderByDescending(d => d.NgayBatDauDangKy)
            .Take(take)
            .ToListAsync(cancellationToken);

    public IQueryable<LichKhamSangLoc> GetLichKhamQueryableWithDetails() =>
        context.LichKhamSangLocs
            .Include(l => l.PhieuDangKyHienMau!)
                .ThenInclude(p => p.NguoiHienMau)
            .Include(l => l.PhieuDangKyHienMau!)
                .ThenInclude(p => p.DotHienMau!)
                    .ThenInclude(d => d.DonViToChuc)
            .AsQueryable();

    public Task<int> CountLichKhamAsync(string? trangThai = null, CancellationToken cancellationToken = default) =>
        string.IsNullOrEmpty(trangThai)
            ? context.LichKhamSangLocs.CountAsync(cancellationToken)
            : context.LichKhamSangLocs.CountAsync(l => l.TrangThai == trangThai, cancellationToken);

    public IQueryable<KetQuaHienMau> GetKetQuaQueryableWithDetails() =>
        context.KetQuaHienMaus
            .Include(k => k.PhieuDangKyHienMau!)
                .ThenInclude(p => p.NguoiHienMau)
            .Include(k => k.PhieuDangKyHienMau!)
                .ThenInclude(p => p.DotHienMau!)
                    .ThenInclude(d => d.DonViToChuc)
            .Include(k => k.PhieuDangKyHienMau!)
                .ThenInclude(p => p.LichKhamSangLocs)
            .AsQueryable();

    public Task<int> CountKetQuaAsync(string? ketQua = null, CancellationToken cancellationToken = default) =>
        string.IsNullOrEmpty(ketQua)
            ? context.KetQuaHienMaus.CountAsync(cancellationToken)
            : context.KetQuaHienMaus.CountAsync(k => k.KetQua == ketQua, cancellationToken);

    public Task<KetQuaHienMau?> GetKetQuaDetailsByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.KetQuaHienMaus
            .Include(k => k.PhieuDangKyHienMau!)
                .ThenInclude(p => p.NguoiHienMau!)
                    .ThenInclude(n => n.TaiKhoan)
            .Include(k => k.PhieuDangKyHienMau!)
                .ThenInclude(p => p.DotHienMau!)
                    .ThenInclude(d => d.DonViToChuc)
            .Include(k => k.PhieuDangKyHienMau!)
                .ThenInclude(p => p.LichKhamSangLocs)
            .FirstOrDefaultAsync(k => k.MaKetQuaHienMau == id, cancellationToken);

    public Task<int> CountPhieuAsync(string? trangThai = null, CancellationToken cancellationToken = default) =>
        string.IsNullOrEmpty(trangThai)
            ? context.PhieuDangKyHienMaus.CountAsync(cancellationToken)
            : context.PhieuDangKyHienMaus.CountAsync(p => p.TrangThai == trangThai, cancellationToken);
}

internal sealed class EfPhieuRepositoryTransaction(IDbContextTransaction transaction) : IPhieuRepositoryTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default) => transaction.CommitAsync(cancellationToken);
    public Task RollbackAsync(CancellationToken cancellationToken = default) => transaction.RollbackAsync(cancellationToken);
    public ValueTask DisposeAsync() => transaction.DisposeAsync();
}

