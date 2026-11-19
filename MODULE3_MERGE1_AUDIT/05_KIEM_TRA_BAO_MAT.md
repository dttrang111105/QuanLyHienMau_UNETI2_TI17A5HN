# 05. KẾT QUẢ KIỂM TRA BẢO MẬT VÀ PHÂN QUYỀN (MODULE 3)

Tài liệu này ghi nhận kết quả kiểm toán và kiểm thử thực tế về cơ chế xác thực (Authentication), phân quyền theo vai trò (Role-Based Access Control), chống giả mạo yêu cầu (CSRF), chống gán dữ liệu trái phép (Overposting/Mass Assignment) và chống tham chiếu đối tượng trực tiếp không an toàn (IDOR) của Module 3 trên nhánh `origin/merge1`.

---

## 1. Kiểm tra Truy cập khi Chưa Đăng nhập (Unauthenticated Access Control)

- **Cấu hình trong Code:**
  - `NguoiHienMauController`: gắn `[Authorize]` ở cấp class (`Controllers/NguoiHienMauController.cs` dòng 11).
  - `DangKyController`: gắn `[Authorize]` ở cấp class (`Controllers/DangKyController.cs` dòng 15).
  - `AdminController`: gắn `[Authorize(Roles = "Admin,Medical Staff")]` ở cấp class (`Controllers/AdminController.cs` dòng 12).
  - `Program.cs` (dòng 48–54): cấu hình `CookieAuthenticationDefaults.AuthenticationScheme` với `LoginPath = "/Account/Login"`, `AccessDeniedPath = "/Account/AccessDenied"`, `HttpOnly = true`, `SameSite = SameSiteMode.Lax`.
- **Kết quả kiểm thử thực tế (Live Test):**
  - `GET /NguoiHienMau/HoSoCaNhan` (chưa đăng nhập) -> `HTTP 302 Found`, `Location: /Account/Login?ReturnUrl=%2FNguoiHienMau%2FHoSoCaNhan` (**PASS**).
  - `GET /DangKy/Create` (chưa đăng nhập) -> `HTTP 302 Found`, `Location: /Account/Login?ReturnUrl=%2FDangKy%2FCreate` (**PASS**).
  - `GET /DangKy/TheoDoi` (chưa đăng nhập) -> `HTTP 302 Found`, `Location: /Account/Login?ReturnUrl=%2FDangKy%2FTheoDoi` (**PASS**).
  - `GET /Admin/NguoiHienMau` (chưa đăng nhập) -> `HTTP 302 Found`, `Location: /Account/Login?ReturnUrl=%2FAdmin%2FNguoiHienMau` (**PASS**).

---

## 2. Kiểm tra Phân quyền giữa Người hiến máu (`Donor`) và Quản trị viên (`Admin` / `Medical Staff`)

- **Cấu hình trong Code:**
  - `AdminController` yêu cầu `Roles = "Admin,Medical Staff"`.
  - Riêng các action nhạy cảm quản lý tài khoản và khóa/mở khóa (`NhanSu`, `TaoTaiKhoan`, `KhoaMoKhoaTaiKhoan`) yêu cầu `[Authorize(Roles = "Admin")]` (`AdminController.cs` dòng 1235, 1276, 1309).
- **Kết quả kiểm thử thực tế (Live Test):**
  - Đăng nhập tài khoản `donor` (`VaiTro = "Donor"`) và truy cập `GET /Admin/NguoiHienMau` (`TC-E01`) -> Server chặn truy cập và trả về `HTTP 302 Found`, `Location: /Account/AccessDenied?ReturnUrl=%2FAdmin%2FNguoiHienMau` (**PASS**).
  - Đăng nhập tài khoản `admin` (`VaiTro = "Admin"`) và truy cập `GET /Admin/NguoiHienMau` (`TC-E02`) -> Server trả về `HTTP 200 OK` (**PASS**).

---

## 3. Kiểm tra Chống thay đổi ID trên URL / Form để truy cập dữ liệu người khác (IDOR Protection)

Toàn bộ các điểm tiếp nhận dữ liệu của Module 3 đều xác định danh tính người dùng từ `ClaimTypes.NameIdentifier` trong Cookie xác thực đã mã hóa ở phía Server, tuyệt đối không tin tưởng ID truyền từ Client:

