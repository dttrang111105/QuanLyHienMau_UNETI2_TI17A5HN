# 03. DANH SÁCH LỖI, MỨC ĐỘ NGHIÊM TRỌNG VÀ NGUYÊN NHÂN (MODULE 3)

Tài liệu này liệt kê chi tiết toàn bộ các lỗi và điểm bất cập được phát hiện trong quá trình kiểm toán tĩnh và kiểm thử thực nghiệm Module 3 trên nhánh `origin/merge1` (commit `0b0d7d6926094f398cdbfd88e18658570650be63`).

---

## Tổng quan Phân loại Lỗi

| Mức độ nghiêm trọng | Số lượng | Danh sách mã lỗi |
| :--- | :---: | :--- |
| **Critical** | 0 | *(Không có)* |
| **High** | 2 | `BUG-M3-01`, `BUG-M3-02` |
| **Medium** | 3 | `BUG-M3-03`, `BUG-M3-04`, `BUG-M3-05` |
| **Low** | 3 | `BUG-M3-06`, `BUG-M3-07`, `BUG-M3-08` |
| **Tổng cộng** | **8** | |

---

## Chi tiết từng Lỗi

### 1. [BUG-M3-01] Tự động chọn nhầm và ghi đè mất nhóm máu hiếm `Rh(-)` (`O-`, `A-`, `B-`, `AB-`) thành `Rh(+)` khi nộp phiếu đăng ký hiến máu

