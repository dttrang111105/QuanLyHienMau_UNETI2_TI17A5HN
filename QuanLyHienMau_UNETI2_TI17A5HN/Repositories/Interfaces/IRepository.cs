// ============================================================================
// DỰ ÁN QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VITALIS - UNETI (TI17A5HN)
// MODULE 3: QUẢN LÝ NGƯỜI HIẾN MÁU & PHIẾU ĐĂNG KÝ HIẾN MÁU
// Họ và tên: Đàm Thu Trang
// Mã sinh viên: 23103100243
// Nội dung thực hiện: Module 3 - Quản lý người hiến máu & Phiếu đăng ký hiến máu
// ============================================================================

using System.Linq.Expressions;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces
{
    /// <summary>
    /// Generic Repository Interface cung cấp các thao tác truy cập dữ liệu cơ bản (CRUD) bất đồng bộ.
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể (Entity Model)</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
    }
}
