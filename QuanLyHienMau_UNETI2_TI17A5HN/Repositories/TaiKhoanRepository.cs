// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Module 1 - Triển khai Repository cho Tài khoản, Đăng nhập, Đăng ký & Phân quyền

using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories
{
    public class TaiKhoanRepository : Repository<TaiKhoan>, ITaiKhoanRepository
    {
        public TaiKhoanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return null;
            var clean = tenDangNhap.Trim();
            return await _dbSet
                .AsNoTracking()
                .Include(t => t.NguoiHienMau)
                .FirstOrDefaultAsync(t => t.TenDangNhap == clean);
        }

        public async Task<TaiKhoan?> FindForLoginAsync(string usernameOrEmailOrPhone)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmailOrPhone)) return null;
            var trimmed = usernameOrEmailOrPhone.Trim();

            return await _dbSet
                .AsNoTracking()
                .Include(t => t.NguoiHienMau)
                .FirstOrDefaultAsync(t => t.TenDangNhap == trimmed ||
                                          (t.Email != null && t.Email == trimmed) ||
                                          (t.NguoiHienMau != null && t.NguoiHienMau.SoDienThoai == trimmed));
        }

        public async Task<bool> IsTenDangNhapExistsAsync(string tenDangNhap, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return false;
            var clean = tenDangNhap.Trim();
            var query = _dbSet.AsNoTracking();
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.MaTaiKhoan != excludeId.Value);
            }
            return await query.AnyAsync(t => t.TenDangNhap == clean);
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var clean = email.Trim();
            var query = _dbSet.AsNoTracking();
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.MaTaiKhoan != excludeId.Value);
            }
            return await query.AnyAsync(t => t.Email != null && t.Email == clean);
        }

        public async Task<bool> IsSoDienThoaiExistsAsync(string soDienThoai, int? excludeMaTaiKhoan = null)
        {
            if (string.IsNullOrWhiteSpace(soDienThoai)) return false;
            var clean = soDienThoai.Trim();
            var query = _context.NguoiHienMaus.AsNoTracking();
            if (excludeMaTaiKhoan.HasValue)
            {
                query = query.Where(n => n.MaTaiKhoan != excludeMaTaiKhoan.Value);
            }
            return await query.AnyAsync(n => n.SoDienThoai != null && n.SoDienThoai == clean);
        }

        public async Task<IEnumerable<TaiKhoan>> GetAllWithProfileAsync(string? search = null, string? vaiTro = null, string? trangThai = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(t => t.NguoiHienMau)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();
                query = query.Where(t => t.TenDangNhap.Contains(keyword) ||
                                         t.HoTen.Contains(keyword) ||
                                         (t.Email != null && t.Email.Contains(keyword)) ||
                                         (t.NguoiHienMau != null && t.NguoiHienMau.SoDienThoai != null && t.NguoiHienMau.SoDienThoai.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(vaiTro) && vaiTro != "all")
            {
                query = query.Where(t => t.VaiTro == vaiTro);
            }

            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "all")
            {
                query = query.Where(t => t.TrangThai == trangThai);
            }

            return await query.OrderByDescending(t => t.MaTaiKhoan).ToListAsync();
        }

        public async Task<TaiKhoan?> GetByIdWithProfileAsync(int id)
        {
            return await _dbSet
                .Include(t => t.NguoiHienMau)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
        }

        public async Task<AccountStats> GetAccountStatsAsync()
        {
            var grouped = await _dbSet
                .AsNoTracking()
                .GroupBy(t => new { t.VaiTro, t.TrangThai })
                .Select(g => new { g.Key.VaiTro, g.Key.TrangThai, Count = g.Count() })
                .ToListAsync();

            int total = grouped.Sum(x => x.Count);
            int admin = grouped.Where(x => x.VaiTro == "Admin").Sum(x => x.Count);
            int staff = grouped.Where(x => x.VaiTro == "Medical Staff").Sum(x => x.Count);
            int donor = grouped.Where(x => x.VaiTro == "Donor").Sum(x => x.Count);
            int locked = grouped.Where(x => x.TrangThai == "Bị khóa").Sum(x => x.Count);
            int active = total - locked;

            return new AccountStats(total, admin, staff, donor, active, locked);
        }

        public async Task<bool> RegisterDonorAccountAsync(TaiKhoan account, NguoiHienMau? donorProfile)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _dbSet.AddAsync(account);
                await _context.SaveChangesAsync();

                if (donorProfile != null)
                {
                    donorProfile.MaTaiKhoan = account.MaTaiKhoan;
                    if (string.IsNullOrWhiteSpace(donorProfile.HoTen))
                    {
                        donorProfile.HoTen = account.HoTen;
                    }
                    if (string.IsNullOrWhiteSpace(donorProfile.Email))
                    {
                        donorProfile.Email = account.Email;
                    }
                    donorProfile.TrangThai = account.TrangThai ?? "Hoạt động";
                    await _context.NguoiHienMaus.AddAsync(donorProfile);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAccountStatusAsync(int id, string newStatus)
        {
            var account = await _dbSet
                .Include(t => t.NguoiHienMau)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);

            if (account == null) return false;

            account.TrangThai = newStatus;
            if (account.NguoiHienMau != null)
            {
                account.NguoiHienMau.TrangThai = newStatus;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangePasswordAsync(int id, string hashedPassword)
        {
            var account = await _dbSet.FindAsync(id);
            if (account == null) return false;
            account.MatKhau = hashedPassword;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> CountByRoleAsync(string vaiTro)
        {
            return await _dbSet.CountAsync(t => t.VaiTro == vaiTro);
        }

        public async Task<int> CountByStatusAsync(string trangThai)
        {
            return await _dbSet.CountAsync(t => t.TrangThai == trangThai);
        }
    }
}
