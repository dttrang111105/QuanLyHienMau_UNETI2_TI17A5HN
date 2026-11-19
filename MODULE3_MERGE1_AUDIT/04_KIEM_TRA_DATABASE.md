# 04. KẾT QUẢ KIỂM TRA DATABASE VÀ TÍNH TOÀN VẸN DỮ LIỆU

Tài liệu này ghi nhận kết quả kiểm tra cấu trúc Database, Entity, `ApplicationDbContext`, Migrations, truy vấn LINQ/CRUD và khả năng khởi chạy khi clone dự án trên nhánh `origin/merge1` (commit `0b0d7d6926094f398cdbfd88e18658570650be63`).

> **Cam kết an toàn dữ liệu:** Toàn bộ quá trình kiểm thử ghi dữ liệu (Insert/Update/Delete) đã được thực hiện trên một Database thử nghiệm tách biệt hoàn toàn (`QuanLyHienMau_UNETI_AuditM3`) thông qua biến môi trường `ConnectionStrings__DefaultConnection` và đã được xóa sạch sau khi kiểm thử xong. Database thật (`QuanLyHienMau_UNETI`) không bị tác động.

---

## 1. Kiểm tra các Entity, Khóa chính (PK), Khóa ngoại (FK) và Quan hệ giữa các bảng

Hệ thống có **7 bảng nghiệp vụ** (và 1 bảng `__EFMigrationsHistory`), trong đó 2 bảng trung tâm của Module 3 là `NguoiHienMau` và `PhieuDangKyHienMau`:

### 1.1. Bảng `NguoiHienMau` (`Models/NguoiHienMau.cs`)
- **Khóa chính (PK):** `MaNguoiHien` (`int IDENTITY(1,1)`).
- **Khóa ngoại (FK):** `MaTaiKhoan` (`int?` – nullable) tham chiếu tới `TaiKhoan(MaTaiKhoan)`.
- **Quan hệ:** **1 – 0..1 (One-to-One tùy chọn)** giữa `TaiKhoan` và `NguoiHienMau`.
  - Được bảo vệ bằng **Filtered Unique Index** `IX_NguoiHienMau_MaTaiKhoan` (`WHERE [MaTaiKhoan] IS NOT NULL`) trong `Migrations/20260405134900_InitialCreate.cs` (dòng 247–252).
  - Cho phép mỗi `TaiKhoan` chỉ liên kết tối đa 1 `NguoiHienMau`, đồng thời vẫn hỗ trợ các bản ghi `NguoiHienMau` độc lập (`MaTaiKhoan = NULL`) phục vụ dữ liệu lịch sử hoặc dữ liệu mẫu.
- **Ràng buộc cột (Constraints):**
  - `HoTen`: `nvarchar(100) NOT NULL`
  - `NgaySinh`: `datetime2 NULL`
  - `GioiTinh`: `nvarchar(10) NULL`
  - `SoDienThoai`: `nvarchar(15) NULL` (`[RegularExpression(@"^0[0-9]{9,10}$")]`)
  - `Email`: `nvarchar(100) NULL` (`[EmailAddress]`)
  - `DiaChi`: `nvarchar(255) NULL`
  - `NhomMau`: `nvarchar(20) NULL`
  - `NgheNghiep`: `nvarchar(100) NULL`
  - `CanNang`: `float NULL` (`[Range(30.0, 200.0)]`)
  - `GhiChuSucKhoe`: `nvarchar(500) NULL`
  - `TrangThai`: `nvarchar(50) NOT NULL` (Mặc định: `"Hoạt động"`)
  - `NgayTao`: `datetime2 NOT NULL` (Mặc định: `DateTime.Now`)

### 1.2. Bảng `PhieuDangKyHienMau` (`Models/PhieuDangKyHienMau.cs`)
- **Khóa chính (PK):** `MaPhieu` (`int IDENTITY(1,1)`).
- **Khóa ngoại (FK):**
  1. `MaNguoiHien` (`int NOT NULL`) tham chiếu `NguoiHienMau(MaNguoiHien)` – Quan hệ **1 – N** (`NguoiHienMau` -> `PhieuDangKyHienMaus`).
  2. `MaDot` (`int NOT NULL`) tham chiếu `DotHienMau(MaDot)` – Quan hệ **1 – N** (`DotHienMau` -> `PhieuDangKyHienMaus`).
