using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuanLyHienMau_UNETI2_TI17A5HN.Data;

#nullable disable

namespace QuanLyHienMau_UNETI2_TI17A5HN.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.DonViToChuc", b =>
                {
                    b.Property<int>("MaDonViToChuc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaDonViToChuc"));

                    b.Property<string>("TenDonViToChuc")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("MaDonViToChuc");

                    b.ToTable("DonViToChucs", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.DotHienMau", b =>
                {
                    b.Property<int>("MaDot")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaDot"));

                    b.Property<int>("ChiTieuDonViMau")
                        .HasColumnType("int");

                    b.Property<int>("MaDonViToChuc")
                        .HasColumnType("int");

                    b.Property<string>("TenDot")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("TrangThai")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaDot");

                    b.HasIndex("MaDonViToChuc");

                    b.ToTable("DotHienMaus", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.KetQuaHienMau", b =>
                {
                    b.Property<int>("MaKetQuaHienMau")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaKetQuaHienMau"));

                    b.Property<decimal?>("DiemDanhGia")
                        .HasPrecision(5, 2)
                        .HasColumnType("decimal(5,2)");

                    b.Property<string>("KetQua")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("MaPhieu")
                        .HasColumnType("int");

                    b.Property<DateTime>("NgayCapNhat")
                        .HasColumnType("datetime2");

                    b.Property<string>("NhanXet")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("MaKetQuaHienMau");

                    b.HasIndex("MaPhieu")
                        .IsUnique();

                    b.ToTable("KetQuaHienMaus", null, t =>
                        {
                            t.HasCheckConstraint("CK_KetQua_DiemDanhGia", "[DiemDanhGia] IS NULL OR ([DiemDanhGia] >= 0 AND [DiemDanhGia] <= 100)");
                        });
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.LichKhamSangLoc", b =>
                {
                    b.Property<int>("MaLichKhamSangLoc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaLichKhamSangLoc"));

                    b.Property<string>("BacSiSangLoc")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("DiaDiemHoacLienKet")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("GhiChu")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<string>("HinhThucTiepNhan")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("MaPhieu")
                        .HasColumnType("int");

                    b.Property<DateTime>("ThoiGianBatDau")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ThoiGianKetThuc")
                        .HasColumnType("datetime2");

                    b.Property<string>("TrangThai")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaLichKhamSangLoc");

                    b.HasIndex("BacSiSangLoc", "ThoiGianBatDau");

                    b.HasIndex("MaPhieu", "TrangThai");

                    b.HasIndex("TrangThai", "ThoiGianBatDau");

                    b.ToTable("LichKhamSangLocs", null, t =>
                        {
                            t.HasCheckConstraint("CK_LichKham_ThoiGian", "[ThoiGianKetThuc] > [ThoiGianBatDau]");
                        });
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.NguoiHienMau", b =>
                {
                    b.Property<int>("MaNguoiHien")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaNguoiHien"));

                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("MaTaiKhoan")
                        .HasColumnType("int");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.HasKey("MaNguoiHien");

                    b.ToTable("NguoiHienMaus", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.PhieuDangKyHienMau", b =>
                {
                    b.Property<int>("MaPhieu")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaPhieu"));

                    b.Property<int>("MaDot")
                        .HasColumnType("int");

                    b.Property<int>("MaNguoiHien")
                        .HasColumnType("int");

                    b.Property<DateTime>("NgayNop")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayXuLy")
                        .HasColumnType("datetime2");

                    b.Property<string>("TrangThai")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaPhieu");

                    b.HasIndex("MaDot");

                    b.HasIndex("MaNguoiHien");

                    b.ToTable("PhieuDangKyHienMaus", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.TaiKhoan", b =>
                {
                    b.Property<int>("MaTaiKhoan")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaTaiKhoan"));

                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");

                    b.Property<string>("VaiTro")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaTaiKhoan");

                    b.ToTable("TaiKhoans", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.DotHienMau", b =>
                {
                    b.HasOne("QuanLyHienMau_UNETI2_TI17A5HN.Models.DonViToChuc", "DonViToChuc")
                        .WithMany("DotHienMaus")
                        .HasForeignKey("MaDonViToChuc")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DonViToChuc");
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.KetQuaHienMau", b =>
                {
                    b.HasOne("QuanLyHienMau_UNETI2_TI17A5HN.Models.PhieuDangKyHienMau", "PhieuDangKyHienMau")
                        .WithOne("KetQuaHienMau")
                        .HasForeignKey("QuanLyHienMau_UNETI2_TI17A5HN.Models.KetQuaHienMau", "MaPhieu")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("PhieuDangKyHienMau");
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.LichKhamSangLoc", b =>
                {
                    b.HasOne("QuanLyHienMau_UNETI2_TI17A5HN.Models.PhieuDangKyHienMau", "PhieuDangKyHienMau")
                        .WithMany("LichKhamSangLocs")
                        .HasForeignKey("MaPhieu")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("PhieuDangKyHienMau");
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.PhieuDangKyHienMau", b =>
                {
                    b.HasOne("QuanLyHienMau_UNETI2_TI17A5HN.Models.DotHienMau", "DotHienMau")
                        .WithMany("PhieuDangKyHienMaus")
                        .HasForeignKey("MaDot")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("QuanLyHienMau_UNETI2_TI17A5HN.Models.NguoiHienMau", "NguoiHienMau")
                        .WithMany("PhieuDangKyHienMaus")
                        .HasForeignKey("MaNguoiHien")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DotHienMau");

                    b.Navigation("NguoiHienMau");
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.DonViToChuc", b =>
                {
                    b.Navigation("DotHienMaus");
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.DotHienMau", b =>
                {
                    b.Navigation("PhieuDangKyHienMaus");
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.NguoiHienMau", b =>
                {
                    b.Navigation("PhieuDangKyHienMaus");
                });

            modelBuilder.Entity("QuanLyHienMau_UNETI2_TI17A5HN.Models.PhieuDangKyHienMau", b =>
                {
                    b.Navigation("KetQuaHienMau");

                    b.Navigation("LichKhamSangLocs");
                });
#pragma warning restore 612, 618
        }
    }
}
