# BÁO CÁO KIỂM TRA TOÀN DIỆN MODULE 3 – QUẢN LÝ NGƯỜI HIẾN MÁU VÀ NỘP PHIẾU ĐĂNG KÝ HIẾN MÁU

---

## PHẦN I. TỔNG QUAN

### 1. Thông tin chung
- **Repository và thư mục dự án đã kiểm tra**: `e:\InformationTechnology\visual tím\QuanLyHienMau_UNETI2_TI17A5HN` (Project chính: `QuanLyHienMau_UNETI2_TI17A5HN\QuanLyHienMau_UNETI2_TI17A5HN.csproj`).
- **Nhánh Git được kiểm tra**: `integration` (tracking `origin/integration`). Tại thời điểm kiểm tra, working tree đang có 15 file ở trạng thái `Modified (M)` chưa commit; toàn bộ quá trình kiểm tra giữ nguyên hiện trạng working tree, không can thiệp mã nguồn.
- **Thời điểm kiểm tra**: `2026-10-28 21:04:26+07:00` – `2026-10-28 21:12:00+07:00`.
- **Công nghệ sử dụng**:
  - **Framework**: ASP.NET Core MVC trên nền **.NET 10.0** (`<TargetFramework>net10.0</TargetFramework>`).
  - **ORM & CSDL**: Entity Framework Core **10.0.12** (`Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`), kết nối SQL Server LocalDB (`(localdb)\mssqllocaldb`, CSDL `QuanLyHienMau_UNETI`).
  - **Xác thực & Phân quyền**: Cookie Authentication (`CookieAuthenticationDefaults.AuthenticationScheme`, cookie `"QuanLyHienMau.Auth"`, thời hạn 7 ngày, `SlidingExpiration = true`) và Session (`IdleTimeout = 30 phút`).
  - **Giao diện**: Razor Views (`.cshtml`), Tailwind CSS, Material Symbols Outlined.

### 2. Các file chính của Module 3
- **Entities**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Models/NguoiHienMau.cs`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Models/PhieuDangKyHienMau.cs`
- **Controllers**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs` (Quản lý hồ sơ cá nhân phía người hiến máu)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (Nộp phiếu, Theo dõi phiếu, Hủy phiếu đăng ký hiến máu)
- **Views trực tiếp thuộc Module 3**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSoCaNhan.cshtml`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml`

### 3. Phạm vi đã kiểm tra
- Kiểm tra tĩnh (Static Code Analysis) toàn diện 100% mã nguồn thuộc Module 3 và các thành phần tích hợp liên quan (Module 1 – Tài khoản, Module 2 – Đợt hiến máu & Đơn vị tổ chức, Module 4 – Quản trị người hiến máu & Duyệt phiếu đăng ký trong `AdminController`, Module 5 – Lịch khám sàng lọc & Kết quả hiến máu, Module 6 – Thống kê).
- Kiểm tra cấu hình `ApplicationDbContext`, các file Migration (`InitialCreate`, `Thao`, `Thai`, `ApplicationDbContextModelSnapshot`), `DbInitializer`, `Program.cs`, và toàn bộ các Razor Views liên quan.
- Kiểm tra biên dịch thực tế bằng lệnh `dotnet build "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --no-restore`.

### 4. Phạm vi chưa thể kiểm tra (Giới hạn môi trường)
- **Kết nối trực tiếp CSDL runtime (`(localdb)\mssqllocaldb`)**: Khi thử kết nối read-only bằng `sqlcmd -S "(localdb)\mssqllocaldb" -d QuanLyHienMau_UNETI`, môi trường sandbox cô lập tiến trình trả về lỗi `Login timeout expired / Server is not found or not accessible` (Exit code `1`). Do đó không truy vấn trực tiếp dữ liệu vật lý đang nằm trong file `.mdf` của LocalDB, mà xác minh cấu trúc bảng/ràng buộc thông qua `ApplicationDbContext.cs`, `Migrations/*` và `DbInitializer.cs`.
- **Kiểm thử tự động (Unit Test / Integration Test)**: Giải pháp (`QuanLyHienMau_UNETI2_TI17A5HN.slnx`) hiện chỉ có duy nhất 1 project web, chưa có project kiểm thử tự động (xUnit/NUnit/MSTest).
- **Kiểm thử thực thi HTTP End-to-End (Runtime UI/HTTP)**: Do không khởi chạy kết nối LocalDB trong sandbox và tuân thủ nguyên tắc không làm thay đổi dữ liệu thật trong DB, các tình huống kiểm thử `TC01`–`TC26` được đánh giá dựa trên đối chiếu mã nguồn thực tế và phân loại minh bạch giữa kiểm tra tĩnh mã nguồn với kiểm thử thực thi runtime (`BLOCKED` ở bước thực thi runtime DB).

### 5. Kết quả build và kiểm thử
- **Kết quả `dotnet build`**: **THÀNH CÔNG (`Build succeeded` - Exit code `0`, `0 Error(s)`, `27 Warning(s)`)**.
- **Phân tích cảnh báo build**: Toàn bộ 27 cảnh báo đều là `warning CS8602: Dereference of a possibly null reference` nằm ở các file thuộc Module 5 và Module 6 (`Services/ThongKeService.cs`, `Controllers/KetQuaHienMauController.cs`, `Views/ThongKe/Dashboard.cshtml`, `Views/KetQuaHienMau/Create.cshtml`, `Views/LichKhamSangLoc/Details.cshtml`, `Views/LichKhamSangLoc/Index.cshtml`). **Không có bất kỳ lỗi hay cảnh báo biên dịch nào trong các file trực tiếp thuộc Module 3.**

---

## PHẦN II. DANH SÁCH FILE MODULE 3

Bảng dưới đây liệt kê toàn bộ các file đã kiểm tra, phân biệt rõ **File trực tiếp thuộc Module 3** và **File dùng chung / thuộc module khác có liên quan**:

