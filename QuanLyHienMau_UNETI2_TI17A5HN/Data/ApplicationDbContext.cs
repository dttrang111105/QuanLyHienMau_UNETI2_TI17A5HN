using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<DonViToChuc> DonViToChucs { get; set; }
        public DbSet<DotHienMau> DotHienMaus { get; set; }
        public DbSet<NguoiHienMau> NguoiHienMaus { get; set; }
        public DbSet<PhieuDangKyHienMau> PhieuDangKyHienMaus { get; set; }
        public DbSet<LichKhamSangLoc> LichKhamSangLocs { get; set; }
        public DbSet<KetQuaHienMau> KetQuaHienMaus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. TaiKhoan: TenDangNhap không được trùng
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            // 2. DonViToChuc - DotHienMau
            modelBuilder.Entity<DotHienMau>(entity =>
            {
                entity.HasOne(d => d.DonViToChuc)
                    .WithMany(dv => dv.DotHienMaus)
                    .HasForeignKey(d => d.MaDonViToChuc)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. TaiKhoan - NguoiHienMau
            modelBuilder.Entity<NguoiHienMau>(entity =>
            {
                entity.HasOne(n => n.TaiKhoan)
                    .WithOne(t => t.NguoiHienMau)
                    .HasForeignKey<NguoiHienMau>(n => n.MaTaiKhoan)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // 4. NguoiHienMau - PhieuDangKyHienMau
            modelBuilder.Entity<PhieuDangKyHienMau>(entity =>
            {
                entity.HasOne(p => p.NguoiHienMau)
                    .WithMany(n => n.PhieuDangKyHienMaus)
                    .HasForeignKey(p => p.MaNguoiHien)
                    .OnDelete(DeleteBehavior.Restrict);

                // 5. DotHienMau - PhieuDangKyHienMau
                entity.HasOne(p => p.DotHienMau)
                    .WithMany(d => d.PhieuDangKyHienMaus)
                    .HasForeignKey(p => p.MaDot)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 6. PhieuDangKyHienMau - LichKhamSangLoc
            modelBuilder.Entity<LichKhamSangLoc>(entity =>
            {
                entity.HasOne(l => l.PhieuDangKyHienMau)
                    .WithMany(p => p.LichKhamSangLocs)
                    .HasForeignKey(l => l.MaPhieu)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 7. KetQuaHienMau:
            // Mỗi phiếu đăng ký có tối đa một kết quả
            modelBuilder.Entity<KetQuaHienMau>(entity =>
            {
                entity.HasIndex(k => k.MaPhieu)
                    .IsUnique();

                entity.HasOne(k => k.PhieuDangKyHienMau)
                    .WithOne(p => p.KetQuaHienMau)
                    .HasForeignKey<KetQuaHienMau>(k => k.MaPhieu)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 8. Các chỉ mục bổ sung từ nhánh minh
            // Chỉ giữ các cấu hình này nếu những thuộc tính
            // tương ứng có trong model LichKhamSangLoc.
            modelBuilder.Entity<LichKhamSangLoc>(entity =>
            {
                entity.HasIndex(l => new
                {
                    l.BacSiChuanHoa,
                    l.ThoiGianBatDau
                });
            });
        }
    }
}