- **Tên chức năng:** Đăng ký tham gia hiến máu & Cập nhật nhóm máu người hiến máu.
- **File và vị trí dòng code:**
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml` (dòng `274–296`)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (dòng `549–556`)
- **Phân loại nguồn gốc:** Lỗi tồn tại từ trước trong `Views/DangKy/Create.cshtml` và `DangKyController.cs`.
- **Các bước tái hiện:**
  1. Người hiến máu đăng nhập và cập nhật nhóm máu hiếm trong Hồ sơ cá nhân (`/NguoiHienMau/HoSoCaNhan`) là `"O-"` (hoặc `"A-"`, `"B-"`, `"AB-"`).
  2. Người hiến máu mở trang đăng ký hiến máu `/DangKy/Create?maDot=1`.
  3. Quan sát mục **"Nhóm máu ABO (nếu đã biết)"** và bấm nộp phiếu đăng ký.
- **Kết quả mong đợi:**
  - Giao diện phải giữ đúng nhóm máu `"O-"` (`O Rh(-)`) của người hiến máu hoặc không được tự ý đổi từ `Rh(-)` sang `Rh(+)`.
  - Khi nộp phiếu, trường `NguoiHienMau.NhomMau` trong Database phải giữ nguyên là `"O-"`.
- **Kết quả thực tế (Đã kiểm chứng bằng `[BUG-VERIFY-RH]: True`):**
  - Trên `Views/DangKy/Create.cshtml`, chỉ có 4 nút radio cho nhóm máu dương (`O Rh(+)`, `A Rh(+)`, `B Rh(+)`, `AB Rh(+)`) và 1 nút `"Chưa biết"` (`UNKNOWN`), hoàn toàn thiếu 4 nhóm máu âm (`O-`, `A-`, `B-`, `AB-`).
  - Biểu thức Razor `donor?.NhomMau?.Contains("O") == true ? "checked" : ""` trả về `true` đối với `"O-"` (vì `"O-"` có chứa ký tự `"O"`), làm nút radio `value="O Rh(+)"` bị tự động `checked`.
  - Khi submit form, `DangKyController` (dòng 554) thực thi `nguoiHienMau.NhomMau = cleanBloodGroup;`, ghi đè nhóm máu hiếm `"O-"` trong Database thành `"O Rh(+)"`!
- **Nguyên nhân:**
  - Dùng `.Contains("O")`, `.Contains("A")`, `.Contains("B")` không phân biệt yếu tố `Rh(+)` / `Rh(-)` (thậm chí `"AB+"` vừa `.Contains("A")` vừa `.Contains("B")` vừa `.Contains("AB")`).
  - Danh sách tùy chọn nhóm máu trên `Views/DangKy/Create.cshtml` chưa đồng bộ với 9 giá trị chuẩn trên `Views/NguoiHienMau/HoSoCaNhan.cshtml` (`O+`, `A+`, `B+`, `AB+`, `O-`, `A-`, `B-`, `AB-`, `Chưa rõ`).
- **Mức độ:** **High** (Làm sai lệch dữ liệu y tế quan trọng về nhóm máu hiếm `Rh(-)` của người hiến máu).
- **Hướng khắc phục đề xuất:**
  - Đồng bộ danh sách lựa chọn nhóm máu trên `Views/DangKy/Create.cshtml` với `Views/NguoiHienMau/HoSoCaNhan.cshtml` (`O+`, `A+`, `B+`, `AB+`, `O-`, `A-`, `B-`, `AB-`, `UNKNOWN`) và dùng phép so sánh chính xác (`==`) thay vì `.Contains(...)`.

---

### 2. [BUG-M3-02] Trang Admin Quản lý người hiến máu (`/Admin/NguoiHienMau`) tính nhầm phiếu `"Không đủ điều kiện hiến"` thành lần hiến máu thành công và tính sai thời gian chờ 84 ngày

- **Tên chức năng:** Quản lý danh sách người hiến máu phía Admin & Thống kê chỉ số người hiến máu.
- **File và vị trí dòng code:**
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs` (dòng `681–691`, `764–800`)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/NguoiHienMau.cshtml` (dòng `391`, `401–404`, `481–503`)
- **Phân loại nguồn gốc:** Lỗi tồn tại từ trước trong `AdminController.cs` và `Views/Admin/NguoiHienMau.cshtml` (khi cập nhật quy tắc 84 ngày ở `DangKyController` và `HoSoCaNhan.cshtml` nhưng chưa đồng bộ sang `AdminController.NguoiHienMau`).
- **Các bước tái hiện:**
  1. Một người hiến máu mới hoàn toàn (chưa từng hiến máu thành công) nộp phiếu đăng ký, đến điểm khám lâm sàng nhưng bị kết luận **không đạt sàng lọc** (bản ghi `KetQuaHienMau` được tạo với `KetQua = "Không đủ điều kiện hiến"`, `PhieuDangKyHienMau.TrangThai = "Không đủ điều kiện hiến"`).
  2. Admin truy cập `/Admin/NguoiHienMau` và lọc theo `trangThai=waiting` (Đang chờ 84 ngày) hoặc xem cột **Đóng góp & Thể tích** của người hiến máu đó.
- **Kết quả mong đợi:**
  - Người bị loại lúc khám sàng lọc (`"Không đủ điều kiện hiến"`) chưa hiến máu nên số lần hiến thành công phải là `0 lần (0 ml)` và không bị đưa vào diện phải chờ 84 ngày giữa 2 lần hiến máu thành công.
  - Ngày tính 84 ngày đối với người hiến máu thành công phải lấy theo `KetQuaHienMau.NgayCapNhat` đồng nhất với `DangKyController`.
- **Kết quả thực tế (Đã kiểm chứng bằng `[BUG-VERIFY-ADMIN-KPI]: True`):**
  - Người hiến chỉ có 1 phiếu `"Không đủ điều kiện hiến"` bị trang `/Admin/NguoiHienMau` tính là đã hiến máu thành công `1 lần (350 ml)`, xếp hạng `"Tích cực"`, và bị xếp vào trạng thái `"Đang chờ (84 ngày)"`!
  - Thẻ KPI `"Cần nghỉ ngơi (<84 ngày)"` và `"Sẵn sàng hiến máu"` trên đầu trang `/Admin/NguoiHienMau` bị sai lệch theo.
- **Nguyên nhân:**
  - Trong `AdminController.cs` (dòng 681, 690, 764, 793) và `Views/Admin/NguoiHienMau.cshtml` (dòng 391), code kiểm tra:
    ```csharp
    p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null
    ```
    Khi bác sĩ ghi nhận kết quả `"Không đủ điều kiện hiến"` vào bảng `KetQuaHienMau`, thuộc tính `p.KetQuaHienMau != null` luôn bằng `true`!
  - Đồng thời, `AdminController.cs` (dòng 685, 768) và `Views/Admin/NguoiHienMau.cshtml` (dòng 403) lấy ngày `p.NgayXuLy ?? p.NgayNop` thay vì `DangKyController.GetActualDonationDate(p)` (`KetQuaHienMau.NgayCapNhat`).
- **Mức độ:** **High** (Làm sai lệch số liệu thống kê Admin, số lần hiến máu, tổng thể tích máu và bộ lọc trạng thái 84 ngày giữa cổng Admin và cổng Donor).
- **Hướng khắc phục đề xuất:**
  - Thay thế toàn bộ điều kiện `p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null` và `p.NgayXuLy ?? p.NgayNop` trong `AdminController.NguoiHienMau` và `Views/Admin/NguoiHienMau.cshtml` bằng `DangKyController.IsConfirmedSuccessfulDonation(p)` và `DangKyController.GetActualDonationDate(p)` (giống hệt cách `Views/NguoiHienMau/HoSoCaNhan.cshtml` dòng 15 & 47 đang sử dụng).

---

### 3. [BUG-M3-03] Trang `/Admin/NguoiHienMau` thiếu nút Khóa / Mở khóa trực tiếp hồ sơ người hiến máu

- **Tên chức năng:** Khóa/mở khóa người hiến máu phía Admin.
- **File và vị trí dòng code:**
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/NguoiHienMau.cshtml` (dòng `539–554`)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs` (dòng `650–832`, `1274–1306`)
- **Phân loại nguồn gốc:** Hạn chế thiết kế tồn tại từ trước.
- **Các bước tái hiện:**
  1. Admin truy cập trang Quản lý Người hiến máu `/Admin/NguoiHienMau`.
  2. Quan sát cột **Thao tác** trên từng dòng người hiến máu.
- **Kết quả mong đợi:**
  - Có nút khóa/mở khóa trực tiếp trạng thái `NguoiHienMau.TrangThai` (và `TaiKhoan.TrangThai` nếu có gắn tài khoản) ngay tại danh sách `/Admin/NguoiHienMau`, bao gồm cả các hồ sơ người hiến máu không gắn tài khoản (`MaTaiKhoan == null`).
- **Kết quả thực tế:**
  - Cột **Thao tác** tại `/Admin/NguoiHienMau` chỉ có nút `"Xem các phiếu đăng ký của người này"`.
  - Muốn khóa/mở khóa người hiến máu có tài khoản, Admin phải chuyển sang trang `/Admin/NhanSu` (`POST /Admin/KhoaMoKhoaTaiKhoan`). Đối với 8 hồ sơ người hiến máu mẫu trong `DbInitializer.SeedTestData` (có `MaTaiKhoan == null`), chúng không xuất hiện ở `/Admin/NhanSu` nên Admin không có nút giao diện nào để khóa/mở khóa.
- **Nguyên nhân:** Chưa bổ sung action `ToggleDonorStatus(int maNguoiHien)` và nút bấm tương ứng trên `Views/Admin/NguoiHienMau.cshtml`.
- **Mức độ:** **Medium**
- **Hướng khắc phục đề xuất:**
  - Bổ sung action `[HttpPost] [ValidateAntiForgeryToken] KhoaMoKhoaNguoiHienMau(int id)` trong `AdminController` (cập nhật `NguoiHienMau.TrangThai` và đồng bộ `TaiKhoan.TrangThai` nếu `MaTaiKhoan != null`) và thêm nút Khóa/Mở khóa trên bảng `Views/Admin/NguoiHienMau.cshtml`.

---

### 4. [BUG-M3-04] Liên kết điều hướng chưa chuẩn trên trang `/Admin/NguoiHienMau` (Nút `"Thêm Hồ Sơ Mới"` và `"Xem các phiếu đăng ký"`)

- **Tên chức năng:** Quản lý danh sách người hiến máu phía Admin.
- **File và vị trí dòng code:**
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/NguoiHienMau.cshtml` (dòng `62–65` và dòng `547–551`)
- **Phân loại nguồn gốc:** Lỗi tồn tại từ trước trong `Views/Admin/NguoiHienMau.cshtml`.
- **Các bước tái hiện:**
  1. Đăng nhập bằng tài khoản `admin`, vào `/Admin/NguoiHienMau`.
  2. Bấm nút `"Thêm Hồ Sơ Mới"` ở góc trên bên phải (dòng 62).
  3. Hoặc bấm biểu tượng con mắt `"Xem các phiếu đăng ký của người này"` ở cột Thao tác (dòng 547).
