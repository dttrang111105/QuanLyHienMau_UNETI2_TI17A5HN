using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHienMau_UNETI2_TI17A5HN.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonViToChuc",
                columns: table => new
                {
                    MaDonViToChuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDonViToChuc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmailLienHe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonViToChuc", x => x.MaDonViToChuc);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "DotHienMau",
                columns: table => new
                {
                    MaDot = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaDonViToChuc = table.Column<int>(type: "int", nullable: false),
                    ChiTieuDonViMau = table.Column<int>(type: "int", nullable: false),
                    NhomMauUuTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CanNangToiThieu = table.Column<double>(type: "float", nullable: false),
                    NgayBatDauDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoTaDot = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DieuKienNguoiHien = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotHienMau", x => x.MaDot);
                    table.ForeignKey(
                        name: "FK_DotHienMau_DonViToChuc_MaDonViToChuc",
                        column: x => x.MaDonViToChuc,
                        principalTable: "DonViToChuc",
                        principalColumn: "MaDonViToChuc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NguoiHienMau",
                columns: table => new
                {
                    MaNguoiHien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NhomMau = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NgheNghiep = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CanNang = table.Column<double>(type: "float", nullable: true),
                    GhiChuSucKhoe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiHienMau", x => x.MaNguoiHien);
                    table.ForeignKey(
                        name: "FK_NguoiHienMau_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PhieuDangKyHienMau",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiHien = table.Column<int>(type: "int", nullable: false),
                    MaDot = table.Column<int>(type: "int", nullable: false),
                    NgayNop = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDoHienMau = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChuXacNhan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuDangKyHienMau", x => x.MaPhieu);
                    table.ForeignKey(
                        name: "FK_PhieuDangKyHienMau_DotHienMau_MaDot",
                        column: x => x.MaDot,
                        principalTable: "DotHienMau",
                        principalColumn: "MaDot",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuDangKyHienMau_NguoiHienMau_MaNguoiHien",
                        column: x => x.MaNguoiHien,
                        principalTable: "NguoiHienMau",
                        principalColumn: "MaNguoiHien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaHienMau",
                columns: table => new
                {
                    MaKetQuaHienMau = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    DiemDanhGia = table.Column<double>(type: "float", nullable: true),
                    NhanXet = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    KetQua = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaHienMau", x => x.MaKetQuaHienMau);
                    table.ForeignKey(
                        name: "FK_KetQuaHienMau_PhieuDangKyHienMau_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PhieuDangKyHienMau",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichKhamSangLoc",
                columns: table => new
                {
                    MaLichKhamSangLoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HinhThucTiepNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaDiemHoacLienKet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BacSiSangLoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichKhamSangLoc", x => x.MaLichKhamSangLoc);
                    table.ForeignKey(
                        name: "FK_LichKhamSangLoc_PhieuDangKyHienMau_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PhieuDangKyHienMau",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DotHienMau_MaDonViToChuc",
                table: "DotHienMau",
                column: "MaDonViToChuc");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaHienMau_MaPhieu",
                table: "KetQuaHienMau",
                column: "MaPhieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichKhamSangLoc_MaPhieu",
                table: "LichKhamSangLoc",
                column: "MaPhieu");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiHienMau_MaTaiKhoan",
                table: "NguoiHienMau",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDangKyHienMau_MaDot",
                table: "PhieuDangKyHienMau",
                column: "MaDot");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDangKyHienMau_MaNguoiHien",
                table: "PhieuDangKyHienMau",
                column: "MaNguoiHien");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KetQuaHienMau");

            migrationBuilder.DropTable(
                name: "LichKhamSangLoc");

            migrationBuilder.DropTable(
                name: "PhieuDangKyHienMau");

            migrationBuilder.DropTable(
                name: "DotHienMau");

            migrationBuilder.DropTable(
                name: "NguoiHienMau");

            migrationBuilder.DropTable(
                name: "DonViToChuc");

            migrationBuilder.DropTable(
                name: "TaiKhoan");
        }
    }
}
