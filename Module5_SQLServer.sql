IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE TABLE [DonViToChucs] (
        [MaDonViToChuc] int NOT NULL IDENTITY,
        [TenDonViToChuc] nvarchar(160) NOT NULL,
        [MoTa] nvarchar(max) NOT NULL,
        [EmailLienHe] nvarchar(max) NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_DonViToChucs] PRIMARY KEY ([MaDonViToChuc])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE TABLE [TaiKhoans] (
        [MaTaiKhoan] int NOT NULL IDENTITY,
        [TenDangNhap] nvarchar(60) NOT NULL,
        [MatKhau] nvarchar(max) NOT NULL,
        [HoTen] nvarchar(120) NOT NULL,
        [Email] nvarchar(160) NOT NULL,
        [VaiTro] nvarchar(30) NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_TaiKhoans] PRIMARY KEY ([MaTaiKhoan])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE TABLE [DotHienMaus] (
        [MaDot] int NOT NULL IDENTITY,
        [TenDot] nvarchar(160) NOT NULL,
        [MaDonViToChuc] int NOT NULL,
        [ChiTieuDonViMau] int NOT NULL,
        [NhomMauUuTien] nvarchar(max) NOT NULL,
        [CanNangToiThieu] float NOT NULL,
        [NgayBatDauDangKy] datetime2 NOT NULL,
        [HanDangKy] datetime2 NOT NULL,
        [MoTaDot] nvarchar(max) NOT NULL,
        [DieuKienNguoiHien] nvarchar(max) NOT NULL,
        [TrangThai] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_DotHienMaus] PRIMARY KEY ([MaDot]),
        CONSTRAINT [FK_DotHienMaus_DonViToChucs_MaDonViToChuc] FOREIGN KEY ([MaDonViToChuc]) REFERENCES [DonViToChucs] ([MaDonViToChuc]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE TABLE [NguoiHienMaus] (
        [MaNguoiHien] int NOT NULL IDENTITY,
        [MaTaiKhoan] int NOT NULL,
        [HoTen] nvarchar(max) NOT NULL,
        [NgaySinh] datetime2 NOT NULL,
        [GioiTinh] nvarchar(max) NOT NULL,
        [SoDienThoai] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [DiaChi] nvarchar(max) NOT NULL,
        [NhomMau] nvarchar(max) NOT NULL,
        [NgheNghiep] nvarchar(max) NOT NULL,
        [CanNang] float NOT NULL,
        [GhiChuSucKhoe] nvarchar(max) NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_NguoiHienMaus] PRIMARY KEY ([MaNguoiHien]),
        CONSTRAINT [FK_NguoiHienMaus_TaiKhoans_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoans] ([MaTaiKhoan]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE TABLE [PhieuDangKyHienMaus] (
        [MaPhieu] int NOT NULL IDENTITY,
        [MaNguoiHien] int NOT NULL,
        [MaDot] int NOT NULL,
        [NgayNop] datetime2 NOT NULL,
        [LyDoHienMau] nvarchar(max) NOT NULL,
        [GhiChu] nvarchar(max) NOT NULL,
        [TrangThai] nvarchar(max) NOT NULL,
        [NgayXuLy] datetime2 NULL,
        [GhiChuXacNhan] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_PhieuDangKyHienMaus] PRIMARY KEY ([MaPhieu]),
        CONSTRAINT [FK_PhieuDangKyHienMaus_DotHienMaus_MaDot] FOREIGN KEY ([MaDot]) REFERENCES [DotHienMaus] ([MaDot]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PhieuDangKyHienMaus_NguoiHienMaus_MaNguoiHien] FOREIGN KEY ([MaNguoiHien]) REFERENCES [NguoiHienMaus] ([MaNguoiHien]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE TABLE [KetQuaHienMaus] (
        [MaKetQuaHienMau] int NOT NULL IDENTITY,
        [MaPhieu] int NOT NULL,
        [DiemDanhGia] int NULL,
        [NhanXet] nvarchar(2000) NOT NULL,
        [KetQua] nvarchar(60) NOT NULL,
        [NgayCapNhat] datetime2 NOT NULL,
        CONSTRAINT [PK_KetQuaHienMaus] PRIMARY KEY ([MaKetQuaHienMau]),
        CONSTRAINT [FK_KetQuaHienMaus_PhieuDangKyHienMaus_MaPhieu] FOREIGN KEY ([MaPhieu]) REFERENCES [PhieuDangKyHienMaus] ([MaPhieu]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE TABLE [LichKhamSangLocs] (
        [MaLichKhamSangLoc] int NOT NULL IDENTITY,
        [MaPhieu] int NOT NULL,
        [ThoiGianBatDau] datetime2 NOT NULL,
        [ThoiGianKetThuc] datetime2 NOT NULL,
        [HinhThucTiepNhan] nvarchar(80) NOT NULL,
        [DiaDiemHoacLienKet] nvarchar(250) NOT NULL,
        [BacSiSangLoc] nvarchar(120) NOT NULL,
        [BacSiChuanHoa] nvarchar(120) NOT NULL,
        [GhiChu] nvarchar(1000) NOT NULL,
        [TrangThai] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_LichKhamSangLocs] PRIMARY KEY ([MaLichKhamSangLoc]),
        CONSTRAINT [FK_LichKhamSangLocs_PhieuDangKyHienMaus_MaPhieu] FOREIGN KEY ([MaPhieu]) REFERENCES [PhieuDangKyHienMaus] ([MaPhieu]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE INDEX [IX_DotHienMaus_MaDonViToChuc] ON [DotHienMaus] ([MaDonViToChuc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_KetQuaHienMaus_MaPhieu] ON [KetQuaHienMaus] ([MaPhieu]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE INDEX [IX_LichKhamSangLocs_BacSiChuanHoa_ThoiGianBatDau] ON [LichKhamSangLocs] ([BacSiChuanHoa], [ThoiGianBatDau]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_LichKhamSangLocs_MaPhieu] ON [LichKhamSangLocs] ([MaPhieu]) WHERE [TrangThai] = N''Đã lên lịch''');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NguoiHienMaus_MaTaiKhoan] ON [NguoiHienMaus] ([MaTaiKhoan]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE INDEX [IX_PhieuDangKyHienMaus_MaDot] ON [PhieuDangKyHienMaus] ([MaDot]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE INDEX [IX_PhieuDangKyHienMaus_MaNguoiHien] ON [PhieuDangKyHienMaus] ([MaNguoiHien]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TaiKhoans_TenDangNhap] ON [TaiKhoans] ([TenDangNhap]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261018110536_Module5Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261018110536_Module5Initial', N'10.0.13');
END;

COMMIT;
GO
