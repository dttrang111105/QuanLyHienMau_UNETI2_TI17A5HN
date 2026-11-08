using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHienMau_UNETI2_TI17A5HN.Migrations
{
    public partial class ThemLichKhamSangLocVaKetQuaHienMau : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KetQuaHienMaus",
                columns: table => new
                {
                    MaKetQuaHienMau = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    DiemDanhGia = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    NhanXet = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    KetQua = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaHienMaus", x => x.MaKetQuaHienMau);
                    table.CheckConstraint("CK_KetQua_DiemDanhGia", "[DiemDanhGia] IS NULL OR ([DiemDanhGia] >= 0 AND [DiemDanhGia] <= 100)");
                    table.ForeignKey(
                        name: "FK_KetQuaHienMaus_PhieuDangKyHienMaus_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PhieuDangKyHienMaus",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichKhamSangLocs",
                columns: table => new
                {
                    MaLichKhamSangLoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HinhThucTiepNhan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiaDiemHoacLienKet = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BacSiSangLoc = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichKhamSangLocs", x => x.MaLichKhamSangLoc);
                    table.CheckConstraint("CK_LichKham_ThoiGian", "[ThoiGianKetThuc] > [ThoiGianBatDau]");
                    table.ForeignKey(
                        name: "FK_LichKhamSangLocs_PhieuDangKyHienMaus_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PhieuDangKyHienMaus",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaHienMaus_MaPhieu",
                table: "KetQuaHienMaus",
                column: "MaPhieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichKhamSangLocs_BacSiSangLoc_ThoiGianBatDau",
                table: "LichKhamSangLocs",
                columns: new[] { "BacSiSangLoc", "ThoiGianBatDau" });

            migrationBuilder.CreateIndex(
                name: "IX_LichKhamSangLocs_MaPhieu_TrangThai",
                table: "LichKhamSangLocs",
                columns: new[] { "MaPhieu", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_LichKhamSangLocs_TrangThai_ThoiGianBatDau",
                table: "LichKhamSangLocs",
                columns: new[] { "TrangThai", "ThoiGianBatDau" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KetQuaHienMaus");

            migrationBuilder.DropTable(
                name: "LichKhamSangLocs");
        }
    }
}