- **Quan hệ với các bảng Module 4 & 5:**
  - **1 – N** với `LichKhamSangLoc` (`MaPhieu`, `DeleteBehavior.Cascade`).
  - **1 – 0..1** với `KetQuaHienMau` (`MaPhieu` có Unique Index `IX_KetQuaHienMau_MaPhieu`, `DeleteBehavior.Cascade`).
- **Ràng buộc cột (Constraints):**
  - `NgayNop`: `datetime2 NOT NULL`
  - `TrangThai`: `nvarchar(50) NOT NULL` (`"Chờ xác nhận"`, `"Đã xác nhận"`, `"Từ chối"`, `"Đã hủy"`, `"Hiến máu thành công"`, `"Không đủ điều kiện hiến"`)
  - `LyDoHienMau`: `nvarchar(500) NULL`
  - `GhiChu`: `nvarchar(500) NULL`
  - `NgayXuLy`: `datetime2 NULL`
  - `GhiChuXacNhan`: `nvarchar(500) NULL`

---

## 2. Kiểm tra `ApplicationDbContext` và Quy tắc Xóa (`DeleteBehavior`)

Trong `Data/ApplicationDbContext.cs` (dòng 24–78), cấu hình Fluent API được thiết lập chặt chẽ, bảo vệ toàn vẹn tham chiếu:

| Quan hệ giữa các bảng | Cấu hình `DeleteBehavior` | Đánh giá tính toàn vẹn dữ liệu |
| :--- | :---: | :--- |
| `TaiKhoan` (1) <-> (0..1) `NguoiHienMau` | `DeleteBehavior.SetNull` | **Rất tốt:** Nếu xóa tài khoản đăng nhập, hồ sơ y tế và lịch sử hiến máu của `NguoiHienMau` không bị mất mà chỉ đặt `MaTaiKhoan = NULL`. |
| `NguoiHienMau` (1) -> (N) `PhieuDangKyHienMau` | `DeleteBehavior.Restrict` | **Rất tốt:** Ngăn chặn việc xóa nhầm hồ sơ người hiến máu khi người đó đã có phiếu đăng ký hoặc lịch sử hiến máu. |
| `DotHienMau` (1) -> (N) `PhieuDangKyHienMau` | `DeleteBehavior.Restrict` | **Rất tốt:** Ngăn chặn xóa đợt hiến máu đã có người đăng ký tham gia. |
| `PhieuDangKyHienMau` (1) -> (N) `LichKhamSangLoc` | `DeleteBehavior.Cascade` | Hợp lý: Lịch khám sàng lọc phụ thuộc hoàn toàn vào phiếu đăng ký. |
| `PhieuDangKyHienMau` (1) -> (0..1) `KetQuaHienMau` | `DeleteBehavior.Cascade` | Hợp lý: Kết quả hiến máu gắn 1–1 với phiếu đăng ký. |

---

## 3. Kiểm tra Migration, `DbInitializer` và Khả năng Clone & Run

- **Migrations hiện có:**
  - `20260405134900_InitialCreate.cs`
  - `ApplicationDbContextModelSnapshot.cs`
- **Cơ chế tự động khởi tạo trong `Program.cs` (dòng 104–120):**
  ```csharp
  using (var scope = app.Services.CreateScope())
  {
      var services = scope.ServiceProvider;
      var context = services.GetRequiredService<ApplicationDbContext>();
      DbInitializer.Initialize(context);
      DbInitializer.SeedTestData(context);
  }
  ```
