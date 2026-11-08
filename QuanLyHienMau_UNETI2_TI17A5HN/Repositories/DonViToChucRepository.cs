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
                .AsNoTracking()
                .Include(d => d.DotHienMaus)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();
                query = query.Where(d => d.TenDonViToChuc.Contains(keyword) ||
                                         (d.EmailLienHe != null && d.EmailLienHe.Contains(keyword)) ||
                                         (d.MoTa != null && d.MoTa.Contains(keyword)));
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
                .AsNoTracking()
                .Include(d => d.DotHienMaus)
                .FirstOrDefaultAsync(d => d.MaDonViToChuc == id);
        }

        public async Task<DonViStats> GetDonViStatsAsync()
        {
            var statusCounts = await _dbSet
                .AsNoTracking()
                .GroupBy(d => d.TrangThai)
                .Select(g => new { TrangThai = g.Key, Count = g.Count() })
                .ToListAsync();

            int total = statusCounts.Sum(x => x.Count);
            int active = statusCounts.Where(x => x.TrangThai == "Hoạt động").Sum(x => x.Count);
            int inactive = total - active;
            int totalDot = await _context.DotHienMaus.CountAsync();

            return new DonViStats(total, active, inactive, totalDot);
        }

        public async Task<bool> IsTenDonViExistsAsync(string tenDonVi, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(tenDonVi)) return false;
            var clean = tenDonVi.Trim();
            var query = _dbSet.AsNoTracking();
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.MaDonViToChuc != excludeId.Value);
            }
            return await query.AnyAsync(d => d.TenDonViToChuc == clean);
        }

        public async Task<bool> HasDotHienMauAsync(int maDonViToChuc)
        {
            return await _context.DotHienMaus.AnyAsync(d => d.MaDonViToChuc == maDonViToChuc);
        }
    }
}
