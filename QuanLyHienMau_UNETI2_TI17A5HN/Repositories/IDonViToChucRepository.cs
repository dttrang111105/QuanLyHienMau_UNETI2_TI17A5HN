// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Module 1 - Interface Repository quản lý Đơn vị tổ chức

using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories
{
    public interface IDonViToChucRepository : IRepository<DonViToChuc>
    {
        Task<IEnumerable<DonViToChuc>> GetAllWithDotHienMauAsync(string? search = null, string? trangThai = null);
        Task<DonViToChuc?> GetByIdWithDotHienMauAsync(int id);
        Task<bool> IsTenDonViExistsAsync(string tenDonVi, int? excludeId = null);
        Task<bool> HasDotHienMauAsync(int maDonViToChuc);
    }
}
