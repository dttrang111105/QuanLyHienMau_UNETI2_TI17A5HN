// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: Module 5 và phần nền dữ liệu cần thiết để chạy thử.
using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<DonViToChuc> DonViToChucs => Set<DonViToChuc>();
    public DbSet<DotHienMau> DotHienMaus => Set<DotHienMau>();
    public DbSet<NguoiHienMau> NguoiHienMaus => Set<NguoiHienMau>();
    public DbSet<PhieuDangKyHienMau> PhieuDangKyHienMaus => Set<PhieuDangKyHienMau>();
    public DbSet<LichKhamSangLoc> LichKhamSangLocs => Set<LichKhamSangLoc>();
    public DbSet<KetQuaHienMau> KetQuaHienMaus => Set<KetQuaHienMau>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<TaiKhoan>().HasIndex(x => x.TenDangNhap).IsUnique();
        b.Entity<NguoiHienMau>().HasIndex(x => x.MaTaiKhoan).IsUnique();
        b.Entity<DotHienMau>().HasOne(x => x.DonViToChuc).WithMany().HasForeignKey(x => x.MaDonViToChuc).OnDelete(DeleteBehavior.Restrict);
        b.Entity<NguoiHienMau>().HasOne(x => x.TaiKhoan).WithOne().HasForeignKey<NguoiHienMau>(x => x.MaTaiKhoan).OnDelete(DeleteBehavior.Restrict);
        b.Entity<PhieuDangKyHienMau>().HasOne(x => x.NguoiHien).WithMany().HasForeignKey(x => x.MaNguoiHien).OnDelete(DeleteBehavior.Restrict);
        b.Entity<PhieuDangKyHienMau>().HasOne(x => x.Dot).WithMany().HasForeignKey(x => x.MaDot).OnDelete(DeleteBehavior.Restrict);
        b.Entity<LichKhamSangLoc>().HasOne(x => x.Phieu).WithMany(x => x.LichKhams).HasForeignKey(x => x.MaPhieu).OnDelete(DeleteBehavior.Restrict);
        b.Entity<LichKhamSangLoc>().HasIndex(x => x.MaPhieu).IsUnique().HasFilter(Database.IsSqlServer() ? "[TrangThai] = N'Đã lên lịch'" : "[TrangThai] = 'Đã lên lịch'");
        b.Entity<LichKhamSangLoc>().HasIndex(x => new { x.BacSiChuanHoa, x.ThoiGianBatDau });
        b.Entity<KetQuaHienMau>().HasOne(x => x.Phieu).WithOne(x => x.KetQuaHienMau).HasForeignKey<KetQuaHienMau>(x => x.MaPhieu).OnDelete(DeleteBehavior.Restrict);
    }
}
