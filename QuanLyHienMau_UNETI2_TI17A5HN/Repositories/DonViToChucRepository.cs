// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Module 1 - Triển khai Repository cho Đơn vị tổ chức

using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories
{
    public class DonViToChucRepository : Repository<DonViToChuc>, IDonViToChucRepository
    {
        public DonViToChucRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DonViToChuc>> GetAllWithDotHienMauAsync(string? search = null, string? trangThai = null)
        {
            var query = _dbSet
                .Include(d => d.DotHienMaus)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(d => d.TenDonViToChuc.ToLower().Contains(searchLower) ||
                                         (d.EmailLienHe != null && d.EmailLienHe.ToLower().Contains(searchLower)) ||
                                         (d.MoTa != null && d.MoTa.ToLower().Contains(searchLower)));
            }

            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "all")
            {
                query = query.Where(d => d.TrangThai == trangThai);
            }

            return await query.OrderByDescending(d => d.MaDonViToChuc).ToListAsync();
        }

        public async Task<DonViToChuc?> GetByIdWithDotHienMauAsync(int id)
        {
            return await _dbSet
                .Include(d => d.DotHienMaus)
                .FirstOrDefaultAsync(d => d.MaDonViToChuc == id);
        }

        public async Task<bool> IsTenDonViExistsAsync(string tenDonVi, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(tenDonVi)) return false;
            var clean = tenDonVi.Trim().ToLower();
            var query = _dbSet.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.MaDonViToChuc != excludeId.Value);
            }
            return await query.AnyAsync(d => d.TenDonViToChuc.ToLower() == clean);
        }

        public async Task<bool> HasDotHienMauAsync(int maDonViToChuc)
        {
            return await _context.DotHienMaus.AnyAsync(d => d.MaDonViToChuc == maDonViToChuc);
        }
    }
}
