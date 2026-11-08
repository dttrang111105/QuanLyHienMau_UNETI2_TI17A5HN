using Microsoft.EntityFrameworkCore;
using QuanLyHienMau_UNETI2_TI17A5HN.Models;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data;

public static class CauHinhKhamSangLoc
{
    public static void ConfigureKhamSangLoc(this ModelBuilder modelBuilder, bool giuSchemaHienTai = false)
    {
        var lich = modelBuilder.Entity<LichKhamSangLoc>();
        var ketQua = modelBuilder.Entity<KetQuaHienMau>();

        lich.Property(x => x.TrangThai).IsConcurrencyToken();
        ketQua.Property(x => x.NgayCapNhat).IsConcurrencyToken();
        modelBuilder.Entity<PhieuDangKyHienMau>().Property(x => x.TrangThai).IsConcurrencyToken();

        // ZIP main(10)(3) đã có migration dùng tên bảng số nhiều.
        // Khi ghép, cấu hình mặc định giữ tên bảng và quan hệ từ DbContext chung.
        if (!giuSchemaHienTai) return;

        lich.ToTable("LichKhamSangLocs", t => t.HasCheckConstraint(
            "CK_LichKham_ThoiGian", "[ThoiGianKetThuc] > [ThoiGianBatDau]"));
        lich.HasOne(x => x.PhieuDangKyHienMau).WithMany(x => x.LichKhamSangLocs)
            .HasForeignKey(x => x.MaPhieu).OnDelete(DeleteBehavior.Restrict);
        lich.HasIndex(x => new { x.BacSiSangLoc, x.ThoiGianBatDau });
        lich.HasIndex(x => new { x.MaPhieu, x.TrangThai });
        lich.HasIndex(x => new { x.TrangThai, x.ThoiGianBatDau });
        lich.Property(x => x.HinhThucTiepNhan).HasMaxLength(50).IsRequired();
        lich.Property(x => x.DiaDiemHoacLienKet).HasMaxLength(500).IsRequired();
        lich.Property(x => x.BacSiSangLoc).HasMaxLength(150).IsRequired();
        lich.Property(x => x.GhiChu).HasMaxLength(2000);

        ketQua.ToTable("KetQuaHienMaus", t => t.HasCheckConstraint(
            "CK_KetQua_DiemDanhGia", "[DiemDanhGia] IS NULL OR ([DiemDanhGia] >= 0 AND [DiemDanhGia] <= 100)"));
        ketQua.HasOne(x => x.PhieuDangKyHienMau).WithOne(x => x.KetQuaHienMau)
            .HasForeignKey<KetQuaHienMau>(x => x.MaPhieu).OnDelete(DeleteBehavior.Restrict);
        ketQua.HasIndex(x => x.MaPhieu).IsUnique();
        ketQua.Property(x => x.DiemDanhGia).HasConversion<decimal>().HasPrecision(5, 2);
        ketQua.Property(x => x.NhanXet).HasMaxLength(2000);
        ketQua.Property(x => x.KetQua).HasMaxLength(50);
        ketQua.Property(x => x.NgayCapNhat).IsRequired();
    }
}
