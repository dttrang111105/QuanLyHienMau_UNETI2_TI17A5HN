// ============================================================================
// DỰ ÁN QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VITALIS - UNETI (TI17A5HN)
// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Phụ trách: Nhóm thực hiện Module 3 - Lớp DHTI17A5HN
// ============================================================================

using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories
{
    /// <summary>
    /// Triển khai Specific Repository cho thực thể NguoiHienMau thuộc Module 3.
    /// </summary>
    public class NguoiHienMauRepository : Repository<NguoiHienMau>, INguoiHienMauRepository
    {
        public NguoiHienMauRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TaiKhoan?> GetTaiKhoanByIdAsync(int maTaiKhoan)
        {
            return await _context.TaiKhoans.FindAsync(maTaiKhoan);
        }

        public async Task<NguoiHienMau?> GetByTaiKhoanAsync(int maTaiKhoan)
        {
            return await _context.NguoiHienMaus
                .FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan);
        }

        public async Task<NguoiHienMau?> GetByTaiKhoanWithAccountAsync(int maTaiKhoan)
        {
            return await _context.NguoiHienMaus
                .Include(n => n.TaiKhoan)
                .FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan);
        }

        public async Task<NguoiHienMau?> GetProfileWithFullHistoryAsync(int maTaiKhoan)
        {
            return await _context.NguoiHienMaus
                .Include(n => n.TaiKhoan)
                .Include(n => n.PhieuDangKyHienMaus)
                    .ThenInclude(p => p.DotHienMau!)
                        .ThenInclude(d => d.DonViToChuc)
                .Include(n => n.PhieuDangKyHienMaus)
                    .ThenInclude(p => p.LichKhamSangLocs)
                .Include(n => n.PhieuDangKyHienMaus)
                    .ThenInclude(p => p.KetQuaHienMau)
                .FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan);
        }

        public async Task<bool> IsEmailTakenByOtherAccountAsync(int currentMaTaiKhoan, string email)
        {
            var normalizedEmail = email.ToLower();
            return await _context.TaiKhoans
                .AnyAsync(t => t.MaTaiKhoan != currentMaTaiKhoan && t.Email != null && t.Email.ToLower() == normalizedEmail);
        }

        public async Task LoadDonationHistoryCollectionAsync(NguoiHienMau nguoiHienMau)
        {
            if (nguoiHienMau.MaNguoiHien > 0)
            {
                await _context.Entry(nguoiHienMau)
                    .Collection(n => n.PhieuDangKyHienMaus)
                    .Query()
                    .Include(p => p.DotHienMau!)
                        .ThenInclude(d => d.DonViToChuc)
                    .Include(p => p.LichKhamSangLocs)
                    .Include(p => p.KetQuaHienMau)
                    .LoadAsync();
            }
        }
    }
}
