# 06. KIỂM TOÁN KIẾN TRÚC, CƠ SỞ DỮ LIỆU, BẢO MẬT & CHẤT LƯỢNG CODE

Tài liệu này đánh giá chuyên sâu về mặt kỹ thuật của dự án `QuanLyHienMau_UNETI2_TI17A5HN` đối chiếu với các yêu cầu tại **Mục 4, Mục 10, Mục 11, Mục 13, Mục 14, Mục 15 và Mục 17** của đề bài chính thức.

---

## 1. Đánh giá Kiến trúc MVC, Repository Pattern & Service Layer (Mục 11 Đề bài)

### 1.1. Mức độ tuân thủ Repository Pattern
- **Điểm đạt:**
  - Module 1 (`TaiKhoanController`, `DonViToChucController`), Module 2 (`DotHienMauController`), Module 3 (`NguoiHienMauController`, `DangKyController`), và Module 5 (`LichKhamSangLocController`, `KetQuaHienMauController`, `ThongKeController`) **hoàn toàn không tiêm trực tiếp `ApplicationDbContext` vào Controller**, mà làm việc 100% thông qua `Repository` hoặc `Service`.
- **Điểm vi phạm (Bất nhất kiến trúc):**
  - [`AdminController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L24-L41) vẫn tiêm trực tiếp `ApplicationDbContext _context` và sử dụng `_context` trong nhiều action (`Dashboard`, `KiemTraDieuKienXacNhanAsync`, `DamBaoCanhBaoChiTieuAsync`, `LichKhamSangLoc`, `NguoiHienMau`, `ChiTietNguoiHien`, `KhoaMoKhoaNguoiHien`, `KetQuaHienMau`, `ChiTietKetQua`).
  - Trong khi đề bài Mục 11 nêu rõ: *"Khuyến khích mạnh việc sử dụng Repository Pattern: Controller không nên viết quá nhiều truy vấn phức tạp trực tiếp với DbContext"* và Mục 21 cảnh báo lỗi: *"Controller ôm toàn bộ xử lý dữ liệu phức tạp, không tách hàm hoặc lớp hợp lý"*.

### 1.2. Đánh giá việc sử dụng ViewModel
- **Điểm đạt:**
  - Module 1 sử dụng `LoginViewModel`, `RegisterViewModel`, `CreateAccountAdminViewModel` ([`AccountViewModels.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/ViewModels/AccountViewModels.cs)).
  - Module 5 sử dụng bộ 7 ViewModel tách biệt rõ ràng trong thư mục `ViewModels/` (`LichKhamFormViewModel`, `LichKhamListViewModel`, `KetQuaFormViewModel`, `KetQuaListViewModel`, `DashboardViewModel`, `ThongKeViewModel`, `ThongKeRows`).
- **Điểm hạn chế:**
  - Một số action trong `AdminController` và `DotHienMauController` truyền khá nhiều dữ liệu qua `ViewBag` (10–15 thuộc tính `ViewBag` trên một action). Tuy không sai trong ASP.NET Core MVC nhưng làm giảm tính chặt chẽ về kiểu dữ liệu (strongly-typed view).

---

## 2. Kiểm toán Thiết kế Cơ sở dữ liệu, Thực thể & Quan hệ (Mục 4 & Mục 10 Đề bài)

### 2.1. Đối chiếu 7 Thực thể bắt buộc
| STT | Bảng theo Đề bài (Mục 4) | Class trong `Models/` | Khóa chính | Các trường thuộc tính so với Đề bài | Đánh giá |
|---:|---|---|---|---|---|
| 1 | `TaiKhoan` | [`TaiKhoan.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/TaiKhoan.cs) | `MaTaiKhoan` | Khớp 100% (`MaTaiKhoan`, `TenDangNhap`, `MatKhau`, `HoTen`, `Email`, `VaiTro`, `TrangThai`) | **Đạt 100%** |
| 2 | `DonViToChuc` | [`DonViToChuc.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/DonViToChuc.cs) | `MaDonViToChuc` | Khớp 100% (`MaDonViToChuc`, `TenDonViToChuc`, `MoTa`, `EmailLienHe`, `TrangThai`) | **Đạt 100%** |
| 3 | `DotHienMau` | [`DotHienMau.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/DotHienMau.cs) | `MaDot` | Khớp 100% cả 11 thuộc tính | **Đạt 100%** |
| 4 | `NguoiHienMau` | [`NguoiHienMau.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/NguoiHienMau.cs) | `MaNguoiHien` | Khớp 100% cả 14 thuộc tính (`MaNguoiHien`, `MaTaiKhoan`, `HoTen`, `NgaySinh`, `GioiTinh`, `SoDienThoai`, `Email`, `DiaChi`, `NhomMau`, `CanNang`, `ChieuCao`, `TinhTrangSucKhoe`, `NgayHienMauGanNhat`, `TrangThai`) | **Đạt 100%** |
| 5 | `PhieuDangKyHienMau` | [`PhieuDangKyHienMau.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/PhieuDangKyHienMau.cs) | `MaPhieu` | Khớp 100% cả 8 thuộc tính (`MaPhieu`, `MaNguoiHien`, `MaDot`, `NgayNop`, `NgayXuLy`, `GhiChuSucKhoe`, `LyDoTuChoi`, `TrangThai`) | **Đạt 100%** |
| 6 | `LichKhamSangLoc` | [`LichKhamSangLoc.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/LichKhamSangLoc.cs) | `MaLichKhamSangLoc` | Khớp 100% cả 9 thuộc tính (`MaLichKhamSangLoc`, `MaPhieu`, `ThoiGianBatDau`, `ThoiGianKetThuc`, `HinhThucTiepNhan`, `DiaDiemHoacLienKet`, `BacSiSangLoc`, `GhiChu`, `TrangThai`) | **Đạt 100%** |
| 7 | `KetQuaHienMau` | [`KetQuaHienMau.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/KetQuaHienMau.cs) | `MaKetQuaHienMau` | Khớp 100% cả 6 thuộc tính (`MaKetQuaHienMau`, `MaPhieu`, `DiemDanhGia`, `NhanXet`, `KetQua`, `NgayCapNhat`) | **Đạt 100%** |
| *(Phụ)* | *(Bảng phụ hỗ trợ cảnh báo chỉ tiêu)* | [`DotQuotaAlert.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/DotQuotaAlert.cs) | `MaDot` (PK & FK) | Bảng phụ 1–1 với `DotHienMau` dùng để lưu thời điểm đợt đạt chỉ tiêu | Hợp lệ, hỗ trợ Mục 8.4 |