- **Kết quả mong đợi:**
  - Nút `"Thêm Hồ Sơ Mới"` phải mở modal/form tạo hồ sơ người hiến máu mới của Admin (hoặc dẫn tới `/Admin/NhanSu`).
  - Nút `"Xem các phiếu đăng ký của người này"` nên lọc chính xác theo mã định danh `MaNguoiHien` hoặc hiển thị modal chi tiết người hiến máu.
- **Kết quả thực tế:**
  - Nút `"Thêm Hồ Sơ Mới"` gắn `asp-controller="DangKy" asp-action="Create"`, khiến Admin bị điều hướng sang cổng tự đăng ký hiến máu dành cho Donor (và bị đẩy sang `/NguoiHienMau/HoSoCaNhan` của chính Admin).
  - Nút `"Xem các phiếu đăng ký của người này"` truyền `tenNguoiHien=@donor.HoTen` sang `/Admin/PhieuDangKyHienMau`; nếu có 2 người hiến máu trùng họ tên thì kết quả lọc sẽ gộp cả 2 người.
- **Nguyên nhân:** Gắn nhầm controller/action đích trên View.
- **Mức độ:** **Medium**
- **Hướng khắc phục đề xuất:**
  - Đổi nút `"Thêm Hồ Sơ Mới"` trỏ về `/Admin/NhanSu` (hoặc tạo form thêm hồ sơ riêng cho Admin) và hỗ trợ lọc theo `maNguoiHien` tại `/Admin/PhieuDangKyHienMau`.

