// Họ và tên: NGUYỄN THỊ PHƯƠNG THẢO
// Mã sinh viên: 23103100272
// Nội dung thực hiện: Quản lý đợt hiến máu,
// tìm kiếm, lọc, sắp xếp và phân trang.
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories
{
    /// <summary>
    /// Interface Repository cho Module 2 - Quản lý Đợt Hiến Máu
    /// Định nghĩa các phương thức CRUD, tìm kiếm, lọc, sắp xếp và phân trang
    /// </summary>
    public interface IDotHienMauRepository
    {
        // ─── CRUD ───────────────────────────────────────────────────────────────

        /// <summary>Lấy toàn bộ truy vấn đợt hiến máu (có Include DonViToChuc)</summary>
        IQueryable<DotHienMau> GetQueryable();

        /// <summary>Lấy đợt hiến máu theo mã</summary>
        Task<DotHienMau?> GetByIdAsync(int id);

        /// <summary>Thêm đợt hiến máu mới</summary>
        Task AddAsync(DotHienMau dotHienMau);

        /// <summary>Cập nhật đợt hiến máu</summary>
        Task UpdateAsync(DotHienMau dotHienMau);

        /// <summary>Xóa đợt hiến máu</summary>
        Task DeleteAsync(DotHienMau dotHienMau);

        /// <summary>Lưu thay đổi</summary>
        Task SaveChangesAsync();

        // ─── KIỂM TRA NGHIỆP VỤ ────────────────────────────────────────────────

        /// <summary>Kiểm tra đợt hiến máu có phiếu đăng ký chưa (để chặn xóa)</summary>
        Task<bool> HasPhieuDangKyAsync(int maDot);

        // ─── TÌM KIẾM / LỌC / SẮP XẾP ─────────────────────────────────────────

        /// <summary>
        /// Áp dụng bộ lọc tìm kiếm (theo tên đợt hoặc tên đơn vị tổ chức)
        /// </summary>
        IQueryable<DotHienMau> ApplySearch(IQueryable<DotHienMau> query, string? search);

        /// <summary>Lọc theo đơn vị tổ chức</summary>
        IQueryable<DotHienMau> ApplyFilterDonVi(IQueryable<DotHienMau> query, int? donViId);

        /// <summary>Lọc theo nhóm máu ưu tiên</summary>
        IQueryable<DotHienMau> ApplyFilterNhomMau(IQueryable<DotHienMau> query, string? nhomMau);

        /// <summary>Lọc theo trạng thái</summary>
        IQueryable<DotHienMau> ApplyFilterTrangThai(IQueryable<DotHienMau> query, string? status);

        /// <summary>Lọc theo cân nặng tối thiểu</summary>
        IQueryable<DotHienMau> ApplyFilterCanNang(IQueryable<DotHienMau> query, double? canNang);

        /// <summary>Lọc theo còn hạn / hết hạn</summary>
        IQueryable<DotHienMau> ApplyFilterThoiHan(IQueryable<DotHienMau> query, string? thoiHan);

        /// <summary>Áp dụng sắp xếp</summary>
        IQueryable<DotHienMau> ApplySort(IQueryable<DotHienMau> query, string? sort);

        // ─── PHÂN TRANG ─────────────────────────────────────────────────────────

        /// <summary>Đếm tổng số bản ghi sau khi lọc/tìm kiếm</summary>
        Task<int> CountAsync(IQueryable<DotHienMau> query);

        /// <summary>Lấy dữ liệu phân trang bằng Skip/Take trên EF Core LINQ</summary>
        Task<List<DotHienMau>> GetPagedAsync(IQueryable<DotHienMau> query, int page, int pageSize);

        // ─── DROPDOWN ───────────────────────────────────────────────────────────

        /// <summary>Lấy danh sách đơn vị tổ chức đang hoạt động (cho dropdown)</summary>
        Task<List<DonViToChuc>> GetActiveDonViToChucsAsync();
    }
}
