// ============================================================================
// DỰ ÁN QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VITALIS - UNETI (TI17A5HN)
// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Phụ trách: Nhóm thực hiện Module 3 - Lớp DHTI17A5HN
// ============================================================================

using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories
{
    /// <summary>
    /// Triển khai Specific Repository cho thực thể PhieuDangKyHienMau thuộc Module 3.
    /// </summary>
    public class PhieuDangKyRepository : Repository<PhieuDangKyHienMau>, IPhieuDangKyRepository
    {
        public PhieuDangKyRepository(ApplicationDbContext context) : base(context)
        {
        }

        private IQueryable<PhieuDangKyHienMau> BuildTrackingQuery()
        {
            return _context.PhieuDangKyHienMaus
                .Include(p => p.DotHienMau!)
                    .ThenInclude(d => d.DonViToChuc)
                .Include(p => p.NguoiHienMau)
                .Include(p => p.LichKhamSangLocs)
                .Include(p => p.KetQuaHienMau);
        }

        public async Task<DotHienMau?> GetDotHienMauWithDonViByIdAsync(int maDot)
        {
            return await _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .FirstOrDefaultAsync(d => d.MaDot == maDot);
        }

        public async Task<DotHienMau?> GetLatestOpenDotHienMauAsync(DateTime referenceDate)
        {
            var today = referenceDate.Date;
            return await _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .Where(d => d.TrangThai == "Đang mở đăng ký" && d.NgayBatDauDangKy <= today && d.HanDangKy >= today)
                .OrderByDescending(d => d.NgayBatDauDangKy)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PhieuDangKyHienMau>> GetHistoryByNguoiHienAsync(int maNguoiHien)
        {
            return await _context.PhieuDangKyHienMaus
                .Include(p => p.KetQuaHienMau)
                .Include(p => p.DotHienMau)
                .Where(p => p.MaNguoiHien == maNguoiHien)
                .ToListAsync();
        }

        public async Task<List<PhieuDangKyHienMau>> GetTrackingListByNguoiHienAsync(int maNguoiHien)
        {
            return await BuildTrackingQuery()
                .Where(p => p.MaNguoiHien == maNguoiHien)
                .OrderByDescending(p => p.NgayNop)
                .ToListAsync();
        }

        public async Task<List<PhieuDangKyHienMau>> GetRecentTrackingListForStaffAsync(int count = 30)
        {
            return await BuildTrackingQuery()
                .OrderByDescending(p => p.NgayNop)
                .Take(count)
                .ToListAsync();
        }

        public async Task<PhieuDangKyHienMau?> GetByIdWithTrackingDetailsAsync(int maPhieu)
        {
            return await BuildTrackingQuery()
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);
        }

        public async Task<PhieuDangKyHienMau?> GetByIdForCancelAsync(int maPhieu)
        {
            return await _context.PhieuDangKyHienMaus
                .Include(p => p.LichKhamSangLocs)
                .Include(p => p.KetQuaHienMau)
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);
        }

        public async Task<IDbContextTransaction> BeginSerializableTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        }
    }
}
