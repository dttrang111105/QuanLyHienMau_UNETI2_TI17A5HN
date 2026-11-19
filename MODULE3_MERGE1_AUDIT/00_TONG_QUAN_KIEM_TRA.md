# 00. TỔNG QUAN KIỂM TRA & KIỂM TOÁN MODULE 3 (NHÁNH `merge1`)

- **Dự án:** Hệ thống Quản lý Hiến máu Tình nguyện UNETI (`QuanLyHienMau_UNETI2_TI17A5HN`)
- **Nền tảng kỹ thuật:** ASP.NET Core MVC (.NET 10.0), Entity Framework Core 10.0.5, SQL Server LocalDB
- **Phạm vi kiểm toán:** Toàn bộ **Module 3 – Quản lý Người hiến máu & Phiếu đăng ký hiến máu** sau khi hợp nhất tại nhánh `origin/merge1`
- **Nguyên tắc thực hiện:** Chỉ kiểm toán và kiểm thử thực nghiệm trên Database thử nghiệm cô lập (`QuanLyHienMau_UNETI_AuditM3`); **tuyệt đối không sửa source code, không sửa Database thật, không tạo commit và không push**.

---

## 1. Kết quả cập nhật mã nguồn từ GitHub (`origin/merge1`)

| Hạng mục | Thông tin chi tiết |
| :--- | :--- |
| **Remote URL** | `https://github.com/dttrang111105/QuanLyHienMau_UNETI2_TI17A5HN.git` (`origin`) |
| **Nhánh trước khi cập nhật** | `DamThuTrang_23103100243` (commit `2434c70631c534c08888b7b3069dd7a0b6f34f03` – *"chuyển DbCotext sang Repository"*) |
| **Nhánh mục tiêu trên Remote** | `origin/merge1` |
| **Commit mới nhất của `origin/merge1`** | `0b0d7d6926094f398cdbfd88e18658570650be63` (*"Merge"*, Author: `vutrunganh27012005`, Date: `2026-04-16 09:05:31 +0700`) |
| **Quan hệ lịch sử Git** | Commit `2434c70` của nhánh `DamThuTrang_23103100243` là tổ tiên trực tiếp (ancestor) của `origin/merge1` (đã được merge vào `merge1` tại commit `f5a20cf`). |
| **Trạng thái cập nhật về máy** | Đã fetch toàn bộ `origin` và chuyển sang nhánh `merge1` đồng bộ 100% với `origin/merge1` (`HEAD = 0b0d7d6926094f398cdbfd88e18658570650be63`). |
| **Bảo toàn dữ liệu làm việc** | Không có xung đột (conflict). Toàn bộ các tài liệu chưa commit trong workspace (`MODULE3_*.md`, `MODULE3_DEFENSE_PREPARATION/`) được giữ nguyên vẹn 100%. |

### Lịch sử hợp nhất gần nhất trên `origin/merge1`
1. `0b0d7d6` – *Merge* (Hợp nhất nhánh `main` và giải quyết đăng ký DI trong `Program.cs`).
2. `f5a20cf` – *Merge remote-tracking branch 'origin/DamThuTrang_23103100243' into merge1* (Hợp nhất Module 3 sử dụng Repository Pattern).
3. `627126d` – *chuyển các chức năng từ dùng trực tiếp DbContext sang thông qua Repository* (Hợp nhất Module 4).
4. `ba1ac5e` – *Chuyển trực tiếp DbContext sang thông qua Repository cho TaiKhoan* (Hợp nhất Module 1).

---

## 2. Kết quả Build & Kiểm tra Kỹ thuật

- **Lệnh kiểm tra:** `dotnet build QuanLyHienMau_UNETI2_TI17A5HN.slnx --no-incremental`
- **Kết quả biên dịch (Compile):** **Thành công (`Build succeeded`)**
  - **Số lỗi biên dịch (Compile Errors):** `0 Error(s)`
  - **Số cảnh báo C# (Compiler Warnings):** `0 Warning(s)` (chỉ có 2 cảnh báo `NU1900` của NuGet khi chạy trong môi trường cô lập mạng).
