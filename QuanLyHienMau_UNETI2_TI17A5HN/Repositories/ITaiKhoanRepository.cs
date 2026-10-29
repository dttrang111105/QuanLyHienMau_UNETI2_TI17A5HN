// Họ và tên: Nguyễn Tiến Nam
// Mã sinh viên: 23103100275
// Nội dung thực hiện: Module 1 - Interface Repository quản lý Tài khoản, Đăng nhập, Đăng ký & Phân quyền

using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories
{
    public interface ITaiKhoanRepository : IRepository<TaiKhoan>
    {
        Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap);
        Task<TaiKhoan?> FindForLoginAsync(string usernameOrEmailOrPhone);
        Task<bool> IsTenDangNhapExistsAsync(string tenDangNhap, int? excludeId = null);
        Task<bool> IsEmailExistsAsync(string email, int? excludeId = null);
        Task<IEnumerable<TaiKhoan>> GetAllWithProfileAsync(string? search = null, string? vaiTro = null, string? trangThai = null);
        Task<TaiKhoan?> GetByIdWithProfileAsync(int id);
        Task<bool> RegisterDonorAccountAsync(TaiKhoan account, NguoiHienMau? donorProfile);
        Task<bool> UpdateAccountStatusAsync(int id, string newStatus);
        Task<bool> ChangePasswordAsync(int id, string hashedPassword);
        Task<int> CountByRoleAsync(string vaiTro);
        Task<int> CountByStatusAsync(string trangThai);
    }
}