| Chức năng | Cơ chế bảo vệ phía Server | Kết quả kiểm thử tấn công IDOR thực tế | Đánh giá |
| :--- | :--- | :--- | :---: |
| **Xem & Sửa Hồ sơ cá nhân (`/NguoiHienMau/HoSoCaNhan`)** | Không nhận tham số `id` trên URL. Luôn lấy `maTaiKhoan` từ `User.FindFirst(ClaimTypes.NameIdentifier)` và truy vấn `_nguoiHienMauRepo.GetByTaiKhoanWithHistoryAsync(maTaiKhoan)`. | Người dùng không có bất kỳ tham số URL nào để xem hồ sơ người khác. Khi gửi kèm `MaNguoiHien=9999`, `MaTaiKhoan=1` trong body POST (`TC-A02`), Server bỏ qua hoàn toàn. | **PASS** |
| **Theo dõi phiếu đăng ký (`GET /DangKy/TheoDoi?id=...` hoặc `?code=...`)** | `DangKyController.TheoDoi` (dòng 648–664) chỉ đối chiếu `parsedId` trong danh sách `userPhieuList` thuộc về chính `nguoiHienMau.MaNguoiHien` đang đăng nhập (`userPhieuList.Any(p => p.MaPhieu == parsedId)`). | Tài khoản `donor` (`MaNguoiHien = 1`) truyền `?id=1` (phiếu của `'Nguyễn Minh Anh'`) (`TC-C03`): Server từ chối hiển thị phiếu, báo lỗi `"Không tìm thấy phiếu đăng ký phù hợp hoặc bạn không có quyền truy cập"` và không rò rỉ bất kỳ thông tin nào của `'Nguyễn Minh Anh'`. | **PASS** |
| **Hủy phiếu đăng ký (`POST /DangKy/Huy`)** | `DangKyController.Huy` (dòng 708–711): kiểm tra `if (nguoiHienMau == null \|\| phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien) return Forbid();` trước khi thực hiện bất kỳ kiểm tra hay cập nhật nào. | Tài khoản `donor` gửi `POST /DangKy/Huy` với `id = 1` (phiếu của `'Nguyễn Minh Anh'`) (`TC-D01`): Server trả về `Forbid` (chuyển hướng sang `/Account/AccessDenied`), trạng thái phiếu `id = 1` trong DB vẫn giữ nguyên `"Chờ xác nhận"`. | **PASS** |

---

## 4. Kiểm tra Phương thức GET/POST và Chống giả mạo yêu cầu (CSRF Protection)

- Tất cả các thao tác làm thay đổi dữ liệu trong Module 3 đều bắt buộc sử dụng phương thức `[HttpPost]` kết hợp `[ValidateAntiForgeryToken]`:
  - `POST /NguoiHienMau/HoSoCaNhan` (`NguoiHienMauController.cs` dòng 72–73)
  - `POST /DangKy/Create` (`DangKyController.cs` dòng 302–303)
  - `POST /DangKy/Huy` (`DangKyController.cs` dòng 681–682)
  - `POST /Admin/KhoaMoKhoaTaiKhoan` (`AdminController.cs` dòng 1274–1275)
- Không có bất kỳ thao tác ghi/xóa/hủy dữ liệu nào được thực hiện qua phương thức `GET`.

---

## 5. Kiểm tra Chống sửa dữ liệu ngoài danh sách cho phép (Overposting / Mass Assignment)

- Trong `POST /NguoiHienMau/HoSoCaNhan` (`NguoiHienMauController.cs` dòng 96–105, 219–243):
  - Controller xóa khỏi `ModelState` các trường hệ thống (`MaNguoiHien`, `MaTaiKhoan`, `TrangThai`, `NgayTao`, `TaiKhoan`, `PhieuDangKyHienMaus`) và chỉ gán thủ công (whitelist) đúng 9 trường thông tin cá nhân (`HoTen`, `NgaySinh`, `GioiTinh`, `CanNang`, `NhomMau`, `SoDienThoai`, `Email`, `NgheNghiep`, `DiaChi`, `GhiChuSucKhoe`) vào đối tượng `existingProfile` được tải từ DB theo `maTaiKhoan`.
  - Đã kiểm thử thực tế (`TC-A02`): gửi `MaNguoiHien = 9999`, `MaTaiKhoan = 1`, `TrangThai = "Hacked"` trong form POST — các trường nhạy cảm này hoàn toàn không bị thay đổi trong Database.
- **Điểm cần hoàn thiện thêm (`BUG-M3-05`):**
  - Ở phía Server, `NguoiHienMauController` mới kiểm tra độ dài chuỗi `GioiTinh` (`<= 10`) và `NhomMau` (`<= 20`) mà chưa kiểm tra giá trị phải nằm trong tập danh mục cố định (`{"Nam", "Nữ", "Khác"}` và `{"O+", "A+", "B+", "AB+", "O-", "A-", "B-", "AB-", "Chưa rõ"}`). Cần bổ sung kiểm tra whitelist giá trị cho 2 trường này.

---

## 6. Kiểm tra Thu hồi Phiên đăng nhập Tức thời khi Tài khoản bị Khóa (`OnValidatePrincipal`)

- **Cơ chế trong `Program.cs` (dòng 56–90):**
  - Trên mỗi HTTP request có kèm Cookie xác thực, sự kiện `OnValidatePrincipal` truy vấn trực tiếp `TaiKhoanRepository.GetByIdAsync(maTaiKhoan)` để kiểm tra `taiKhoan.TrangThai == "Hoạt động"`.
  - Ngay khi Admin khóa tài khoản (`TrangThai = "Bị khóa"`), request tiếp theo của người hiến máu sẽ bị `context.RejectPrincipal()` và `context.HttpContext.SignOutAsync(...)` ngay ở tầng Middleware trước khi vào tới Controller.
  - Ngoài ra, bản thân `NguoiHienMauController` và `DangKyController` cũng kiểm tra kép cả `taiKhoan.TrangThai == "Hoạt động"` và `nguoiHienMau.TrangThai == "Hoạt động"`.
- **Kết quả kiểm thử thực tế (`TC-B12 / SEC-LOCK`):**
  - Khi Admin khóa tài khoản `donor` (`MaTaiKhoan = 3`), phiên đăng nhập đang mở sẵn của `donor` gọi `GET /DangKy/Create` lập tức bị thu hồi Cookie và nhận `HTTP 302 Found -> /Account/Login?ReturnUrl=%2FDangKy%2FCreate` (**PASS**).