- **Khả năng khởi chạy & Khởi tạo DB tự động (`Clone & Run`):**
  - Khi chạy ứng dụng với chuỗi kết nối trỏ tới một Database mới hoàn toàn (`QuanLyHienMau_UNETI_AuditM3`), hệ thống tự động thực thi `context.Database.Migrate()`, `DbInitializer.Initialize(context)` và `DbInitializer.SeedTestData(context)` trong `Program.cs` mà không gặp bất kỳ lỗi runtime nào.
  - Tạo đầy đủ 8 bảng (`__EFMigrationsHistory`, `DonViToChuc`, `DotHienMau`, `KetQuaHienMau`, `LichKhamSangLoc`, `NguoiHienMau`, `PhieuDangKyHienMau`, `TaiKhoan`) cùng dữ liệu mẫu (3 tài khoản, 9 hồ sơ người hiến máu, 5 đợt hiến máu, 8 phiếu đăng ký).
- **Kiểm tra Unit Test / Integration Test có sẵn trong Solution:**
  - Trong Solution hiện chỉ có duy nhất 1 project web `QuanLyHienMau_UNETI2_TI17A5HN.csproj`, chưa có project Unit Test (`xUnit`/`NUnit`/`MSTest`) đi kèm.
  - Do đó, quá trình kiểm toán đã kết hợp **kiểm tra tĩnh toàn bộ source code (Static Code Audit)** và **kiểm thử tích hợp tự động qua HTTP & SQL Server (Live HTTP Integration Testing)** trên Database thử nghiệm riêng (`QuanLyHienMau_UNETI_AuditM3`), sau đó dọn dẹp sạch Database thử nghiệm.

---

## 3. Tổng hợp Kết quả Kiểm thử 12 Nghiệp vụ Module 3

