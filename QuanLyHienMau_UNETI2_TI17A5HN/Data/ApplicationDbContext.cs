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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DotHienMau>()
            .HasOne(x => x.DonViToChuc).WithMany(x => x.DotHienMaus)
            .HasForeignKey(x => x.MaDonViToChuc).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PhieuDangKyHienMau>()
            .HasOne(x => x.NguoiHienMau).WithMany(x => x.PhieuDangKyHienMaus)
            .HasForeignKey(x => x.MaNguoiHien).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PhieuDangKyHienMau>()
            .HasOne(x => x.DotHienMau).WithMany(x => x.PhieuDangKyHienMaus)
            .HasForeignKey(x => x.MaDot).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoans", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<DonViToChuc>().ToTable("DonViToChucs", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<DotHienMau>().ToTable("DotHienMaus", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<NguoiHienMau>().ToTable("NguoiHienMaus", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<PhieuDangKyHienMau>().ToTable("PhieuDangKyHienMaus", t => t.ExcludeFromMigrations());
        modelBuilder.ConfigureModule5();
    }
}
