# 03. BÁO CÁO CÁC CHỨC NĂNG CÒN THIẾU, CHƯA HOÀN THIỆN VÀ LỖI NGHIỆP VỤ

Báo cáo này phân loại toàn bộ các lỗi nghiệp vụ, lỗ hổng phân quyền, chức năng thiếu hoặc chưa hoàn thiện theo 4 mức độ ưu tiên: `Critical`, `High`, `Medium`, `Low`. Mỗi vấn đề đều kèm theo bằng chứng dòng code cụ thể và hướng khắc phục.

---

## I. Mức độ `CRITICAL` (Bắt buộc sửa ngay vì vi phạm trực tiếp yêu cầu chấm điểm của đề bài)

### `[CRIT-01]` Nhánh `AdminController.TaoLichKham` & `SuaLichKham` bỏ sót hoàn toàn kiểm tra trùng lịch người hiến, trùng lịch bác sĩ và thời gian quá khứ
- **Module liên quan:** Module 5 (`LichKhamSangLoc`)
- **Yêu cầu đề bài bị ảnh hưởng:** Mục 9.2 (*"Không lập lịch trong quá khứ; Không trùng lịch của cùng một người hiến máu; Không trùng lịch của cùng một bác sĩ hoặc người phụ trách sàng lọc"*).
- **Nguyên nhân gốc (Root Cause):**
  Project đang có 2 nơi xử lý lập lịch khám sàng lọc:
  1. [`LichKhamSangLocController.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/LichKhamSangLocController.cs) gọi [`KhamSangLocService.LuuLichAsync`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Services/KhamSangLocService.cs#L30-L86) — **đã kiểm tra đầy đủ** ngày quá khứ, trùng lịch người hiến (`CoLichTrungNguoiHienAsync`) và trùng lịch bác sĩ (`CoLichTrungBacSiAsync`).
  2. [`AdminController.TaoLichKham`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L434-L473) và [`AdminController.SuaLichKham`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L476-L505) — chỉ kiểm tra `thoiGianKetThuc <= thoiGianBatDau` rồi lưu thẳng vào Database!
  Nghiêm trọng hơn, menu `"Lịch khám sàng lọc"` trên [`_AdminLayout.cshtml`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/Shared/_AdminLayout.cshtml#L182) và form lập lịch trong [`Views/Admin/ChiTietPhieuDangKy.cshtml`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/ChiTietPhieuDangKy.cshtml) đều đang gọi vào `AdminController` thay vì `LichKhamSangLocController`!
- **Bằng chứng trong Code:**
  - [`AdminController.cs#L444-L470`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L444-L470): Chỉ kiểm tra `thoiGianKetThuc <= thoiGianBatDau` và `phieu.LichKhamSangLocs.Any(l => l.TrangThai == "Đã lên lịch")`, không hề kiểm tra `thoiGianBatDau < DateTime.Now`, không kiểm tra trùng lịch người hiến ở đợt khác, không kiểm tra trùng lịch bác sĩ.
- **Tác động:** Khi giảng viên test lập lịch khám trên giao diện Admin (màn hình mặc định khi bấm từ menu), hệ thống sẽ cho phép tạo lịch khám năm 2020 hoặc xếp 1 bác sĩ khám 2 người cùng giờ → **Mất điểm trực tiếp ở Mục 9.2 và Mục 21**.
- **Hướng khắc phục đề xuất:**
  - Tiêm `KhamSangLocService` vào `AdminController` để `TaoLichKham`, `SuaLichKham`, `HuyLichKham`, `HoanThanhKham`, `TaoKetQua`, `SuaKetQua` gọi qua `KhamSangLocService`, **HOẶC** đổi link trên `_AdminLayout.cshtml` sang `LichKhamSangLocController` và `KetQuaHienMauController`.

---

### `[CRIT-02]` Dữ liệu mẫu (`DbInitializer.cs`) thiếu nghiêm trọng so với số lượng tối thiểu bắt buộc tại Mục 16 Đề bài
- **Module liên quan:** Toàn bộ 5 Module (`Data/DbInitializer.cs`)
- **Yêu cầu đề bài bị ảnh hưởng:** Mục 16 (*Yêu cầu dữ liệu mẫu Seed Data*) và Mục 21 (*Các lỗi bị trừ điểm*).
- **Nguyên nhân gốc & Bằng chứng trong Code:**
  Đối chiếu trực tiếp [`DbInitializer.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Data/DbInitializer.cs) với Mục 16:
  | Thực thể | Yêu cầu tối thiểu (Mục 16) | Thực tế trong `DbInitializer.cs` | Đánh giá |
  |---|---:|---:|---|
  | `DonViToChuc` | **>= 05** đơn vị | **04** đơn vị (dòng 24–63) | **Thiếu 1 đơn vị** |
  | `DotHienMau` | **>= 15** đợt (đủ chia nhiều trang) | **05** đợt (dòng 77–156) | **Thiếu 10 đợt** (Với `PageSize = 6` hoặc `10`, 5 đợt chỉ có đúng 1 trang, không test được phân trang!) |
  | `TaiKhoan` (`Admin`) | **01 – 02** tài khoản | **01** tài khoản (`admin`) | Đạt |
  | `TaiKhoan` (`Medical Staff`) | **02 – 03** tài khoản | **01** tài khoản (`bacsi_nam`) | **Thiếu ít nhất 1 tài khoản NVYT** |
  | `NguoiHienMau` + `TaiKhoan` (`Donor`) | **>= 30** người hiến có tài khoản | **09** người hiến (chỉ **01** người có tài khoản `nguoihien_an`, 8 người còn lại `MaTaiKhoan = null`) | **Thiếu 21 người hiến & 29 tài khoản Donor** |
  | `PhieuDangKyHienMau` | **40 – 50** phiếu ở nhiều trạng thái | **08** phiếu (dòng 252–307) | **Thiếu 32 – 42 phiếu** |
  | `LichKhamSangLoc` | **15 – 20** lịch khám | **0** lịch khám | **Thiếu 100% (0 bản ghi)** |
  | `KetQuaHienMau` | Đủ cả `"Hiến máu thành công"` và `"Không đủ điều kiện hiến"` | **0** kết quả | **Thiếu 100% (0 bản ghi)** |
- **Tác động:** Khi giảng viên clone code về máy mới và chạy ứng dụng lần đầu, trang Phân trang Đợt hiến máu chỉ có 1 trang, trang Lịch khám sàng lọc trống rỗng (0 bản ghi), trang Kết quả hiến máu trống rỗng (0 bản ghi), trang Thống kê tỷ lệ hiến máu thành công hiển thị `0%`!
- **Hướng khắc phục đề xuất:** Cập nhật `DbInitializer.SeedAsync` để tạo đủ tối thiểu: 6 `DonViToChuc`, 16 `DotHienMau`, 2 `Admin`, 3 `Medical Staff`, 30 `NguoiHienMau` kèm `TaiKhoan`, 45 `PhieuDangKyHienMau`, 18 `LichKhamSangLoc`, và 12 `KetQuaHienMau` (gồm cả thành công và không đủ điều kiện).

---

### `[CRIT-03]` Thiếu thông tin Họ tên, Mã sinh viên và Nội dung thực hiện ở đầu các file thuộc Module 3, Module 4, Module 5
- **Module liên quan:** Module 3, Module 4, Module 5
- **Yêu cầu đề bài bị ảnh hưởng:** Mục 18 (*Quy định ghi chú tác giả trên Source Code*) và Mục 21 (*"Không ghi họ tên và mã sinh viên trên đầu các file phụ trách"* là lỗi bị trừ điểm).
- **Bằng chứng trong Code:**
  - Module 1 (`Nguyễn Tiến Nam - 23103100275`) và Module 2 (`Nguyễn Thị Phương Thảo - 23103100272`) đã ghi đầy đủ.
  - Module 3 (`NguoiHienMau.cs`, `PhieuDangKyHienMau.cs`, `NguoiHienMauController.cs`, `DangKyController.cs`, `NguoiHienMauRepository.cs`, `PhieuDangKyRepository.cs`): Chỉ ghi chung chung `// Phụ trách: Nhóm thực hiện Module 3 - Lớp DHTI17A5HN`, **không có Họ tên và Mã sinh viên cụ thể**.
  - Module 4 (`EfPhieuDangKyRepository.cs`, `IPhieuDangKyRepository.cs`, `DotQuotaAlert.cs`, phần Module 4 trong `AdminController.cs`, `Views/Admin/PhieuDangKyHienMau.cshtml`, `ChiTietPhieuDangKy.cshtml`): **Hoàn toàn không có comment Họ tên và Mã sinh viên**.
  - Module 5 (`LichKhamSangLoc.cs`, `KetQuaHienMau.cs`, `LichKhamSangLocController.cs`, `KetQuaHienMauController.cs`, `ThongKeController.cs`, `KhamSangLocService.cs`, `LichKhamSangLocRepository.cs`, `KetQuaHienMauRepository.cs`, `ThongKeRepository.cs`, `ViewModels/*.cs`, `Views/LichKhamSangLoc/*`, `Views/KetQuaHienMau/*`, `Views/ThongKe/*`): **Hoàn toàn không có comment Họ tên và Mã sinh viên**.
- **Hướng khắc phục đề xuất:** Bổ sung đúng mẫu 3 dòng comment ở đầu tất cả các file Model, Controller, ViewModel, Repository, Service, View chính của từng thành viên.

---

## II. Mức độ `HIGH` (Lỗi phân quyền, xung đột luồng giữa các Module và màn hình giả)

### `[HIGH-01]` Lỗ hổng phân quyền: Nhân viên y tế (`Medical Staff`) có thể khóa/mở khóa người hiến máu và xem danh sách quản trị người hiến máu
- **Module liên quan:** Module 1 & Module 3 (`AdminController.cs`)
- **Yêu cầu đề bài bị ảnh hưởng:** Mục 5.1 (*"Quản trị viên (Admin): Quản lý danh sách người hiến máu, khóa hoặc mở khóa người hiến máu khi cần"* — quyền của Nhân viên y tế không bao gồm quản lý/khóa hồ sơ người hiến máu).
- **Bằng chứng trong Code:**
  - [`AdminController.cs#L15`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L15) đặt `[Authorize(Roles = "Admin,Medical Staff")]` cho toàn bộ class.
  - Các action của Module 1 như `NhanSu`, `KhoaMoKhoaTaiKhoan`, `DonViToChuc` đều có gắn `[Authorize(Roles = "Admin")]` riêng để chặn `Medical Staff`.
  - Nhưng [`AdminController.NguoiHienMau`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L550) và [`AdminController.KhoaMoKhoaNguoiHien`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L655) **không gắn `[Authorize(Roles = "Admin")]`**!
- **Hướng khắc phục đề xuất:** Thêm `[Authorize(Roles = "Admin")]` vào `KhoaMoKhoaNguoiHien` (và cân nhắc giới hạn hoặc ẩn nút Khóa đối với `Medical Staff` trên `Views/Admin/NguoiHienMau.cshtml`).

---

### `[HIGH-02]` Xung đột trạng thái phiếu `"Chờ khám sàng lọc"` giữa Module 4 và Module 5
- **Module liên quan:** Module 4 & Module 5
- **Yêu cầu đề bài bị ảnh hưởng:** Mục 8.3 & Mục 9.1 (*"Lập lịch khám sàng lọc cho các phiếu đăng ký ở trạng thái Đã xác nhận; Cập nhật trạng thái phiếu sang Chờ khám sàng lọc khi đã lập lịch"*).
- **Nguyên nhân gốc & Bằng chứng trong Code:**
  - Trong [`AdminController.LaChuyenTrangThaiHopLe`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L347-L353) (Module 4), nhân viên y tế được phép bấm nút chuyển trạng thái phiếu từ `"Đã xác nhận"` sang `"Chờ khám sàng lọc"` ngay tại màn hình duyệt phiếu **mà không tạo bản ghi `LichKhamSangLoc`**.
  - Khi sang Module 5 ([`KhamSangLocService.LuuLichAsync`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Services/KhamSangLocService.cs#L53-L57)), hệ thống chỉ cho phép lập lịch mới cho phiếu đang ở trạng thái `"Đã xác nhận"`, khiến các phiếu đã bị chuyển sang `"Chờ khám sàng lọc"` từ Module 4 không xuất hiện trong dropdown lập lịch!
- **Hướng khắc phục đề xuất:** Không cho phép `CapNhatTrangThaiPhieu` chuyển chay sang `"Chờ khám sàng lọc"` hoặc `"Đã khám sàng lọc"` nếu không đi qua luồng Tạo lịch khám / Hoàn thành lịch khám.

---

### `[HIGH-03]` 6 màn hình giao diện tĩnh (Mock UI) ngoài phạm vi đề tài đang hiển thị công khai trên Sidebar `_AdminLayout.cshtml`
- **Module liên quan:** Giao diện quản trị chung (`_AdminLayout.cshtml`, `AdminController.cs`)
- **Yêu cầu đề bài bị ảnh hưởng:** Mục 3 (*Phạm vi đề tài*) và Quy tắc kiểm toán (*"Nếu một chức năng chỉ có giao diện nhưng chưa có xử lý backend thì phải đánh giá là chưa hoàn thiện"*).
- **Bằng chứng trong Code:**
  - [`_AdminLayout.cshtml#L190-L201`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/Shared/_AdminLayout.cshtml#L190-L201), [`#L215-L218`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/Shared/_AdminLayout.cshtml#L215-L218), [`#L232-L239`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/Shared/_AdminLayout.cshtml#L232-L239): Hiển thị 6 menu `HoSoBenhSu`, `TieuHuy`, `GiamSatTuiMau`, `KhoLuuTruMau`, `RBAC`, `KhoLanhIoT`.
- **Tác động:** Giảng viên hỏi vấn đáp dễ bấm vào các menu này và phát hiện toàn bộ số liệu/nút bấm trên đó là HTML cứng (fake UI).
- **Hướng khắc phục đề xuất:** Ẩn 6 mục menu này khỏi `_AdminLayout.cshtml` để giao diện quản trị tập trung 100% vào đúng 5 Module thật của Đề tài 29.

---

## III. Mức độ `MEDIUM` (Bất nhất kỹ thuật, thiếu phân trang phụ, xung đột Session/Cookie)

### `[MED-01]` Không đồng bộ cơ chế đọc thông tin đăng nhập giữa `[KiemTraQuyen]` (Module 5) và Cookie Authentication (Module 1–4)
- **Module liên quan:** Module 1 & Module 5
- **Bằng chứng trong Code:**
  - [`KiemTraQuyenAttribute.cs#L19`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Filters/KiemTraQuyenAttribute.cs#L19) chỉ lấy `maTaiKhoan` từ `context.HttpContext.Session.GetInt32("MaTaiKhoan")` mà không fallback sang `User.FindFirstValue(ClaimTypes.NameIdentifier)` khi Session bị reset (ví dụ khi restart app trong quá trình dev hoặc hết hạn Session trước Cookie).
  - Trong [`LichKhamSangLocController.LayMaTaiKhoan`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/LichKhamSangLocController.cs#L112) và [`KetQuaHienMauController.LayMaTaiKhoan`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/KetQuaHienMauController.cs#L79) cũng chỉ đọc `HttpContext.Session.GetInt32("MaTaiKhoan")`.
- **Hướng khắc phục đề xuất:** Bổ sung fallback đọc từ `ClaimTypes.NameIdentifier` nếu Session trả về `null` (giống cách `NguoiHienMauController.GetCurrentAccountId()` đã làm).

### `[MED-02]` Danh sách Người hiến máu (`AdminController.NguoiHienMau`) và Danh sách Tài khoản (`AdminController.NhanSu`) chưa có phân trang `Skip/Take`
- **Module liên quan:** Module 1 & Module 3
- **Bằng chứng trong Code:**
  - [`AdminController.cs#L588-L590`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs#L588-L590): Gọi `.ToListAsync()` tải toàn bộ người hiến máu về View mà không `Skip/Take` (trên View `Views/Admin/NguoiHienMau.cshtml` lại hiển thị thanh phân trang tĩnh `1, 2, 3`).
- **Hướng khắc phục đề xuất:** Bổ sung phân trang thật bằng `Skip((page - 1) * pageSize).Take(pageSize)` khi tăng Seed Data lên >= 30 người hiến máu.

### `[MED-03]` Ô tìm kiếm toàn cục trên Topbar `_AdminLayout.cshtml` chưa gắn form xử lý
- **Module liên quan:** Giao diện quản trị chung (`Views/Shared/_AdminLayout.cshtml`)
- **Bằng chứng trong Code:**
  - [`_AdminLayout.cshtml#L264-L267`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/Shared/_AdminLayout.cshtml#L264-L267): Thẻ `<input type="search" placeholder="Tìm mã phiếu, số CCCD, họ tên người hiến, tên đợt..." />` nằm lẻ loi không bọc trong `<form>`, khi nhấn Enter không có tác dụng gì.
- **Hướng khắc phục đề xuất:** Bọc trong `<form method="get" asp-controller="Admin" asp-action="PhieuDangKyHienMau">` với `name="tenNguoiHien"` để ô tìm kiếm hoạt động thật.

---

## IV. Mức độ `LOW` (Dead Code, cấu hình dư thừa)

### `[LOW-01]` File `Data/CauHinhKhamSangLoc.cs` và `Services/ThongKeService.cs` là Dead Code (không được gọi ở bất kỳ đâu)
- **Bằng chứng trong Code:**
  - [`CauHinhKhamSangLoc.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Data/CauHinhKhamSangLoc.cs) định nghĩa extension method `ConfigureKhamSangLoc(this ModelBuilder modelBuilder)` nhưng [`ApplicationDbContext.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Data/ApplicationDbContext.cs) tự viết cấu hình trực tiếp mà không gọi hàm này.
  - [`ThongKeService.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Services/ThongKeService.cs) trùng 100% code với [`ThongKeRepository.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/ThongKeRepository.cs) và không được đăng ký trong DI Container (`DangKyDichVu.cs`).
