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
            query = query.Where(p => p.NguoiHienMau != null && p.NguoiHienMau.HoTen.Contains(name));
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
}

internal sealed class EfPhieuRepositoryTransaction(IDbContextTransaction transaction) : IPhieuRepositoryTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default) => transaction.CommitAsync(cancellationToken);
    public Task RollbackAsync(CancellationToken cancellationToken = default) => transaction.RollbackAsync(cancellationToken);
    public ValueTask DisposeAsync() => transaction.DisposeAsync();
}

