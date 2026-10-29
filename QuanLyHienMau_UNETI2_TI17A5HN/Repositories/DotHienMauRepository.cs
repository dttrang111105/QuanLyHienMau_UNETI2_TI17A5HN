// Họ và tên: NGUYỄN THỊ PHƯƠNG THẢO
// Mã sinh viên: 23103100272
// Nội dung thực hiện: Quản lý đợt hiến máu,
// tìm kiếm, lọc, sắp xếp và phân trang.
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories
{
    /// <summary>
    /// Triển khai Repository Pattern cho Module 2 - Quản lý Đợt Hiến Máu.
    /// Toàn bộ truy vấn dữ liệu sử dụng EF Core 10 + LINQ.
    /// Controller chỉ phụ thuộc vào interface IDotHienMauRepository,
    /// không trực tiếp sử dụng ApplicationDbContext.
    /// </summary>
    public class DotHienMauRepository : IDotHienMauRepository
    {
        private readonly ApplicationDbContext _context;

        public DotHienMauRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  CRUD
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Trả về IQueryable có Include DonViToChuc để controller/repository
        /// áp dụng tiếp các bộ lọc/sắp xếp mà chưa thực thi SQL ngay.
        /// </summary>
        public IQueryable<DotHienMau> GetQueryable()
        {
            return _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .AsQueryable();
        }

        /// <summary>Lấy một đợt hiến máu theo mã, kèm đơn vị tổ chức.</summary>
        public async Task<DotHienMau?> GetByIdAsync(int id)
        {
            return await _context.DotHienMaus
                .Include(d => d.DonViToChuc)
                .FirstOrDefaultAsync(d => d.MaDot == id);
        }

        /// <summary>Thêm đợt hiến máu mới vào DbContext (chưa SaveChanges).</summary>
        public async Task AddAsync(DotHienMau dotHienMau)
        {
            await _context.DotHienMaus.AddAsync(dotHienMau);
        }

        /// <summary>
        /// Cập nhật các trường được phép sửa cho đợt hiến máu.
        /// Không ghi đè TrangThai qua action Edit, chỉ dùng ChangeStatus.
        /// </summary>
        public Task UpdateAsync(DotHienMau dotHienMau)
        {
            // Entity đã được track bởi EF Core,
            // chỉ cần đánh dấu Modified để SaveChanges ghi xuống DB.
            _context.Entry(dotHienMau).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        /// <summary>Xóa đợt hiến máu khỏi DbContext (chưa SaveChanges).</summary>
        public Task DeleteAsync(DotHienMau dotHienMau)
        {
            _context.DotHienMaus.Remove(dotHienMau);
            return Task.CompletedTask;
        }

        /// <summary>Ghi toàn bộ thay đổi xuống SQL Server.</summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  KIỂM TRA NGHIỆP VỤ
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Kiểm tra đợt hiến máu đã có phiếu đăng ký chưa.
        /// Dùng LINQ Any() để tránh tải dữ liệu không cần thiết.
        /// Nếu true thì không cho phép xóa (tránh mất dữ liệu lịch sử).
        /// </summary>
        public async Task<bool> HasPhieuDangKyAsync(int maDot)
        {
            return await _context.PhieuDangKyHienMaus
                .AnyAsync(p => p.MaDot == maDot);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  TÌM KIẾM / LỌC / SẮP XẾP (áp dụng trên IQueryable, chưa thực thi SQL)
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Tìm kiếm theo tên đợt hiến máu hoặc tên đơn vị tổ chức bằng LINQ Contains.
        /// So sánh không phân biệt hoa thường.
        /// </summary>
        public IQueryable<DotHienMau> ApplySearch(IQueryable<DotHienMau> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var keyword = search.Trim().ToLower();

            return query.Where(d =>
                d.TenDot.ToLower().Contains(keyword) ||
                (d.DonViToChuc != null &&
                 d.DonViToChuc.TenDonViToChuc.ToLower().Contains(keyword)));
        }

        /// <summary>Lọc theo mã đơn vị tổ chức.</summary>
        public IQueryable<DotHienMau> ApplyFilterDonVi(IQueryable<DotHienMau> query, int? donViId)
        {
            if (!donViId.HasValue || donViId.Value <= 0)
                return query;

            return query.Where(d => d.MaDonViToChuc == donViId.Value);
        }

        /// <summary>Lọc theo nhóm máu ưu tiên (Contains để khớp chuỗi như "O, A").</summary>
        public IQueryable<DotHienMau> ApplyFilterNhomMau(IQueryable<DotHienMau> query, string? nhomMau)
        {
            if (string.IsNullOrWhiteSpace(nhomMau))
                return query;

            return query.Where(d => d.NhomMauUuTien != null &&
                                    d.NhomMauUuTien.Contains(nhomMau));
        }

        /// <summary>Lọc theo trạng thái đợt hiến máu.</summary>
        public IQueryable<DotHienMau> ApplyFilterTrangThai(IQueryable<DotHienMau> query, string? status)
        {
            if (string.IsNullOrWhiteSpace(status) || status == "all")
                return query;

            return query.Where(d => d.TrangThai == status);
        }

        /// <summary>
        /// Lọc theo cân nặng tối thiểu của đợt:
        /// chỉ hiển thị đợt có yêu cầu cân nặng <= cân nặng người nhập
        /// (nghĩa là người dùng đủ điều kiện).
        /// </summary>
        public IQueryable<DotHienMau> ApplyFilterCanNang(IQueryable<DotHienMau> query, double? canNang)
        {
            if (!canNang.HasValue || canNang.Value <= 0)
                return query;

            return query.Where(d => d.CanNangToiThieu <= canNang.Value);
        }

        /// <summary>
        /// Lọc còn hạn (HanDangKy >= hôm nay) hoặc hết hạn (HanDangKy < hôm nay).
        /// Giúp người hiến máu tránh nộp nhầm vào đợt đã quá hạn.
        /// </summary>
        public IQueryable<DotHienMau> ApplyFilterThoiHan(IQueryable<DotHienMau> query, string? thoiHan)
        {
            var today = DateTime.Today;

            return thoiHan switch
            {
                "conhan"  => query.Where(d => d.HanDangKy >= today),
                "hethan"  => query.Where(d => d.HanDangKy < today),
                _         => query
            };
        }

        /// <summary>
        /// Sắp xếp theo các tiêu chí:
        /// name_asc / name_desc / deadline_asc / deadline_desc / quota_asc / quota_desc.
        /// Mặc định sắp xếp theo NgayBatDauDangKy giảm dần (mới nhất lên trên).
        /// </summary>
        public IQueryable<DotHienMau> ApplySort(IQueryable<DotHienMau> query, string? sort)
        {
            return sort switch
            {
                "name_asc"      => query.OrderBy(d => d.TenDot),
                "name_desc"     => query.OrderByDescending(d => d.TenDot),
                "deadline_asc"  => query.OrderBy(d => d.HanDangKy),
                "deadline_desc" => query.OrderByDescending(d => d.HanDangKy),
                "quota_asc"     => query.OrderBy(d => d.ChiTieuDonViMau),
                "quota_desc"    => query.OrderByDescending(d => d.ChiTieuDonViMau),
                _               => query.OrderByDescending(d => d.NgayBatDauDangKy)
            };
        }

        // ════════════════════════════════════════════════════════════════════════
        //  PHÂN TRANG (thực thi SQL tại đây với CountAsync / Skip + Take)
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Đếm tổng số bản ghi sau khi áp dụng toàn bộ bộ lọc/tìm kiếm.
        /// Dùng LINQ CountAsync() — EF Core chỉ sinh câu SELECT COUNT(*).
        /// </summary>
        public async Task<int> CountAsync(IQueryable<DotHienMau> query)
        {
            return await query.CountAsync();
        }

        /// <summary>
        /// Lấy dữ liệu một trang bằng Skip() + Take() trên truy vấn EF Core.
        /// Không tải toàn bộ dữ liệu về rồi phân trang ở tầng ứng dụng/trình duyệt.
        /// Thứ tự xử lý: Truy vấn → Tìm kiếm → Lọc → Sắp xếp → Phân trang → Hiển thị.
        /// </summary>
        public async Task<List<DotHienMau>> GetPagedAsync(
            IQueryable<DotHienMau> query,
            int page,
            int pageSize)
        {
            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  DROPDOWN
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Lấy danh sách đơn vị tổ chức đang hoạt động, sắp xếp A-Z,
        /// dùng cho dropdown trong form thêm/sửa đợt hiến máu và bộ lọc.
        /// </summary>
        public async Task<List<DonViToChuc>> GetActiveDonViToChucsAsync()
        {
            return await _context.DonViToChucs
                .Where(dv => dv.TrangThai == "Hoạt động")
                .OrderBy(dv => dv.TenDonViToChuc)
                .ToListAsync();
        }
    }
}
