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
            var clean = tenDangNhap.Trim().ToLower();
            return await _dbSet
                .Include(t => t.NguoiHienMau)
                .FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == clean);
        }

        public async Task<TaiKhoan?> FindForLoginAsync(string usernameOrEmailOrPhone)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmailOrPhone)) return null;
            var trimmed = usernameOrEmailOrPhone.Trim();
            var trimmedLower = trimmed.ToLower();

            return await _dbSet
                .Include(t => t.NguoiHienMau)
                .FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == trimmedLower ||
                                         (t.Email != null && t.Email.ToLower() == trimmedLower) ||
                                         (t.NguoiHienMau != null && t.NguoiHienMau.SoDienThoai == trimmed));
        }

        public async Task<bool> IsTenDangNhapExistsAsync(string tenDangNhap, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return false;
            var clean = tenDangNhap.Trim().ToLower();
            var query = _dbSet.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.MaTaiKhoan != excludeId.Value);
            }
            return await query.AnyAsync(t => t.TenDangNhap.ToLower() == clean);
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var clean = email.Trim().ToLower();
            var query = _dbSet.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.MaTaiKhoan != excludeId.Value);
            }
            return await query.AnyAsync(t => t.Email != null && t.Email.ToLower() == clean);
        }

        public async Task<IEnumerable<TaiKhoan>> GetAllWithProfileAsync(string? search = null, string? vaiTro = null, string? trangThai = null)
        {
            var query = _dbSet
                .Include(t => t.NguoiHienMau)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(searchLower) ||
                                         t.HoTen.ToLower().Contains(searchLower) ||
                                         (t.Email != null && t.Email.ToLower().Contains(searchLower)) ||
                                         (t.NguoiHienMau != null && t.NguoiHienMau.SoDienThoai != null && t.NguoiHienMau.SoDienThoai.Contains(searchLower)));
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
            var account = await _dbSet.FindAsync(id);
            if (account == null) return false;
            account.TrangThai = newStatus;
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
