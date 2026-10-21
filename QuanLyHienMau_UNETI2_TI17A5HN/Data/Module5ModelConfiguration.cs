using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data;

public static class Module5ModelConfiguration
{
    public static ModelBuilder ConfigureModule5(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LichKhamSangLoc>()
            .ToTable("LichKhamSangLocs", t => t.HasCheckConstraint(
                "CK_LichKham_ThoiGian", "[ThoiGianKetThuc] > [ThoiGianBatDau]"));
        modelBuilder.Entity<LichKhamSangLoc>()
            .HasOne(x => x.PhieuDangKyHienMau).WithMany(x => x.LichKhamSangLocs)
            .HasForeignKey(x => x.MaPhieu).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LichKhamSangLoc>()
            .HasIndex(x => new { x.BacSiSangLoc, x.ThoiGianBatDau });
        modelBuilder.Entity<LichKhamSangLoc>()
            .HasIndex(x => new { x.MaPhieu, x.TrangThai });
        modelBuilder.Entity<LichKhamSangLoc>()
            .HasIndex(x => new { x.TrangThai, x.ThoiGianBatDau });
        modelBuilder.Entity<LichKhamSangLoc>()
            .Property(x => x.TrangThai).IsConcurrencyToken();

        modelBuilder.Entity<KetQuaHienMau>()
            .ToTable("KetQuaHienMaus", t => t.HasCheckConstraint(
                "CK_KetQua_DiemDanhGia", "[DiemDanhGia] IS NULL OR ([DiemDanhGia] >= 0 AND [DiemDanhGia] <= 100)"));
        modelBuilder.Entity<KetQuaHienMau>()
            .HasOne(x => x.PhieuDangKyHienMau).WithOne(x => x.KetQuaHienMau)
            .HasForeignKey<KetQuaHienMau>(x => x.MaPhieu).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<KetQuaHienMau>().HasIndex(x => x.MaPhieu).IsUnique();
        modelBuilder.Entity<KetQuaHienMau>().Property(x => x.DiemDanhGia).HasPrecision(5, 2);
        modelBuilder.Entity<PhieuDangKyHienMau>()
            .Property(x => x.TrangThai).IsConcurrencyToken();

        return modelBuilder;
    }
}