- **Kiểm chứng thực tế khi thành viên khác clone repository về máy:**
  - Chuỗi kết nối trong `appsettings.json` sử dụng `Server=(localdb)\mssqllocaldb;Database=QuanLyHienMau_UNETI;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True` — chuẩn mặc định của Visual Studio / .NET SDK trên Windows, không chứa đường dẫn cứng hay mật khẩu cá nhân.
  - Khi chạy trên máy mới chưa có Database (đã kiểm chứng bằng cách trỏ sang DB mới tinh `QuanLyHienMau_UNETI_AuditM3`), `DbInitializer.Initialize(context)` tự động gọi `context.Database.Migrate()` để tạo toàn bộ 8 bảng và nạp sẵn:
    - **3 tài khoản chuẩn:** `admin` / `Admin@123`, `bacsi` / `BacSi@123`, `donor` / `Donor@123`.
    - **9 hồ sơ người hiến máu** (1 hồ sơ gắn với tài khoản `donor` + 8 hồ sơ mẫu).
    - **4 đơn vị tổ chức**, **5 đợt hiến máu** và **8 phiếu đăng ký mẫu**.
  - **Kết luận Clone & Run:** Thành viên khác trong nhóm clone nhánh `merge1` về máy có thể bấm chạy ngay (`F5` hoặc `dotnet run`) mà không cần chạy lệnh `Update-Database` thủ công.

---

## 4. Kiểm tra các Truy vấn LINQ, Thao tác CRUD và Xử lý Đồng thời (Concurrency)

1. **Truy vấn LINQ trong `NguoiHienMauRepository` và `PhieuDangKyRepository`:**
   - Sử dụng `.Include(...)` và `.ThenInclude(...)` đầy đủ (`PhieuDangKyHienMaus` -> `DotHienMau` -> `DonViToChuc`, `LichKhamSangLocs`, `KetQuaHienMau`), tránh lỗi `NullReferenceException` khi View truy cập các thuộc tính điều hướng (navigation properties).
   - Các truy vấn chỉ đọc (`GetHistoryByNguoiHienAsync`, `GetTrackingListByNguoiHienAsync`, `GetByIdWithTrackingDetailsAsync`) đều có `.AsNoTracking()`, tối ưu hiệu năng bộ nhớ cho EF Core.
2. **Bảo vệ chống Race Condition khi nộp phiếu đăng ký (`DangKyController.Create` dòng 511–576):**
   - Kết hợp khóa đồng bộ trong tiến trình (`ConcurrentDictionary<int, SemaphoreSlim> _submissionLocks` theo `MaNguoiHien`) và giao dịch cơ sở dữ liệu mức cô lập cao nhất (`BeginTransactionAsync(IsolationLevel.Serializable)`).
   - Đảm bảo nếu người dùng bấm đúp nút "Nộp phiếu" hoặc gửi 2 request song song, hệ thống không bao giờ tạo ra 2 phiếu đăng ký trùng lặp cho cùng một đợt hiến máu.
3. **Kiểm tra dữ liệu sau mỗi nghiệp vụ CRUD (Đã xác nhận qua SQL Query trực tiếp):**
   - Sau `GET /NguoiHienMau/HoSoCaNhan` với tài khoản mới: bản ghi `NguoiHienMau` được `INSERT` chính xác.
   - Sau `POST /NguoiHienMau/HoSoCaNhan`: bản ghi `NguoiHienMau` và `TaiKhoan` cùng được `UPDATE` chính xác.
   - Sau `POST /DangKy/Create`: bản ghi `PhieuDangKyHienMau` được `INSERT` với `TrangThai = "Chờ xác nhận"`, `NgayNop` chính xác.
   - Sau `POST /DangKy/Huy`: bản ghi `PhieuDangKyHienMau` được `UPDATE` sang `TrangThai = "Đã hủy"`, `NgayXuLy` và `GhiChuXacNhan` được ghi nhận đầy đủ.
4. **Một số điểm cần lưu ý ở tầng Database/LINQ:**
   - `AdminController.NguoiHienMau` (dòng 657–662) nạp toàn bộ bảng `NguoiHienMaus` kèm `PhieuDangKyHienMaus` vào bộ nhớ (`.ToListAsync()`) rồi mới lọc và phân trang trên RAM. Với quy mô dữ liệu bài tập lớn (vài trăm bản ghi), tốc độ rất nhanh; tuy nhiên như đã nêu tại `BUG-M3-02`, biểu thức lọc `p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null` trong action này đang tính nhầm cả bản ghi `KetQuaHienMau` có kết quả `"Không đủ điều kiện hiến"`.
   - Bảng `TaiKhoan` và `NguoiHienMau` chưa có Unique Index vật lý cho cột `Email` ở mức SQL Server (`BUG-M3-08`).
