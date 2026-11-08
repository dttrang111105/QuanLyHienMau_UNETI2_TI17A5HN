# Hệ Thống Quản Lý Hiến Máu Nhân Đạo Vitalis - UNETI (`QuanLyHienMau_UNETI2_TI17A5HN`)

Dự án ASP.NET Core MVC (.NET 10.0) sử dụng **Entity Framework Core 10.0.12 (Code First + Migrations)** kết nối **SQL Server LocalDB**.

---

## 1. Yêu cầu môi trường (Prerequisites)

Thành viên mới trước khi clone và chạy dự án cần cài đặt:

1. **.NET 10.0 SDK** (khuyến nghị `10.0.100` trở lên; đã kiểm tra trên `.NET SDK 10.0.301`).
   - Kiểm tra phiên bản:
     ```powershell
     dotnet --version
     ```
2. **SQL Server Express LocalDB** (đi kèm bộ cài Visual Studio 2022/2026 workload *ASP.NET and web development* hoặc *Data storage and processing*, hoặc cài đặt từ bộ cài SQL Server Express).
   - Kiểm tra và khởi động instance `(localdb)\MSSQLLocalDB`:
     ```powershell
     sqllocaldb info MSSQLLocalDB
     sqllocaldb start MSSQLLocalDB
     ```
3. **Công cụ `dotnet-ef` CLI** (nếu muốn chạy lệnh migration từ terminal):
   ```powershell
   dotnet tool install --global dotnet-ef
   ```

---

## 2. Cấu trúc thư mục dự án

```text
QuanLyHienMau_UNETI2_TI17A5HN/                  # Thư mục gốc repository
├── QuanLyHienMau_UNETI2_TI17A5HN.slnx          # Solution file (định dạng .slnx)
└── QuanLyHienMau_UNETI2_TI17A5HN/              # Thư mục Web Project
    ├── QuanLyHienMau_UNETI2_TI17A5HN.csproj    # File cấu hình project (.NET 10.0)
    ├── Program.cs                              # Điểm khởi chạy, đăng ký DI & gọi DbInitializer
    ├── appsettings.json                        # Chứa ConnectionStrings:DefaultConnection
    ├── Controllers/                            # Các MVC Controllers
    ├── Data/
    │   ├── ApplicationDbContext.cs             # DbContext & cấu hình Fluent API
    │   └── DbInitializer.cs                    # Tự động chạy Migrate() và Seed dữ liệu mẫu
    ├── Migrations/                             # Chứa EF Core Migrations (InitialCreate)
    ├── Models/                                 # 7 Entity Models + ErrorViewModel
    └── Views/                                  # Giao diện Razor Views (.cshtml)
```

---

## 3. Hướng dẫn Clone và Chạy dự án cho thành viên trong nhóm

### Bước 1: Clone repository và di chuyển vào thư mục gốc
```powershell
git clone <URL_REPOSITORY_GITHUB>
cd QuanLyHienMau_UNETI2_TI17A5HN
```

### Bước 2: Khôi phục các gói thư viện (Restore packages)
```powershell
dotnet restore "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj"
```

### Bước 3: Biên dịch dự án (Build)
```powershell
dotnet build "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --no-restore
```

### Bước 4: Tạo / Cập nhật cơ sở dữ liệu từ EF Core Migrations
Cấu hình kết nối mặc định trong `QuanLyHienMau_UNETI2_TI17A5HN/appsettings.json` sử dụng Windows Authentication tới `(localdb)\mssqllocaldb`, tên database là `QuanLyHienMau_UNETI` (không cần cấu hình mật khẩu):

```powershell
dotnet ef database update --project "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --startup-project "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj"
```
*(Hoặc trong **Package Manager Console** của Visual Studio, chọn Default project là `QuanLyHienMau_UNETI2_TI17A5HN` và chạy lệnh `Update-Database`).*

> **Lưu ý:** Khi ứng dụng khởi chạy (`Program.cs`), hàm `DbInitializer.Initialize(context)` cũng sẽ tự động gọi `context.Database.Migrate()` để áp dụng các migration còn thiếu và tự động nạp dữ liệu mẫu ban đầu nếu bảng đang trống.

### Bước 5: Chạy ứng dụng (Run Website)
```powershell
dotnet run --project "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj"
```
Truy cập địa chỉ hiển thị trên terminal:
- HTTP: `http://localhost:5117`
- HTTPS: `https://localhost:7217`

---

## 4. Tài khoản dữ liệu mẫu (Seed Data)

Sau khi chạy lần đầu trên môi trường sạch, `DbInitializer` tự động khởi tạo:
- **3 tài khoản mặc định** (mật khẩu được băm bằng `PasswordHasher<TaiKhoan>`):
  - Quản trị viên (`Admin`): Tên đăng nhập `admin` | Mật khẩu: Xem trong `Data/DbInitializer.cs`
  - Nhân viên y tế (`Medical Staff`): Tên đăng nhập `bacsi` | Mật khẩu: Xem trong `Data/DbInitializer.cs`
  - Người hiến máu (`Donor`): Tên đăng nhập `donor` | Mật khẩu: Xem trong `Data/DbInitializer.cs`
- **4 Đơn vị tổ chức** (`DonViToChuc`) và **5 Đợt hiến máu** (`DotHienMau`).
