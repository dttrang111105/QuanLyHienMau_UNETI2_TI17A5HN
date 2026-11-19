# 00. TỔNG QUAN PROJECT & BÁO CÁO KIỂM TOÁN TOÀN DIỆN (ĐỀ TÀI 29)

- **Tên đề tài chính thức:** ĐỀ TÀI 29 – XÂY DỰNG HỆ THỐNG QUẢN LÝ HIẾN MÁU NHÂN ĐẠO VÀ PHIẾU ĐĂNG KÝ HIẾN MÁU
- **Tài liệu chuẩn đối chiếu:** `De_tai_29_quan_ly_hien_mau_nhan_dao_va_phieu_dang_ky_hien_mau_ASP_NET_Core_10_MVC_Nhom_5_ban (2).docx`
- **Repository:** `https://github.com/dttrang111105/QuanLyHienMau_UNETI2_TI17A5HN.git`
- **Nhánh kiểm toán:** `merge1` (Commit `0b0d7d6926094f398cdbfd88e18658570650be63`)
- **Chế độ kiểm toán:** Read-Only Audit (Không chỉnh sửa source code, không xóa file, không tác động Database thật, không commit/push)

---

## 1. Kiến trúc tổng thể & Công nghệ sử dụng

| Hạng mục | Thực tế triển khai trong Project | Đối chiếu Mục 2 & Mục 11 Đề bài | Đánh giá |
|---|---|---|---|
| **Framework & Runtime** | ASP.NET Core 10.0 MVC (`net10.0`), C# (`Nullable: enable`, `ImplicitUsings: enable`) | Bắt buộc ASP.NET Core 10.0 MVC + C# | **Đạt (100%)** |
| **Giao diện (UI)** | Razor Views (`.cshtml`), TailwindCSS CDN + Custom Design System, Material Symbols | Razor View + HTML5, CSS3, Bootstrap 5, JS | **Đạt** (Giao diện đồng bộ, hiện đại, có tách `_Layout.cshtml` cho cổng cộng đồng và `_AdminLayout.cshtml` cho cổng quản trị) |
| **Truy cập dữ liệu (ORM)** | Entity Framework Core 10.0.5 (`Microsoft.EntityFrameworkCore.SqlServer`, `Tools`), LINQ | EF Core 10 + LINQ + SQL Server Code First | **Đạt** |
| **Kiến trúc phân tầng** | `Models` → `Data (DbContext)` → `Repositories` → `Services` → `Controllers` → `Views` + `ViewModels` | Khuyến khích mạnh sử dụng **Repository Pattern** và **ViewModel** | **Một phần (PARTIAL)** – Nhiều action trong [`AdminController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs) vẫn truy cập trực tiếp `_context` thay vì qua Repository; tồn tại **2 hệ thống Repository song song** |
| **Xác thực & Phân quyền** | Cookie Authentication (`UNETI_BloodDonation_Auth`) + Session (`IdleTimeout = 60 phút`) + `[Authorize(Roles = ...)]` + Custom Filter [`KiemTraQuyenAttribute`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Filters/KiemTraQuyenAttribute.cs) | Phân quyền 3 vai trò: `Admin`, `Nhân viên y tế`, `Người hiến máu`; Session lưu trạng thái đăng nhập | **Một phần (PARTIAL)** – Có xung đột tên Role và cơ chế đọc Session giữa Module 1–4 và Module 5 |
| **Mã hóa mật khẩu** | `PasswordHasher<TaiKhoan>` (PBKDF2 chuẩn của ASP.NET Core Identity) | Không lưu mật khẩu dạng plain text (Mục 5.2 & Mục 15) | **Đạt (100%)** |

---

## 2. Cấu trúc thư mục Project hiện tại

```text
QuanLyHienMau_UNETI2_TI17A5HN/
├── Controllers/                  # 11 Controllers (Tồn tại trùng lặp nặng giữa AdminController và 6 Controller chuyên biệt)
│   ├── AccountController.cs      # Đăng nhập, Đăng ký, Đăng xuất, AccessDenied (Module 1)
│   ├── TaiKhoanController.cs     # Quản lý tài khoản, tạo TK nội bộ, khóa/mở khóa, đổi MK (Module 1 - bị trùng với AdminController)
│   ├── DonViToChucController.cs  # CRUD Đơn vị tổ chức (Module 1 - bị trùng với AdminController)
│   ├── DotHienMauController.cs   # CRUD & Tra cứu Đợt hiến máu (Module 2 - phần Admin bị trùng với AdminController)
│   ├── NguoiHienMauController.cs # Hồ sơ cá nhân người hiến máu (Module 3)
│   ├── DangKyController.cs       # Đăng ký hiến máu, Theo dõi & Hủy phiếu (Module 3)
│   ├── LichKhamSangLocController.cs # Lịch khám sàng lọc (Module 5 - bị ẩn khỏi menu _AdminLayout, trùng với AdminController)
│   ├── KetQuaHienMauController.cs   # Kết quả hiến máu (Module 5 - bị ẩn khỏi menu _AdminLayout, trùng với AdminController)
│   ├── ThongKeController.cs      # Dashboard & Thống kê tổng hợp (Module 5)
│   ├── AdminController.cs        # "God Controller" (1.648 dòng) ôm đồm cả Module 1, 2, 3, 4, 5 và 7 màn hình ngoài phạm vi đề tài
│   └── HomeController.cs         # Trang chủ cộng đồng, Giới thiệu, Quyền lợi, Liên hệ
├── Data/
│   ├── ApplicationDbContext.cs   # EF Core DbContext (7 DbSet chính + 1 DbSet phụ DotQuotaAlert)
│   ├── DbInitializer.cs          # Seed Data tự động khi khởi động (Chưa đủ số lượng tối thiểu theo Mục 16)
│   └── CauHinhKhamSangLoc.cs     # File cấu hình Fluent API của Module 5 nhưng KHÔNG ĐƯỢC GỌI trong OnModelCreating (Dead Code)
├── Filters/
│   └── KiemTraQuyenAttribute.cs  # Custom ActionFilter kiểm tra quyền của Module 5
├── Migrations/                   # 3 bản Migration Code First (InitialCreate, AddDotQuotaAlert, AddAccountAndScreeningFields)
├── Models/                       # 7 thực thể bắt buộc + 1 thực thể phụ (DotQuotaAlert) + hằng số TrangThaiHienMau + AccountViewModels
├── Repositories/                 # 2 nhánh Repository tồn tại song song (Interfaces/IRepository vs EfGenericRepository + Concrete Repos)
├── Services/                     # DangKyDichVu, KhamSangLocService, ThongKeService (Dead Code), DongHoHeThong, KetQuaXuLy
├── ViewModels/                   # 7 ViewModel phục vụ Module 5 (LichKham, KetQua, Dashboard, ThongKe)
└── Views/                        # 11 thư mục View tương ứng với 11 Controller + Shared (_Layout, _AdminLayout)
```

---

## 3. Phân ranh giới 5 Module thực tế trong Codebase

| Module | Thành viên phụ trách (Theo Header Code) | Các thành phần trong Codebase | Đánh giá mức độ hoàn thiện |
|---|---|---|---|
| **Module 1: Tài khoản, Phân quyền & Đơn vị tổ chức** | **Nguyễn Tiến Nam** (`23103100275`) | [`AccountController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AccountController.cs), [`TaiKhoanController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/TaiKhoanController.cs), [`DonViToChucController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DonViToChucController.cs), [`TaiKhoanRepository.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/TaiKhoanRepository.cs), [`DonViToChucRepository.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/DonViToChucRepository.cs), cùng các action trùng lặp trong [`AdminController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs) | **Tốt (92%)** – Đầy đủ nghiệp vụ, có mã hóa mật khẩu, tự tạo `NguoiHienMau` khi đăng ký. Bị lặp code giữa `TaiKhoanController`/`DonViToChucController` và `AdminController`. |
| **Module 2: Quản lý Đợt hiến máu, Tìm kiếm, Lọc, Sắp xếp, Phân trang** | **Nguyễn Thị Phương Thảo** (`23103100272`) | [`DotHienMau.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/DotHienMau.cs), [`DotHienMauController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DotHienMauController.cs), [`DotHienMauRepository.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/DotHienMauRepository.cs), `Views/DotHienMau/*`, cùng các action `DotHienMau`/`AdminCreateDot`/`AdminEditDot` trong [`AdminController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs) | **Rất tốt (95%)** – Tuân thủ chuẩn Repository Pattern, `IValidatableObject`, tìm kiếm/lọc/sắp xếp/phân trang `Skip/Take` chuẩn mực. |
| **Module 3: Quản lý Người hiến máu & Đăng ký tham gia hiến máu** | **Đỗ Thu Trang** (Nhóm Module 3 – Chưa ghi rõ MSSV trên đầu file) | [`NguoiHienMau.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/NguoiHienMau.cs), [`PhieuDangKyHienMau.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Models/PhieuDangKyHienMau.cs), [`NguoiHienMauController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs), [`DangKyController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs), [`NguoiHienMauRepository.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/NguoiHienMauRepository.cs), [`PhieuDangKyRepository.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs), `AdminController.NguoiHienMau` | **Tốt (90%)** – Kiểm tra chặt chẽ tuổi 18–60, cân nặng tối thiểu của đợt, sức khỏe, trùng đợt, 84 ngày giữa 2 lần hiến thành công, khóa giao dịch `Serializable`. Tuy nhiên `AdminController.NguoiHienMau` dùng trực tiếp `_context` và thiếu phân trang. |
| **Module 4: Xét duyệt Phiếu đăng ký & Quản lý quy trình xử lý phiếu** | **Chưa ghi Header tác giả** | `AdminController` (`PhieuDangKyHienMau`, `ChiTietPhieuDangKy`, `CapNhatTrangThaiPhieu`, `XacNhanHangLoat`), [`EfPhieuDangKyRepository.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/EfPhieuDangKyRepository.cs), `Views/Admin/PhieuDangKyHienMau.cshtml`, `Views/Admin/ChiTietPhieuDangKy.cshtml` | **Khá (82%)** – Có lọc/sắp xếp/phân trang, lịch sử người hiến, cảnh báo chỉ tiêu (`DotQuotaAlert`), duyệt/từ chối/chuyển chờ khám. Tuy nhiên cho phép chuyển trạng thái sang `"Chờ khám sàng lọc"` mà không cần tạo `LichKhamSangLoc`, gây lệch trạng thái với Module 5. |
| **Module 5: Lịch khám sàng lọc, Kết quả hiến máu & Thống kê** | **Chưa ghi Header tác giả** | **Nhánh 1 (Chuẩn):** [`LichKhamSangLocController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/LichKhamSangLocController.cs), [`KetQuaHienMauController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/KetQuaHienMauController.cs), [`ThongKeController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/ThongKeController.cs), [`KhamSangLocService.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Services/KhamSangLocService.cs).<br>**Nhánh 2 (Trong AdminController):** `LichKhamSangLoc`, `TaoLichKham`, `SuaLichKham`, `HuyLichKham`, `HoanThanhKham`, `KetQuaHienMau`, `TaoKetQua`, `SuaKetQua` | **Rủi ro cao do phân mảnh 2 nhánh (75%)** – Nhánh `LichKhamSangLocController` + `KhamSangLocService` viết rất chuẩn nghiệp vụ (kiểm tra trùng lịch người hiến, trùng lịch bác sĩ, ngày quá khứ), nhưng Sidebar `_AdminLayout.cshtml` lại trỏ vào `AdminController.LichKhamSangLoc` (nơi **thiếu toàn bộ kiểm tra trùng lịch**!). |

