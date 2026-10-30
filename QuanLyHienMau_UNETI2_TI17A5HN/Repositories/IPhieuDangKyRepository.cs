using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

public sealed record PhieuDangKyFilter(
    string? TenNguoiHien,
    string? TenDot,
    int? MaDonViToChuc,
    string? TrangThai,
    DateTime? TuNgay,
    DateTime? DenNgay,
    string? SapXep,
    string? ChieuSapXep,
    int Page,
    int PageSize);

public sealed record PhieuTrangThaiCounts(
    int Tong,
    int ChoXacNhan,
    int DaXacNhan,
    int ChoKham,
    int DaKham,
    int TuChoi,
    int DaHuy);

public sealed record PhieuDangKyPage(
    IReadOnlyList<PhieuDangKyHienMau> Items,
    IReadOnlyList<DonViToChuc> DonViToChucs,
    PhieuTrangThaiCounts Counts,
    int TotalItems,
    int TotalPages,
    int Page,
    string SapXep,
    string ChieuSapXep);

public sealed record PhieuDangKyDetails(
    PhieuDangKyHienMau Phieu,
    IReadOnlyList<PhieuDangKyHienMau> LichSu,
    int SoPhieuThanhCongDot);

public interface IPhieuDangKyRepository
{
    Task<PhieuDangKyPage> GetPageAsync(PhieuDangKyFilter filter, CancellationToken cancellationToken = default);
    Task<PhieuDangKyDetails?> GetDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<PhieuDangKyHienMau?> GetForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<KetQuaHienMau?> GetKetQuaForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<LichKhamSangLoc?> GetLichKhamForUpdateAsync(int id, CancellationToken cancellationToken = default);
    void AddLichKham(LichKhamSangLoc lichKham);
    Task<bool> HasLichKhamHoanThanhAsync(int maPhieu, CancellationToken cancellationToken = default);
    Task<int> CountHienMauThanhCongAsync(int maDot, CancellationToken cancellationToken = default);
    void AddKetQua(KetQuaHienMau ketQua);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IPhieuRepositoryTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable, CancellationToken cancellationToken = default);
}

public interface IPhieuRepositoryTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