---

### 5. [BUG-M3-05] Không đồng nhất định dạng `NhomMau`, Regex `SoDienThoai` giữa các Module và thiếu kiểm tra danh mục `GioiTinh`/`NhomMau` phía Server

- **Tên chức năng:** Cập nhật thông tin người hiến máu & Đăng ký hiến máu.
- **File và vị trí dòng code:**
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs` (dòng `164`, `189–196`)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (dòng `441`, `451`, `549–556`)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AccountController.cs` (dòng `25`, `185`)
- **Phân loại nguồn gốc:** Tồn tại từ trước & phát sinh do sự khác biệt quy ước giữa Module 1 và Module 3.
- **Chi tiết & Nguyên nhân:**
  1. **Định dạng Nhóm máu (`NhomMau`):**
     - `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 730–738) và `AccountController` lưu: `"O+"`, `"A+"`, `"B+"`, `"AB+"`, `"O-"`, `"A-"`, `"B-"`, `"AB-"`, `"Chưa rõ"`.
     - `Views/DangKy/Create.cshtml` (dòng 275–295) lưu: `"O Rh(+)"`, `"A Rh(+)"`, `"B Rh(+)"`, `"AB Rh(+)"`. Khi người dùng lưu `"O Rh(+)"` từ trang Đăng ký rồi quay lại trang `HoSoCaNhan`, thẻ `<select asp-for="NhomMau">` trên `HoSoCaNhan.cshtml` không khớp với bất kỳ `<option>` nào (`O+` != `O Rh(+)`) nên dropdown bị nhảy về `"-- Chọn nhóm máu --"`!
  2. **Biểu thức Regex Số điện thoại (`SoDienThoai`):**
     - Module 1 (`AccountController` dòng 25) dùng `^(0[3|5|7|8|9])+([0-9]{8})$` (chỉ nhận đúng 10 số đầu `03, 05, 07, 08, 09`).
     - Module 3 (`NguoiHienMauController` dòng 164, `DangKyController` dòng 451) dùng `^0[0-9]{9,10}$` (nhận 10–11 số bắt đầu bằng `0`, ví dụ `01234567890`).
  3. **Kiểm tra danh mục `GioiTinh` và `NhomMau` phía Server:**
     - `NguoiHienMauController` (dòng 189–196) chỉ kiểm tra `GioiTinh.Length <= 10` và `NhomMau.Length <= 20`, chưa kiểm tra giá trị có nằm trong danh sách cho phép (`{"Nam", "Nữ", "Khác"}` / `{"O+", "A+", ...}`) nếu người dùng gửi HTTP POST trực tiếp không qua trình duyệt.
- **Mức độ:** **Medium**
- **Hướng khắc phục đề xuất:**
  - Thống nhất chuẩn lưu `NhomMau` trên toàn hệ thống là `"O+"`, `"A+"`, `"B+"`, `"AB+"`, `"O-"`, `"A-"`, `"B-"`, `"AB-"`, `"Chưa rõ"` và bổ sung kiểm tra `HashSet<string>` cho `GioiTinh` và `NhomMau` ở phía Server.

---

### 6. [BUG-M3-06] Hành vi fallback âm thầm khi gửi rỗng `ngaySinh`/`canNang` tại `POST /DangKy/Create` và so sánh `.Date` trong `GetLatestOpenDotHienMauAsync`

- **Tên chức năng:** Đăng ký tham gia hiến máu.
- **File và vị trí dòng code:**
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (dòng `408`, `418`, `446`, `478`)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs` (dòng `42–50`)
- **Phân loại nguồn gốc:** Tồn tại từ trước trong `DangKyController.cs` và `PhieuDangKyRepository.cs`.
- **Chi tiết & Nguyên nhân:**
  1. Tại `POST /DangKy/Create`, các dòng `var finalNgaySinh = ngaySinh ?? nguoiHienMau.NgaySinh;` và `var finalWeight = canNang ?? nguoiHienMau.CanNang;` sẽ lấy lại giá trị cũ trong Database nếu người dùng xóa trắng ô Ngày sinh hoặc Cân nặng trên form (gửi chuỗi rỗng `""` -> bind thành `null`), thay vì báo lỗi bắt buộc nhập trên form.
  2. Tại `PhieuDangKyRepository.GetLatestOpenDotHienMauAsync(DateTime referenceDate)` (dòng 44–49):
     ```csharp
     var today = referenceDate.Date;
     return await _context.DotHienMaus ...
         .Where(d => d.TrangThai == "Đang mở đăng ký" && d.NgayBatDauDangKy <= today && d.HanDangKy >= today)
     ```
     Nếu `NgayBatDauDangKy` trong DB có chứa thành phần giờ lớn hơn `00:00:00` vào đúng ngày bắt đầu mở đăng ký (ví dụ `2026-11-08 08:00:00` như cách `DbInitializer` tạo bằng `DateTime.Now`), phép so sánh `d.NgayBatDauDangKy <= today` (`2026-11-08 08:00:00 <= 2026-11-08 00:00:00`) sẽ trả về `false` trong suốt ngày đầu tiên mở đợt khi truy cập `/DangKy/Create` không truyền `maDot`.