---

## 4. Điểm mạnh nổi bật của Project hiện tại

1. **Đúng chuẩn công nghệ yêu cầu:** Sử dụng đúng ASP.NET Core 10.0 MVC (`net10.0`) và EF Core 10.0.5 Code First kết nối SQL Server, không dùng thư viện ngoài luồng.
2. **Mô hình dữ liệu (7 bảng bắt buộc) khớp 100% với Mục 4 Đề bài:** Đầy đủ `TaiKhoan`, `DonViToChuc`, `DotHienMau`, `NguoiHienMau`, `PhieuDangKyHienMau`, `LichKhamSangLoc`, `KetQuaHienMau` với cấu hình khóa ngoại `DeleteBehavior.Restrict` và `SetNull` hợp lý trong [`ApplicationDbContext.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Data/ApplicationDbContext.cs#L15-L120).
3. **Nghiệp vụ cốt lõi ở Module 2, Module 3 và Service Module 5 rất sâu:**
   - Kiểm tra đầy đủ 7 điều kiện đăng ký hiến máu (tuổi 18–60, cân nặng theo đợt, 84 ngày giữa 2 lần hiến thành công, trùng đợt, sức khỏe, trạng thái đợt, trạng thái tài khoản) trong [`DangKyController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L308-L410).
   - Có sử dụng `IsolationLevel.Serializable` để chống race condition khi nộp phiếu, duyệt phiếu, lập lịch và ghi kết quả.
   - Có tính năng nâng cao: Tự động cảnh báo đạt chỉ tiêu đơn vị máu của đợt (`DotQuotaAlert`) và Khôi phục phiếu bị kẹt ở trạng thái `"Chờ khám sàng lọc"` (`KhoiPhucPhieuChoKhamAsync`).

