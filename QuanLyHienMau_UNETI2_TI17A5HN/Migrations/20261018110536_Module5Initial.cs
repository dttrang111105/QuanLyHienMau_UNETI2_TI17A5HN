using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace QuanLyHienMau_UNETI2_TI17A5HN.Migrations
{
    /// <inheritdoc/>
    public partial class Module5Initial : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "DonViToChucs", columns: table => new { MaDonViToChuc = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"), TenDonViToChuc = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false), MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false), EmailLienHe = table.Column<string>(type: "nvarchar(max)", nullable: false), TrangThai = table.Column<bool>(type: "bit", nullable: false) }, constraints: table =>
            {
                table.PrimaryKey("PK_DonViToChucs", x => x.MaDonViToChuc);
            });
            migrationBuilder.CreateTable(name: "TaiKhoans", columns: table => new { MaTaiKhoan = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"), TenDangNhap = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false), MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false), HoTen = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false), Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false), VaiTro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false), TrangThai = table.Column<bool>(type: "bit", nullable: false) }, constraints: table =>
            {
                table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
            });
            migrationBuilder.CreateTable(name: "DotHienMaus", columns: table => new { MaDot = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"), TenDot = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false), MaDonViToChuc = table.Column<int>(type: "int", nullable: false), ChiTieuDonViMau = table.Column<int>(type: "int", nullable: false), NhomMauUuTien = table.Column<string>(type: "nvarchar(max)", nullable: false), CanNangToiThieu = table.Column<double>(type: "float", nullable: false), NgayBatDauDangKy = table.Column<DateTime>(type: "datetime2", nullable: false), HanDangKy = table.Column<DateTime>(type: "datetime2", nullable: false), MoTaDot = table.Column<string>(type: "nvarchar(max)", nullable: false), DieuKienNguoiHien = table.Column<string>(type: "nvarchar(max)", nullable: false), TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false) }, constraints: table =>
            {
                table.PrimaryKey("PK_DotHienMaus", x => x.MaDot);
                table.ForeignKey(name: "FK_DotHienMaus_DonViToChucs_MaDonViToChuc", column: x => x.MaDonViToChuc, principalTable: "DonViToChucs", principalColumn: "MaDonViToChuc", onDelete: ReferentialAction.Restrict);
            });
            migrationBuilder.CreateTable(name: "NguoiHienMaus", columns: table => new { MaNguoiHien = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"), MaTaiKhoan = table.Column<int>(type: "int", nullable: false), HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false), NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false), GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: false), SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false), Email = table.Column<string>(type: "nvarchar(max)", nullable: false), DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false), NhomMau = table.Column<string>(type: "nvarchar(max)", nullable: false), NgheNghiep = table.Column<string>(type: "nvarchar(max)", nullable: false), CanNang = table.Column<double>(type: "float", nullable: false), GhiChuSucKhoe = table.Column<string>(type: "nvarchar(max)", nullable: false), TrangThai = table.Column<bool>(type: "bit", nullable: false) }, constraints: table =>
            {
                table.PrimaryKey("PK_NguoiHienMaus", x => x.MaNguoiHien);
                table.ForeignKey(name: "FK_NguoiHienMaus_TaiKhoans_MaTaiKhoan", column: x => x.MaTaiKhoan, principalTable: "TaiKhoans", principalColumn: "MaTaiKhoan", onDelete: ReferentialAction.Restrict);
            });
            migrationBuilder.CreateTable(name: "PhieuDangKyHienMaus", columns: table => new { MaPhieu = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"), MaNguoiHien = table.Column<int>(type: "int", nullable: false), MaDot = table.Column<int>(type: "int", nullable: false), NgayNop = table.Column<DateTime>(type: "datetime2", nullable: false), LyDoHienMau = table.Column<string>(type: "nvarchar(max)", nullable: false), GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false), TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false), NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true), GhiChuXacNhan = table.Column<string>(type: "nvarchar(max)", nullable: false) }, constraints: table =>
            {
                table.PrimaryKey("PK_PhieuDangKyHienMaus", x => x.MaPhieu);
                table.ForeignKey(name: "FK_PhieuDangKyHienMaus_DotHienMaus_MaDot", column: x => x.MaDot, principalTable: "DotHienMaus", principalColumn: "MaDot", onDelete: ReferentialAction.Restrict);
                table.ForeignKey(name: "FK_PhieuDangKyHienMaus_NguoiHienMaus_MaNguoiHien", column: x => x.MaNguoiHien, principalTable: "NguoiHienMaus", principalColumn: "MaNguoiHien", onDelete: ReferentialAction.Restrict);
            });
            migrationBuilder.CreateTable(name: "KetQuaHienMaus", columns: table => new { MaKetQuaHienMau = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"), MaPhieu = table.Column<int>(type: "int", nullable: false), DiemDanhGia = table.Column<int>(type: "int", nullable: true), NhanXet = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false), KetQua = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false), NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false) }, constraints: table =>
            {
                table.PrimaryKey("PK_KetQuaHienMaus", x => x.MaKetQuaHienMau);
                table.ForeignKey(name: "FK_KetQuaHienMaus_PhieuDangKyHienMaus_MaPhieu", column: x => x.MaPhieu, principalTable: "PhieuDangKyHienMaus", principalColumn: "MaPhieu", onDelete: ReferentialAction.Restrict);
            });
            migrationBuilder.CreateTable(name: "LichKhamSangLocs", columns: table => new { MaLichKhamSangLoc = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"), MaPhieu = table.Column<int>(type: "int", nullable: false), ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false), ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false), HinhThucTiepNhan = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false), DiaDiemHoacLienKet = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false), BacSiSangLoc = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false), BacSiChuanHoa = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false), GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false), TrangThai = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false) }, constraints: table =>
            {
                table.PrimaryKey("PK_LichKhamSangLocs", x => x.MaLichKhamSangLoc);
                table.ForeignKey(name: "FK_LichKhamSangLocs_PhieuDangKyHienMaus_MaPhieu", column: x => x.MaPhieu, principalTable: "PhieuDangKyHienMaus", principalColumn: "MaPhieu", onDelete: ReferentialAction.Restrict);
            });
            migrationBuilder.CreateIndex(name: "IX_DotHienMaus_MaDonViToChuc", table: "DotHienMaus", column: "MaDonViToChuc");
            migrationBuilder.CreateIndex(name: "IX_KetQuaHienMaus_MaPhieu", table: "KetQuaHienMaus", column: "MaPhieu", unique: true);
            migrationBuilder.CreateIndex(name: "IX_LichKhamSangLocs_BacSiChuanHoa_ThoiGianBatDau", table: "LichKhamSangLocs", columns: new[] { "BacSiChuanHoa", "ThoiGianBatDau" });
            migrationBuilder.CreateIndex(name: "IX_LichKhamSangLocs_MaPhieu", table: "LichKhamSangLocs", column: "MaPhieu", unique: true, filter: "[TrangThai] = N'Đã lên lịch'");
            migrationBuilder.CreateIndex(name: "IX_NguoiHienMaus_MaTaiKhoan", table: "NguoiHienMaus", column: "MaTaiKhoan", unique: true);
            migrationBuilder.CreateIndex(name: "IX_PhieuDangKyHienMaus_MaDot", table: "PhieuDangKyHienMaus", column: "MaDot");
            migrationBuilder.CreateIndex(name: "IX_PhieuDangKyHienMaus_MaNguoiHien", table: "PhieuDangKyHienMaus", column: "MaNguoiHien");
            migrationBuilder.CreateIndex(name: "IX_TaiKhoans_TenDangNhap", table: "TaiKhoans", column: "TenDangNhap", unique: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "KetQuaHienMaus");
            migrationBuilder.DropTable(name: "LichKhamSangLocs");
            migrationBuilder.DropTable(name: "PhieuDangKyHienMaus");
            migrationBuilder.DropTable(name: "DotHienMaus");
            migrationBuilder.DropTable(name: "NguoiHienMaus");
            migrationBuilder.DropTable(name: "DonViToChucs");
            migrationBuilder.DropTable(name: "TaiKhoans");
        }
    }
}