### 2.2. Kiểm toán Quan hệ Khóa ngoại & Ràng buộc toàn vẹn trong `ApplicationDbContext`
Kiểm tra [`ApplicationDbContext.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Data/ApplicationDbContext.cs#L15-L120):
1. `TaiKhoan` (1) – (0..1) `NguoiHienMau`: Unique Index trên `MaTaiKhoan` (`[MaTaiKhoan] IS NOT NULL`), `DeleteBehavior.SetNull`. → **Đúng chuẩn Mục 10**.
2. `DonViToChuc` (1) – (N) `DotHienMau`: `DeleteBehavior.Restrict` (Cấm xóa đơn vị đang có đợt hiến máu). → **Đúng chuẩn Mục 5.3 & Mục 10**.
3. `NguoiHienMau` (1) – (N) `PhieuDangKyHienMau`: `DeleteBehavior.Restrict`. → **Đúng chuẩn Mục 10**.
4. `DotHienMau` (1) – (N) `PhieuDangKyHienMau`: `DeleteBehavior.Restrict` (Cấm xóa đợt đang có phiếu đăng ký). → **Đúng chuẩn Mục 6.2 & Mục 10**.
5. `PhieuDangKyHienMau` (1) – (N) `LichKhamSangLoc`: `DeleteBehavior.Restrict`. → **Đúng chuẩn Mục 10**.
6. `PhieuDangKyHienMau` (1) – (0..1) `KetQuaHienMau`: Unique Index trên `KetQuaHienMau.MaPhieu` (Đảm bảo mỗi phiếu chỉ có tối đa 01 kết quả cuối cùng), `DeleteBehavior.Restrict`. → **Đúng chuẩn Mục 9.4 & Mục 10**.

---

## 3. Kiểm toán Validation, Bảo mật & Phân quyền (Mục 13 & Mục 15 Đề bài)

| Hạng mục bảo mật / Validation | Thực trạng trong Codebase | Đánh giá |
|---|---|---|
| **Chống giả mạo Request (CSRF)** | Hầu hết các action `POST` trong cả 10 Controller đều gắn `[ValidateAntiForgeryToken]` và có `@Html.AntiForgeryToken()` trên form. | **Rất tốt** |
| **Chống SQL Injection** | 100% truy vấn dữ liệu sử dụng LINQ to Entities qua EF Core, không nối chuỗi SQL thô (`FromSqlRaw`). | **An toàn tuyệt đối** |
| **Chống xem/sửa dữ liệu người khác (IDOR)** | - `NguoiHienMauController`: Chỉ lấy hồ sơ theo `MaTaiKhoan` của phiên đăng nhập.<br>- `DangKyController`: Kiểm tra `phieu.MaNguoiHien != nguoiHien.MaNguoiHien` trước khi cho hủy phiếu.<br>- `LichKhamSangLocRepository` & `KetQuaHienMauRepository`: Khi người dùng là `Donor`, tự động gắn điều kiện `.Where(x => x.PhieuDangKyHienMau.NguoiHienMau.MaTaiKhoan == maTaiKhoan)`. | **Rất tốt** |
| **Bảo mật mật khẩu** | Sử dụng `PasswordHasher<TaiKhoan>` (PBKDF2-HMAC-SHA512 với salt ngẫu nhiên). Không lưu plain text. | **Đạt chuẩn sản phẩm thật** |
| **Điểm yếu phân quyền cần khắc phục** | 1. `AdminController.KhoaMoKhoaNguoiHien` thiếu `[Authorize(Roles = "Admin")]`.<br>2. `KiemTraQuyenAttribute` chỉ đọc `Session` mà chưa fallback sang `Cookie Claims`. | **Cần sửa (Xem `03_CHUC_NANG_CON_THIEU_VA_LOI.md`)** |

---

## 4. Kiểm toán Cấu hình kết nối & Khả năng chạy trên máy khác (Portability)

Kiểm tra [`appsettings.json`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/appsettings.json) và [`Program.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Program.cs):
- Chuỗi kết nối hiện tại trong `appsettings.json`:
  `Server=(localdb)\\mssqllocaldb;Database=QuanLyHienMau_UNETI2;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`
- Trong [`Program.cs#L43-L57`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Program.cs#L43-L57), khi khởi động ứng dụng sẽ tự động gọi `DbInitializer.SeedAsync(context)` (trong đó gọi `await context.Database.MigrateAsync()`).
- **Ưu điểm:** Bất kỳ máy nào có Visual Studio 2022/2026 kèm SQL Server Express LocalDB đều chỉ cần bấm `F5` là tự động tạo Database `QuanLyHienMau_UNETI2`, chạy đủ 3 bản Migration và nạp dữ liệu mẫu mà không cần chạy lệnh `Update-Database` thủ công!
