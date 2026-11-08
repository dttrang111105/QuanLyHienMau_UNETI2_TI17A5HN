// ============================================================================
// DỰ ÁN QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VITALIS - UNETI (TI17A5HN)
// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Phụ trách: Nhóm thực hiện Module 3 - Lớp DHTI17A5HN
// ============================================================================

using Microsoft.EntityFrameworkCore.Storage;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces
{
    /// <summary>
    /// Specific Repository Interface cho thực thể PhieuDangKyHienMau và các truy vấn nghiệp vụ đăng ký hiến máu thuộc Module 3.
    /// </summary>
    public interface IPhieuDangKyRepository : IRepository<PhieuDangKyHienMau>
    {
        /// <summary>
        /// Lấy thông tin đợt hiến máu theo mã đợt kèm thông tin đơn vị tổ chức.
        /// </summary>
        Task<DotHienMau?> GetDotHienMauWithDonViByIdAsync(int maDot);

        /// <summary>
        /// Lấy đợt hiến máu đang mở đăng ký mới nhất tính tại ngày truyền vào kèm đơn vị tổ chức.
        /// </summary>
        Task<DotHienMau?> GetLatestOpenDotHienMauAsync(DateTime referenceDate);

        /// <summary>
        /// Lấy toàn bộ lịch sử phiếu đăng ký của một người hiến máu kèm KetQuaHienMau và DotHienMau (phục vụ kiểm tra quy tắc 84 ngày và chống đăng ký trùng).
        /// </summary>
        Task<List<PhieuDangKyHienMau>> GetHistoryByNguoiHienAsync(int maNguoiHien);

        /// <summary>
        /// Lấy danh sách phiếu đăng ký của một người hiến máu kèm đầy đủ thông tin chi tiết (DotHienMau.DonViToChuc, NguoiHienMau, LichKhamSangLocs, KetQuaHienMau), sắp xếp theo NgayNop giảm dần.
        /// </summary>
        Task<List<PhieuDangKyHienMau>> GetTrackingListByNguoiHienAsync(int maNguoiHien);

        /// <summary>
        /// Lấy danh sách các phiếu đăng ký mới nhất toàn hệ thống (dành cho Admin / Nhân viên y tế trên màn hình TheoDoi).
        /// </summary>
        Task<List<PhieuDangKyHienMau>> GetRecentTrackingListForStaffAsync(int count = 30);

        /// <summary>
        /// Lấy chi tiết một phiếu đăng ký theo mã phiếu kèm đầy đủ thông tin (DotHienMau.DonViToChuc, NguoiHienMau, LichKhamSangLocs, KetQuaHienMau).
        /// </summary>
        Task<PhieuDangKyHienMau?> GetByIdWithTrackingDetailsAsync(int maPhieu);

        /// <summary>
        /// Lấy thông tin phiếu đăng ký theo mã phiếu kèm LichKhamSangLocs và KetQuaHienMau (phục vụ kiểm tra điều kiện hủy phiếu).
        /// </summary>
        Task<PhieuDangKyHienMau?> GetByIdForCancelAsync(int maPhieu);

        /// <summary>
        /// Khởi tạo giao dịch cơ sở dữ liệu với mức cô lập IsolationLevel.Serializable để chống Race Condition khi nộp phiếu đồng thời.
        /// </summary>
        Task<IDbContextTransaction> BeginSerializableTransactionAsync();
    }
}
