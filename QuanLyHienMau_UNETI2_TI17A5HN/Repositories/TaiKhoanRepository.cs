using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Repositories;

public sealed class TaiKhoanRepository(ApplicationDbContext db)
{
    public Task<TaiKhoan?> TimTheoMaAsync(int maTaiKhoan) =>
        db.TaiKhoans.AsNoTracking().SingleOrDefaultAsync(x => x.MaTaiKhoan == maTaiKhoan);
}