| STT | Nghiệp vụ Module 3 | Trạng thái triển khai | Kết quả kiểm thử | Đánh giá chi tiết |
| :---: | :--- | :---: | :---: | :--- |
| 1 | Quản lý hồ sơ cá nhân người hiến máu | Đã triển khai đầy đủ | **PASS** | Tự động tạo hồ sơ `NguoiHienMau` nếu tài khoản `Donor` chưa có hồ sơ; hiển thị chính xác dữ liệu từ DB và thẻ hiến máu điện tử. |
| 2 | Cập nhật thông tin người hiến máu | Đã triển khai đầy đủ | **PASS (Có lưu ý nhỏ)** | Cập nhật theo cơ chế whitelist, đồng bộ `HoTen` & `Email` sang bảng `TaiKhoan`. Có kiểm tra rỗng, tuổi 18–60, cân nặng 30–200kg, SĐT, Email trùng. Lưu ý nhỏ: server chỉ kiểm tra độ dài `GioiTinh` (<=10) và `NhomMau` (<=20), chưa chặn giá trị ngoài danh mục nếu gửi POST trực tiếp (`BUG-M3-05`). |
| 3 | Đăng ký tham gia hiến máu | Đã triển khai đầy đủ | **PASS (Có lỗi nhóm máu Rh-)** | Luồng tạo phiếu hoạt động tốt, dùng `SemaphoreSlim` + `IsolationLevel.Serializable` chống race condition. **Lỗi phát hiện (`BUG-M3-01` - High):** Form `Views/DangKy/Create.cshtml` chỉ có 4 radio `O Rh(+)`, `A Rh(+)`, `B Rh(+)`, `AB Rh(+)` và dùng `.Contains("O")` nên người hiến có nhóm máu hiếm Rh(-) (`O-`, `A-`, `B-`, `AB-`) bị tự động chọn nhầm thành `Rh(+)` và bị ghi đè mất nhóm máu Rh(-) trong hồ sơ khi nộp phiếu. |
| 4 | Kiểm tra điều kiện đăng ký hiến máu | Đã triển khai đầy đủ | **PASS** | Chặn chính xác khi đợt hiến máu chưa mở, tạm dừng, đã kết thúc hoặc ngoài khoảng `NgayBatDauDangKy` – `HanDangKy`. |
| 5 | Kiểm tra độ tuổi, cân nặng và tình trạng sức khỏe | Đã triển khai đầy đủ | **PASS (Có lưu ý nhỏ)** | Chặn chính xác tuổi < 18 hoặc > 60, cân nặng < `CanNangToiThieu` của đợt (hoặc ngoài 30–200kg), và câu hỏi sàng lọc `health_q1 = yes` / `health_q2 = yes`. Lưu ý nhỏ: nếu POST thiếu `ngaySinh`/`canNang` (`null`), controller fallback về giá trị cũ trong DB (`BUG-M3-06`). |
| 6 | Kiểm tra thời gian tối thiểu 84 ngày giữa hai lần hiến máu thành công | Đã triển khai đầy đủ ở cổng Donor | **PASS ở cổng Donor / LỖI ở trang Admin** | Tại `DangKyController` và `HoSoCaNhan.cshtml`: tính đúng 84 ngày dựa trên `TrangThai == "Hiến máu thành công"` và ngày thực tế `KetQuaHienMau.NgayCapNhat` (phiếu hủy/từ chối/không đạt sàng lọc không bị tính). Tuy nhiên tại `AdminController.NguoiHienMau` (`BUG-M3-02` - High) vẫn dùng điều kiện cũ. |
| 7 | Kiểm tra đăng ký trùng đợt hiến máu | Đã triển khai đầy đủ | **PASS** | Chặn đăng ký trùng đợt khi đang có phiếu hoạt động; cho phép đăng ký lại nếu phiếu cũ của đợt đó đã ở trạng thái `"Đã hủy"` hoặc `"Từ chối"`. |
| 8 | Theo dõi trạng thái phiếu đăng ký | Đã triển khai đầy đủ | **PASS** | Hiển thị đúng danh sách phiếu của người đang đăng nhập, mã phiếu `#DKHM-xxxxx`, tiến trình 4 bước, lịch khám sàng lọc và kết quả lâm sàng. Chặn tra cứu phiếu của người khác. |
| 9 | Hủy phiếu đăng ký hiến máu | Đã triển khai đầy đủ | **PASS** | Cho phép hủy khi `"Chờ xác nhận"` hoặc `"Đã xác nhận"` (chưa có lịch khám hoạt động / kết quả). Chặn hủy lại phiếu đã hủy, đã hoàn thành, đã có lịch khám hoặc phiếu của người khác (`403 Forbid`). |
| 10 | Quản lý danh sách người hiến máu phía Admin | Đã triển khai | **PARTIAL / CÓ LỖI LOGIC** | Hiển thị danh sách, phân trang, tìm kiếm, lọc theo nhóm máu ABO, Rh, trạng thái, mức đóng góp. Có lỗi tính toán số lần hiến/trạng thái chờ 84 ngày (`BUG-M3-02`), nút `"Thêm Hồ Sơ Mới"` trỏ nhầm sang `/DangKy/Create` (`BUG-M3-04`), và `AdminController.NguoiHienMau` vẫn dùng `_context` trực tiếp thay vì Repository (`BUG-M3-07`). |
| 11 | Khóa/mở khóa người hiến máu | Triển khai qua Module Quản lý Tài khoản (`/Admin/NhanSu`) | **PARTIAL** | Khi Admin khóa/mở khóa tài khoản tại `/Admin/NhanSu` (`POST /Admin/KhoaMoKhoaTaiKhoan`), hệ thống tự động đồng bộ `NguoiHienMau.TrangThai = "Bị khóa" / "Hoạt động"` và đăng xuất cưỡng bức phiên đăng nhập của người hiến máu. Tuy nhiên trên trang `/Admin/NguoiHienMau` chưa có nút khóa/mở khóa trực tiếp cho từng hồ sơ (đặc biệt với các hồ sơ mẫu không gắn `MaTaiKhoan`) (`BUG-M3-03`). |
| 12 | Thống kê người hiến máu và các chỉ số liên quan | Đã triển khai | **CÓ LỖI SAI LỆCH SỐ LIỆU** | Có 4 thẻ KPI trên `/Admin/NguoiHienMau` và thống kê cá nhân trên `/NguoiHienMau/HoSoCaNhan`. Tuy nhiên trên `/Admin/NguoiHienMau`, biểu thức `p.TrangThai == "Hiến máu thành công" \|\| p.KetQuaHienMau != null` khiến phiếu có kết quả `"Không đủ điều kiện hiến"` bị tính nhầm là đã hiến máu thành công (`BUG-M3-02`). |