| STT | Đường dẫn file | Phân loại | Vai trò | Chức năng phụ trách | Tình trạng |
|---|---|---|---|---|---|
| 1 | `QuanLyHienMau_UNETI2_TI17A5HN/Models/NguoiHienMau.cs` | **Trực tiếp Module 3** | Entity / Model | Định nghĩa thực thể `NguoiHienMau` (13 thuộc tính + 2 navigation properties `TaiKhoan`, `PhieuDangKyHienMaus`) và các DataAnnotations validation. | Đầy đủ thuộc tính; có ràng buộc DataAnnotations. |
| 2 | `QuanLyHienMau_UNETI2_TI17A5HN/Models/PhieuDangKyHienMau.cs` | **Trực tiếp Module 3** | Entity / Model | Định nghĩa thực thể `PhieuDangKyHienMau` (9 thuộc tính + navigation tới `NguoiHienMau`, `DotHienMau`, `LichKhamSangLocs`, `KetQuaHienMau` và các alias `[NotMapped]`). | Đầy đủ thuộc tính và khóa ngoại. |
| 3 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs` | **Trực tiếp Module 3** | Controller | Quản lý hồ sơ người hiến máu phía cổng thông tin người hiến (`Index`, `HoSoCaNhan [GET]`, `HoSoCaNhan [POST]`). | Hoạt động nhưng có lỗi lặp thông báo validation, thiếu kiểm tra trùng Email khi đồng bộ sang `TaiKhoan` và chưa kiểm tra `TrangThai == "Hoạt động"` khi cập nhật. |
| 4 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` | **Trực tiếp Module 3** | Controller | Xử lý nộp phiếu (`Create [GET/POST]`), theo dõi phiếu (`TheoDoi [GET]`, `Index [GET]`) và hủy phiếu (`Huy [POST]`). | Đã triển khai đầy đủ luồng chính nhưng còn lỗ hổng validation khi cập nhật nhanh hồ sơ ở `POST Create`, cho phép hủy phiếu khi đã có lịch khám sàng lọc, và nguy cơ race condition. |
| 5 | `QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSoCaNhan.cshtml` | **Trực tiếp Module 3** | Razor View | Giao diện xem/cập nhật hồ sơ cá nhân, lịch sử hiến máu, chứng nhận điện tử, và trạng thái phục hồi 84 ngày. | Giao diện chi tiết nhưng hardcode nhiều số liệu giả định (tính cả phiếu chưa hoàn thành vào tổng lần hiến, hardcode 84 ngày "Sẵn sàng", hardcode kết quả xét nghiệm âm tính). |
| 6 | `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml` | **Trực tiếp Module 3** | Razor View | Giao diện biểu mẫu nộp phiếu đăng ký hiến máu theo đợt, tích hợp cập nhật nhanh thông tin cá nhân và câu hỏi tự sàng lọc. | Giao diện đầy đủ nhưng gán giá trị mặc định giả (`"0988 123 456"`, `64 kg`, `"sinhvien.uneti@gmail.com"`) khi hồ sơ trống, và thiếu thuộc tính `name` ở các ô Ngày sinh, Giới tính, Nghề nghiệp. |
| 7 | `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml` | **Trực tiếp Module 3** | Razor View | Giao diện theo dõi tiến trình phiếu đăng ký (5 bước), xem lịch khám sàng lọc, kết quả hiến máu và modal xác nhận hủy phiếu. | Hiển thị tốt cho luồng thành công/đã hủy, nhưng không hiển thị lý do từ chối (`GhiChuXacNhan`) khi phiếu ở trạng thái `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`. |
| 8 | `QuanLyHienMau_UNETI2_TI17A5HN/Models/Module5TrangThai.cs` | Dùng chung (Module 3, 4, 5) | Constants / Helper | Định nghĩa hằng số 8 trạng thái của `PhieuDangKyHienMau`, trạng thái `LichKhamSangLoc` và `KetQuaHienMau`. | Nhất quán với các trạng thái nghiệp vụ yêu cầu. |
| 9 | `QuanLyHienMau_UNETI2_TI17A5HN/Data/ApplicationDbContext.cs` | Dùng chung toàn hệ thống | DbContext | Khai báo `DbSet<NguoiHienMau>`, `DbSet<PhieuDangKyHienMau>` và cấu hình Fluent API cho các quan hệ khóa ngoại. | Cấu hình đúng quan hệ 1–0..1 (`TaiKhoan`–`NguoiHienMau`) và 1–n (`PhieuDangKyHienMau`), nhưng thiếu Unique Filtered Index chống nộp phiếu trùng đồng thời. |
| 10 | `QuanLyHienMau_UNETI2_TI17A5HN/Data/DbInitializer.cs` | Dùng chung toàn hệ thống | Data Seeder | Tự động chạy `Migrate()` và khởi tạo dữ liệu mẫu (`TaiKhoan`, `NguoiHienMau`, `DotHienMau`, `PhieuDangKyHienMau`, `LichKhamSangLoc`, `KetQuaHienMau`). | Hoạt động tốt khi khởi chạy ứng dụng. |
| 11 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AccountController.cs` | Liên quan (Module 1) | Controller | Đăng nhập, đăng ký tài khoản (tự động tạo hồ sơ `NguoiHienMau` mới khi đăng ký tài khoản), đăng xuất. | Tích hợp trực tiếp với việc khởi tạo `NguoiHienMau`. |
| 12 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DotHienMauController.cs` & `Views/DotHienMau/*.cshtml` | Liên quan (Module 2) | Controller / View | Hiển thị danh sách và chi tiết đợt hiến máu, kiểm tra trạng thái đăng ký hiện tại của người dùng (`ExistingPhieu`) và điều hướng sang `/DangKy/Create` hoặc `/DangKy/TheoDoi`. | Tích hợp tốt với Module 3. |
| 13 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs` & `Views/Admin/*.cshtml` | Liên quan (Module 4 & 5) | Controller / View | Quản lý danh sách hồ sơ người hiến máu (`NguoiHienMau`), quản lý danh sách/chi tiết phiếu đăng ký (`PhieuDangKyHienMau`, `ChiTietPhieuDangKy`), xác nhận/từ chối phiếu (`XacNhanPhieu`, `TuChoiPhieu`, `ChuyenChoKham`). | Tích hợp chặt chẽ nhưng có link hỏng (`asp-action="PhieuDangKy"`) trong `Views/Admin/NguoiHienMau.cshtml` và chưa có chức năng khóa/mở khóa `NguoiHienMau.TrangThai`. |
| 14 | `QuanLyHienMau_UNETI2_TI17A5HN/Services/Module5Service.cs`, `Controllers/LichKhamSangLocController.cs`, `Controllers/KetQuaHienMauController.cs` | Liên quan (Module 5) | Service / Controller | Xếp lịch khám sàng lọc, hoàn thành khám, ghi nhận kết quả hiến máu và cập nhật trạng thái `PhieuDangKyHienMau`. | Đồng bộ trạng thái phiếu nhưng `Module5Service` không cập nhật `PhieuDangKyHienMau.NgayXuLy` khi chuyển trạng thái. |
| 15 | `QuanLyHienMau_UNETI2_TI17A5HN/Migrations/*.cs` | Dùng chung toàn hệ thống | EF Core Migrations | Định nghĩa schema CSDL (`20261007035106_InitialCreate.cs`, `20261016064804_Thao.cs`, `20261021150813_Thai.cs`). | Khớp với Model hiện tại. |

---

## PHẦN III. BẢNG ĐỐI CHIẾU YÊU CẦU

Bảng dưới đây đối chiếu toàn bộ **46 yêu cầu nghiệp vụ và kỹ thuật** trong đề bài (từ mục 4.1 đến 4.7) với mã nguồn thực tế:

| STT | Yêu cầu nghiệp vụ | Vị trí triển khai | Bằng chứng | Kết quả | Mức độ ưu tiên |
|---|---|---|---|---|---|
| **4.1** | **Quản lý người hiến máu (`NguoiHienMau`)** | | | | |
| 1 | Entity `NguoiHienMau` có đầy đủ 13 thuộc tính tối thiểu (`MaNguoiHien`, `MaTaiKhoan`, `HoTen`, `NgaySinh`, `GioiTinh`, `SoDienThoai`, `Email`, `DiaChi`, `NhomMau`, `NgheNghiep`, `CanNang`, `GhiChuSucKhoe`, `TrangThai`) | `Models/NguoiHienMau.cs` (dòng 9–55) | Khai báo đủ 13 thuộc tính đúng tên yêu cầu. | `HOÀN THÀNH` | HIGH |
| 2 | Kiểu dữ liệu của từng thuộc tính trong `NguoiHienMau` hợp lý | `Models/NguoiHienMau.cs` (dòng 10–55) | `MaNguoiHien` (`int`), `MaTaiKhoan` (`int?`), `HoTen` (`string`), `NgaySinh` (`DateTime?`), `CanNang` (`double?`), các trường chuỗi đều có `[StringLength]` phù hợp (10 đến 500 ký tự). | `HOÀN THÀNH` | MEDIUM |
| 3 | Thiết lập khóa chính, khóa ngoại và quan hệ 1–0..1 giữa `NguoiHienMau` và `TaiKhoan` | `Models/NguoiHienMau.cs` (dòng 10, 58–59), `Data/ApplicationDbContext.cs` (dòng 41–47), `Migrations/20261007035106_InitialCreate.cs` (dòng 203–208) | `[Key] MaNguoiHien`, `[ForeignKey("MaTaiKhoan")]`, `HasOne(n => n.TaiKhoan).WithOne(t => t.NguoiHienMau).HasForeignKey<NguoiHienMau>(n => n.MaTaiKhoan).OnDelete(DeleteBehavior.SetNull)`, có Unique Index `IX_NguoiHienMau_MaTaiKhoan` (`[MaTaiKhoan] IS NOT NULL`). | `HOÀN THÀNH` | HIGH |
| 4 | Có cơ chế tạo hồ sơ người hiến máu khi cần thiết | `Controllers/AccountController.cs` (dòng 248–260), `Controllers/NguoiHienMauController.cs` (dòng 54–72), `Controllers/DangKyController.cs` (dòng 37–54) | Tạo khi `Register`; tự động khởi tạo (auto-provision) khi truy cập `GET HoSoCaNhan` hoặc `GET DangKy/Create`. Tuy nhiên nếu gọi trực tiếp `POST HoSoCaNhan` hoặc `POST DangKy/Create` khi chưa có hồ sơ thì bị trả về `NotFound` (404); đồng thời chưa chặn tài khoản `Admin`/`Staff` tự động sinh hồ sơ `NguoiHienMau`. | `HOÀN THÀNH MỘT PHẦN` | MEDIUM |
| 5 | Người hiến máu có thể xem hồ sơ cá nhân của mình | `Controllers/NguoiHienMauController.cs` (`HoSoCaNhan [GET]`, dòng 41–80), `Views/NguoiHienMau/HoSoCaNhan.cshtml` | Truy vấn theo `maTaiKhoan` từ Claims, kèm lịch sử `PhieuDangKyHienMaus`, `DotHienMau`, `LichKhamSangLocs`, `KetQuaHienMau`. Tuy nhiên View hiển thị một số dữ liệu giả định/hardcode (tính cả phiếu chưa hoàn thành vào số lần hiến, hardcode 84 ngày phục hồi và tab xét nghiệm). | `HOÀN THÀNH MỘT PHẦN` | MEDIUM |
| 6 | Người hiến máu có thể cập nhật hồ sơ cá nhân của mình | `Controllers/NguoiHienMauController.cs` (`HoSoCaNhan [POST]`, dòng 84–216) | Cập nhật các trường cá nhân và lưu xuống DB qua `_context.SaveChangesAsync()`. | `HOÀN THÀNH` | HIGH |
| 7 | Chống sửa hồ sơ của người khác bằng cách đổi ID trên URL hoặc gửi request thủ công (Kiểm tra quyền sở hữu ở server) | `Controllers/NguoiHienMauController.cs` (dòng 86–95, 191–201) | Lấy `maTaiKhoan` từ `ClaimTypes.NameIdentifier`, truy vấn `_context.NguoiHienMaus.FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan)` và chỉ gán từng thuộc tính cho phép, bỏ qua `model.MaNguoiHien` và `model.MaTaiKhoan` từ client. | `HOÀN THÀNH` | HIGH |
| 8 | Kiểm tra trạng thái hoạt động của người hiến máu (`TrangThai == "Hoạt động"`) | `Controllers/DangKyController.cs` (dòng 57, 159), `Controllers/NguoiHienMauController.cs` (dòng 84–216), `Controllers/AdminController.cs` (dòng 367–385) | Có kiểm tra `nguoiHienMau.TrangThai != "Hoạt động"` khi nộp phiếu (`DangKyController.Create`), nhưng chưa kiểm tra trong `NguoiHienMauController.HoSoCaNhan [POST]` và phía Admin chưa có Action nào để khóa/tạm hoãn trạng thái `NguoiHienMau`. | `HOÀN THÀNH MỘT PHẦN` | MEDIUM |
| 9 | Validation hồ sơ ở phía server (Họ tên bắt buộc, Ngày sinh/Độ tuổi 18–60, Email đúng định dạng, SĐT hợp lệ `^0[0-9]{9,10}$`, Cân nặng 30–200 kg) | `Models/NguoiHienMau.cs` (dòng 15–52), `Controllers/NguoiHienMauController.cs` (dòng 97–159), `Controllers/DangKyController.cs` (dòng 196–266) | Trong `HoSoCaNhan [POST]` có kiểm tra đầy đủ ở server (nhưng bị lặp thông báo lỗi do trùng giữa DataAnnotations và `ModelState.AddModelError`); trong khi đó ở `DangKyController.Create [POST]` khi cập nhật nhanh hồ sơ lại **không** kiểm tra định dạng SĐT, định dạng Email, giới hạn trên của Cân nặng và độ dài chuỗi. | `CÓ LỖI` | HIGH |
| **4.2** | **Entity `PhieuDangKyHienMau` và Trạng thái nghiệp vụ** | | | | |
| 10 | Entity `PhieuDangKyHienMau` có đầy đủ 9 thuộc tính (`MaPhieu`, `MaNguoiHien`, `MaDot`, `NgayNop`, `LyDoHienMau`, `GhiChu`, `TrangThai`, `NgayXuLy`, `GhiChuXacNhan`) | `Models/PhieuDangKyHienMau.cs` (dòng 9–42) | Khai báo đầy đủ 9 thuộc tính đúng tên và kiểu dữ liệu hợp lý. | `HOÀN THÀNH` | HIGH |
| 11 | Thiết lập khóa chính, khóa ngoại và quan hệ với `NguoiHienMau`, `DotHienMau`, `LichKhamSangLoc`, `KetQuaHienMau` | `Models/PhieuDangKyHienMau.cs` (dòng 45–50), `Data/ApplicationDbContext.cs` (dòng 55–82) | Khóa ngoại `MaNguoiHien` và `MaDot` cấu hình `DeleteBehavior.Restrict`; quan hệ 1–n với `LichKhamSangLoc` và 1–0..1 với `KetQuaHienMau` cấu hình `DeleteBehavior.Cascade`. | `HOÀN THÀNH` | HIGH |
| 12 | Định nghĩa và sử dụng nhất quán 8 trạng thái nghiệp vụ của phiếu (`Chờ xác nhận`, `Đã xác nhận`, `Từ chối`, `Chờ khám sàng lọc`, `Đã khám sàng lọc`, `Hiến máu thành công`, `Không đủ điều kiện hiến`, `Đã hủy`) | `Models/Module5TrangThai.cs` (dòng 5–22), `Controllers/DangKyController.cs`, `Controllers/AdminController.cs`, `Services/Module5Service.cs` | Cả 8 trạng thái đều được định nghĩa và có luồng thiết lập ở backend. Tuy nhiên `Views/DangKy/TheoDoi.cshtml` chưa xử lý hiển thị riêng cho trạng thái `"Từ chối"` và `"Không đủ điều kiện hiến"` (gán `stepNumber = -1` làm xám toàn bộ 5 bước). | `HOÀN THÀNH MỘT PHẦN` | MEDIUM |
| 13 | Cập nhật nhất quán `NgayXuLy` và `GhiChuXacNhan` khi chuyển trạng thái phiếu | `Controllers/DangKyController.cs` (dòng 442–443), `Controllers/AdminController.cs` (dòng 443, 474, 505, 578, 639, 739, 801), `Services/Module5Service.cs` (dòng 60, 89, 108, 161–164) | `DangKyController` và `AdminController` có cập nhật `NgayXuLy = DateTime.Now`, nhưng khi chuyển trạng thái qua `Module5Service` (`LuuLichAsync`, `HuyLichAsync`, `HoanThanhLichAsync`, `GhiKetQuaAsync`) thì **không** cập nhật `phieu.NgayXuLy`. | `HOÀN THÀNH MỘT PHẦN` | LOW |
| **4.3** | **Chức năng nộp phiếu đăng ký hiến máu (`DangKyController.Create`)** | | | | |
| 14 | Bắt buộc người dùng đăng nhập trước khi nộp phiếu | `Controllers/DangKyController.cs` (dòng 12) | Gắn `[Authorize]` ở cấp độ class `DangKyController`. | `HOÀN THÀNH` | HIGH |
| 15 | Xác định đúng hồ sơ `NguoiHienMau` thuộc tài khoản hiện tại từ Claims | `Controllers/DangKyController.cs` (dòng 26–35, 144–156) | Lấy `maTaiKhoan` từ `ClaimTypes.NameIdentifier` và truy vấn `FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan)`. | `HOÀN THÀNH` | HIGH |
| 16 | Kiểm tra người hiến máu phải đang ở trạng thái `"Hoạt động"` (cả GET và POST) | `Controllers/DangKyController.cs` (dòng 57–62, 159–164) | Kiểm tra `if (nguoiHienMau.TrangThai != "Hoạt động")` ở cả GET và POST. | `HOÀN THÀNH` | HIGH |
| 17 | Kiểm tra đợt hiến máu phải tồn tại (cả GET và POST) | `Controllers/DangKyController.cs` (dòng 77–84, 167–174) | Truy vấn `_context.DotHienMaus.FirstOrDefaultAsync(d => d.MaDot == maDot)` và từ chối nếu `dotHienMau == null`. | `HOÀN THÀNH` | HIGH |
| 18 | Kiểm tra đợt hiến máu phải ở trạng thái `"Đang mở đăng ký"` (cả GET và POST) | `Controllers/DangKyController.cs` (dòng 87–91, 176–181) | Kiểm tra `if (dotHienMau.TrangThai != "Đang mở đăng ký")`. | `HOÀN THÀNH` | HIGH |
| 19 | Kiểm tra ngày hiện tại nằm trong khoảng thời gian nhận đăng ký (`NgayBatDauDangKy.Date <= now.Date <= HanDangKy.Date`) | `Controllers/DangKyController.cs` (dòng 94–104, 184–193) | Kiểm tra ở cả GET và POST. | `HOÀN THÀNH` | HIGH |
| 20 | Kiểm tra người hiến máu không có hai phiếu đang xử lý cho cùng một đợt hiến máu | `Controllers/DangKyController.cs` (dòng 107–117, 246–256) | Truy vấn LINQ kiểm tra `p.MaNguoiHien == nguoiHienMau.MaNguoiHien && p.MaDot == maDot && p.TrangThai != "Đã hủy" && p.TrangThai != "Từ chối"` ở cả GET và POST. | `HOÀN THÀNH` | HIGH |
| 21 | Kiểm tra hồ sơ cá nhân có đầy đủ thông tin bắt buộc trước khi nộp phiếu | `Controllers/DangKyController.cs` (dòng 196–222), `Views/DangKy/Create.cshtml` (dòng 217, 224, 286) | Server chỉ kiểm tra `HoTen`, `SoDienThoai`, `CanNang` khác rỗng, chưa kiểm tra `NgaySinh`/`GioiTinh`/`DiaChi`. Đặc biệt View `Create.cshtml` tự điền sẵn giá trị giả (`"0988 123 456"`, `64`, `"sinhvien.uneti@gmail.com"`) khi hồ sơ trống, làm vô hiệu hóa việc bắt buộc người dùng nhập SĐT/Cân nặng thật trên form. | `CÓ LỖI` | HIGH |
| 22 | Kiểm tra cân nặng đáp ứng điều kiện tối thiểu của đợt hiến máu (`CanNang >= dotHienMau.CanNangToiThieu`) | `Controllers/DangKyController.cs` (dòng 224–232) | Kiểm tra `if (finalWeight.Value < dotHienMau.CanNangToiThieu)` ở server khi POST. | `HOÀN THÀNH` | HIGH |
| 23 | `NgayNop` do hệ thống xác định (`DateTime.Now`) và trạng thái phiếu mới là `"Chờ xác nhận"` | `Controllers/DangKyController.cs` (dòng 269–279) | Khởi tạo `new PhieuDangKyHienMau { NgayNop = DateTime.Now, TrangThai = "Chờ xác nhận", NgayXuLy = null, GhiChuXacNhan = null }`. | `HOÀN THÀNH` | HIGH |
| 24 | Lưu dữ liệu vào database và thông báo kết quả / lỗi phù hợp | `Controllers/DangKyController.cs` (dòng 281–287) | Gọi `_context.PhieuDangKyHienMaus.Add(phieu); await _context.SaveChangesAsync();`, gán `TempData["SuccessMessage"]` và chuyển hướng tới `TheoDoi`. | `HOÀN THÀNH` | HIGH |
| 25 | Chống tạo phiếu trùng khi có 2 request đồng thời (Race Condition / Concurrency) | `Controllers/DangKyController.cs` (dòng 246–282), `Data/ApplicationDbContext.cs` | Chưa có Transaction (`Serializable`) hoặc Unique Filtered Index trên `(MaNguoiHien, MaDot)` cho các phiếu đang hiệu lực; 2 request đồng thời có thể cùng vượt qua bước `FirstOrDefaultAsync` và tạo 2 phiếu trùng. | `CHƯA HOÀN THÀNH` | HIGH |
| **4.4** | **Chức năng hủy phiếu (`DangKyController.Huy`)** | | | | |
| 26 | Người dùng xem được thao tác hủy đối với phiếu hợp lệ của mình trên giao diện | `Views/DangKy/TheoDoi.cshtml` (dòng 35, 487–495, 567–609) | Nút `"Hủy phiếu đăng ký"` và Modal xác nhận hủy chỉ hiển thị khi `canCancel == true` (`"Chờ xác nhận"` hoặc `"Đã xác nhận"`). | `HOÀN THÀNH` | MEDIUM |
| 27 | Xác định chủ sở hữu phiếu từ tài khoản đăng nhập và chặn hủy phiếu của người khác | `Controllers/DangKyController.cs` (dòng 389–415) | Lấy `maTaiKhoan` từ Claims, kiểm tra `if (!isStaffOrAdmin && (nguoiHienMau == null || phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien)) return Forbid();`. | `HOÀN THÀNH` | HIGH |
| 28 | Kiểm tra trạng thái trước khi hủy: cho phép hủy `"Chờ xác nhận"` và `"Đã xác nhận"` nhưng **chưa có lịch khám sàng lọc** | `Controllers/DangKyController.cs` (dòng 418–450) | Chỉ kiểm tra `phieu.TrangThai` thuộc `{"Chờ xác nhận", "Đã xác nhận"}`, **không** chặn khi phiếu đã có lịch khám sàng lọc đang hoạt động mà lại tự động hủy luôn các lịch khám sàng lọc liên quan (dòng 446–449). | `CÓ LỖI` | HIGH |
| 29 | Chặn hủy phiếu ở các trạng thái `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`, `"Hiến máu thành công"`, `"Không đủ điều kiện hiến"`, `"Từ chối"`, `"Đã hủy"` | `Controllers/DangKyController.cs` (dòng 418–438) | Chặn đầy đủ và trả về thông báo lỗi chi tiết qua `TempData["ErrorMessage"]`. | `HOÀN THÀNH` | HIGH |
| 30 | Lưu trạng thái `"Đã hủy"` vào database và hiển thị thông báo thành công/thất bại | `Controllers/DangKyController.cs` (dòng 441–454) | Cập nhật `phieu.TrangThai = "Đã hủy"`, `phieu.NgayXuLy = DateTime.Now`, lưu DB và gán `TempData["SuccessMessage"]`. | `HOÀN THÀNH` | HIGH |
| 31 | Chống giả mạo request POST khi hủy phiếu (`[HttpPost]` + `[ValidateAntiForgeryToken]`) | `Controllers/DangKyController.cs` (dòng 386–387), `Views/DangKy/TheoDoi.cshtml` (dòng 582) | Action có `[HttpPost]` và `[ValidateAntiForgeryToken]`, form sử dụng Tag Helper tự sinh CSRF token. | `HOÀN THÀNH` | HIGH |
| **4.5** | **Chức năng theo dõi phiếu (`DangKyController.TheoDoi`)** | | | | |
| 32 | Hiển thị danh sách phiếu đã nộp của người hiến máu | `Controllers/DangKyController.cs` (dòng 311–329), `Views/DangKy/TheoDoi.cshtml` (dòng 60–145) | Truyền danh sách qua `ViewBag.DanhSachPhieu` (sắp xếp `NgayNop` giảm dần) và hiển thị ở cột trái. | `HOÀN THÀNH` | HIGH |
| 33 | Hiển thị mã phiếu, đợt hiến máu, đơn vị tổ chức, ngày nộp, trạng thái hiện tại, lịch khám sàng lọc và kết quả hiến máu | `Controllers/DangKyController.cs` (dòng 312–320), `Views/DangKy/TheoDoi.cshtml` (dòng 150–475) | Eager loading đầy đủ `DotHienMau.DonViToChuc`, `NguoiHienMau`, `LichKhamSangLocs`, `KetQuaHienMau` và hiển thị trên giao diện. | `HOÀN THÀNH` | HIGH |
| 34 | Hiển thị ghi chú hoặc thông tin xử lý (`GhiChuXacNhan`, `NgayXuLy`) khi phiếu bị từ chối hoặc xử lý | `Views/DangKy/TheoDoi.cshtml` (dòng 16–30, 224–297) | `GhiChuXacNhan` chỉ được hiển thị khi `trangThai == "Đã hủy"` (dòng 230). Khi phiếu bị `"Từ chối"`, người hiến máu **không nhìn thấy lý do từ chối (`GhiChuXacNhan`)** và thanh tiến trình 5 bước bị xám toàn bộ (`stepNumber = -1`). | `HOÀN THÀNH MỘT PHẦN` | MEDIUM |
| 35 | Bảo vệ quyền riêng tư: lọc danh sách phiếu theo người dùng đăng nhập và chặn xem phiếu người khác khi đổi `id` hoặc `code` trên URL | `Controllers/DangKyController.cs` (dòng 321–374) | Dữ liệu được lọc trong `userPhieuList` của chính người dùng nên **không lộ dữ liệu của người khác**. Tuy nhiên khi tìm theo `?code=DKHM-xxxxx` không thuộc quyền sở hữu, hệ thống không báo lỗi mà âm thầm hiển thị phiếu đầu tiên của người dùng; khi truyền `?id=999` của người khác thì vừa báo lỗi vừa hiển thị phiếu đầu tiên của mình bên dưới. | `HOÀN THÀNH MỘT PHẦN` | MEDIUM |
| **4.6** | **Kiểm tra đăng nhập và phân quyền** | | | | |
| 36 | Các Controller/Action thuộc Module 3 yêu cầu xác thực (`[Authorize]`) | `Controllers/NguoiHienMauController.cs` (dòng 11), `Controllers/DangKyController.cs` (dòng 12) | Cả 2 Controller đều gắn `[Authorize]` ở mức class. | `HOÀN THÀNH` | HIGH |
| 37 | Kiểm tra đúng vai trò người hiến máu và kiểm tra trạng thái tài khoản (`TaiKhoan.TrangThai == "Hoạt động"`) trên mỗi request | `Controllers/NguoiHienMauController.cs`, `Controllers/DangKyController.cs` | Chỉ dùng `[Authorize]` chung mà không giới hạn vai trò cho `HoSoCaNhan` / `DangKy/Create` (tài khoản `Admin`/`Medical Staff` vào sẽ bị tự động tạo hồ sơ `NguoiHienMau`); đồng thời không kiểm tra lại `TaiKhoan.TrangThai == "Hoạt động"` trong DB nếu tài khoản bị khóa khi cookie 7 ngày còn hạn. | `HOÀN THÀNH MỘT PHẦN` | MEDIUM |
| 38 | Không tin tưởng `MaNguoiHien`, `MaTaiKhoan` do client gửi lên; chống IDOR trên các thao tác đọc/ghi | `Controllers/NguoiHienMauController.cs`, `Controllers/DangKyController.cs` | Luôn lấy `maTaiKhoan` từ `User.FindFirst(ClaimTypes.NameIdentifier)` và truy vấn `NguoiHienMau` tương ứng từ DB. | `HOÀN THÀNH` | HIGH |
| 39 | Bảo vệ toàn bộ các Action POST bằng `[ValidateAntiForgeryToken]` chống CSRF | `Controllers/NguoiHienMauController.cs` (dòng 85), `Controllers/DangKyController.cs` (dòng 133, 387) | Cả 3 Action POST (`HoSoCaNhan`, `Create`, `Huy`) đều có `[HttpPost]` và `[ValidateAntiForgeryToken]`. | `HOÀN THÀNH` | HIGH |
| **4.7** | **Kiểm tra database và tích hợp** | | | | |
| 40 | Khai báo `DbSet`, cấu hình khóa ngoại, ràng buộc dữ liệu và Migration cho `NguoiHienMau`, `PhieuDangKyHienMau` | `Data/ApplicationDbContext.cs` (dòng 16–84), `Migrations/20261007035106_InitialCreate.cs` | Đầy đủ `DbSet`, cấu hình Fluent API và 3 bản Migration hợp lệ. | `HOÀN THÀNH` | HIGH |
| 41 | Tích hợp với Module 1 (`TaiKhoan` – Đăng ký/Đăng nhập/Đồng bộ thông tin) | `Controllers/AccountController.cs` (dòng 248–260), `Controllers/NguoiHienMauController.cs` (dòng 203–211) | Tạo `NguoiHienMau` khi đăng ký tài khoản. Tuy nhiên khi cập nhật Email ở `HoSoCaNhan [POST]` (dòng 203–211), hệ thống ghi đè `TaiKhoan.Email` mà **không kiểm tra trùng Email với tài khoản khác** và không làm mới Claims (`ClaimTypes.Name`) trong Cookie. | `CÓ LỖI` | MEDIUM |
| 42 | Tích hợp với Module 2 (`DotHienMau` & `DonViToChuc`) | `Controllers/DotHienMauController.cs` (dòng 87–111, 146–164), `Views/DotHienMau/Index.cshtml`, `Views/DotHienMau/Details.cshtml` | Hiển thị trạng thái đã đăng ký trên từng đợt hiến máu và truyền `maDot` sang `/DangKy/Create` hoặc `/DangKy/TheoDoi`. | `HOÀN THÀNH` | HIGH |
| 43 | Tích hợp với Module 4 (`AdminController` – Quản lý người hiến máu & Duyệt phiếu đăng ký) | `Controllers/AdminController.cs` (dòng 367–560), `Views/Admin/NguoiHienMau.cshtml` (dòng 62, 547) | Quản lý danh sách/chi tiết phiếu hoạt động tốt, nhưng trong `Views/Admin/NguoiHienMau.cshtml` nút `"Xem các phiếu đăng ký của người này"` trỏ sai tới `asp-action="PhieuDangKy"` (gây lỗi 404 vì Action đúng là `PhieuDangKyHienMau`) và nút `"Thêm Hồ Sơ Mới"` trỏ sang `/DangKy/Create`. | `CÓ LỖI` | MEDIUM |
| 44 | Tích hợp với Module 5 (`LichKhamSangLoc` & `KetQuaHienMau`) | `Controllers/DangKyController.cs` (dòng 316–317), `Services/Module5Service.cs`, `Controllers/AdminController.cs` | Đồng bộ hiển thị lịch khám và kết quả hiến máu trên trang `TheoDoi`. | `HOÀN THÀNH` | HIGH |
| 45 | Kiểm tra quy tắc khoảng cách tối thiểu giữa 2 lần hiến máu (84 ngày) khi nộp phiếu và trên hồ sơ cá nhân | `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 151–167), `Controllers/DangKyController.cs` | Trong `Admin/NguoiHienMau.cshtml` có tính toán 84 ngày từ lần hiến thành công gần nhất, nhưng `HoSoCaNhan.cshtml` lại hardcode `"Sẵn sàng"` 100%, và `DangKyController.Create` chưa kiểm tra khoảng cách ngày tối thiểu giữa các lần hiến máu. | `HOÀN THÀNH MỘT PHẦN` | MEDIUM |
| 46 | Xác minh dữ liệu vật lý trực tiếp trong SQL Server LocalDB runtime | `appsettings.json`, `(localdb)\mssqllocaldb` | Đã xác minh đầy đủ qua `Migrations` và `DbInitializer.cs`; chưa truy vấn trực tiếp tiến trình LocalDB runtime do giới hạn cô lập mạng/named-pipe của môi trường sandbox. | `CHƯA XÁC MINH` | LOW |

---

## PHẦN IV. ĐÁNH GIÁ CHI TIẾT

### 1. Quản lý hồ sơ người hiến máu

- **Yêu cầu**:
  - Đảm bảo entity `NguoiHienMau` có đủ 13 thuộc tính tối thiểu với kiểu dữ liệu hợp lý, thiết lập khóa chính/khóa ngoại với `TaiKhoan`, có cơ chế khởi tạo hồ sơ, cho phép người hiến máu xem và cập nhật hồ sơ của chính mình, kiểm tra trạng thái hoạt động, chống sửa hồ sơ người khác (IDOR) và thực hiện validation ở phía server.
- **Thực trạng trong source code**:
  1. **Kiểm tra 13 thuộc tính và kiểu dữ liệu**: Trong `Models/NguoiHienMau.cs` (dòng 9–55), entity `NguoiHienMau` có đầy đủ 13 thuộc tính:
     - `MaNguoiHien` (`int`, `[Key]`)
     - `MaTaiKhoan` (`int?`, `[ForeignKey("MaTaiKhoan")]`)
     - `HoTen` (`string`, `[Required]`, `[StringLength(100)]`)
     - `NgaySinh` (`DateTime?`)
     - `GioiTinh` (`string?`, `[StringLength(10)]`)
     - `SoDienThoai` (`string?`, `[StringLength(20)]`, `[RegularExpression(@"^0[0-9]{9,10}$")]`)
     - `Email` (`string?`, `[StringLength(100)]`, `[EmailAddress]`)
     - `DiaChi` (`string?`, `[StringLength(255)]`)
     - `NhomMau` (`string?`, `[StringLength(20)]`)
     - `NgheNghiep` (`string?`, `[StringLength(100)]`)
     - `CanNang` (`double?`, `[Range(30.0, 200.0)]`)
     - `GhiChuSucKhoe` (`string?`, `[StringLength(500)]`)
     - `TrangThai` (`string?`, `[StringLength(50)]`, mặc định `"Hoạt động"`).
  2. **Quan hệ với `TaiKhoan` (1 tài khoản có liên kết được nhiều hồ sơ không?)**:
     - Trong `Data/ApplicationDbContext.cs` (dòng 41–47):
       ```csharp
       modelBuilder.Entity<NguoiHienMau>()
           .HasOne(n => n.TaiKhoan)
           .WithOne(t => t.NguoiHienMau)
           .HasForeignKey<NguoiHienMau>(n => n.MaTaiKhoan)
           .OnDelete(DeleteBehavior.SetNull);
       ```
     - Trong `Migrations/20261007035106_InitialCreate.cs` (dòng 203–208), EF Core tạo Unique Filtered Index `IX_NguoiHienMau_MaTaiKhoan` với điều kiện `[MaTaiKhoan] IS NOT NULL`.
     - **Đánh giá nghiệp vụ**: Thiết kế này đảm bảo **một `TaiKhoan` chỉ có thể liên kết tối đa với một `NguoiHienMau` duy nhất** (quan hệ 1 – 0..1), đồng thời cho phép các hồ sơ người hiến máu vãng lai hoặc dữ liệu lịch sử có `MaTaiKhoan = null` (như trong `DbInitializer.SeedTestData`). Điều này hoàn toàn hợp lý về mặt nghiệp vụ.
  3. **Cơ chế tạo hồ sơ người hiến máu**:
     - Tạo tự động khi đăng ký tài khoản mới tại `AccountController.Register` (dòng 248–260).
     - Tự động tạo bổ sung (auto-provisioning) khi người dùng đã đăng nhập truy cập `GET /NguoiHienMau/HoSoCaNhan` (dòng 54–72) hoặc `GET /DangKy/Create` (dòng 37–54) nếu tài khoản chưa có bản ghi `NguoiHienMau`.
  4. **Xem và cập nhật hồ sơ cá nhân & Chống IDOR**:
     - Tại `NguoiHienMauController.HoSoCaNhan` (GET dòng 41–80, POST dòng 84–216), hệ thống lấy `maTaiKhoan` trực tiếp từ `User.FindFirst(ClaimTypes.NameIdentifier)?.Value` và truy vấn `_context.NguoiHienMaus.FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan)`.
     - Khi POST, dù nhận tham số `NguoiHienMau model`, controller chỉ gán 10 thuộc tính thông tin cá nhân vào thực thể `nguoiHienMau` tìm được từ `maTaiKhoan` (dòng 191–201), hoàn toàn bỏ qua `model.MaNguoiHien`, `model.MaTaiKhoan` và `model.TrangThai`. Người dùng không thể sửa hồ sơ của người khác bằng cách đổi ID trên URL hay gửi form giả mạo.
  5. **Quy tắc validation thực tế đang áp dụng**:
     - `HoTen`: Bắt buộc, tối đa 100 ký tự (`NguoiHienMauController.cs` dòng 98–105).
     - `NgaySinh`: Nếu có nhập thì không được lớn hơn `DateTime.Today`, và độ tuổi tính theo năm phải nằm trong khoảng **18 đến 60 tuổi** (dòng 108–123).
     - `CanNang`: Bắt buộc khi lưu hồ sơ cá nhân (`!model.CanNang.HasValue`), giá trị từ **30 đến 200 kg** (dòng 126–136).
     - `SoDienThoai`: Nếu có nhập thì phải khớp biểu thức chính quy `^0[0-9]{9,10}$` (bắt đầu bằng số 0, dài 10–11 chữ số, dòng 139–146).
     - `Email`: Nếu có nhập thì phải đúng định dạng `EmailAddressAttribute` và tối đa 100 ký tự (dòng 149–159).
- **Bằng chứng**:
  - `Models/NguoiHienMau.cs` (dòng 9–61), `Controllers/NguoiHienMauController.cs` (dòng 41–216), `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 1–789), `Views/Admin/NguoiHienMau.cshtml` (dòng 62, 547).
- **Phần đã hoàn thành**:
  - Entity, khóa chính/khóa ngoại, quan hệ 1–0..1 với `TaiKhoan`, xem/cập nhật hồ sơ cá nhân, bảo vệ quyền sở hữu hồ sơ ở server.
- **Phần còn thiếu**:
  - Chưa kiểm tra `nguoiHienMau.TrangThai == "Hoạt động"` trước khi cho phép cập nhật hồ sơ trong `NguoiHienMauController.HoSoCaNhan [POST]`.
  - Phía Admin (`AdminController`) chưa có chức năng cập nhật trạng thái `NguoiHienMau.TrangThai` (ví dụ chuyển sang `"Tạm khóa"` / `"Tạm hoãn hiến máu"`).
- **Lỗi hoặc rủi ro**:
  1. **Đồng bộ Email sang `TaiKhoan` không kiểm tra trùng lặp**: Tại `NguoiHienMauController.cs` (dòng 203–211), khi người dùng đổi Email trên hồ sơ cá nhân, code gán thẳng `nguoiHienMau.TaiKhoan.Email = nguoiHienMau.Email` mà không kiểm tra xem Email đó đã thuộc về một `TaiKhoan` khác hay chưa, dẫn đến nguy cơ hai tài khoản có cùng Email đăng nhập.
  2. **Lặp thông báo lỗi validation (Duplicate ModelState Errors)**: Vì `Models/NguoiHienMau.cs` đã gắn DataAnnotations (`[Required]`, `[Range]`, `[RegularExpression]`, `[EmailAddress]`), ASP.NET Core tự động thêm lỗi vào `ModelState` khi binding; sau đó `NguoiHienMauController.cs` (dòng 98–159) lại tiếp tục gọi `ModelState.AddModelError` cho cùng các trường `HoTen`, `CanNang`, `SoDienThoai`, `Email` khiến `ValidationSummary` hiển thị 2 dòng lỗi trùng nhau cho cùng 1 trường.
  3. **Số liệu thống kê và thông tin y tế trên `HoSoCaNhan.cshtml` bị hardcode/sai lệch**:
     - Dòng 16: `var totalCount = successfulDonations.Count > 0 ? successfulDonations.Count : phieuList.Count(p => p.TrangThai != "Đã hủy");` — nếu người dùng chưa có lần hiến máu thành công nào nhưng có 1 phiếu `"Chờ xác nhận"` hoặc `"Từ chối"`, giao diện vẫn tính là đã hiến máu 1 lần (`350 mL`, cứu `3` người, đạt `"Huy Hiệu Đồng"`).
     - Dòng 151–167: Khối `"Thời gian phục hồi (84 ngày)"` hiển thị tĩnh cố định `"Sẵn sàng - Đủ điều kiện hiến đợt tiếp theo"` (`width: 100%`) cho mọi người dùng thay vì tính từ ngày hiến máu thành công gần nhất.
     - Dòng 437–472 & 492–554: Tab `"Hồ sơ xét nghiệm"` (HIV, HBV, HCV, Giang mai, Sốt rét) hiển thị tĩnh `"Âm tính / Đạt"` và thẻ `"Chứng nhận hiến máu số"` luôn hiển thị với dữ liệu fallback `"NGUYỄN HOÀNG NAM"` ngay cả khi người dùng chưa từng hiến máu.
  4. **Link hỏng (404) trên trang Admin Quản lý người hiến máu**: Tại `Views/Admin/NguoiHienMau.cshtml` (dòng 547), nút `"Xem các phiếu đăng ký của người này"` gọi `asp-controller="Admin" asp-action="PhieuDangKy" asp-route-search="@donor.HoTen"`, nhưng trong `AdminController.cs` không có Action `PhieuDangKy` (tên đúng là `PhieuDangKyHienMau` với tham số `tenNguoiHien`).
- **Hướng khắc phục đề xuất**:
  - Kiểm tra trùng Email trong bảng `TaiKhoans` trước khi đồng bộ Email ở `HoSoCaNhan [POST]`.
  - Xóa các lệnh `ModelState.AddModelError` bị trùng với DataAnnotations hoặc dùng ViewModel riêng cho `HoSoCaNhan`.
  - Sửa công thức tính `totalCount` trong `HoSoCaNhan.cshtml` chỉ đếm `successfulDonations.Count`, tính động số ngày phục hồi 84 ngày từ lần hiến thành công gần nhất, và sửa link `asp-action="PhieuDangKyHienMau" asp-route-tenNguoiHien="@donor.HoTen"` trong `Views/Admin/NguoiHienMau.cshtml`.

---

### 2. Entity phiếu đăng ký hiến máu (`PhieuDangKyHienMau`)

- **Yêu cầu**:
  - Có đủ 9 thuộc tính (`MaPhieu`, `MaNguoiHien`, `MaDot`, `NgayNop`, `LyDoHienMau`, `GhiChu`, `TrangThai`, `NgayXuLy`, `GhiChuXacNhan`), thiết lập đúng khóa chính/khóa ngoại với `NguoiHienMau` và `DotHienMau`, quản lý nhất quán 8 trạng thái nghiệp vụ.
- **Thực trạng trong source code**:
  1. **Thuộc tính và quan hệ**: `Models/PhieuDangKyHienMau.cs` (dòng 9–57) khai báo đầy đủ 9 thuộc tính và các navigation properties (`NguoiHienMau`, `DotHienMau`, `LichKhamSangLocs`, `KetQuaHienMau`), kèm 3 thuộc tính alias `[NotMapped]` (`NguoiHien`, `Dot`, `LichKhams`) để tương thích giữa các module.
  2. **Đối chiếu chi tiết 8 trạng thái nghiệp vụ**:

| STT | Trạng thái nghiệp vụ | Đã định nghĩa trong code? | Chức năng / Phương thức thiết lập | Vai trò có quyền thay đổi | Ghi chú tích hợp & Tính nhất quán |
|---|---|---|---|---|---|
| 1 | `Chờ xác nhận` | Có (`Module5TrangThai.Phieu.ChoXacNhan`, giá trị mặc định của `PhieuDangKyHienMau.TrangThai`) | `DangKyController.Create [POST]` (Module 3, dòng 275) | Người dùng đã đăng nhập (Người hiến máu) | Trạng thái khởi tạo mặc định khi nộp phiếu mới. |
| 2 | `Đã xác nhận` | Có (`Module5TrangThai.Phieu.DaXacNhan`) | `AdminController.XacNhanPhieu` (Module 4, dòng 442); hoặc hoàn tác về `"Đã xác nhận"` khi hủy lịch khám trong `AdminController.HuyLichKham` (dòng 638) / `Module5Service.HuyLichAsync` (dòng 89) | `Admin`, `Medical Staff` (`NhanVienYTe`) | Chỉ cho phép duyệt từ trạng thái `"Chờ xác nhận"`. |
| 3 | `Từ chối` | Có (`Module5TrangThai.Phieu.TuChoi`) | `AdminController.TuChoiPhieu` (Module 4, dòng 473) | `Admin`, `Medical Staff` (`NhanVienYTe`) | Cập nhật `NgayXuLy`, `GhiChuXacNhan` và tự động hủy các lịch khám chưa hoàn thành. Tuy nhiên `Views/DangKy/TheoDoi.cshtml` chưa hiển thị `GhiChuXacNhan` cho trạng thái này. |
| 4 | `Chờ khám sàng lọc` | Có (`Module5TrangThai.Phieu.ChoKhamSangLoc`) | `AdminController.ChuyenChoKham` (dòng 504), `AdminController.TaoLichKham` (dòng 577), `Module5Service.LuuLichAsync` (Module 5, dòng 60) | `Admin`, `Medical Staff` (`NhanVienYTe`) | Chuyển từ `"Đã xác nhận"` khi chuyển bước hoặc khi tạo lịch khám sàng lọc. |
| 5 | `Đã khám sàng lọc` | Có (`Module5TrangThai.Phieu.DaKhamSangLoc`) | `AdminController.HoanThanhKham` (dòng 599), `Module5Service.HoanThanhLichAsync` (Module 5, dòng 108) | `Admin`, `Medical Staff` (`NhanVienYTe`) | Thiết lập khi nhân viên y tế xác nhận hoàn thành lịch khám sàng lọc. |
| 6 | `Hiến máu thành công` | Có (`Module5TrangThai.Phieu.HienMauThanhCong`) | `AdminController.TaoKetQua` / `SuaKetQua` (dòng 738, 800), `Module5Service.GhiKetQuaAsync` (Module 5, dòng 162) | `Admin`, `Medical Staff` (`NhanVienYTe`) | Thiết lập khi ghi nhận kết quả hiến máu có `KetLuan == "Đạt"`. |
| 7 | `Không đủ điều kiện hiến` | Có (`Module5TrangThai.Phieu.KhongDuDieuKienHien`) | `AdminController.TaoKetQua` / `SuaKetQua` (dòng 738, 800), `Module5Service.GhiKetQuaAsync` (Module 5, dòng 164) | `Admin`, `Medical Staff` (`NhanVienYTe`) | Thiết lập khi ghi nhận kết quả hiến máu có `KetLuan == "Không đạt"` hoặc `"Tạm hoãn"`. |
| 8 | `Đã hủy` | Có (`Module5TrangThai.Phieu.DaHuy`) | `DangKyController.Huy [POST]` (Module 3, dòng 441) | Chủ sở hữu phiếu (`NguoiHienMau`) hoặc `Admin`/`Medical Staff` | Cho phép hủy từ `"Chờ xác nhận"` hoặc `"Đã xác nhận"`. |

- **Bằng chứng**:
  - `Models/PhieuDangKyHienMau.cs` (dòng 9–57), `Models/Module5TrangThai.cs` (dòng 5–22), `Controllers/AdminController.cs` (dòng 424–809), `Services/Module5Service.cs` (dòng 24–169).
- **Phần đã hoàn thành**:
  - Toàn bộ 9 thuộc tính, khóa ngoại và 8 trạng thái nghiệp vụ đều có mặt trong backend và có kiểm soát chuyển trạng thái theo từng bước.
- **Phần còn thiếu**:
  - `Module5Service` chưa cập nhật `PhieuDangKyHienMau.NgayXuLy = DateTime.Now` khi chuyển trạng thái phiếu sang `ChoKhamSangLoc`, `DaKhamSangLoc`, `HienMauThanhCong`, `KhongDuDieuKienHien`.
- **Lỗi hoặc rủi ro**:
  - Giao diện `Views/DangKy/TheoDoi.cshtml` (dòng 16–30) gán `stepNumber = -1` cho cả `"Từ chối"`, `"Không đủ điều kiện hiến"` và `"Đã hủy"`, nhưng ở dòng 224 chỉ kiểm tra `if (trangThai == "Đã hủy")`. Do đó khi phiếu ở trạng thái `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`, giao diện hiển thị thanh tiến trình 5 bước bị xám toàn bộ và không hiển thị hộp thông báo trạng thái bị từ chối/không đủ điều kiện.
- **Hướng khắc phục đề xuất**:
  - Bổ sung gán `phieu.NgayXuLy = DateTime.Now` trong `Module5Service.cs` và bổ sung nhánh hiển thị cảnh báo kèm `Model.GhiChuXacNhan` cho trạng thái `"Từ chối"` và `"Không đủ điều kiện hiến"` trong `Views/DangKy/TheoDoi.cshtml`.

---

### 3. Nộp phiếu đăng ký (`DangKyController.Create`)

- **Yêu cầu**:
  - Kiểm tra đầy đủ 14 điều kiện trong luồng nộp phiếu từ giao diện đến database, bao gồm xác thực, xác định đúng `NguoiHienMau`, trạng thái `"Hoạt động"`, đợt hiến máu tồn tại và `"Đang mở đăng ký"`, trong thời hạn đăng ký, không trùng phiếu đang xử lý, hồ sơ đủ thông tin bắt buộc, đủ cân nặng tối thiểu của đợt, `NgayNop` do server xác định, trạng thái `"Chờ xác nhận"`, lưu DB và chống tạo phiếu trùng khi gửi request trực tiếp hoặc đồng thời.
- **Thực trạng trong source code**:
  - Tại `Controllers/DangKyController.cs`, cả `Create [GET]` (dòng 19–129) và `Create [POST]` (dòng 132–290) đều thực hiện kiểm tra lại ở phía server các điều kiện cốt lõi:
    1. Đăng nhập (`[Authorize]` dòng 12) và đọc `maTaiKhoan` từ Claims (dòng 144–148).
    2. Truy vấn `nguoiHienMau` theo `maTaiKhoan` (dòng 150–156) và kiểm tra `nguoiHienMau.TrangThai != "Hoạt động"` (dòng 159–164).
    3. Truy vấn `dotHienMau` theo `maDot` (dòng 167–174), kiểm tra `dotHienMau.TrangThai != "Đang mở đăng ký"` (dòng 176–181) và kiểm tra `now.Date < dotHienMau.NgayBatDauDangKy.Date` hoặc `now.Date > dotHienMau.HanDangKy.Date` (dòng 184–193).
    4. Kiểm tra cân nặng `finalWeight.Value < dotHienMau.CanNangToiThieu` (dòng 224–232) và 2 câu hỏi tự sàng lọc `health_q1 != "none" || health_q2 != "none"` (dòng 234–243).
    5. Kiểm tra phiếu trùng bằng LINQ: `p.MaNguoiHien == nguoiHienMau.MaNguoiHien && p.MaDot == maDot && p.TrangThai != "Đã hủy" && p.TrangThai != "Từ chối"` (dòng 246–256).
    6. Tạo phiếu với `NgayNop = DateTime.Now`, `TrangThai = "Chờ xác nhận"` và gọi `await _context.SaveChangesAsync()` (dòng 269–282).
- **Bằng chứng**:
  - `Controllers/DangKyController.cs` (dòng 19–290), `Views/DangKy/Create.cshtml` (dòng 167–459).
- **Phần đã hoàn thành**:
  - Đã triển khai 12/14 yêu cầu trong luồng nộp phiếu và kiểm tra lại các điều kiện đợt hiến máu ở phía server khi POST.
- **Phần còn thiếu**:
  - Chưa có cơ chế chống Race Condition (2 request POST đồng thời tạo ra 2 phiếu trùng cho cùng một đợt).
  - Chưa kiểm tra các trường hồ sơ cá nhân như `NgaySinh` (độ tuổi), `GioiTinh`, `DiaChi` khi nộp phiếu, và các ô `NgaySinh`, `GioiTinh`, `NgheNghiep` trên `Views/DangKy/Create.cshtml` (dòng 203, 207, 230) không có thuộc tính `name` nên dữ liệu người dùng nhập vào các ô này trên form đăng ký bị bỏ qua hoàn toàn.
- **Lỗi hoặc rủi ro**:
  1. **Bỏ qua validation định dạng khi cập nhật nhanh hồ sơ trong `POST /DangKy/Create`**: Dòng 259–266 của `DangKyController.cs` ghi đè trực tiếp `nguoiHienMau.HoTen`, `SoDienThoai`, `Email`, `NhomMau`, `CanNang` xuống DB mà không kiểm tra Regex số điện thoại (`^0[0-9]{9,10}$`), không kiểm tra định dạng Email, không kiểm tra giới hạn trên của cân nặng (`<= 200 kg`) và không kiểm tra độ dài chuỗi (`StringLength`).
  2. **Giao diện `Create.cshtml` tự điền dữ liệu giả định khi hồ sơ người hiến máu chưa có thông tin**:
     - Dòng 217: `value="@(donor?.SoDienThoai ?? "0988 123 456")"`
     - Dòng 224: `value="@(donor?.Email ?? "sinhvien.uneti@gmail.com")"`
     - Dòng 286: `value="@(donor?.CanNang ?? 64)"`
     - Hệ quả: Một tài khoản mới chưa cập nhật hồ sơ khi mở trang nộp phiếu sẽ được điền sẵn cân nặng `64 kg` và SĐT `"0988 123 456"`. Khi bấm nút nộp phiếu, các giá trị giả này được gửi lên server, vượt qua bước kiểm tra rỗng/cân nặng tối thiểu và ghi đè luôn vào hồ sơ `NguoiHienMau` trong CSDL!
- **Hướng khắc phục đề xuất**:
  - Xóa toàn bộ các giá trị fallback giả (`"0988 123 456"`, `"sinhvien.uneti@gmail.com"`, `64`) trong `Views/DangKy/Create.cshtml`.
  - Áp dụng cùng bộ quy tắc validation server-side của `HoSoCaNhan` cho `DangKyController.Create [POST]`.
  - Bổ sung Unique Filtered Index trên `(MaNguoiHien, MaDot)` với điều kiện `TrangThai NOT IN (N'Đã hủy', N'Từ chối')` hoặc sử dụng Transaction `IsolationLevel.Serializable` kèm bắt ngoại lệ `DbUpdateException` để triệt tiêu nguy cơ tạo phiếu trùng khi gửi 2 request đồng thời.

---

### 4. Hủy phiếu (`DangKyController.Huy`)

- **Yêu cầu**:
  - Xác định chủ sở hữu phiếu từ tài khoản đăng nhập, chặn hủy phiếu người khác, kiểm tra trạng thái trước khi hủy (chỉ cho phép hủy khi `"Chờ xác nhận"` hoặc `"Đã xác nhận"` nhưng **chưa có lịch khám sàng lọc**), chặn hủy phiếu đã khám sàng lọc / đã có kết quả cuối cùng / đã hủy, lưu DB, thông báo kết quả và chống CSRF.
- **Thực trạng trong source code**:
  - Phương thức `DangKyController.Huy(int maPhieu, string? lyDoHuy)` (dòng 386–455) được bảo vệ bởi `[HttpPost]` và `[ValidateAntiForgeryToken]`.
  - Truy vấn phiếu kèm `Include(p => p.LichKhamSangLocs)` (dòng 399–401).
  - Kiểm tra quyền sở hữu: nếu người dùng không phải `Admin`/`Medical Staff` và `phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien` thì trả về `Forbid()` (dòng 412–415).
  - Kiểm tra trạng thái: chỉ cho phép `trangThaiChoPhep = new[] { "Chờ xác nhận", "Đã xác nhận" }` (dòng 418–438). Các trạng thái khác đều bị từ chối với thông báo lỗi cụ thể.
- **Bằng chứng**:
  - `Controllers/DangKyController.cs` (dòng 386–455), `Views/DangKy/TheoDoi.cshtml` (dòng 35, 487–495, 567–609).
- **Phần đã hoàn thành**:
  - Chặn hủy phiếu của người khác, chặn hủy ở các trạng thái từ `"Chờ khám sàng lọc"` trở đi, cập nhật DB và chống CSRF.
- **Phần còn thiếu / Lỗi hoặc rủi ro**:
  1. **Cho phép hủy phiếu `"Đã xác nhận"` ngay cả khi đã có lịch khám sàng lọc đang hoạt động**: Thay vì kiểm tra và từ chối hủy nếu `phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy")`, code hiện tại tại dòng 446–449 lại tự động chuyển toàn bộ lịch khám sàng lọc chưa hoàn thành sang `"Đã hủy"`:
     ```csharp
     foreach (var lich in phieu.LichKhamSangLocs.Where(l => l.TrangThai != "Đã hoàn thành"))
     {
         lich.TrangThai = "Đã hủy";
     }
     ```
     Điều này không khớp với quy tắc nghiệp vụ yêu cầu chỉ được hủy khi `"Chờ xác nhận"` hoặc `"Đã xác nhận nhưng chưa có lịch khám sàng lọc"`.
  2. **Ghi chú xác nhận (`GhiChuXacNhan`) ghi cứng là do người hiến hủy**: Tại dòng 443, nếu `lyDoHuy` để trống, hệ thống luôn gán `"Người hiến đã chủ động hủy phiếu đăng ký trên cổng thông tin."` kể cả khi người thực hiện thao tác hủy là `Admin` hoặc `Medical Staff`.
- **Hướng khắc phục đề xuất**:
  - Bổ sung kiểm tra `if (phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy"))` trước khi hủy; nếu đã có lịch khám sàng lọc đang hoạt động thì từ chối cho người hiến tự hủy và thông báo liên hệ nhân viên y tế.

---

### 5. Theo dõi phiếu (`DangKyController.TheoDoi`)

- **Yêu cầu**:
  - Người hiến máu xem được danh sách phiếu đã nộp, mã phiếu, đợt hiến máu, đơn vị tổ chức, ngày nộp, trạng thái hiện tại, lịch khám sàng lọc, kết quả hiến máu, ghi chú xử lý; đồng thời đảm bảo quyền riêng tư tuyệt đối (không xem được phiếu của người khác qua URL/request trực tiếp).
- **Thực trạng trong source code**:
  - `DangKyController.TheoDoi(int? id, string? code)` (dòng 293–383) truy vấn `userPhieuList` đã được lọc theo `p.MaNguoiHien == nguoiHienMau.MaNguoiHien` (đối với người hiến máu, dòng 321–324) kèm `.Include(p => p.DotHienMau).ThenInclude(d => d.DonViToChuc)`, `.Include(p => p.NguoiHienMau)`, `.Include(p => p.LichKhamSangLocs)`, `.Include(p => p.KetQuaHienMau)`.
  - Mọi thao tác tìm kiếm theo `id` (dòng 341) hoặc `code` (dòng 365) đều tìm kiếm bên trong `userPhieuList` (danh sách phiếu của chính người dùng đó), do đó **không bao giờ trả về dữ liệu phiếu hay ghi chú sức khỏe của người hiến máu khác**.
- **Bằng chứng**:
  - `Controllers/DangKyController.cs` (dòng 293–383), `Views/DangKy/TheoDoi.cshtml` (dòng 1–628).
- **Phần đã hoàn thành**:
  - Hiển thị đầy đủ danh sách phiếu, chi tiết phiếu, đợt hiến máu, đơn vị tổ chức, lịch khám sàng lọc, kết quả hiến máu; bảo vệ dữ liệu khỏi truy cập chéo giữa các người hiến máu.
- **Phần còn thiếu / Lỗi hoặc rủi ro**:
  1. **Không hiển thị lý do từ chối (`GhiChuXacNhan`) khi phiếu ở trạng thái `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`**: Trên `Views/DangKy/TheoDoi.cshtml` (dòng 224–233), `Model.GhiChuXacNhan` chỉ được render khi `trangThai == "Đã hủy"`. Khi nhân viên y tế từ chối phiếu (`"Từ chối"`) kèm lý do trong `GhiChuXacNhan`, người hiến máu vào trang `TheoDoi` chỉ thấy 5 bước bị xám (`stepNumber = -1`) mà không biết vì sao phiếu bị từ chối.
  2. **Hành vi fallback gây hiểu nhầm khi truy cập `id` hoặc `code` không tồn tại / không thuộc quyền sở hữu**:
     - Khi truyền `?id=999` (phiếu của người khác hoặc không tồn tại), dòng 351 gán `TempData["ErrorMessage"]`, nhưng dòng 371–374 lại gán `selectedPhieu = userPhieuList.FirstOrDefault()`, khiến giao diện vừa hiện thông báo lỗi vừa hiển thị chi tiết phiếu mới nhất của người dùng bên dưới.
     - Khi truyền `?code=DKHM-00999` không thuộc quyền sở hữu (dòng 357–368), hệ thống **không gán thông báo lỗi nào** mà âm thầm hiển thị phiếu đầu tiên trong danh sách của người dùng.
  3. **Hiển thị giá trị fallback giả trên `TheoDoi.cshtml`**: Dòng 188 hiển thị nhóm máu `"O Rh(+)"` nếu `NhomMau` đang trống, và dòng 442 hiển thị cân nặng `64 kg` nếu `CanNang` đang `null`.
- **Hướng khắc phục đề xuất**:
  - Bổ sung khối hiển thị lý do từ chối (`Model.GhiChuXacNhan`) và thời gian xử lý (`Model.NgayXuLy`) khi `trangThai == "Từ chối"` hoặc `"Không đủ điều kiện hiến"` trong `Views/DangKy/TheoDoi.cshtml`.
  - Khi người dùng truy cập `id` hoặc `code` không hợp lệ / không thuộc quyền sở hữu, thông báo lỗi rõ ràng và không tự động hiển thị một phiếu khác nếu không chuyển hướng (`RedirectToAction("TheoDoi", new { id = (int?)null })`).

---

### 6. Phân quyền và bảo mật

- **Yêu cầu**:
  - Kiểm tra xác thực `[Authorize]`, kiểm tra vai trò người hiến máu, xác định danh tính từ Claims, chống tin tưởng dữ liệu định danh từ client, chống IDOR và chống CSRF.
- **Thực trạng trong source code**:
  - Cả `NguoiHienMauController` và `DangKyController` đều gắn `[Authorize]` ở cấp class.
  - Định danh người dùng luôn được lấy từ `User.FindFirst(ClaimTypes.NameIdentifier)?.Value` (`MaTaiKhoan`), không tin tưởng `MaNguoiHien` hoặc `MaTaiKhoan` từ client.
  - Tất cả các phương thức POST (`NguoiHienMau/HoSoCaNhan`, `DangKy/Create`, `DangKy/Huy`) đều có `[HttpPost]` và `[ValidateAntiForgeryToken]`.
- **Bằng chứng**:
  - `Program.cs` (dòng 26–40), `Controllers/NguoiHienMauController.cs` (dòng 11, 24–28, 84–95), `Controllers/DangKyController.cs` (dòng 12, 26–30, 132–148, 386–415).
- **Phần đã hoàn thành**:
  - Xác thực bắt buộc, chống IDOR hiệu quả ở tầng truy vấn dữ liệu, chống CSRF đầy đủ trên tất cả các form POST.
- **Phần còn thiếu / Lỗi hoặc rủi ro**:
  1. **Chưa giới hạn Role cho cổng người hiến máu (`NguoiHienMauController` và `DangKyController.Create`)**: Do chỉ dùng `[Authorize]` không kèm Role, nếu tài khoản `Admin` hoặc `Medical Staff` truy cập `/NguoiHienMau/HoSoCaNhan` hoặc `/DangKy/Create`, hệ thống sẽ tự động tạo một bản ghi `NguoiHienMau` mới gắn với tài khoản Admin/Staff đó.
  2. **Chưa kiểm tra trạng thái hoạt động của `TaiKhoan` trên mỗi request**: Cookie đăng nhập có thời hạn 7 ngày (`Program.cs` dòng 33). Khác với `Module5AuthorizeAttribute` (có truy vấn DB kiểm tra `TaiKhoan.TrangThai == "Hoạt động"`), `NguoiHienMauController` và `DangKyController` chỉ dùng `[Authorize]` mặc định nên một tài khoản bị khóa giữa chừng vẫn có thể tiếp tục sử dụng cookie cũ cho đến khi hết hạn nếu chỉ kiểm tra ở mức `TaiKhoan`. (Tuy nhiên ở mức `NguoiHienMau`, `DangKyController.Create` đã kiểm tra `nguoiHienMau.TrangThai == "Hoạt động"`).
- **Hướng khắc phục đề xuất**:
  - Giới hạn vai trò phù hợp hoặc chặn tự động sinh hồ sơ `NguoiHienMau` đối với tài khoản `Admin`/`Medical Staff` trừ khi nghiệp vụ cho phép nhân viên y tế đồng thời là người hiến máu.
  - Kiểm tra `TaiKhoan.TrangThai == "Hoạt động"` trong filter hoặc đầu mỗi Action quan trọng.

---

### 7. Database và tích hợp

- **Yêu cầu**:
  - Kiểm tra `DbSet`, cấu hình EF Core, khóa ngoại, kết nối database, migration, cơ chế khởi tạo dữ liệu và tích hợp với các module `TaiKhoan`, `DotHienMau`, `DonViToChuc`, `LichKhamSangLoc`, `KetQuaHienMau`.
- **Thực trạng trong source code**:
  - `Data/ApplicationDbContext.cs` khai báo đầy đủ 7 `DbSet`: `TaiKhoans`, `NguoiHienMaus`, `DonViToChucs`, `DotHienMaus`, `PhieuDangKyHienMaus`, `LichKhamSangLocs`, `KetQuaHienMaus`.
  - Khóa ngoại từ `PhieuDangKyHienMau` tới `NguoiHienMau` và `DotHienMau` đều đặt `DeleteBehavior.Restrict` (tránh xóa nhầm lịch sử hiến máu khi xóa đợt hoặc hồ sơ).
  - `Data/DbInitializer.cs` tự động chạy `context.Database.Migrate()` khi ứng dụng khởi động (`Program.cs` dòng 51) và tạo dữ liệu mẫu đầy đủ cho cả 7 bảng.
- **Bằng chứng**:
  - `Data/ApplicationDbContext.cs` (dòng 16–84), `Data/DbInitializer.cs` (dòng 10–295), `Migrations/ApplicationDbContextModelSnapshot.cs`.
- **Phần đã hoàn thành**:
  - Schema CSDL, khóa ngoại, migration và tích hợp dữ liệu giữa các module hoạt động đồng bộ.
- **Phần còn thiếu / Lỗi hoặc rủi ro**:
  - Thiếu Unique Filtered Index ở tầng DB cho `(MaNguoiHien, MaDot)` trên bảng `PhieuDangKyHienMau` đối với các phiếu đang hiệu lực.
  - Lỗi liên kết tích hợp tại `Views/Admin/NguoiHienMau.cshtml` (dòng 547): gọi sai tên Action `PhieuDangKy` thay vì `PhieuDangKyHienMau`.
- **Hướng khắc phục đề xuất**:
  - Bổ sung Unique Filtered Index trong `ApplicationDbContext.OnModelCreating` và sửa đường dẫn Action trong `Views/Admin/NguoiHienMau.cshtml`.

---

### 8. Validation và xử lý lỗi

- **Yêu cầu**:
  - Kiểm tra validation cả phía giao diện và phía server cho hồ sơ người hiến máu và biểu mẫu nộp phiếu, đảm bảo thông báo lỗi rõ ràng, không phát sinh ngoại lệ chưa xử lý (`DbUpdateException` / HTTP 500).
- **Thực trạng trong source code**:
  - Trong `NguoiHienMauController.HoSoCaNhan [POST]`, validation server-side được viết chi tiết cho `HoTen`, `NgaySinh` (18–60 tuổi), `CanNang` (30–200 kg), `SoDienThoai` (`^0[0-9]{9,10}$`), `Email`.
  - Trong `DangKyController.Create [POST]`, controller nhận các tham số rời (`int maDot, string? hoTen, string? soDienThoai, string? email, string? nhomMau, double? canNang, ...`) nên **không kích hoạt DataAnnotations của model `NguoiHienMau`**.
- **Bằng chứng**:
  - `Controllers/NguoiHienMauController.cs` (dòng 97–159), `Controllers/DangKyController.cs` (dòng 135–266).
- **Phần đã hoàn thành**:
  - Validation server-side đầy đủ tại trang cập nhật hồ sơ cá nhân (`HoSoCaNhan`) và kiểm tra cân nặng tối thiểu theo đợt tại trang nộp phiếu (`Create`).
- **Phần còn thiếu / Lỗi hoặc rủi ro**:
  - `DangKyController.Create [POST]` thiếu kiểm tra định dạng `soDienThoai`, `email`, giới hạn trên của `canNang` và độ dài chuỗi (`StringLength`) trước khi lưu vào `NguoiHienMau` và `PhieuDangKyHienMau`. Nếu người dùng gửi chuỗi vượt quá độ dài cột SQL (ví dụ `hoTen` > 100 ký tự hoặc `ghiChu` > 500 ký tự), `_context.SaveChangesAsync()` sẽ ném lỗi `DbUpdateException` (HTTP 500).
  - `NguoiHienMauController.HoSoCaNhan [POST]` bị lặp thông báo lỗi validation do vừa dùng DataAnnotations trên Entity vừa gọi `ModelState.AddModelError` thủ công.
- **Hướng khắc phục đề xuất**:
  - Tách hàm dùng chung `ValidateDonorProfile(...)` để kiểm tra thống nhất ở cả `HoSoCaNhan [POST]` và `DangKy/Create [POST]`, đồng thời kiểm tra độ dài `lyDoHienMau` và `ghiChu` (`<= 500` ký tự).

---

### 9. Kiểm thử

- **Yêu cầu**:
  - Thực hiện kiểm tra biên dịch, chạy kiểm thử tự động nếu có, và đối chiếu toàn bộ 26 tình huống nghiệp vụ (`TC01`–`TC26`).
- **Thực trạng trong source code**:
  - Dự án biên dịch thành công (`0 Error(s)`).
  - Giải pháp chưa có project Unit Test / Integration Test.
  - Môi trường sandbox hiện tại chặn kết nối named-pipe tới `(localdb)\mssqllocaldb`, do đó các kịch bản kiểm thử đã được xác minh chi tiết qua phân tích luồng mã nguồn (Static Code Verification) và ghi nhận trạng thái thực thi trung thực tại Phần VI.
- **Bằng chứng**:
  - Kết quả lệnh `dotnet build` và `sqlcmd` trong quá trình kiểm tra; bảng kiểm thử chi tiết tại Phần VI.


---

## PHẦN V. DANH SÁCH CHỨC NĂNG CHƯA HOÀN THIỆN

> **Lưu ý về phân loại mức độ ưu tiên**: Qua kiểm tra toàn bộ mã nguồn thực tế của Module 3, các cơ chế bảo mật cốt lõi gồm `[Authorize]`, `[ValidateAntiForgeryToken]` và xác định chủ sở hữu từ `ClaimTypes.NameIdentifier` đều đã được triển khai ở server, không có lỗ hổng IDOR cho phép người hiến máu đọc hoặc sửa hồ sơ/phiếu của người hiến máu khác. Do đó, tuân thủ nguyên tắc *"Không gán mức CRITICAL nếu không có bằng chứng phù hợp"*, báo cáo phân loại các lỗi và mục thiếu đã xác minh từ mức **HIGH** xuống **LOW** theo đúng tính chất kỹ thuật.

---

### Lỗi/Mục thiếu số 1: Giao diện nộp phiếu tự điền sẵn thông tin giả định (SĐT, Email, Cân nặng) và Server bỏ qua kiểm tra định dạng khi cập nhật nhanh hồ sơ trong `POST /DangKy/Create`

- **Yêu cầu nghiệp vụ**: Hồ sơ cá nhân phải có đầy đủ thông tin bắt buộc và hợp lệ (SĐT đúng định dạng `^0[0-9]{9,10}$`, Email hợp lệ, Cân nặng thực tế của người hiến máu đáp ứng điều kiện của đợt) trước khi nộp phiếu đăng ký hiến máu.
- **Thực trạng hiện tại**:
  1. Trong `Views/DangKy/Create.cshtml`, khi hồ sơ `NguoiHienMau` mới được khởi tạo tự động (`SoDienThoai == null`, `CanNang == null`), giao diện tự động gán các giá trị giả vào thuộc tính `value` của thẻ `<input>`:
     - `SoDienThoai` mặc định là `"0988 123 456"` (dòng 217).
     - `Email` mặc định là `"sinhvien.uneti@gmail.com"` (dòng 224).
     - `CanNang` mặc định là `64` (dòng 286).
  2. Trong `DangKyController.Create [POST]` (dòng 196–266), server nhận các giá trị rời (`string? hoTen, string? soDienThoai, string? email, double? canNang`) và ghi đè trực tiếp vào thực thể `nguoiHienMau` trong CSDL mà **không kiểm tra định dạng SĐT (`^0[0-9]{9,10}$`), không kiểm tra định dạng Email, không kiểm tra giới hạn trên của cân nặng (`<= 200 kg`) và không kiểm tra độ dài chuỗi (`StringLength`)**.
  3. Ngoài ra, các ô `NgaySinh` (dòng 203), `GioiTinh` (dòng 207), `NgheNghiep` (dòng 230) trên `Views/DangKy/Create.cshtml` không có thuộc tính `name`, nên mọi thông tin người dùng nhập vào 3 ô này trên trang đăng ký đều bị mất khi gửi form.
- **File liên quan**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`
- **Phương thức hoặc Action liên quan**: `DangKyController.Create(int maDot, string? hoTen, string? soDienThoai, string? email, string? nhomMau, double? canNang, ...)` (`[HttpPost]`).
- **Bằng chứng**:
  - `Views/DangKy/Create.cshtml` dòng 217: `value="@(donor?.SoDienThoai ?? "0988 123 456")"`
  - `Views/DangKy/Create.cshtml` dòng 224: `value="@(donor?.Email ?? "sinhvien.uneti@gmail.com")"`
  - `Views/DangKy/Create.cshtml` dòng 286: `value="@(donor?.CanNang ?? 64)"`
  - `Controllers/DangKyController.cs` dòng 259–266: Gán trực tiếp `nguoiHienMau.SoDienThoai = finalPhone;`, `nguoiHienMau.Email = email.Trim();`, `nguoiHienMau.CanNang = finalWeight;` và gọi `SaveChangesAsync()` mà không kiểm tra Regex SĐT hay Email.
- **Hậu quả nếu chưa khắc phục**: Người dùng chưa khai báo cân nặng và số điện thoại thật vẫn nộp được phiếu với cân nặng giả `64 kg` và SĐT giả `"0988 123 456"` (chứa khoảng trắng, sai chuẩn Regex của chính hệ thống), làm sai lệch dữ liệu y tế và thông tin liên lạc trong CSDL; đồng thời nếu gửi chuỗi quá dài qua POST sẽ gây lỗi `DbUpdateException` (HTTP 500).
- **Mức độ**: **HIGH**
- **Hướng khắc phục đề xuất**:
  1. Trong `Views/DangKy/Create.cshtml`: Xóa các giá trị fallback giả (`"0988 123 456"`, `"sinhvien.uneti@gmail.com"`, `64`), chỉ dùng `placeholder` để gợi ý; bổ sung thuộc tính `name` cho `NgaySinh`, `GioiTinh`, `NgheNghiep` hoặc điều hướng người dùng sang cập nhật hồ sơ nếu thiếu thông tin bắt buộc.
  2. Trong `DangKyController.Create [POST]`: Bổ sung kiểm tra Regex SĐT `^0[0-9]{9,10}$`, định dạng `EmailAddressAttribute`, khoảng cân nặng `30–200 kg`, độ tuổi `18–60` và giới hạn độ dài chuỗi (`StringLength`) trước khi lưu.
- **Tiêu chí xác nhận đã sửa xong**:
  - Người dùng mới chưa có SĐT/Cân nặng khi mở `/DangKy/Create` thấy ô nhập trống (chỉ có placeholder).
  - Gửi POST với SĐT sai định dạng (ví dụ `"0988 123 456"` hoặc `"abc"`), Email sai định dạng hoặc Cân nặng `> 200` bị server từ chối và hiển thị thông báo lỗi hợp lệ.

---

### Lỗi/Mục thiếu số 2: Cho phép người hiến máu tự hủy phiếu ở trạng thái `"Đã xác nhận"` ngay cả khi đã có lịch khám sàng lọc đang hoạt động

- **Yêu cầu nghiệp vụ**: Chỉ cho phép người hiến máu hủy phiếu trong các trạng thái hợp lệ: `"Chờ xác nhận"` hoặc `"Đã xác nhận"` **nhưng chưa có lịch khám sàng lọc**.
- **Thực trạng hiện tại**: Trong `DangKyController.Huy [POST]` (dòng 418–450), hệ thống chỉ kiểm tra `phieu.TrangThai` có nằm trong mảng `{"Chờ xác nhận", "Đã xác nhận"}` hay không. Nếu phiếu đang ở trạng thái `"Đã xác nhận"` và đã có bản ghi `LichKhamSangLoc` (chưa hoàn thành), hệ thống **không chặn hủy** mà tự động chuyển luôn các lịch khám sàng lọc đó sang `"Đã hủy"` (dòng 446–449).
- **File liên quan**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml`
- **Phương thức hoặc Action liên quan**: `DangKyController.Huy(int maPhieu, string? lyDoHuy)` (`[HttpPost]`).
- **Bằng chứng**:
  - `Controllers/DangKyController.cs` dòng 418: `var trangThaiChoPhep = new[] { "Chờ xác nhận", "Đã xác nhận" };`
  - `Controllers/DangKyController.cs` dòng 446–449:
    ```csharp
    foreach (var lich in phieu.LichKhamSangLocs.Where(l => l.TrangThai != "Đã hoàn thành"))
    {
        lich.TrangThai = "Đã hủy";
    }
    ```
  - `Views/DangKy/TheoDoi.cshtml` dòng 35: `bool canCancel = trangThai == "Chờ xác nhận" || trangThai == "Đã xác nhận";` (không kiểm tra điều kiện chưa có lịch khám sàng lọc).
- **Hậu quả nếu chưa khắc phục**: Người hiến máu có thể tự ý hủy phiếu và làm hủy luôn lịch khám sàng lọc đã được bố trí bác sĩ mà không qua kiểm soát của nhân viên y tế trong trường hợp phiếu đang ở trạng thái `"Đã xác nhận"` có kèm lịch khám.
- **Mức độ**: **HIGH**
- **Hướng khắc phục đề xuất**:
  - Trong `DangKyController.Huy`: Kiểm tra thêm `bool coLichKhamHoatDong = phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy");`. Nếu `coLichKhamHoatDong == true`, từ chối thao tác hủy và trả về thông báo lỗi yêu cầu liên hệ đơn vị y tế.
  - Trong `Views/DangKy/TheoDoi.cshtml` dòng 35: Sửa điều kiện hiển thị nút hủy thành `bool canCancel = (trangThai == "Chờ xác nhận" || trangThai == "Đã xác nhận") && (lichKham == null || lichKham.TrangThai == "Đã hủy");`.
- **Tiêu chí xác nhận đã sửa xong**: Phiếu ở trạng thái `"Đã xác nhận"` nhưng đã có lịch khám sàng lọc (`TrangThai != "Đã hủy"`) không hiển thị nút hủy trên giao diện và bị từ chối ở server nếu gửi POST trực tiếp tới `/DangKy/Huy`.

---

### Lỗi/Mục thiếu số 3: Nguy cơ tạo phiếu đăng ký trùng khi gửi hai request POST đồng thời (Race Condition / Concurrency)

- **Yêu cầu nghiệp vụ**: Một người hiến máu không được có hai phiếu đang trong tiến trình xử lý cho cùng một đợt hiến máu, kể cả khi gửi hai request gần như đồng thời (`TC26`).
- **Thực trạng hiện tại**: Trong `DangKyController.Create [POST]` (dòng 246–282), hệ thống kiểm tra phiếu trùng bằng một truy vấn đọc `_context.PhieuDangKyHienMaus.FirstOrDefaultAsync(...)` rồi mới gọi `_context.PhieuDangKyHienMaus.Add(phieu)` và `await _context.SaveChangesAsync()` mà không đặt trong Transaction có mức cô lập phù hợp (`Serializable`), đồng thời trong `ApplicationDbContext` chưa có Unique Filtered Index ở tầng SQL Server.
- **File liên quan**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Data/ApplicationDbContext.cs`
- **Phương thức hoặc Action liên quan**: `DangKyController.Create [POST]`, `ApplicationDbContext.OnModelCreating`.
- **Bằng chứng**:
  - `Controllers/DangKyController.cs` dòng 246–282: Truy vấn `FirstOrDefaultAsync` và `SaveChangesAsync` tách rời, không có Transaction/Lock.
  - `Migrations/20261007035106_InitialCreate.cs` dòng 210–218: Bảng `PhieuDangKyHienMau` chỉ có index thường (non-unique) `IX_PhieuDangKyHienMau_MaNguoiHien` và `IX_PhieuDangKyHienMau_MaDot`.
- **Hậu quả nếu chưa khắc phục**: Khi người dùng bấm đúp (double-click) nút gửi phiếu hoặc gửi 2 request HTTP đồng thời, cả 2 request đều đọc thấy `existingPhieu == null` trước khi request kia kịp commit, dẫn đến tạo ra 2 bản ghi `PhieuDangKyHienMau` ở trạng thái `"Chờ xác nhận"` cho cùng một người trong cùng một đợt hiến máu.
- **Mức độ**: **HIGH**
- **Hướng khắc phục đề xuất**:
  1. Bổ sung Unique Filtered Index trong `ApplicationDbContext.OnModelCreating`:
     ```csharp
     modelBuilder.Entity<PhieuDangKyHienMau>()
         .HasIndex(p => new { p.MaNguoiHien, p.MaDot })
         .IsUnique()
         .HasFilter("[TrangThai] NOT IN (N'Đã hủy', N'Từ chối')");
     ```
  2. Bọc khối kiểm tra và tạo phiếu trong `DangKyController.Create [POST]` bằng Transaction hoặc bắt `DbUpdateException` để trả về thông báo phiếu đã tồn tại.
- **Tiêu chí xác nhận đã sửa xong**: Gửi 2 request `POST /DangKy/Create` đồng thời cho cùng `MaNguoiHien` và `MaDot` chỉ tạo tối đa 1 phiếu hợp lệ trong CSDL.

---

### Lỗi/Mục thiếu số 4: Đồng bộ Email từ `NguoiHienMau` sang `TaiKhoan` khi cập nhật hồ sơ cá nhân không kiểm tra trùng Email

- **Yêu cầu nghiệp vụ**: Mỗi tài khoản (`TaiKhoan`) có một địa chỉ Email định danh duy nhất dùng để đăng nhập (`AccountController.Login` và `Register` kiểm tra trùng Email).
- **Thực trạng hiện tại**: Trong `NguoiHienMauController.HoSoCaNhan [POST]` (dòng 203–211), sau khi cập nhật `nguoiHienMau.Email`, hệ thống gán trực tiếp `nguoiHienMau.TaiKhoan.Email = nguoiHienMau.Email` mà **không kiểm tra xem Email mới này đã được sử dụng bởi một `TaiKhoan` khác hay chưa**, và cũng không cập nhật lại Claims (`ClaimTypes.Name`) khi đổi `HoTen`.
- **File liên quan**: `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs`
- **Phương thức hoặc Action liên quan**: `NguoiHienMauController.HoSoCaNhan(NguoiHienMau model)` (`[HttpPost]`).
- **Bằng chứng**:
  - `Controllers/NguoiHienMauController.cs` dòng 203–211:
    ```csharp
    if (nguoiHienMau.TaiKhoan != null)
    {
        nguoiHienMau.TaiKhoan.HoTen = nguoiHienMau.HoTen;
        if (!string.IsNullOrEmpty(nguoiHienMau.Email))
        {
            nguoiHienMau.TaiKhoan.Email = nguoiHienMau.Email;
        }
    }
    ```
- **Hậu quả nếu chưa khắc phục**: Người hiến máu có thể đổi Email trong hồ sơ cá nhân trùng với Email của một tài khoản khác (kể cả tài khoản Admin), gây xung đột dữ liệu khi đăng nhập bằng Email tại `AccountController.Login` (`FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower())`).
- **Mức độ**: **MEDIUM**
- **Hướng khắc phục đề xuất**: Kiểm tra `await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != maTaiKhoan && t.Email.ToLower() == model.Email.Trim().ToLower())` trong `HoSoCaNhan [POST]`; nếu đã tồn tại thì báo lỗi `ModelState.AddModelError("Email", "Email này đã được sử dụng bởi một tài khoản khác.")`.
- **Tiêu chí xác nhận đã sửa xong**: Không thể cập nhật Email trong hồ sơ cá nhân trùng với Email của tài khoản khác đang có trong bảng `TaiKhoan`.

---

### Lỗi/Mục thiếu số 5: Trang theo dõi phiếu (`TheoDoi.cshtml`) không hiển thị lý do từ chối (`GhiChuXacNhan`) và làm xám toàn bộ tiến trình khi phiếu ở trạng thái `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`

- **Yêu cầu nghiệp vụ**: Người hiến máu khi theo dõi phiếu phải xem được trạng thái hiện tại cùng ghi chú xử lý / lý do từ chối (`GhiChuXacNhan`) nếu phiếu bị từ chối hoặc không đủ điều kiện.
- **Thực trạng hiện tại**: Trong `Views/DangKy/TheoDoi.cshtml` (dòng 16–30), khi `trangThai` là `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`, biến `stepNumber` bị gán bằng `-1`. Đến phần hiển thị tiến trình (dòng 224–297), code chỉ kiểm tra `if (trangThai == "Đã hủy")` để hiển thị hộp thông báo kèm `Model.GhiChuXacNhan`, còn các trạng thái khác rơi vào nhánh `else` hiển thị 5 bước tiến trình với toàn bộ 5 bước đều bị tô xám (`stepNumber = -1 < 1`), và **không có chỗ nào trên trang hiển thị `Model.GhiChuXacNhan`** cho người hiến máu biết lý do bị từ chối.
- **File liên quan**: `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml`
- **Phương thức hoặc Action liên quan**: `DangKyController.TheoDoi`, View `Views/DangKy/TheoDoi.cshtml`.
- **Bằng chứng**:
  - `Views/DangKy/TheoDoi.cshtml` dòng 24–30: Gán `stepNumber = -1` cho nhánh `else` (`"Từ chối"`, `"Không đủ điều kiện hiến"`, `"Đã hủy"`).
  - `Views/DangKy/TheoDoi.cshtml` dòng 224–233: Chỉ kiểm tra `@if (trangThai == "Đã hủy")` để in `@(string.IsNullOrEmpty(Model.GhiChuXacNhan) ? ... : Model.GhiChuXacNhan)`.
- **Hậu quả nếu chưa khắc phục**: Người hiến máu có phiếu bị nhân viên y tế từ chối không thể xem được lý do từ chối trên cổng thông tin, đồng thời giao diện tiến trình hiển thị như chưa từng nộp phiếu.
- **Mức độ**: **MEDIUM**
- **Hướng khắc phục đề xuất**: Bổ sung khối hiển thị thông báo cảnh báo màu đỏ kèm `Model.GhiChuXacNhan` và `Model.NgayXuLy` khi `trangThai == "Từ chối"` hoặc `trangThai == "Không đủ điều kiện hiến"`.
- **Tiêu chí xác nhận đã sửa xong**: Khi phiếu ở trạng thái `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`, trang `TheoDoi` hiển thị rõ hộp thông báo trạng thái, thời gian xử lý (`NgayXuLy`) và nội dung `GhiChuXacNhan`.

---

### Lỗi/Mục thiếu số 6: Hành vi tra cứu phiếu theo `id` hoặc `code` không tồn tại / không thuộc quyền sở hữu trên `DangKyController.TheoDoi` gây hiểu nhầm

- **Yêu cầu nghiệp vụ**: Khi truy cập chức năng theo dõi với mã phiếu không tồn tại hoặc của người khác (`TC18`, `TC25`), hệ thống phải từ chối an toàn và thông báo rõ ràng.
- **Thực trạng hiện tại**:
  1. Khi người hiến máu truy cập `/DangKy/TheoDoi?code=DKHM-99999` (mã không tồn tại hoặc của người khác), tại `DangKyController.cs` dòng 357–374, `selectedPhieu` không tìm thấy trong `userPhieuList` nhưng hệ thống **không gán thông báo lỗi nào** và tự động fallback về `selectedPhieu = userPhieuList.FirstOrDefault()` (hiển thị phiếu đầu tiên của người dùng).
  2. Khi truy cập `/DangKy/TheoDoi?id=999` (ID phiếu của người khác), hệ thống gán `TempData["ErrorMessage"]` (dòng 351) nhưng vẫn thực thi dòng 371–374 (`selectedPhieu = userPhieuList.FirstOrDefault()`), khiến giao diện vừa báo lỗi không có quyền truy cập vừa hiển thị chi tiết phiếu của chính mình ngay bên dưới.
- **File liên quan**: `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`
- **Phương thức hoặc Action liên quan**: `DangKyController.TheoDoi(int? id, string? code)`.
- **Bằng chứng**: `Controllers/DangKyController.cs` dòng 338–374.
- **Hậu quả nếu chưa khắc phục**: Gây nhầm lẫn cho người dùng khi tra cứu theo mã phiếu (`code`) hoặc `id` không tồn tại vì màn hình vẫn hiển thị thông tin của một phiếu khác.
- **Mức độ**: **MEDIUM**
- **Hướng khắc phục đề xuất**: Nếu `code` được truyền vào nhưng không tìm thấy trong `userPhieuList`, gán `TempData["ErrorMessage"] = "Không tìm thấy phiếu đăng ký phù hợp hoặc bạn không có quyền truy cập."` và thực hiện `return RedirectToAction("TheoDoi", new { id = (int?)null });` (tương tự cho `id`).
- **Tiêu chí xác nhận đã sửa xong**: Truy cập `?id=999` hoặc `?code=DKHM-99999` đều hiển thị thông báo lỗi nhất quán và điều hướng sạch về URL `/DangKy/TheoDoi`.

---

### Lỗi/Mục thiếu số 7: Chưa giới hạn Role cho cổng người hiến máu và chưa kiểm tra trạng thái hoạt động của `TaiKhoan` / `NguoiHienMau` khi cập nhật hồ sơ

- **Yêu cầu nghiệp vụ**: Kiểm tra đúng vai trò người hiến máu, kiểm tra trạng thái hoạt động của người hiến máu (`TrangThai == "Hoạt động"`) và tài khoản trên các chức năng Module 3.
- **Thực trạng hiện tại**:
  1. `NguoiHienMauController` và `DangKyController` chỉ dùng `[Authorize]` mà không kiểm tra Role. Nếu `Admin` hoặc `Medical Staff` truy cập `/NguoiHienMau/HoSoCaNhan` hoặc `/DangKy/Create`, hệ thống tự động tạo một bản ghi `NguoiHienMau` mới cho tài khoản Admin/Staff đó (`NguoiHienMauController.cs` dòng 54–72, `DangKyController.cs` dòng 37–54).
  2. `NguoiHienMauController.HoSoCaNhan [POST]` không kiểm tra `nguoiHienMau.TrangThai == "Hoạt động"` trước khi cho phép sửa hồ sơ.
  3. Cả 2 controller chưa kiểm tra `TaiKhoan.TrangThai == "Hoạt động"` trong DB (nếu tài khoản bị khóa khi cookie 7 ngày vẫn còn hạn).
  4. Nếu người dùng chưa có hồ sơ `NguoiHienMau` mà gửi trực tiếp request `POST /NguoiHienMau/HoSoCaNhan` hoặc `POST /DangKy/Create` (không qua GET trước), hệ thống trả về HTTP 404 (`NotFound`) thay vì tự khởi tạo hồ sơ hoặc hướng dẫn bổ sung hồ sơ.
- **File liên quan**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`
- **Phương thức hoặc Action liên quan**: `NguoiHienMauController.HoSoCaNhan [GET/POST]`, `DangKyController.Create [GET/POST]`.
- **Bằng chứng**:
  - `Controllers/NguoiHienMauController.cs` dòng 11, 54–72, 92–95.
  - `Controllers/DangKyController.cs` dòng 12, 37–54, 153–156.
- **Hậu quả nếu chưa khắc phục**: Sinh dữ liệu rác trong bảng `NguoiHienMau` khi tài khoản quản trị/nhân viên y tế bấm vào mục Hồ sơ cá nhân; người hiến máu đang bị khóa trạng thái vẫn sửa được hồ sơ cá nhân.
- **Mức độ**: **MEDIUM**
- **Hướng khắc phục đề xuất**: Kiểm tra vai trò và trạng thái `TaiKhoan.TrangThai == "Hoạt động"` cũng như `nguoiHienMau.TrangThai == "Hoạt động"` trong `NguoiHienMauController` và `DangKyController`.
- **Tiêu chí xác nhận đã sửa xong**: Tài khoản bị khóa hoặc hồ sơ `NguoiHienMau` không ở trạng thái `"Hoạt động"` bị từ chối khi gọi `POST HoSoCaNhan`; tài khoản Admin/Staff không bị tự động tạo hồ sơ `NguoiHienMau` ngoài ý muốn.

---

### Lỗi/Mục thiếu số 8: Liên kết xem danh sách phiếu đăng ký của người hiến máu trên trang Admin (`Views/Admin/NguoiHienMau.cshtml`) bị lỗi 404 và thiếu chức năng khóa/mở khóa trạng thái `NguoiHienMau`

- **Yêu cầu nghiệp vụ**: Tích hợp đồng bộ giữa quản lý hồ sơ người hiến máu, trạng thái hoạt động của người hiến máu và danh sách phiếu đăng ký.
- **Thực trạng hiện tại**:
  1. Trong `Views/Admin/NguoiHienMau.cshtml` (dòng 547), nút `"Xem các phiếu đăng ký của người này"` trỏ tới `asp-controller="Admin" asp-action="PhieuDangKy" asp-route-search="@donor.HoTen"`, nhưng trong `AdminController.cs` tên Action thực tế là `PhieuDangKyHienMau(string? tenNguoiHien, ...)`. Khi bấm vào nút này sẽ gặp lỗi HTTP 404.
  2. Tại dòng 62 của `Views/Admin/NguoiHienMau.cshtml`, nút `"Thêm Hồ Sơ Mới"` dành cho Admin lại trỏ sang `asp-controller="DangKy" asp-action="Create"`.
  3. Trong `AdminController.cs`, chưa có Action nào cho phép Admin cập nhật `NguoiHienMau.TrangThai` (ví dụ chuyển sang `"Tạm hoãn"` / `"Ngừng hoạt động"`).
- **File liên quan**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/NguoiHienMau.cshtml`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs`
- **Phương thức hoặc Action liên quan**: `AdminController.NguoiHienMau`, `AdminController.PhieuDangKyHienMau`.
- **Bằng chứng**:
  - `Views/Admin/NguoiHienMau.cshtml` dòng 62 và dòng 547.
  - `Controllers/AdminController.cs` dòng 367–420.
- **Hậu quả nếu chưa khắc phục**: Nhân viên quản trị bấm xem phiếu đăng ký từ hồ sơ người hiến máu bị lỗi 404; không có giao diện để đổi trạng thái `NguoiHienMau.TrangThai`.
- **Mức độ**: **MEDIUM**
- **Hướng khắc phục đề xuất**: Sửa dòng 547 trong `Views/Admin/NguoiHienMau.cshtml` thành `asp-controller="Admin" asp-action="PhieuDangKyHienMau" asp-route-tenNguoiHien="@donor.HoTen"` và bổ sung Action đổi trạng thái `NguoiHienMau` trong `AdminController`.
- **Tiêu chí xác nhận đã sửa xong**: Bấm nút `"Xem các phiếu đăng ký của người này"` điều hướng đúng sang danh sách phiếu đăng ký đã lọc theo tên người hiến máu đó.

---

### Lỗi/Mục thiếu số 9: Trang hồ sơ cá nhân (`HoSoCaNhan.cshtml`) hiển thị sai số lần hiến máu thành công, hardcode trạng thái phục hồi 84 ngày và hardcode kết quả xét nghiệm

- **Yêu cầu nghiệp vụ**: Hiển thị chính xác thông tin hồ sơ, lịch sử hiến máu và điều kiện sức khỏe thực tế của người hiến máu.
- **Thực trạng hiện tại**: Trong `Views/NguoiHienMau/HoSoCaNhan.cshtml`:
  1. Dòng 16: `var totalCount = successfulDonations.Count > 0 ? successfulDonations.Count : phieuList.Count(p => p.TrangThai != "Đã hủy");` — nếu người dùng chưa có lần hiến máu thành công nào nhưng vừa nộp 1 phiếu `"Chờ xác nhận"` (hoặc có phiếu `"Từ chối"`), `totalCount` vẫn tính bằng `1`, kéo theo tổng lượng máu `350 mL`, cứu `3` người và đạt `"Huy Hiệu Đồng"`.
  2. Dòng 151–167: Khối `"Thời gian phục hồi (84 ngày)"` hiển thị tĩnh `"Sẵn sàng - Đủ điều kiện hiến đợt tiếp theo"` (`width: 100%`) mà không tính toán từ ngày hiến máu thành công gần nhất (ngoài ra `DangKyController.Create` cũng chưa kiểm tra khoảng cách 84 ngày khi nộp phiếu mới).
  3. Dòng 437–472 & 492–554: Tab `"Hồ sơ xét nghiệm"` hiển thị tĩnh 5 chỉ số âm tính và thẻ `"Chứng nhận hiến máu số"` luôn hiển thị với tên fallback `"NGUYỄN HOÀNG NAM"` kể cả khi chưa hiến máu lần nào.
- **File liên quan**: `QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSoCaNhan.cshtml`
- **Phương thức hoặc Action liên quan**: `NguoiHienMauController.HoSoCaNhan [GET]`.
- **Bằng chứng**: `Views/NguoiHienMau/HoSoCaNhan.cshtml` dòng 14–31, 151–167, 437–472, 492–554.
- **Hậu quả nếu chưa khắc phục**: Hiển thị sai lệch số lần hiến máu thực tế, chứng nhận hiến máu và trạng thái đủ điều kiện 84 ngày của người dùng.
- **Mức độ**: **LOW**
- **Hướng khắc phục đề xuất**: Sửa `var totalCount = successfulDonations.Count;`, tính động số ngày kể từ lần hiến thành công gần nhất tương tự logic tại `Views/Admin/NguoiHienMau.cshtml` (dòng 155–183), và ẩn chứng nhận điện tử khi `successfulDonations.Count == 0`.
- **Tiêu chí xác nhận đã sửa xong**: Tài khoản mới hoặc chỉ có phiếu `"Chờ xác nhận"` hiển thị `0` lần hiến, `0 mL` và thông báo chưa có chứng nhận/xét nghiệm.

---

### Lỗi/Mục thiếu số 10: Lặp thông báo lỗi validation trong `NguoiHienMauController.HoSoCaNhan [POST]` và thiếu cập nhật `NgayXuLy` trong `Module5Service`

- **Yêu cầu nghiệp vụ**: Thông báo lỗi validation rõ ràng, không trùng lặp; cập nhật nhất quán `NgayXuLy` khi trạng thái phiếu thay đổi.
- **Thực trạng hiện tại**:
  1. `Models/NguoiHienMau.cs` đã khai báo `[Required]`, `[Range]`, `[RegularExpression]`, `[EmailAddress]`, nhưng `NguoiHienMauController.HoSoCaNhan [POST]` (dòng 98–159) lại tiếp tục gọi `ModelState.AddModelError` cho cùng các thuộc tính đó mà không `ModelState.Remove(...)` trước, làm xuất hiện 2 thông báo lỗi cho cùng 1 trường.
  2. Trong `Services/Module5Service.cs` (dòng 60, 89, 108, 161–164), khi cập nhật `phieu.TrangThai` sang `ChoKhamSangLoc`, `DaXacNhan`, `DaKhamSangLoc`, `HienMauThanhCong`, `KhongDuDieuKienHien`, service không cập nhật `phieu.NgayXuLy = DateTime.Now` (khác với `AdminController` luôn cập nhật `phieu.NgayXuLy = DateTime.Now`).
- **File liên quan**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Services/Module5Service.cs`
- **Phương thức hoặc Action liên quan**: `NguoiHienMauController.HoSoCaNhan [POST]`, `Module5Service.LuuLichAsync`, `HuyLichAsync`, `HoanThanhLichAsync`, `GhiKetQuaAsync`.
- **Bằng chứng**: `Controllers/NguoiHienMauController.cs` dòng 98–159; `Services/Module5Service.cs` dòng 60, 89, 108, 161–164.
- **Hậu quả nếu chưa khắc phục**: Giao diện hiển thị lặp câu thông báo lỗi khi người dùng nhập sai; trường `NgayXuLy` không phản ánh thời điểm cập nhật mới nhất khi thao tác qua `LichKhamSangLocController` / `KetQuaHienMauController`.
- **Mức độ**: **LOW**
- **Hướng khắc phục đề xuất**: Gọi `ModelState.Clear()` trước khi chạy bộ kiểm tra tùy chỉnh trong `HoSoCaNhan [POST]` (hoặc bỏ các `AddModelError` trùng lặp), và bổ sung `phieu.NgayXuLy = DateTime.Now;` trong `Module5Service.cs`.
- **Tiêu chí xác nhận đã sửa xong**: Mỗi trường nhập sai chỉ hiển thị đúng 1 thông báo lỗi tiếng Việt; `NgayXuLy` luôn được cập nhật khi trạng thái phiếu thay đổi.

---

## PHẦN VI. BẢNG KIỂM THỬ (TC01 – TC26)

> **Quy tắc ghi nhận trạng thái trong bảng kiểm thử**:
> - Toàn bộ 26 trường hợp kiểm thử dưới đây đã được **kiểm tra trực tiếp trên mã nguồn thực tế (Static Code Verification)** kết hợp biên dịch `dotnet build` (`0 Error(s)`).
> - Do môi trường sandbox chặn kết nối named-pipe tới `(localdb)\mssqllocaldb` (`Sqlcmd: Login timeout expired`) và tuân thủ nguyên tắc không làm thay đổi dữ liệu thật trong DB cũng như **không tự tạo kết quả `PASS` khi chưa chạy thực thi runtime**, cột **Trạng thái** sử dụng:
>   - `FAIL (Static Audit)`: Khi ngay từ kiểm tra mã nguồn đã xác minh chắc chắn logic code không đáp ứng yêu cầu hoặc có lỗi nghiệp vụ.
>   - `BLOCKED (Runtime DB) / Đạt ở mức Code`: Khi logic mã nguồn đã triển khai đúng và đầy đủ (có bằng chứng dòng code cụ thể), nhưng bước chạy HTTP runtime với LocalDB bị chặn bởi sandbox.

| Mã | Tiền điều kiện | Các bước thực hiện | Kết quả mong đợi | Kết quả thực tế (Từ kiểm tra mã nguồn) | Trạng thái | Bằng chứng hoặc vị trí code liên quan | Mức độ nghiêm trọng (nếu lỗi) |
|---|---|---|---|---|---|---|---|
| **TC01** | Người dùng chưa đăng nhập (không có cookie `"QuanLyHienMau.Auth"`). | Gửi request `GET /DangKy/Create?maDot=1` hoặc `POST /DangKy/Create`. | Hệ thống từ chối truy cập và chuyển hướng về `/Account/Login?ReturnUrl=...`. | `DangKyController` gắn `[Authorize]` ở cấp class; `Program.cs` cấu hình `options.LoginPath = "/Account/Login"`. Người chưa đăng nhập bị chuyển hướng về trang đăng nhập. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 12), `Program.cs` (dòng 31). | - |
| **TC02** | Người dùng đã đăng nhập (`TaiKhoan` hợp lệ) nhưng chưa có bản ghi trong bảng `NguoiHienMau`. | 1. Truy cập `GET /DangKy/Create?maDot=1`.<br>2. Hoặc gửi trực tiếp `POST /DangKy/Create` khi chưa từng gọi GET. | Được tự động tạo hồ sơ/hướng dẫn bổ sung hồ sơ hoặc bị chặn an toàn theo nghiệp vụ. | - Nếu gọi `GET /DangKy/Create`: Hệ thống tự động tạo mới `NguoiHienMau` từ `TaiKhoan` (`TrangThai = "Hoạt động"`).<br>- Tuy nhiên nếu gửi trực tiếp `POST /DangKy/Create` mà chưa gọi GET: Hệ thống trả về `NotFound("Không tìm thấy hồ sơ người hiến máu.")` (HTTP 404). | `FAIL (Static Audit)` | `Controllers/DangKyController.cs` (dòng 37–54 cho GET; dòng 150–156 cho POST trả về `NotFound`). | MEDIUM |
| **TC03** | Người dùng đã đăng nhập, có hồ sơ `NguoiHienMau` (`TrangThai == "Hoạt động"`, đủ thông tin, cân nặng `>= CanNangToiThieu`), đợt hiến máu đang mở và trong hạn đăng ký, chưa có phiếu trùng. | Truy cập `GET /DangKy/Create?maDot=1`, điền thông tin hợp lệ, chọn 2 câu hỏi sàng lọc `"none"`, gửi `POST /DangKy/Create`. | Tạo phiếu thành công trong bảng `PhieuDangKyHienMau`, thông báo `TempData["SuccessMessage"]` và chuyển hướng tới `/DangKy/TheoDoi/{MaPhieu}`. | Logic code kiểm tra tất cả điều kiện, khởi tạo `PhieuDangKyHienMau`, gọi `_context.SaveChangesAsync()`, gán `TempData["SuccessMessage"]` và `RedirectToAction("TheoDoi", new { id = phieu.MaPhieu })`. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 132–288). | - |
| **TC04** | Người dùng có hồ sơ `NguoiHienMau.TrangThai != "Hoạt động"` (ví dụ `"Tạm khóa"`). | Truy cập `GET /DangKy/Create?maDot=1` hoặc gửi `POST /DangKy/Create`. | Hệ thống từ chối cho nộp phiếu và hiển thị thông báo lỗi. | Cả GET (dòng 57–62) và POST (dòng 159–164) đều kiểm tra `if (nguoiHienMau.TrangThai != "Hoạt động")`, gán `TempData["ErrorMessage"]` và chuyển hướng về `DotHienMau/Index`. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 57–62, 159–164). | - |
| **TC05** | Người dùng đã đăng nhập, đợt hiến máu `maDot = 9999` không tồn tại trong DB. | Truy cập `GET /DangKy/Create?maDot=9999` hoặc gửi `POST /DangKy/Create` với `maDot = 9999`. | Hệ thống từ chối và báo lỗi đợt hiến máu không tồn tại. | Cả GET (dòng 80–84) và POST (dòng 169–174) đều kiểm tra `if (dotHienMau == null)`, gán `TempData["ErrorMessage"] = "Đợt hiến máu bạn chọn không tồn tại trong hệ thống."` và chuyển hướng về `DotHienMau/Index`. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 77–84, 167–174). | - |
| **TC06** | Đợt hiến máu tồn tại nhưng `TrangThai != "Đang mở đăng ký"` (ví dụ `"Đã kết thúc"` hoặc `"Tạm đóng"`). | Truy cập `GET /DangKy/Create?maDot=2` hoặc gửi `POST /DangKy/Create` với `maDot = 2`. | Hệ thống từ chối tạo phiếu và thông báo đợt không mở đăng ký. | Cả GET (dòng 87–91) và POST (dòng 176–181) đều kiểm tra `if (dotHienMau.TrangThai != "Đang mở đăng ký")` và chuyển hướng về `DotHienMau/Details`. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 87–91, 176–181). | - |
| **TC07** | Đợt hiến máu có `TrangThai == "Đang mở đăng ký"` nhưng ngày hiện tại `< NgayBatDauDangKy.Date` hoặc `> HanDangKy.Date`. | Truy cập `GET /DangKy/Create` hoặc gửi `POST /DangKy/Create` cho đợt đó. | Hệ thống từ chối nhận đăng ký. | Cả GET (dòng 94–104) và POST (dòng 184–193) đều kiểm tra `now.Date < dotHienMau.NgayBatDauDangKy.Date` và `now.Date > dotHienMau.HanDangKy.Date`, từ chối và chuyển hướng về `DotHienMau/Details`. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 94–104, 184–193). | - |
| **TC08** | Hồ sơ người hiến máu đang trống `SoDienThoai` và `CanNang` (`null`). | 1. Mở giao diện `GET /DangKy/Create` rồi bấm nút Gửi ngay.<br>2. Hoặc gửi POST trực tiếp với `hoTen = ""` / `soDienThoai = ""`. | Nếu thiếu thông tin hồ sơ bắt buộc, hệ thống phải từ chối và thông báo lỗi. | - Nếu gửi POST trực tiếp với `hoTen = ""` hoặc `soDienThoai = ""` khi DB cũng trống: Server từ chối (dòng 200–219).<br>- **LỖI**: Trên giao diện `Views/DangKy/Create.cshtml` (dòng 217, 286), khi `donor.SoDienThoai` hoặc `donor.CanNang` là `null`, View tự động điền sẵn `value="0988 123 456"` và `value="64"`, khiến người dùng bấm Submit là vượt qua kiểm tra bắt buộc và lưu dữ liệu giả vào DB! Ngoài ra chưa kiểm tra `NgaySinh`, `GioiTinh`, `DiaChi`. | `FAIL (Static Audit)` | `Views/DangKy/Create.cshtml` (dòng 217, 224, 286), `Controllers/DangKyController.cs` (dòng 196–222). | HIGH |
| **TC09** | Đợt hiến máu có `CanNangToiThieu = 45` kg; người hiến máu có cân nặng `42` kg. | Gửi `POST /DangKy/Create` với `canNang = 42`. | Hệ thống từ chối tạo phiếu và thông báo không đạt cân nặng tối thiểu của đợt. | `DangKyController.Create [POST]` (dòng 224–232) kiểm tra `if (finalWeight.Value < dotHienMau.CanNangToiThieu)`, báo lỗi `TempData["ErrorMessage"]` và trả lại View. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 224–232). | - |
| **TC10** | Người hiến máu đã có 1 phiếu của đợt `maDot = 1` ở trạng thái `"Chờ xác nhận"` (hoặc `"Đã xác nhận"`, `"Chờ khám sàng lọc"`). | Truy cập `GET /DangKy/Create?maDot=1` hoặc gửi `POST /DangKy/Create` với `maDot = 1` (tuần tự). | Hệ thống ngăn chặn tạo phiếu trùng và điều hướng sang trang theo dõi phiếu hiện có. | Cả GET (dòng 107–117) và POST (dòng 246–256) đều truy vấn `existingPhieu` với điều kiện `p.MaNguoiHien == nguoiHienMau.MaNguoiHien && p.MaDot == maDot && p.TrangThai != "Đã hủy" && p.TrangThai != "Từ chối"` và chặn tạo mới. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 107–117, 246–256). | - |
| **TC11** | Người dùng nộp phiếu hợp lệ, cố tình truyền thêm trường `NgayNop = "2020-01-01"` trong body POST. | Gửi `POST /DangKy/Create`. | `NgayNop` phải do hệ thống xác định (`DateTime.Now`), không nhận giá trị từ client. | Action `Create [POST]` không nhận tham số `NgayNop` và gán cứng `NgayNop = DateTime.Now` tại dòng 273. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 135–142, 273). | - |
| **TC12** | Người dùng nộp phiếu hợp lệ, cố tình truyền thêm `TrangThai = "Đã xác nhận"` trong body POST. | Gửi `POST /DangKy/Create`. | Phiếu mới luôn được tạo với trạng thái `"Chờ xác nhận"`. | Action `Create [POST]` không bind `TrangThai` từ client mà gán cứng `TrangThai = "Chờ xác nhận"` tại dòng 275. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 275). | - |
| **TC13** | Người hiến máu có phiếu ở trạng thái `"Chờ xác nhận"` thuộc sở hữu của mình. | Gửi `POST /DangKy/Huy` với `maPhieu` của phiếu đó. | Hủy phiếu thành công, `TrangThai` cập nhật thành `"Đã hủy"`, lưu `NgayXuLy = DateTime.Now` vào DB. | `DangKyController.Huy` kiểm tra quyền sở hữu và cho phép hủy khi `TrangThai == "Chờ xác nhận"`, lưu DB và báo `TempData["SuccessMessage"]`. *(Lưu ý: Nếu phiếu ở trạng thái `"Đã xác nhận"` nhưng đã có lịch khám sàng lọc thì code hiện tại vẫn cho hủy - xem Lỗi số 2).* | `BLOCKED (Runtime DB)` *(Đạt ở mức Code cho trạng thái Chờ xác nhận)* | `Controllers/DangKyController.cs` (dòng 418–454). | - |
| **TC14** | Người hiến máu có phiếu ở trạng thái `"Đã khám sàng lọc"` (hoặc `"Chờ khám sàng lọc"`). | Gửi `POST /DangKy/Huy` với `maPhieu` của phiếu đó. | Hệ thống từ chối hủy phiếu và hiển thị thông báo lỗi. | Dòng 418–428 chặn mọi trạng thái ngoài `{"Chờ xác nhận", "Đã xác nhận"}` và báo lỗi cụ thể: *"Phiếu đã bước vào giai đoạn khám sàng lọc..."*. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 418–428). | - |
| **TC15** | Người hiến máu có phiếu ở trạng thái `"Hiến máu thành công"` hoặc `"Không đủ điều kiện hiến"`. | Gửi `POST /DangKy/Huy` với `maPhieu` của phiếu đó. | Hệ thống từ chối hủy phiếu. | Dòng 429–432 chặn và báo lỗi: *"Phiếu đăng ký đã có kết quả cuối cùng, không thể thực hiện thao tác hủy."* | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 429–432). | - |
| **TC16** | Người hiến máu A đã đăng nhập; phiếu `maPhieu = 5` thuộc về người hiến máu B. | Người hiến máu A gửi `POST /DangKy/Huy` với `maPhieu = 5`. | Hệ thống từ chối truy cập (`Forbid` / HTTP 403) và không thay đổi dữ liệu phiếu số 5. | Dòng 412–415 kiểm tra `if (!isStaffOrAdmin && (nguoiHienMau == null || phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien)) return Forbid();`. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 412–415). | - |
| **TC17** | Người hiến máu A đã đăng nhập và có 2 phiếu; trong DB còn có các phiếu của người hiến máu B, C. | Truy cập `GET /DangKy/TheoDoi`. | Chỉ hiển thị danh sách 2 phiếu của người hiến máu A. | Dòng 321–324 lọc `query = query.Where(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien);`. Chỉ trả về phiếu của A. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 321–324). | - |
| **TC18** | Người hiến máu A đã đăng nhập; phiếu `id = 5` (`code = "DKHM-00005"`) thuộc về người hiến máu B. | Người hiến máu A truy cập `GET /DangKy/TheoDoi?id=5` hoặc `GET /DangKy/TheoDoi?code=DKHM-00005`. | Không được truy cập dữ liệu phiếu của người B và phải xử lý thông báo nhất quán. | - **Bảo mật dữ liệu**: Đạt (không lộ dữ liệu của người B vì chỉ tìm trong `userPhieuList` của A).<br>- **LỖI xử lý luồng hiển thị**: Khi tìm bằng `?code=DKHM-00005` của người khác, hệ thống không báo lỗi mà âm thầm hiển thị phiếu đầu tiên của A; khi tìm bằng `?id=5` thì vừa báo lỗi vừa hiển thị phiếu đầu tiên của A bên dưới. | `FAIL (Static Audit - Lỗi UX/Xử lý luồng khi từ chối)` | `Controllers/DangKyController.cs` (dòng 338–374). | MEDIUM |
| **TC19** | Người hiến máu A đã đăng nhập (`MaNguoiHien = 1`); cố tình gửi kèm `MaNguoiHien = 2` (của người B) trong request `POST /DangKy/Create`. | Gửi `POST /DangKy/Create` với tham số `MaNguoiHien = 2`. | Hệ thống không tin tưởng `MaNguoiHien` từ client; chỉ tạo phiếu cho `MaNguoiHien = 1`. | `DangKyController.Create [POST]` không nhận tham số `MaNguoiHien` từ client mà lấy `nguoiHienMau` từ `ClaimTypes.NameIdentifier` (`maTaiKhoan`) và gán `MaNguoiHien = nguoiHienMau.MaNguoiHien` (dòng 271). | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 135–156, 271). | - |
| **TC20** | Người hiến máu A đã đăng nhập (`MaNguoiHien = 1`); gửi `POST /NguoiHienMau/HoSoCaNhan` kèm `MaNguoiHien = 2` và `MaTaiKhoan = 99`. | Gửi `POST /NguoiHienMau/HoSoCaNhan` với dữ liệu chỉnh sửa. | Không thể cập nhật hồ sơ của người hiến máu khác. | `NguoiHienMauController.HoSoCaNhan [POST]` (dòng 86–95, 191–201) truy vấn `nguoiHienMau` theo `maTaiKhoan` trong Claims và chỉ cập nhật các thuộc tính thông tin cá nhân trên bản ghi của chính người dùng đó, bỏ qua `model.MaNguoiHien` và `model.MaTaiKhoan`. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/NguoiHienMauController.cs` (dòng 86–95, 191–201). | - |
| **TC21** | Người dùng đã đăng nhập. | Gửi `POST /NguoiHienMau/HoSoCaNhan` hoặc `POST /DangKy/Create` với `CanNang = -10`. | Hệ thống từ chối và báo lỗi cân nặng không hợp lệ. | - Tại `HoSoCaNhan [POST]`: Chặn bởi `[Range(30.0, 200.0)]` và `model.CanNang < 30` (dòng 132).<br>- Tại `DangKy/Create [POST]`: Chặn bởi `finalWeight.Value <= 0` (dòng 215). | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Models/NguoiHienMau.cs` (dòng 48), `Controllers/NguoiHienMauController.cs` (dòng 132), `Controllers/DangKyController.cs` (dòng 215). | - |
| **TC22** | Người dùng đã đăng nhập. | 1. Gửi `POST /NguoiHienMau/HoSoCaNhan` với `Email = "sai-dinh-dang"`.<br>2. Gửi `POST /DangKy/Create` với `email = "sai-dinh-dang"`. | Cả hai luồng đều phải từ chối Email sai định dạng ở phía server. | - Tại `HoSoCaNhan [POST]`: Từ chối đúng (dòng 151–154).<br>- **LỖI tại `DangKy/Create [POST]`**: Không hề kiểm tra định dạng Email ở server; dòng 261–263 gán thẳng `nguoiHienMau.Email = email.Trim();` và lưu vào DB! | `FAIL (Static Audit)` | `Controllers/DangKyController.cs` (dòng 261–263). | HIGH |
| **TC23** | Người dùng đã đăng nhập. | Gửi `POST /NguoiHienMau/HoSoCaNhan` với `HoTen = ""` hoặc `CanNang = null`. | Hệ thống từ chối lưu và trả về thông báo lỗi trường bắt buộc. | Server kiểm tra `string.IsNullOrWhiteSpace(model.HoTen)` (dòng 98) và `!model.CanNang.HasValue` (dòng 126), trả về View kèm lỗi (tuy nhiên thông báo lỗi bị hiển thị lặp 2 lần do trùng với DataAnnotations). | `BLOCKED (Runtime DB)` *(Đạt logic chặn ở Code, có lỗi LOW lặp thông báo)* | `Controllers/NguoiHienMauController.cs` (dòng 98–101, 126–129). | LOW |
| **TC24** | Người dùng chưa đăng nhập hoặc tài khoản không đúng vai trò truy cập các Action của Module 3. | 1. Chưa đăng nhập truy cập `/NguoiHienMau/HoSoCaNhan`, `/DangKy/TheoDoi`, `/DangKy/Huy`.<br>2. Tài khoản `Admin` truy cập `/NguoiHienMau/HoSoCaNhan` hoặc `/DangKy/Create`. | 1. Chưa đăng nhập bị chuyển hướng tới `/Account/Login`.<br>2. Kiểm tra đúng vai trò người hiến máu. | 1. Chưa đăng nhập bị chặn bởi `[Authorize]` trên toàn bộ Action.<br>2. **Khiếm khuyết phân quyền vai trò**: `NguoiHienMauController` và `DangKyController.Create` không kiểm tra Role `Donor`, nên nếu `Admin` truy cập sẽ tự động tạo bản ghi `NguoiHienMau` cho Admin. | `FAIL (Static Audit - Thiếu kiểm tra Role)` | `Controllers/NguoiHienMauController.cs` (dòng 11, 54–72), `Controllers/DangKyController.cs` (dòng 12, 37–54). | MEDIUM |
| **TC25** | Người dùng đã đăng nhập nhưng chưa từng nộp phiếu nào (hoặc truyền `id = 9999` khi danh sách phiếu trống). | Truy cập `GET /DangKy/TheoDoi` hoặc `GET /DangKy/TheoDoi?id=9999`. | Xử lý an toàn, không ném lỗi NullReferenceException, không lộ thông tin. | Khi `userPhieuList` rỗng, `selectedPhieu` là `null`; `Views/DangKy/TheoDoi.cshtml` (dòng 150–163) kiểm tra `@if (Model == null)` và hiển thị màn hình trạng thái trống (`"Chưa có thông tin phiếu đăng ký"`) an toàn. | `BLOCKED (Runtime DB)` *(Đạt ở mức Code)* | `Controllers/DangKyController.cs` (dòng 371–382), `Views/DangKy/TheoDoi.cshtml` (dòng 150–163). | - |
| **TC26** | Người hiến máu hợp lệ gửi 2 request `POST /DangKy/Create` cho cùng `maDot = 1` tại cùng một thời điểm (Concurrency). | Gửi 2 HTTP POST request song song. | Hệ thống chỉ cho phép tạo tối đa 1 phiếu đang xử lý, request thứ hai phải bị chặn. | **LỖI**: Không có Transaction `Serializable` và không có Unique Filtered Index trên `(MaNguoiHien, MaDot)` trong CSDL. Cả 2 request đồng thời có thể cùng đọc `existingPhieu == null` và insert 2 phiếu trùng. | `FAIL (Static Audit)` | `Controllers/DangKyController.cs` (dòng 246–282), `Data/ApplicationDbContext.cs`. | HIGH |

---

## PHẦN VII. KẾT LUẬN

### 1. Module 3 hiện đã hoàn thành bao nhiêu phần trăm theo yêu cầu nghiệp vụ?
- Dựa trên phương pháp chấm điểm có trọng số công khai dưới đây, **Module 3 hiện đạt `79.25%` (xấp xỉ `79%`)** so với toàn bộ yêu cầu nghiệp vụ và phi chức năng (bảo mật, kiểm soát đồng thời, validation).

### 2. Cơ sở và công thức tính tỷ lệ hoàn thành
Để đảm bảo tính khách quan, không đánh giá cảm tính theo số lượng file hay số lượng Action, Module 3 được chia thành **8 nhóm chức năng/yêu cầu** với trọng số phản ánh tầm quan trọng nghiệp vụ (tổng trọng số = `100%`). Quy tắc chấm điểm từng yêu cầu con trong mỗi nhóm:
- `HOÀN THÀNH`: `1.0` điểm.
- `HOÀN THÀNH MỘT PHẦN`: `0.5` điểm.
- `CHƯA HOÀN THÀNH` hoặc `CÓ LỖI`: `0.25` điểm (nếu đã có khung xử lý nhưng sai điều kiện/thiếu ràng buộc quan trọng) hoặc `0.0` điểm (nếu hoàn toàn chưa triển khai).

| Nhóm đánh giá | Trọng số (%) | Tỷ lệ đạt trong nhóm | Điểm quy đổi (%) | Cơ sở chấm chi tiết |
|---|---|---|---|---|
| 1. Entity `NguoiHienMau` & Quản lý hồ sơ cá nhân | 15% | 78% | **11.70%** | Đạt 100% cấu trúc Entity, PK/FK 1–0..1 với `TaiKhoan`, xem/cập nhật hồ sơ và chống IDOR. Trừ điểm do lỗi đồng bộ Email không kiểm tra trùng, lặp thông báo lỗi, chưa chặn khi `TrangThai != "Hoạt động"` và hardcode số liệu trên `HoSoCaNhan.cshtml`. |
| 2. Entity `PhieuDangKyHienMau` & 8 trạng thái nghiệp vụ | 10% | 85% | **8.50%** | Đạt 100% cấu trúc Entity, PK/FK và định nghĩa đủ 8 trạng thái. Trừ điểm do `TheoDoi.cshtml` chưa hiển thị đúng cho trạng thái `"Từ chối"`/`"Không đủ điều kiện hiến"` và `Module5Service` chưa cập nhật `NgayXuLy`. |
| 3. Chức năng nộp phiếu đăng ký (`DangKy/Create`) | 25% | 76% | **19.00%** | Đạt 12/14 yêu cầu (kiểm tra đăng nhập, chủ sở hữu, trạng thái hoạt động, đợt tồn tại, đợt đang mở, hạn đăng ký, cân nặng tối thiểu, phiếu trùng tuần tự, `NgayNop`, `TrangThai`, lưu DB). Trừ điểm do lỗi giá trị mặc định giả trên `Create.cshtml`, bỏ qua validation SĐT/Email khi cập nhật nhanh ở POST và chưa chống Race Condition. |
| 4. Chức năng hủy phiếu (`DangKy/Huy`) | 15% | 80% | **12.00%** | Đạt kiểm tra chủ sở hữu, CSRF, chặn hủy từ bước `"Chờ khám sàng lọc"` trở đi. Trừ điểm do cho phép hủy phiếu `"Đã xác nhận"` ngay cả khi đã có lịch khám sàng lọc đang hoạt động. |
| 5. Chức năng theo dõi phiếu (`DangKy/TheoDoi`) | 15% | 80% | **12.00%** | Đạt hiển thị danh sách phiếu, chi tiết đợt, đơn vị tổ chức, lịch khám, kết quả và bảo vệ dữ liệu riêng tư. Trừ điểm do ẩn mất `GhiChuXacNhan` khi phiếu bị `"Từ chối"` và lỗi UX khi tra cứu `code`/`id` không thuộc quyền sở hữu. |
| 6. Phân quyền, bảo mật & chống truy cập trái phép | 10% | 85% | **8.50%** | Đạt `[Authorize]`, `[ValidateAntiForgeryToken]`, lấy danh tính từ Claims và chống IDOR. Trừ điểm do chưa giới hạn Role cho cổng người hiến máu và chưa kiểm tra `TaiKhoan.TrangThai == "Hoạt động"` trên mỗi request. |
| 7. Database, Migration & Tích hợp liên module | 10% | 75.5% | **7.55%** | Đạt cấu hình DbContext, Migration, tích hợp Module 1, 2, 4, 5. Trừ điểm do link hỏng 404 trong `Views/Admin/NguoiHienMau.cshtml` và thiếu Unique Filtered Index chống trùng phiếu. |
| **TỔNG CỘNG** | **100%** | - | **79.25%** | |

### 3. Những chức năng nào đã hoàn thành và có bằng chứng?
1. **Thiết kế CSDL & Entities (`NguoiHienMau`, `PhieuDangKyHienMau`)**: Đầy đủ 100% thuộc tính theo yêu cầu, thiết lập đúng khóa chính, khóa ngoại, quan hệ 1–0..1 giữa `TaiKhoan` và `NguoiHienMau` (Unique Filtered Index `IX_NguoiHienMau_MaTaiKhoan`), quan hệ 1–n với `PhieuDangKyHienMau` (`DeleteBehavior.Restrict`).
2. **Xem và cập nhật hồ sơ cá nhân (`NguoiHienMauController.HoSoCaNhan`)**: Xác định đúng hồ sơ từ `ClaimTypes.NameIdentifier`, chống IDOR khi cập nhật, có kiểm tra độ tuổi (18–60), cân nặng (30–200 kg), SĐT (`^0[0-9]{9,10}$`), Email tại trang hồ sơ.
3. **Luồng kiểm tra điều kiện đợt hiến máu khi nộp phiếu (`DangKyController.Create`)**: Kiểm tra đầy đủ ở cả GET và POST các điều kiện: người hiến máu đang `"Hoạt động"`, đợt hiến máu tồn tại, trạng thái `"Đang mở đăng ký"`, nằm trong khoảng `NgayBatDauDangKy` – `HanDangKy`, cân nặng đạt `CanNangToiThieu` của đợt, câu hỏi tự sàng lọc sức khỏe, chặn phiếu trùng tuần tự, tự động gán `NgayNop = DateTime.Now` và `TrangThai = "Chờ xác nhận"`.
4. **Luồng hủy phiếu cơ bản (`DangKyController.Huy`)**: Có `[HttpPost]`, `[ValidateAntiForgeryToken]`, chặn hủy phiếu của người khác (`Forbid()`), chặn hủy phiếu từ giai đoạn `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`, `"Hiến máu thành công"`, `"Không đủ điều kiện hiến"`, `"Từ chối"`, `"Đã hủy"`.
5. **Luồng theo dõi phiếu (`DangKyController.TheoDoi`)**: Lọc danh sách phiếu theo người đăng nhập, hiển thị thông tin đợt, đơn vị tổ chức, lịch khám sàng lọc và kết quả hiến máu; không làm lộ dữ liệu phiếu của người khác khi thay đổi `id` hoặc `code` trên URL.

### 4. Những chức năng nào chưa hoàn thành?
1. Chống tạo phiếu trùng khi có 2 request POST đồng thời (Race Condition / Unique Filtered Index).
2. Kiểm tra điều kiện *"chưa có lịch khám sàng lọc"* trước khi cho phép hủy phiếu ở trạng thái `"Đã xác nhận"`.
3. Hiển thị lý do từ chối (`GhiChuXacNhan`) trên trang `TheoDoi.cshtml` khi phiếu bị `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`.
4. Chức năng dành cho Admin để khóa / tạm hoãn trạng thái hoạt động của người hiến máu (`NguoiHienMau.TrangThai`).
5. Kiểm tra khoảng cách tối thiểu 84 ngày kể từ lần hiến máu thành công gần nhất khi nộp phiếu mới và tính động tiến trình 84 ngày trên `HoSoCaNhan.cshtml`.

### 5. Những lỗi nào cần ưu tiên sửa trước?
1. **[HIGH - Lỗi số 1]**: Xóa các giá trị mặc định giả (`"0988 123 456"`, `64 kg`, `"sinhvien.uneti@gmail.com"`) trong `Views/DangKy/Create.cshtml` và bổ sung validation định dạng SĐT, Email, Cân nặng, Độ tuổi ở server trong `DangKyController.Create [POST]`.
2. **[HIGH - Lỗi số 2]**: Bổ sung điều kiện chặn người hiến máu tự hủy phiếu trong `DangKyController.Huy` nếu phiếu đã có lịch khám sàng lọc đang hoạt động.
3. **[HIGH - Lỗi số 3]**: Bổ sung Unique Filtered Index hoặc Transaction chống tạo phiếu trùng khi gửi 2 request đồng thời.
4. **[MEDIUM - Lỗi số 4, 5, 8]**: Kiểm tra trùng Email khi đồng bộ từ `HoSoCaNhan` sang `TaiKhoan`; hiển thị `GhiChuXacNhan` khi phiếu bị `"Từ chối"` trên `TheoDoi.cshtml`; sửa link hỏng `asp-action="PhieuDangKy"` trong `Views/Admin/NguoiHienMau.cshtml`.

### 6. Module 3 đã đủ điều kiện để đưa vào giai đoạn phân tích thiết kế và viết báo cáo chưa?
- **ĐÃ ĐỦ ĐIỀU KIỆN** để lập tài liệu Phân tích thiết kế hệ thống (Use Case, Biểu đồ lớp, Biểu đồ tuần tự, Biểu đồ trạng thái, Đặc tả CSDL), vì toàn bộ cấu trúc Entity, quan hệ CSDL, Controller/Action và các luồng nghiệp vụ chính đều đã hiện hữu rõ ràng trong mã nguồn. Tuy nhiên khi viết báo cáo cần phân biệt minh bạch các điểm đã triển khai hoàn chỉnh và các điểm đang hoàn thiện theo Phần IX dưới đây.

### 7. Có thể mô tả những luồng nghiệp vụ nào dựa trên code hiện tại?
Có thể mô tả đầy đủ 4 luồng nghiệp vụ chính từ code thực tế:
1. Luồng xem và cập nhật hồ sơ cá nhân người hiến máu (`NguoiHienMauController.HoSoCaNhan`).
2. Luồng nộp phiếu đăng ký hiến máu theo đợt (`DangKyController.Create`).
3. Luồng theo dõi tiến trình xử lý phiếu đăng ký, lịch khám sàng lọc và kết quả hiến máu (`DangKyController.TheoDoi`).
4. Luồng hủy phiếu đăng ký hiến máu của người hiến máu (`DangKyController.Huy`) và luồng xét duyệt/từ chối phiếu của nhân viên y tế (`AdminController.XacNhanPhieu`, `TuChoiPhieu`).

### 8. Những chức năng nào chỉ nên mô tả là thiết kế đề xuất vì code chưa triển khai đầy đủ?
1. Cơ chế khóa/tạm hoãn trạng thái hồ sơ người hiến máu (`NguoiHienMau.TrangThai`) từ phía quản trị viên.
2. Quy tắc tự động kiểm tra khoảng cách tối thiểu 84 ngày giữa hai lần hiến máu khi nộp phiếu đăng ký mới.
3. Cơ chế khóa đồng thời (Unique Filtered Index / Serializable Transaction) chống nộp phiếu trùng khi gửi request song song.
4. Quản lý kết quả xét nghiệm máu chi tiết theo từng chỉ số (HIV, HBV, HCV, Giang mai, Sốt rét) trong CSDL (hiện tại trên `HoSoCaNhan.cshtml` đang là HTML tĩnh, chưa có các cột này trong bảng `KetQuaHienMau`).

### 9. Có cần kiểm thử bổ sung trước khi nghiệm thu không?
- **CẦN THIẾT**. Trước khi nghiệm thu chính thức, cần thực hiện:
  1. Khởi chạy ứng dụng kết nối trực tiếp với SQL Server LocalDB để thực thi kiểm thử động (Runtime End-to-End) cho toàn bộ các trường hợp `TC01`–`TC26` đang ở trạng thái `BLOCKED (Runtime DB)`.
  2. Kiểm thử hồi quy (Regression Test) sau khi khắc phục 10 lỗi/mục thiếu đã nêu tại Phần V.

---

## PHẦN VIII. KẾ HOẠCH HOÀN THIỆN

### Đợt 1: Lỗi bảo mật và phân quyền
1. Kiểm tra trạng thái `TaiKhoan.TrangThai == "Hoạt động"` và giới hạn vai trò phù hợp trên `NguoiHienMauController` và `DangKyController` để ngăn tài khoản bị khóa vẫn thao tác bằng cookie cũ và ngăn tài khoản `Admin`/`Medical Staff` vô tình tự sinh hồ sơ `NguoiHienMau`.
2. Bổ sung kiểm tra trùng Email trong bảng `TaiKhoans` trước khi đồng bộ Email từ `NguoiHienMauController.HoSoCaNhan [POST]` sang `TaiKhoan`.

### Đợt 2: Điều kiện nghiệp vụ khi nộp phiếu
1. Loại bỏ các giá trị mặc định giả (`"0988 123 456"`, `"sinhvien.uneti@gmail.com"`, `64`) trên `Views/DangKy/Create.cshtml` và gắn thuộc tính `name` đầy đủ cho các trường thông tin hồ sơ.
2. Áp dụng kiểm tra định dạng SĐT (`^0[0-9]{9,10}$`), Email, khoảng cân nặng (`30–200 kg`), độ tuổi (`18–60`) và độ dài chuỗi (`StringLength`) trong `DangKyController.Create [POST]`.
3. Bổ sung Unique Filtered Index cho `(MaNguoiHien, MaDot)` đối với các phiếu đang hiệu lực (`TrangThai NOT IN (N'Đã hủy', N'Từ chối')`) và bắt `DbUpdateException` để xử lý triệt để Race Condition (`TC26`).

### Đợt 3: Điều kiện hủy phiếu và kiểm soát trạng thái
1. Trong `DangKyController.Huy [POST]` và `Views/DangKy/TheoDoi.cshtml`: Bổ sung điều kiện chặn người hiến máu tự hủy phiếu ở trạng thái `"Đã xác nhận"` nếu phiếu đã có lịch khám sàng lọc đang hoạt động (`TrangThai != "Đã hủy"`).
2. Trong `Services/Module5Service.cs`: Bổ sung cập nhật `phieu.NgayXuLy = DateTime.Now` ở tất cả các phương thức chuyển đổi trạng thái phiếu đăng ký.

### Đợt 4: Theo dõi phiếu và dữ liệu liên quan
1. Trong `Views/DangKy/TheoDoi.cshtml`: Bổ sung giao diện hiển thị lý do từ chối (`Model.GhiChuXacNhan`) và ngày xử lý (`Model.NgayXuLy`) khi phiếu ở trạng thái `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`; xóa giá trị fallback giả (`"O Rh(+)"`, `64 kg`).
2. Trong `DangKyController.TheoDoi`: Xử lý thông báo lỗi nhất quán khi tra cứu `id` hoặc `code` không tồn tại / không thuộc quyền sở hữu.
3. Trong `Views/Admin/NguoiHienMau.cshtml`: Sửa link `asp-action="PhieuDangKy"` thành `asp-action="PhieuDangKyHienMau" asp-route-tenNguoiHien="@donor.HoTen"`.
4. Trong `Views/NguoiHienMau/HoSoCaNhan.cshtml`: Sửa công thức đếm số lần hiến máu chỉ tính phiếu `"Hiến máu thành công"` và tính động thời gian phục hồi 84 ngày.

### Đợt 5: Validation, xử lý lỗi và kiểm thử đơn vị
1. Khắc phục hiện tượng lặp thông báo lỗi validation trong `NguoiHienMauController.HoSoCaNhan [POST]`.
2. Xử lý trường hợp gọi trực tiếp `POST /NguoiHienMau/HoSoCaNhan` hoặc `POST /DangKy/Create` khi tài khoản chưa có bản ghi `NguoiHienMau`.

### Đợt 6: Kiểm thử tích hợp và nghiệm thu
1. Chạy thực nghiệm toàn bộ 26 kịch bản `TC01`–`TC26` trên môi trường LocalDB thực tế.
2. Kiểm tra luồng tích hợp xuyên suốt từ Module 1 (Đăng ký/Đăng nhập) → Module 2 (Chọn đợt hiến máu) → Module 3 (Nộp phiếu/Theo dõi/Hủy phiếu) → Module 4 (Duyệt phiếu) → Module 5 (Khám sàng lọc & Ghi kết quả).

---

## PHẦN IX. THÔNG TIN PHỤC VỤ PHÂN TÍCH THIẾT KẾ HỆ THỐNG

### 1. Tác nhân tham gia (Actors)
1. **Người hiến máu (`Donor` / `"Người hiến máu"`)**: Xem và cập nhật hồ sơ cá nhân, đăng ký hiến máu theo đợt, theo dõi tiến trình xử lý phiếu, xem lịch khám sàng lọc & kết quả hiến máu, chủ động hủy phiếu khi đủ điều kiện.
2. **Nhân viên y tế (`Medical Staff` / `"NhanVienYTe"` / `"Nhân viên y tế"`) & Quản trị viên (`Admin`)**: Xem danh sách người hiến máu, xem danh sách và chi tiết phiếu đăng ký, xác nhận hoặc từ chối phiếu đăng ký, xếp lịch khám sàng lọc và cập nhật kết quả hiến máu.

### 2. Các Entity và Quan hệ dữ liệu (Database Schema đã xác minh)
- `TaiKhoan` (`1`) ─── (`0..1`) `NguoiHienMau`: Liên kết qua khóa ngoại `NguoiHienMau.MaTaiKhoan` (`int?`, Unique Filtered Index `[MaTaiKhoan] IS NOT NULL`, `OnDelete: SetNull`).
- `NguoiHienMau` (`1`) ─── (`0..n`) `PhieuDangKyHienMau`: Liên kết qua khóa ngoại `PhieuDangKyHienMau.MaNguoiHien` (`int`, `OnDelete: Restrict`).
- `DonViToChuc` (`1`) ─── (`0..n`) `DotHienMau`: Liên kết qua khóa ngoại `DotHienMau.MaDonViToChuc` (`int`, `OnDelete: Restrict`).
- `DotHienMau` (`1`) ─── (`0..n`) `PhieuDangKyHienMau`: Liên kết qua khóa ngoại `PhieuDangKyHienMau.MaDot` (`int`, `OnDelete: Restrict`).
- `PhieuDangKyHienMau` (`1`) ─── (`0..n`) `LichKhamSangLoc`: Liên kết qua khóa ngoại `LichKhamSangLoc.MaPhieu` (`int`, `OnDelete: Cascade`).
- `PhieuDangKyHienMau` (`1`) ─── (`0..1`) `KetQuaHienMau`: Liên kết qua khóa ngoại `KetQuaHienMau.MaPhieu` (`int`, Unique Index `IX_KetQuaHienMau_MaPhieu`, `OnDelete: Cascade`).

### 3. Bảng tổng hợp Controller / Action xử lý từng chức năng

| STT | Controller | Action | HTTP Method | Route / Tham số chính | Chức năng thực tế |
|---|---|---|---|---|---|
| 1 | `NguoiHienMauController` | `Index` | `GET` | `/NguoiHienMau` | Chuyển hướng (`RedirectToAction`) sang `HoSoCaNhan`. |
| 2 | `NguoiHienMauController` | `HoSoCaNhan` | `GET` | `/NguoiHienMau/HoSoCaNhan` | Xem thông tin hồ sơ cá nhân và lịch sử các phiếu đăng ký hiến máu của người dùng đang đăng nhập (tự động tạo hồ sơ nếu chưa có). |
| 3 | `NguoiHienMauController` | `HoSoCaNhan` | `POST` | `/NguoiHienMau/HoSoCaNhan` (`NguoiHienMau model`) | Kiểm tra hợp lệ và cập nhật thông tin hồ sơ cá nhân của người dùng đang đăng nhập; đồng bộ `HoTen`, `Email` sang `TaiKhoan`. |
| 4 | `DangKyController` | `Create` | `GET` | `/DangKy/Create?maDot={int}` | Kiểm tra điều kiện đợt hiến máu, kiểm tra phiếu trùng và hiển thị biểu mẫu nộp phiếu đăng ký hiến máu. |
| 5 | `DangKyController` | `Create` | `POST` | `/DangKy/Create` (`maDot`, thông tin hồ sơ, `health_q1`, `health_q2`, `lyDoHienMau`, `ghiChu`) | Kiểm tra điều kiện đợt hiến máu, cân nặng tối thiểu, câu hỏi tự sàng lọc, phiếu trùng; cập nhật nhanh thông tin hồ sơ và tạo bản ghi `PhieuDangKyHienMau` ở trạng thái `"Chờ xác nhận"`. |
| 6 | `DangKyController` | `TheoDoi` | `GET` | `/DangKy/TheoDoi/{id?}?code={string?}` | Hiển thị danh sách phiếu đã nộp của người dùng và chi tiết tiến trình 5 bước, lịch khám sàng lọc, kết quả hiến máu của phiếu đang chọn. |
| 7 | `DangKyController` | `Index` | `GET` | `/DangKy` | Chuyển hướng (`RedirectToAction`) sang `TheoDoi`. |
| 8 | `DangKyController` | `Huy` | `POST` | `/DangKy/Huy` (`int maPhieu, string? lyDoHuy`) | Kiểm tra quyền sở hữu và trạng thái phiếu; chuyển trạng thái phiếu sang `"Đã hủy"` và lưu `NgayXuLy`, `GhiChuXacNhan`. |

### 4. Luồng chính, điều kiện rẽ nhánh và trường hợp lỗi của các chức năng cốt lõi

#### 4.1. Luồng nộp phiếu đăng ký hiến máu (`DangKyController.Create`)
1. **Bước 1**: Người dùng đã đăng nhập chọn một đợt hiến máu từ `/DotHienMau` hoặc `/DotHienMau/Details/{id}` và truy cập `GET /DangKy/Create?maDot={id}`.
2. **Bước 2 (Điều kiện rẽ nhánh ở GET & POST)**:
   - Nếu chưa đăng nhập → Chuyển hướng `/Account/Login`.
   - Nếu `nguoiHienMau.TrangThai != "Hoạt động"` → Báo lỗi và chuyển về `/DotHienMau`.
   - Nếu `dotHienMau == null` → Báo lỗi đợt không tồn tại và chuyển về `/DotHienMau`.
   - Nếu `dotHienMau.TrangThai != "Đang mở đăng ký"` → Báo lỗi đợt không mở đăng ký và chuyển về `/DotHienMau/Details/{maDot}`.
   - Nếu `DateTime.Now.Date < NgayBatDauDangKy.Date` hoặc `> HanDangKy.Date` → Báo lỗi ngoài thời gian đăng ký và chuyển về `/DotHienMau/Details/{maDot}`.
   - Nếu đã tồn tại phiếu của người đó trong cùng đợt có `TrangThai NOT IN ("Đã hủy", "Từ chối")` → Báo lỗi phiếu trùng và chuyển sang `/DangKy/TheoDoi/{existingPhieu.MaPhieu}`.
3. **Bước 3 (Kiểm tra dữ liệu biểu mẫu khi POST)**:
   - Nếu thiếu `HoTen`, `SoDienThoai`, `CanNang` hoặc `CanNang <= 0` → Báo lỗi và hiển thị lại biểu mẫu.
   - Nếu `CanNang < dotHienMau.CanNangToiThieu` → Báo lỗi không đạt cân nặng tối thiểu của đợt (`CanNangToiThieu` kg).
   - Nếu câu hỏi tự sàng lọc `health_q1 != "none"` hoặc `health_q2 != "none"` → Báo lỗi phát hiện yếu tố nguy cơ sức khỏe tạm thời.
4. **Bước 4 (Lưu CSDL)**: Cập nhật thông tin hồ sơ `NguoiHienMau`, tạo mới `PhieuDangKyHienMau` với `NgayNop = DateTime.Now`, `TrangThai = "Chờ xác nhận"`, lưu DB và chuyển hướng tới `/DangKy/TheoDoi/{phieu.MaPhieu}`.

#### 4.2. Luồng hủy phiếu đăng ký hiến máu (`DangKyController.Huy`)
1. **Bước 1**: Tại trang `/DangKy/TheoDoi/{id}`, người dùng bấm nút `"Hủy phiếu đăng ký"`, chọn/nhập lý do hủy trong Modal và gửi `POST /DangKy/Huy`.
2. **Bước 2 (Điều kiện rẽ nhánh & Trường hợp lỗi)**:
   - Nếu không tìm thấy `maPhieu` trong DB → Báo lỗi và chuyển về `/DangKy/TheoDoi`.
   - Nếu người dùng không phải chủ sở hữu phiếu (`phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien`) và không phải nhân viên y tế/Admin → Trả về HTTP 403 (`Forbid()`).
   - Nếu `phieu.TrangThai` là `"Chờ khám sàng lọc"` hoặc `"Đã khám sàng lọc"` → Từ chối hủy (báo lỗi phiếu đã vào giai đoạn khám sàng lọc).
   - Nếu `phieu.TrangThai` là `"Hiến máu thành công"` hoặc `"Không đủ điều kiện hiến"` → Từ chối hủy (báo lỗi phiếu đã có kết quả cuối cùng).
   - Nếu `phieu.TrangThai` là `"Đã hủy"` hoặc `"Từ chối"` → Từ chối hủy (báo lỗi phiếu đã đóng trước đó).
3. **Bước 3 (Lưu CSDL)**: Khi `phieu.TrangThai` thuộc `{"Chờ xác nhận", "Đã xác nhận"}`, cập nhật `phieu.TrangThai = "Đã hủy"`, `phieu.NgayXuLy = DateTime.Now`, lưu `GhiChuXacNhan` và gọi `SaveChangesAsync()`.

#### 4.3. Luồng theo dõi phiếu đăng ký hiến máu (`DangKyController.TheoDoi`)
1. **Bước 1**: Người dùng truy cập `GET /DangKy/TheoDoi/{id?}?code={string?}`.
2. **Bước 2**: Hệ thống lấy `maTaiKhoan` từ Claims, truy vấn danh sách `PhieuDangKyHienMaus` của riêng `nguoiHienMau.MaNguoiHien` kèm `DotHienMau.DonViToChuc`, `LichKhamSangLocs`, `KetQuaHienMau`, sắp xếp theo `NgayNop` giảm dần.
3. **Bước 3**: Xác định phiếu đang chọn (`selectedPhieu`) từ `id`, hoặc từ `code` (`DKHM-xxxxx`), hoặc mặc định lấy phiếu mới nhất (`FirstOrDefault()`), sau đó render giao diện 2 cột (danh sách phiếu bên trái, chi tiết tiến trình 5 bước bên phải).

---

### 5. Phân loại trạng thái triển khai phục vụ viết báo cáo Phân tích thiết kế

#### A. Chức năng ĐÃ TRIỂN KHAI hoàn chỉnh trong code
1. Quản lý thực thể `NguoiHienMau` (13 thuộc tính) và `PhieuDangKyHienMau` (9 thuộc tính) cùng đầy đủ khóa chính, khóa ngoại và Migration EF Core.
2. Quan hệ 1–0..1 giữa `TaiKhoan` và `NguoiHienMau` thông qua Unique Filtered Index `IX_NguoiHienMau_MaTaiKhoan`.
3. Tự động khởi tạo hồ sơ `NguoiHienMau` khi đăng ký tài khoản mới (`AccountController.Register`).
4. Xem và cập nhật hồ sơ cá nhân của người hiến máu (`NguoiHienMauController.HoSoCaNhan`) với cơ chế chống IDOR ở phía server.
5. Kiểm tra điều kiện đợt hiến máu (tồn tại, đang mở đăng ký, trong thời hạn đăng ký, cân nặng tối thiểu, câu hỏi tự sàng lọc, chặn trùng phiếu tuần tự) khi nộp phiếu (`DangKyController.Create`).
6. Hủy phiếu đăng ký hiến máu (`DangKyController.Huy`) kèm bảo vệ CSRF, kiểm tra quyền sở hữu và chặn hủy đối với các phiếu đã vào giai đoạn khám sàng lọc hoặc đã có kết quả.
7. Theo dõi danh sách phiếu, chi tiết đợt hiến máu, đơn vị tổ chức, lịch khám sàng lọc và kết quả hiến máu (`DangKyController.TheoDoi`).
8. Quản lý 8 trạng thái nghiệp vụ của phiếu đăng ký xuyên suốt giữa Module 3, Module 4 và Module 5.

#### B. Chức năng ĐÃ TRIỂN KHAI MỘT PHẦN (Cần hoàn thiện thêm)
1. **Validation khi cập nhật nhanh hồ sơ trên trang nộp phiếu (`DangKyController.Create`)**: Đã kiểm tra khác rỗng và cân nặng tối thiểu, nhưng chưa kiểm tra Regex SĐT, định dạng Email, giới hạn trên của cân nặng và đang điền sẵn giá trị mặc định giả trên `Views/DangKy/Create.cshtml`.
2. **Kiểm tra điều kiện hủy phiếu ở trạng thái `"Đã xác nhận"`**: Đã kiểm tra trạng thái phiếu nhưng chưa chặn trường hợp phiếu `"Đã xác nhận"` đã được xếp lịch khám sàng lọc.
3. **Hiển thị trạng thái và ghi chú xử lý trên trang `TheoDoi.cshtml`**: Đã hiển thị tốt cho các bước thành công và trạng thái `"Đã hủy"`, nhưng chưa hiển thị `GhiChuXacNhan` khi phiếu bị `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`.
4. **Thống kê cá nhân trên trang `HoSoCaNhan.cshtml`**: Đã có giao diện thống kê số lần hiến, lượng máu và thời gian phục hồi 84 ngày, nhưng công thức đếm đang tính gộp cả phiếu chưa hoàn thành và thanh tiến trình 84 ngày đang hiển thị tĩnh.

#### C. Chức năng MỚI LÀ ĐỀ XUẤT THIẾT KẾ (Chưa triển khai trong code hiện tại)
1. **Chức năng khóa / tạm hoãn trạng thái hoạt động của người hiến máu (`NguoiHienMau.TrangThai`) dành cho Admin**: Hiện tại cột `TrangThai` đã tồn tại và được kiểm tra khi nộp phiếu, nhưng chưa có Action/UI trên trang Admin để chuyển trạng thái này.
2. **Kiểm tra tự động khoảng cách 84 ngày giữa 2 lần hiến máu khi nộp phiếu mới**: Hiện mới chỉ có logic hiển thị trên trang `Admin/NguoiHienMau.cshtml`, chưa chặn trong `DangKyController.Create`.
3. **Cơ chế chống Race Condition ở tầng CSDL (Unique Filtered Index trên `(MaNguoiHien, MaDot)`)**: Cần bổ sung để ngăn chặn tuyệt đối việc tạo 2 phiếu trùng khi gửi 2 request song song.
4. **Lưu trữ chi tiết từng chỉ số xét nghiệm sàng lọc máu (HIV, HBV, HCV, Giang mai, Sốt rét) trong CSDL**: Hiện tại bảng `KetQuaHienMau` chỉ lưu `KetLuan`, `LuongMauHien`, `NhomMauXacNhan`, `GhiChu`; bảng 5 chỉ số xét nghiệm trên `HoSoCaNhan.cshtml` mới chỉ là giao diện tĩnh minh họa.
