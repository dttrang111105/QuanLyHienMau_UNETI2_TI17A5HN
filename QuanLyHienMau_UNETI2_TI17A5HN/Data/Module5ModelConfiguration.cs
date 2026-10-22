using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data;

public static class Module5ModelConfiguration
{
    public static ModelBuilder ConfigureModule5(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LichKhamSangLoc>()
            .ToTable("LichKhamSangLoc");
        modelBuilder.Entity<LichKhamSangLoc>()
            .HasOne(x => x.PhieuDangKyHienMau).WithMany(x => x.LichKhamSangLocs)
            .HasForeignKey(x => x.MaPhieu).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<KetQuaHienMau>()
            .ToTable("KetQuaHienMau");
        modelBuilder.Entity<KetQuaHienMau>()
            .HasOne(x => x.PhieuDangKyHienMau).WithOne(x => x.KetQuaHienMau)
            .HasForeignKey<KetQuaHienMau>(x => x.MaPhieu).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<KetQuaHienMau>().HasIndex(x => x.MaPhieu).IsUnique();

        return modelBuilder;
    }
}
