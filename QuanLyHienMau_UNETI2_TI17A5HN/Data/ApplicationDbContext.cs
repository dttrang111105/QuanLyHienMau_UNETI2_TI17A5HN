using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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

            // 1. TaiKhoan - Cấu hình Unique TenDangNhap
            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.HasIndex(t => t.TenDangNhap).IsUnique();
            });

            // 2. DonViToChuc 1 - n DotHienMau
            modelBuilder.Entity<DotHienMau>(entity =>
            {
                entity.HasOne(d => d.DonViToChuc)
                      .WithMany(dv => dv.DotHienMaus)
                      .HasForeignKey(d => d.MaDonViToChuc)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. TaiKhoan 1 - 0..1 NguoiHienMau
            modelBuilder.Entity<NguoiHienMau>(entity =>
            {
                entity.HasOne(n => n.TaiKhoan)
                      .WithOne(t => t.NguoiHienMau)
                      .HasForeignKey<NguoiHienMau>(n => n.MaTaiKhoan)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // 4. NguoiHienMau 1 - n PhieuDangKyHienMau
            modelBuilder.Entity<PhieuDangKyHienMau>(entity =>
            {
                entity.HasOne(p => p.NguoiHienMau)
                      .WithMany(n => n.PhieuDangKyHienMaus)
                      .HasForeignKey(p => p.MaNguoiHien)
                      .OnDelete(DeleteBehavior.Restrict);

                // 5. DotHienMau 1 - n PhieuDangKyHienMau
                entity.HasOne(p => p.DotHienMau)
                      .WithMany(d => d.PhieuDangKyHienMaus)
                      .HasForeignKey(p => p.MaDot)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 6. PhieuDangKyHienMau 1 - 0..n LichKhamSangLoc
            modelBuilder.Entity<LichKhamSangLoc>(entity =>
            {
                entity.HasOne(l => l.PhieuDangKyHienMau)
                      .WithMany(p => p.LichKhamSangLocs)
                      .HasForeignKey(l => l.MaPhieu)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 7. PhieuDangKyHienMau 1 - 0..1 KetQuaHienMau
            modelBuilder.Entity<KetQuaHienMau>(entity =>
            {
                entity.HasIndex(k => k.MaPhieu).IsUnique();

                entity.HasOne(k => k.PhieuDangKyHienMau)
                      .WithOne(p => p.KetQuaHienMau)
                      .HasForeignKey<KetQuaHienMau>(k => k.MaPhieu)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
