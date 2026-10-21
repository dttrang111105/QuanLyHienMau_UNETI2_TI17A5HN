using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHienMau_UNETI2_TI17A5HN.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotHienMaus_DonViToChucs_MaDonViToChuc",
                table: "DotHienMaus");

            migrationBuilder.DropForeignKey(
                name: "FK_KetQuaHienMaus_PhieuDangKyHienMaus_MaPhieu",
                table: "KetQuaHienMaus");

            migrationBuilder.DropForeignKey(
                name: "FK_LichKhamSangLocs_PhieuDangKyHienMaus_MaPhieu",
                table: "LichKhamSangLocs");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiHienMaus_TaiKhoans_MaTaiKhoan",
                table: "NguoiHienMaus");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyHienMaus_DotHienMaus_MaDot",
                table: "PhieuDangKyHienMaus");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyHienMaus_NguoiHienMaus_MaNguoiHien",
                table: "PhieuDangKyHienMaus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaiKhoans",
                table: "TaiKhoans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhieuDangKyHienMaus",
                table: "PhieuDangKyHienMaus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NguoiHienMaus",
                table: "NguoiHienMaus");

            migrationBuilder.DropIndex(
                name: "IX_NguoiHienMaus_MaTaiKhoan",
                table: "NguoiHienMaus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LichKhamSangLocs",
                table: "LichKhamSangLocs");

            migrationBuilder.DropIndex(
                name: "IX_LichKhamSangLocs_MaPhieu",
                table: "LichKhamSangLocs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KetQuaHienMaus",
                table: "KetQuaHienMaus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DotHienMaus",
                table: "DotHienMaus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonViToChucs",
                table: "DonViToChucs");

            migrationBuilder.RenameTable(
                name: "TaiKhoans",
                newName: "TaiKhoan");

            migrationBuilder.RenameTable(
                name: "PhieuDangKyHienMaus",
                newName: "PhieuDangKyHienMau");

            migrationBuilder.RenameTable(
                name: "NguoiHienMaus",
                newName: "NguoiHienMau");

            migrationBuilder.RenameTable(
                name: "LichKhamSangLocs",
                newName: "LichKhamSangLoc");

            migrationBuilder.RenameTable(
                name: "KetQuaHienMaus",
                newName: "KetQuaHienMau");

            migrationBuilder.RenameTable(
                name: "DotHienMaus",
                newName: "DotHienMau");

            migrationBuilder.RenameTable(
                name: "DonViToChucs",
                newName: "DonViToChuc");

            migrationBuilder.RenameIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoan",
                newName: "IX_TaiKhoan_TenDangNhap");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyHienMaus_MaNguoiHien",
                table: "PhieuDangKyHienMau",
                newName: "IX_PhieuDangKyHienMau_MaNguoiHien");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyHienMaus_MaDot",
                table: "PhieuDangKyHienMau",
                newName: "IX_PhieuDangKyHienMau_MaDot");

            migrationBuilder.RenameIndex(
                name: "IX_LichKhamSangLocs_BacSiChuanHoa_ThoiGianBatDau",
                table: "LichKhamSangLoc",
                newName: "IX_LichKhamSangLoc_BacSiChuanHoa_ThoiGianBatDau");

            migrationBuilder.RenameIndex(
                name: "IX_KetQuaHienMaus_MaPhieu",
                table: "KetQuaHienMau",
                newName: "IX_KetQuaHienMau_MaPhieu");

            migrationBuilder.RenameIndex(
                name: "IX_DotHienMaus_MaDonViToChuc",
                table: "DotHienMau",
                newName: "IX_DotHienMau_MaDonViToChuc");

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "TaiKhoan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "TaiKhoan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "TenDangNhap",
                table: "TaiKhoan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "MatKhau",
                table: "TaiKhoan",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "TaiKhoan",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "TaiKhoan",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "PhieuDangKyHienMau",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LyDoHienMau",
                table: "PhieuDangKyHienMau",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChuXacNhan",
                table: "PhieuDangKyHienMau",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "PhieuDangKyHienMau",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "NguoiHienMau",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "NguoiHienMau",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NhomMau",
                table: "NguoiHienMau",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NgheNghiep",
                table: "NguoiHienMau",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgaySinh",
                table: "NguoiHienMau",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "MaTaiKhoan",
                table: "NguoiHienMau",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "NguoiHienMau",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinh",
                table: "NguoiHienMau",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChuSucKhoe",
                table: "NguoiHienMau",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NguoiHienMau",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "NguoiHienMau",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "CanNang",
                table: "NguoiHienMau",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LichKhamSangLoc",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "HinhThucTiepNhan",
                table: "LichKhamSangLoc",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "LichKhamSangLoc",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "DiaDiemHoacLienKet",
                table: "LichKhamSangLoc",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "BacSiSangLoc",
                table: "LichKhamSangLoc",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "BacSiChuanHoa",
                table: "LichKhamSangLoc",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "NhanXet",
                table: "KetQuaHienMau",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayCapNhat",
                table: "KetQuaHienMau",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "KetQua",
                table: "KetQuaHienMau",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<double>(
                name: "DiemDanhGia",
                table: "KetQuaHienMau",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "DotHienMau",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TenDot",
                table: "DotHienMau",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "NhomMauUuTien",
                table: "DotHienMau",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MoTaDot",
                table: "DotHienMau",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DieuKienNguoiHien",
                table: "DotHienMau",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "DonViToChuc",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "TenDonViToChuc",
                table: "DonViToChuc",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "DonViToChuc",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EmailLienHe",
                table: "DonViToChuc",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaiKhoan",
                table: "TaiKhoan",
                column: "MaTaiKhoan");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhieuDangKyHienMau",
                table: "PhieuDangKyHienMau",
                column: "MaPhieu");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NguoiHienMau",
                table: "NguoiHienMau",
                column: "MaNguoiHien");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LichKhamSangLoc",
                table: "LichKhamSangLoc",
                column: "MaLichKhamSangLoc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KetQuaHienMau",
                table: "KetQuaHienMau",
                column: "MaKetQuaHienMau");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DotHienMau",
                table: "DotHienMau",
                column: "MaDot");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonViToChuc",
                table: "DonViToChuc",
                column: "MaDonViToChuc");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiHienMau_MaTaiKhoan",
                table: "NguoiHienMau",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LichKhamSangLoc_MaPhieu",
                table: "LichKhamSangLoc",
                column: "MaPhieu");

            migrationBuilder.AddForeignKey(
                name: "FK_DotHienMau_DonViToChuc_MaDonViToChuc",
                table: "DotHienMau",
                column: "MaDonViToChuc",
                principalTable: "DonViToChuc",
                principalColumn: "MaDonViToChuc",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KetQuaHienMau_PhieuDangKyHienMau_MaPhieu",
                table: "KetQuaHienMau",
                column: "MaPhieu",
                principalTable: "PhieuDangKyHienMau",
                principalColumn: "MaPhieu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichKhamSangLoc_PhieuDangKyHienMau_MaPhieu",
                table: "LichKhamSangLoc",
                column: "MaPhieu",
                principalTable: "PhieuDangKyHienMau",
                principalColumn: "MaPhieu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiHienMau_TaiKhoan_MaTaiKhoan",
                table: "NguoiHienMau",
                column: "MaTaiKhoan",
                principalTable: "TaiKhoan",
                principalColumn: "MaTaiKhoan",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyHienMau_DotHienMau_MaDot",
                table: "PhieuDangKyHienMau",
                column: "MaDot",
                principalTable: "DotHienMau",
                principalColumn: "MaDot",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyHienMau_NguoiHienMau_MaNguoiHien",
                table: "PhieuDangKyHienMau",
                column: "MaNguoiHien",
                principalTable: "NguoiHienMau",
                principalColumn: "MaNguoiHien",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotHienMau_DonViToChuc_MaDonViToChuc",
                table: "DotHienMau");

            migrationBuilder.DropForeignKey(
                name: "FK_KetQuaHienMau_PhieuDangKyHienMau_MaPhieu",
                table: "KetQuaHienMau");

            migrationBuilder.DropForeignKey(
                name: "FK_LichKhamSangLoc_PhieuDangKyHienMau_MaPhieu",
                table: "LichKhamSangLoc");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiHienMau_TaiKhoan_MaTaiKhoan",
                table: "NguoiHienMau");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyHienMau_DotHienMau_MaDot",
                table: "PhieuDangKyHienMau");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyHienMau_NguoiHienMau_MaNguoiHien",
                table: "PhieuDangKyHienMau");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaiKhoan",
                table: "TaiKhoan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhieuDangKyHienMau",
                table: "PhieuDangKyHienMau");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NguoiHienMau",
                table: "NguoiHienMau");

            migrationBuilder.DropIndex(
                name: "IX_NguoiHienMau_MaTaiKhoan",
                table: "NguoiHienMau");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LichKhamSangLoc",
                table: "LichKhamSangLoc");

            migrationBuilder.DropIndex(
                name: "IX_LichKhamSangLoc_MaPhieu",
                table: "LichKhamSangLoc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KetQuaHienMau",
                table: "KetQuaHienMau");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DotHienMau",
                table: "DotHienMau");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonViToChuc",
                table: "DonViToChuc");

            migrationBuilder.RenameTable(
                name: "TaiKhoan",
                newName: "TaiKhoans");

            migrationBuilder.RenameTable(
                name: "PhieuDangKyHienMau",
                newName: "PhieuDangKyHienMaus");

            migrationBuilder.RenameTable(
                name: "NguoiHienMau",
                newName: "NguoiHienMaus");

            migrationBuilder.RenameTable(
                name: "LichKhamSangLoc",
                newName: "LichKhamSangLocs");

            migrationBuilder.RenameTable(
                name: "KetQuaHienMau",
                newName: "KetQuaHienMaus");

            migrationBuilder.RenameTable(
                name: "DotHienMau",
                newName: "DotHienMaus");

            migrationBuilder.RenameTable(
                name: "DonViToChuc",
                newName: "DonViToChucs");

            migrationBuilder.RenameIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoans",
                newName: "IX_TaiKhoans_TenDangNhap");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyHienMau_MaNguoiHien",
                table: "PhieuDangKyHienMaus",
                newName: "IX_PhieuDangKyHienMaus_MaNguoiHien");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyHienMau_MaDot",
                table: "PhieuDangKyHienMaus",
                newName: "IX_PhieuDangKyHienMaus_MaDot");

            migrationBuilder.RenameIndex(
                name: "IX_LichKhamSangLoc_BacSiChuanHoa_ThoiGianBatDau",
                table: "LichKhamSangLocs",
                newName: "IX_LichKhamSangLocs_BacSiChuanHoa_ThoiGianBatDau");

            migrationBuilder.RenameIndex(
                name: "IX_KetQuaHienMau_MaPhieu",
                table: "KetQuaHienMaus",
                newName: "IX_KetQuaHienMaus_MaPhieu");

            migrationBuilder.RenameIndex(
                name: "IX_DotHienMau_MaDonViToChuc",
                table: "DotHienMaus",
                newName: "IX_DotHienMaus_MaDonViToChuc");

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "TaiKhoans",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "TaiKhoans",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenDangNhap",
                table: "TaiKhoans",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MatKhau",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "TaiKhoans",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "TaiKhoans",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "PhieuDangKyHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "LyDoHienMau",
                table: "PhieuDangKyHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChuXacNhan",
                table: "PhieuDangKyHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "PhieuDangKyHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "NguoiHienMaus",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "NguoiHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NhomMau",
                table: "NguoiHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NgheNghiep",
                table: "NguoiHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgaySinh",
                table: "NguoiHienMaus",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaTaiKhoan",
                table: "NguoiHienMaus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "NguoiHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinh",
                table: "NguoiHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChuSucKhoe",
                table: "NguoiHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NguoiHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "NguoiHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "CanNang",
                table: "NguoiHienMaus",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LichKhamSangLocs",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "HinhThucTiepNhan",
                table: "LichKhamSangLocs",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "LichKhamSangLocs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiaDiemHoacLienKet",
                table: "LichKhamSangLocs",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BacSiSangLoc",
                table: "LichKhamSangLocs",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BacSiChuanHoa",
                table: "LichKhamSangLocs",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NhanXet",
                table: "KetQuaHienMaus",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayCapNhat",
                table: "KetQuaHienMaus",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KetQua",
                table: "KetQuaHienMaus",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "DiemDanhGia",
                table: "KetQuaHienMaus",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "DotHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "TenDot",
                table: "DotHienMaus",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NhomMauUuTien",
                table: "DotHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MoTaDot",
                table: "DotHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DieuKienNguoiHien",
                table: "DotHienMaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "DonViToChucs",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenDonViToChuc",
                table: "DonViToChucs",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "DonViToChucs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailLienHe",
                table: "DonViToChucs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaiKhoans",
                table: "TaiKhoans",
                column: "MaTaiKhoan");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhieuDangKyHienMaus",
                table: "PhieuDangKyHienMaus",
                column: "MaPhieu");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NguoiHienMaus",
                table: "NguoiHienMaus",
                column: "MaNguoiHien");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LichKhamSangLocs",
                table: "LichKhamSangLocs",
                column: "MaLichKhamSangLoc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KetQuaHienMaus",
                table: "KetQuaHienMaus",
                column: "MaKetQuaHienMau");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DotHienMaus",
                table: "DotHienMaus",
                column: "MaDot");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonViToChucs",
                table: "DonViToChucs",
                column: "MaDonViToChuc");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiHienMaus_MaTaiKhoan",
                table: "NguoiHienMaus",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichKhamSangLocs_MaPhieu",
                table: "LichKhamSangLocs",
                column: "MaPhieu",
                unique: true,
                filter: "[TrangThai] = N'Đã lên lịch'");

            migrationBuilder.AddForeignKey(
                name: "FK_DotHienMaus_DonViToChucs_MaDonViToChuc",
                table: "DotHienMaus",
                column: "MaDonViToChuc",
                principalTable: "DonViToChucs",
                principalColumn: "MaDonViToChuc",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KetQuaHienMaus_PhieuDangKyHienMaus_MaPhieu",
                table: "KetQuaHienMaus",
                column: "MaPhieu",
                principalTable: "PhieuDangKyHienMaus",
                principalColumn: "MaPhieu",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichKhamSangLocs_PhieuDangKyHienMaus_MaPhieu",
                table: "LichKhamSangLocs",
                column: "MaPhieu",
                principalTable: "PhieuDangKyHienMaus",
                principalColumn: "MaPhieu",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiHienMaus_TaiKhoans_MaTaiKhoan",
                table: "NguoiHienMaus",
                column: "MaTaiKhoan",
                principalTable: "TaiKhoans",
                principalColumn: "MaTaiKhoan",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyHienMaus_DotHienMaus_MaDot",
                table: "PhieuDangKyHienMaus",
                column: "MaDot",
                principalTable: "DotHienMaus",
                principalColumn: "MaDot",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyHienMaus_NguoiHienMaus_MaNguoiHien",
                table: "PhieuDangKyHienMaus",
                column: "MaNguoiHien",
                principalTable: "NguoiHienMaus",
                principalColumn: "MaNguoiHien",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