---

## 4. Thống kê Lỗi phát hiện trong đợt Kiểm toán

Tổng cộng phát hiện **08 vấn đề/lỗi** (trong đó **0 Critical**, **02 High**, **03 Medium**, **03 Low**):

| Mã lỗi | Mức độ | Tên lỗi / Vấn đề | Nguồn gốc lỗi |
| :---: | :---: | :--- | :--- |
| **BUG-M3-01** | **High** | Form đăng ký hiến máu (`Views/DangKy/Create.cshtml`) thiếu tùy chọn nhóm máu `Rh(-)` và dùng `.Contains()` gây ghi đè mất nhóm máu hiếm `O-`, `A-`, `B-`, `AB-` trong hồ sơ người hiến máu thành `Rh(+)` | Tồn tại từ trước trong `Views/DangKy/Create.cshtml` & `DangKyController.cs` |
| **BUG-M3-02** | **High** | Trang Admin Quản lý người hiến máu (`AdminController.NguoiHienMau` & `Views/Admin/NguoiHienMau.cshtml`) tính nhầm phiếu `"Không đủ điều kiện hiến"` (có bản ghi `KetQuaHienMau`) thành lần hiến máu thành công và tính sai ngày chờ 84 ngày theo `NgayXuLy ?? NgayNop` | Tồn tại từ trước trong `AdminController.cs` & `Views/Admin/NguoiHienMau.cshtml` (chưa đồng bộ với logic mới của `DangKyController`) |
| **BUG-M3-03** | **Medium** | Trang `/Admin/NguoiHienMau` thiếu nút Khóa/Mở khóa trực tiếp hồ sơ người hiến máu (phải thao tác gián tiếp qua `/Admin/NhanSu`, không khóa được hồ sơ rời có `MaTaiKhoan == null`) | Tồn tại từ trước trong thiết kế UI `/Admin/NguoiHienMau` |
| **BUG-M3-04** | **Medium** | Nút `"Thêm Hồ Sơ Mới"` trên trang `/Admin/NguoiHienMau` điều hướng nhầm sang `/DangKy/Create` (form tự đăng ký hiến máu của người dùng) và link `"Xem các phiếu đăng ký"` lọc theo chuỗi `tenNguoiHien` thay vì `MaNguoiHien` | Tồn tại từ trước trong `Views/Admin/NguoiHienMau.cshtml` |
| **BUG-M3-05** | **Medium** | Không đồng nhất định dạng lưu trữ `NhomMau` (`O+` vs `O Rh(+)`) và biểu thức Regex `SoDienThoai` giữa Module 1 (`AccountController`) và Module 3 (`NguoiHienMauController`, `DangKyController`); thiếu kiểm tra danh mục `GioiTinh`/`NhomMau` phía Server | Phát sinh khi ghép nối giữa các Module & tồn tại từ trước |
| **BUG-M3-06** | **Low** | `POST /DangKy/Create` tự động fallback sang `nguoiHienMau.NgaySinh` và `nguoiHienMau.CanNang` cũ trong DB nếu client gửi giá trị rỗng (`null`), và `GetLatestOpenDotHienMauAsync` so sánh `DateTime` với `referenceDate.Date` (`00:00:00`) | Tồn tại từ trước trong `DangKyController.cs` & `PhieuDangKyRepository.cs` |
| **BUG-M3-07** | **Low** | Chưa đồng bộ kiến trúc Repository sau khi merge nhánh `merge1`: tồn tại song song 2 cặp interface `IRepository<T>` & `IPhieuDangKyRepository` ở 2 namespace khác nhau, và `AdminController.NguoiHienMau` vẫn truy vấn trực tiếp `_context` | Phát sinh từ quá trình hợp nhất nhánh `merge1` |
| **BUG-M3-08** | **Low** | Kiểm tra trùng lặp Email (`IsEmailTakenByOtherAccountAsync`) chỉ kiểm tra trên bảng `TaiKhoan`, chưa kiểm tra các bản ghi `NguoiHienMau` không gắn tài khoản và chưa có Unique Index cho `Email` ở tầng DB | Tồn tại từ trước trong `NguoiHienMauRepository.cs` & `ApplicationDbContext.cs` |
