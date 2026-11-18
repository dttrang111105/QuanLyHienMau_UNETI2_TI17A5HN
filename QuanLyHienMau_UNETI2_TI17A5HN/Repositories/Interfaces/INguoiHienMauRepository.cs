// ============================================================================
// DỰ ÁN QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VITALIS - UNETI (TI17A5HN)
// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Họ và tên: Đàm Thu Trang
// Mã sinh viên: 23103100243
// Nội dung thực hiện: Module 3 - Quản lý người hiến máu & Phiếu đăng ký hiến máu
// ============================================================================

using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces
{
    /// <summary>
    /// Specific Repository Interface cho thực thể NguoiHienMau thuộc Module 3.
    /// </summary>
    public interface INguoiHienMauRepository : IRepository<NguoiHienMau>
    {
        /// <summary>
        /// Tìm thông tin tài khoản theo mã tài khoản (phục vụ kiểm tra trạng thái hoạt động và khởi tạo hồ sơ).
        /// </summary>
        Task<TaiKhoan?> GetTaiKhoanByIdAsync(int maTaiKhoan);

        /// <summary>
        /// Lấy hồ sơ người hiến máu theo mã tài khoản (không kèm lịch sử phiếu).
        /// </summary>
        Task<NguoiHienMau?> GetByTaiKhoanAsync(int maTaiKhoan);

        /// <summary>
        /// Lấy hồ sơ người hiến máu theo mã tài khoản kèm thực thể TaiKhoan liên kết (phục vụ cập nhật và đồng bộ thông tin).
        /// </summary>
        Task<NguoiHienMau?> GetByTaiKhoanWithAccountAsync(int maTaiKhoan);

        /// <summary>
        /// Lấy hồ sơ người hiến máu theo mã tài khoản kèm toàn bộ lịch sử phiếu đăng ký, đợt hiến máu, đơn vị tổ chức, lịch khám và kết quả hiến máu.
        /// </summary>
        Task<NguoiHienMau?> GetProfileWithFullHistoryAsync(int maTaiKhoan);

        /// <summary>
        /// Kiểm tra xem địa chỉ email đã được sử dụng bởi một tài khoản khác (khác maTaiKhoan hiện tại) hay chưa.
        /// </summary>
        Task<bool> IsEmailTakenByOtherAccountAsync(int currentMaTaiKhoan, string email);

        /// <summary>
        /// Nạp tường minh (Explicit Loading) danh sách PhieuDangKyHienMaus và các quan hệ liên quan cho thực thể NguoiHienMau đang được theo dõi.
        /// </summary>
        Task LoadDonationHistoryCollectionAsync(NguoiHienMau nguoiHienMau);

        /// <summary>
        /// Truy vấn danh sách người hiến máu kèm tài khoản và lịch sử phiếu đăng ký/kết quả (phục vụ Admin).
        /// </summary>
        IQueryable<NguoiHienMau> GetQueryableWithDetails();

        /// <summary>
        /// Lấy hồ sơ người hiến máu theo mã người hiến kèm tài khoản liên kết (phục vụ Khóa/Mở khóa).
        /// </summary>
        Task<NguoiHienMau?> GetByIdWithTaiKhoanAsync(int maNguoiHien);
    }
}