---

## 5. Tóm tắt 6 vấn đề lớn nhất đang tồn tại trong Project

> [!CAUTION]
> **1. Tồn tại 2 hệ thống Module 5 chạy song song nhưng Sidebar đang trỏ vào nhánh thiếu kiểm tra nghiệp vụ:**
> Thành viên Module 5 đã xây dựng [`LichKhamSangLocController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/LichKhamSangLocController.cs) và [`KhamSangLocService.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Services/KhamSangLocService.cs) có kiểm tra đầy đủ trùng lịch người hiến, trùng lịch bác sĩ và cấm lập lịch quá khứ (đúng Mục 9.2 đề bài). Tuy nhiên, thanh điều hướng [`_AdminLayout.cshtml`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/Shared/_AdminLayout.cshtml#L182-L189) lại trỏ sang `/Admin/LichKhamSangLoc` và `/Admin/KetQuaHienMau` trong [`AdminController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L434-L548) — nơi **KHÔNG HỀ kiểm tra trùng lịch người hiến, trùng lịch bác sĩ hay thời gian quá khứ**!

> [!WARNING]
> **2. `AdminController` chứa 6 màn hình giao diện "giả" (Mock UI) nằm ngoài phạm vi Đề tài 29:**
> Trong [`_AdminLayout.cshtml`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/Shared/_AdminLayout.cshtml#L190-L240) và [`AdminController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L688-L716) có hiển thị công khai các menu: *Hồ sơ bệnh sử lâm sàng (`HoSoBenhSu`), Kiểm định & Tiêu hủy túi máu (`TieuHuy`), Chuỗi hành trình & Giám sát túi máu (`GiamSatTuiMau`), Kho lưu trữ máu (`KhoLuuTruMau`), Cấu hình phân quyền (`RBAC`), Theo dõi nhiệt độ kho máu (`KhoLanhIoT`)*. Các trang này chỉ là HTML tĩnh, hoàn toàn không có bảng Database hay xử lý Backend. Khi bảo vệ, giảng viên click vào các menu này sẽ đánh giá là làm giao diện giả hoặc vượt phạm vi đề tài (Mục 3 đề bài đã giới hạn rõ: *"Không đi sâu vào nghiệp vụ xét nghiệm y khoa phức tạp sau hiến máu"*).

> [!WARNING]
> **3. Dữ liệu mẫu (`DbInitializer.cs`) chưa đạt số lượng tối thiểu bắt buộc tại Mục 16 Đề bài:**
> Mục 16 yêu cầu tối thiểu: **05** Đơn vị tổ chức, **15** Đợt hiến máu (đủ chia nhiều trang), **30** Người hiến máu có tài khoản, **40–50** Phiếu đăng ký, **15–20** Lịch khám sàng lọc, và dữ liệu Kết quả hiến máu ở cả 2 trạng thái. Hiện tại [`DbInitializer.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Data/DbInitializer.cs) chỉ tạo **4** đơn vị, **5** đợt (chỉ có 1 trang duy nhất vì `PageSize = 6`), **9** người hiến (chỉ 1 người có tài khoản), **8** phiếu đăng ký, **0** lịch khám và **0** kết quả hiến máu!

> [!WARNING]
> **4. Thiếu phân quyền cấp Action cho chức năng chỉ dành cho Admin trong `AdminController`:**
> [`AdminController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L15) khai báo `[Authorize(Roles = "Admin,Medical Staff")]` ở cấp class, nhưng action `KhoaMoKhoaNguoiHien(int id)` (dòng 655) **không gắn `[Authorize(Roles = "Admin")]`**, dẫn đến Nhân viên y tế cũng có thể gọi POST khóa/mở khóa người hiến máu (trái với Mục 5.1 & 7.1 đề bài: chỉ Admin quản lý danh sách người hiến máu và khóa/mở khóa).

> [!IMPORTANT]
> **5. Trùng lặp Controller và Repository giữa các thành viên:**
> - Quản lý Tài khoản có ở cả `TaiKhoanController` và `AdminController.NhanSu`.
> - Quản lý Đơn vị tổ chức có ở cả `DonViToChucController` và `AdminController.DonViToChuc`.
> - Quản lý Đợt hiến máu có ở cả `DotHienMauController` và `AdminController.DotHienMau`.
> - Lịch khám & Kết quả có ở cả `LichKhamSangLocController`/`KetQuaHienMauController` và `AdminController`.
> - Tồn tại 2 Generic Repository (`Repositories/Interfaces/IRepository.cs` và `Repositories/IRepository.cs`) và 2 `IPhieuDangKyRepository`.

> [!IMPORTANT]
> **6. Thiếu Header thông tin sinh viên (Họ tên, MSSV, Nội dung thực hiện) trên nhiều file:**
> Mục 18 Đề bài bắt buộc mọi file `Model`, `Controller`, `ViewModel`, `View` chính phải ghi chú rõ `Họ và tên`, `Mã sinh viên`, `Nội dung thực hiện`. Hiện chỉ có Module 1 và Module 2 tuân thủ đầy đủ; các file thuộc Module 3, Module 4, Module 5 chưa ghi tên và MSSV cá nhân.