- **Mức độ:** **Low**
- **Hướng khắc phục đề xuất:**
  - Kiểm tra trực tiếp các tham số gửi từ form (`hoTen`, `ngaySinh`, `canNang`, `soDienThoai`) không được rỗng; sửa điều kiện trong `GetLatestOpenDotHienMauAsync` thành `d.NgayBatDauDangKy.Date <= today && d.HanDangKy.Date >= today`.

---

### 7. [BUG-M3-07] Trùng lặp cặp Interface Repository sau khi merge nhánh `merge1` và `AdminController.NguoiHienMau` chưa chuyển sang dùng Repository

- **Tên chức năng:** Kiến trúc tầng Repository của Module 3 & các Module liên quan.
- **File và vị trí dòng code:**
  - `QuanLyHienMau_UNETI2_TI17A5HN/Program.cs` (dòng `26–37`)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/IRepository.cs` vs `Repositories/IRepository.cs`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/IPhieuDangKyRepository.cs` vs `Repositories/IPhieuDangKyRepository.cs`
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs` (dòng `650–832`)
- **Phân loại nguồn gốc:** Phát sinh từ quá trình hợp nhất nhánh `merge1` giữa các thành viên.
- **Chi tiết & Nguyên nhân:**
  - Module 3 định nghĩa `IRepository<T>` và `IPhieuDangKyRepository` trong namespace `QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces`, trong khi Module 1 & Module 4 định nghĩa `IRepository<T>` và `IPhieuDangKyRepository` trong namespace `QuanLyHienMau_UNETI2_TI17A5HN.Repositories`.
  - `Program.cs` đã đăng ký DI cho cả hai bằng tên đầy đủ (fully-qualified name) nên dự án vẫn build và chạy không lỗi, nhưng việc tồn tại 2 interface trùng tên cho cùng một Entity `PhieuDangKyHienMau` gây khó khăn khi bảo trì.
  - Ngoài ra, action `AdminController.NguoiHienMau` vẫn đang truy vấn trực tiếp qua `_context.NguoiHienMaus` thay vì thông qua `INguoiHienMauRepository`.
- **Mức độ:** **Low** (Không gây lỗi runtime nhưng ảnh hưởng tính nhất quán kiến trúc).
- **Hướng khắc phục đề xuất:**
  - Hợp nhất 2 interface `IPhieuDangKyRepository` và `IRepository<T>` về chung một namespace và chuyển truy vấn trong `AdminController.NguoiHienMau` vào `INguoiHienMauRepository`.

---

### 8. [BUG-M3-08] Kiểm tra trùng lặp Email chưa quét các hồ sơ `NguoiHienMau` không gắn tài khoản và thiếu Unique Index `Email` trong Database

- **Tên chức năng:** Cập nhật thông tin người hiến máu & Toàn vẹn dữ liệu Database.
- **File và vị trí dòng code:**
  - `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/NguoiHienMauRepository.cs` (dòng `55–60`)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Data/ApplicationDbContext.cs` (dòng `26–43`)
- **Phân loại nguồn gốc:** Tồn tại từ trước.
- **Chi tiết & Nguyên nhân:**
  - Phương thức `IsEmailTakenByOtherAccountAsync(int currentMaTaiKhoan, string email)` chỉ kiểm tra `_context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != currentMaTaiKhoan && t.Email != null && t.Email.ToLower() == normalizedEmail)`. Nếu trong bảng `NguoiHienMau` có hồ sơ rời (`MaTaiKhoan == null`) sử dụng email đó, hoặc có 2 request đồng thời, Database chưa có ràng buộc Unique Index trên cột `Email`.
- **Mức độ:** **Low**
- **Hướng khắc phục đề xuất:**
  - Bổ sung kiểm tra trên cả bảng `NguoiHienMaus` (`n.MaTaiKhoan != currentMaTaiKhoan`) và cân nhắc thêm Filtered Unique Index cho `Email IS NOT NULL` trong `ApplicationDbContext`.
