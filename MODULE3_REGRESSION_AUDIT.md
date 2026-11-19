# BÁO CÁO KIỂM TRA HỒI QUY TOÀN DIỆN MODULE 3 (`MODULE3_REGRESSION_AUDIT.md`)

- **Dự án**: `QuanLyHienMau_UNETI2_TI17A5HN`
- **Phạm vi**: Module 3 – Quản lý người hiến máu và phiếu đăng ký hiến máu (bao gồm kiểm tra hồi quy sau khi sửa 9 lỗi Audit ban đầu và bổ sung quy tắc kiểm tra thời gian chờ 84 ngày kể từ lần hiến máu thành công gần nhất)
- **Nhánh Git kiểm tra**: `integration`
- **Thời điểm thực hiện**: `2026-10-28 22:38:20+07:00` – `2026-10-28 22:43:00+07:00`

---

## 1. TÀI LIỆU ĐỐI CHIẾU & NGUYÊN TẮC ĐÁNH GIÁ

### 1.1. Các tài liệu và báo cáo đã đọc, đối chiếu
1. **Yêu cầu nghiệp vụ Module 3** (Quản lý hồ sơ `NguoiHienMau`, Entity `PhieuDangKyHienMau`, Nộp phiếu, Kiểm tra điều kiện & quy tắc 84 ngày, Chống đăng ký trùng, Theo dõi phiếu, Hủy phiếu, Phân quyền & Bảo vệ dữ liệu cá nhân, Tích hợp liên module).
2. **`MODULE3_AUDIT_REPORT.md`**: Báo cáo kiểm tra toàn diện ban đầu (xác định 10 phát hiện từ `M3-BUG-01` đến `M3-BUG-10`).
3. **`MODULE3_FIX_REPORT.md`**: Báo cáo sửa 9 lỗi thuộc phạm vi Module 3 (`M3-BUG-01` đến `M3-BUG-09`).
4. **`MODULE3_RECENT_DONATION_RULE_REPORT.md`**: Báo cáo kiểm tra và sửa quy tắc ngăn đăng ký hiến máu lại khi chưa đủ 84 ngày kể từ ngày hoàn tất hiến máu thành công thực tế (`KetQuaHienMau.NgayCapNhat`).

### 1.2. Nguyên tắc phân loại trạng thái (`PASS`, `FAIL`, `BLOCKED`, `NOT RUN`)
- **`PASS`**: Đã xác minh trực tiếp trong mã nguồn thực tế, biên dịch thành công (`0 Error(s)`) **và** đã có bằng chứng kiểm thử tự động thực thi trực tiếp trên assembly biên dịch (`.dll` .NET 10) hoặc xác minh tĩnh đầy đủ ở mức cấu trúc/route/attribute.
- **`BLOCKED`**: Logic mã nguồn ở cả phía Server (Controller) và Giao diện (Razor View) đã được triển khai đúng yêu cầu và biên dịch thành công, **nhưng** bước thực thi HTTP End-to-End có kết nối trực tiếp vào cơ sở dữ liệu vật lý SQL Server LocalDB (`(localdb)\mssqllocaldb`) bị chặn bởi giới hạn cô lập Named Pipes của môi trường sandbox (`Login timeout expired`). Không tự đánh dấu `PASS` cho phần phụ thuộc kết nối DB runtime.
- **`FAIL`**: Phát hiện logic trong mã nguồn hiện tại chưa đúng yêu cầu hoặc còn sai lệch (bao gồm các điểm nằm ở module lân cận tích hợp với Module 3 mà chưa được phép chỉnh sửa do quy tắc bảo vệ biên giới module).
- **`NOT RUN`**: Kịch bản chưa được thực thi dưới bất kỳ hình thức nào.

---

## 2. KẾT QUẢ KIỂM TRA HỒI QUY TỪNG NHÓM CHỨC NĂNG MODULE 3

| Mã mục | Chức năng / Hạng mục kiểm tra | File & Vị trí code liên quan | Kết quả kiểm tra mã nguồn & Kiểm thử tự động | Trạng thái Kiểm tra Mã nguồn / Unit Test | Trạng thái Runtime DB (LocalDB) | Mức độ nghiêm trọng (nếu còn tồn tại) |
|---|---|---|---|---|---|---|
| **F01** | **Entity `NguoiHienMau` & Quan hệ `1–0..1` với `TaiKhoan`** | `Models/NguoiHienMau.cs` (dòng 9–61), `Data/ApplicationDbContext.cs` (dòng 41–47), `Migrations/20261007035106_InitialCreate.cs` (dòng 203–208) | Có đầy đủ 13/13 thuộc tính chuẩn, khóa chính `MaNguoiHien`, khóa ngoại `MaTaiKhoan` kèm Unique Filtered Index `IX_NguoiHienMau_MaTaiKhoan` (`[MaTaiKhoan] IS NOT NULL`). Đã chạy test tự động `REG-ENTITY-01` đạt `PASS`. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F02** | **Xem hồ sơ người hiến máu (`HoSoCaNhan [GET]`)** | `Controllers/NguoiHienMauController.cs` (dòng 32–76), `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 14–180) | Lấy `maTaiKhoan` từ `ClaimTypes.NameIdentifier`, kiểm tra `taiKhoan.TrangThai == "Hoạt động"`, tự động khởi tạo hồ sơ nếu chưa có, nạp đầy đủ `PhieuDangKyHienMaus`, `DotHienMau`, `LichKhamSangLocs`, `KetQuaHienMau`. Đã bỏ số liệu đếm sai, đồng bộ sang `DangKyController.EvaluateRecentDonationEligibility`. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F03** | **Cập nhật hồ sơ người hiến máu (`HoSoCaNhan [POST]`)** | `Controllers/NguoiHienMauController.cs` (dòng 79–275) | Kiểm tra `taiKhoan.TrangThai == "Hoạt động"` và `nguoiHienMau.TrangThai == "Hoạt động"`. Gọi `ModelState.Clear()` tránh lặp lỗi đôi; kiểm tra đầy đủ `HoTen`, `NgaySinh` (18–60 tuổi), `CanNang` (30–200 kg), `SoDienThoai` (`^0[0-9]{9,10}$`), `Email` (đúng định dạng & không trùng tài khoản khác), giới hạn độ dài chuỗi; chỉ cập nhật whitelist 10 trường cá nhân (chống Mass Assignment & IDOR). | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F04** | **Entity `PhieuDangKyHienMau` & 8 trạng thái nghiệp vụ** | `Models/PhieuDangKyHienMau.cs` (dòng 9–57), `Models/Module5TrangThai.cs` (dòng 5–22) | Đầy đủ 9/9 thuộc tính chuẩn (`MaPhieu`, `MaNguoiHien`, `MaDot`, `NgayNop`, `LyDoHienMau`, `GhiChu`, `TrangThai`, `NgayXuLy`, `GhiChuXacNhan`), khóa ngoại tới `NguoiHienMau` và `DotHienMau`, 8 trạng thái nhất quán. Đã chạy test tự động `REG-ENTITY-01` đạt `PASS`. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F05** | **Nộp phiếu đăng ký hiến máu (`DangKyController.Create [GET/POST]`)** | `Controllers/DangKyController.cs` (dòng 194–613), `Views/DangKy/Create.cshtml` (dòng 1–474) | Bắt buộc đăng nhập, xác định `NguoiHienMau` từ `ClaimTypes.NameIdentifier`, kiểm tra trạng thái hoạt động của tài khoản & hồ sơ, kiểm tra đợt hiến máu tồn tại, `"Đang mở đăng ký"` và nằm trong hạn `NgayBatDauDangKy <= now.Date <= HanDangKy`. Đã xóa toàn bộ dữ liệu mẫu giả định trên `Create.cshtml` và bổ sung đủ `name="ngaySinh"`, `name="gioiTinh"`, `name="ngheNghiep"`. Thiết lập `NgayNop = DateTime.Now` và `TrangThai = "Chờ xác nhận"` tại server. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F06** | **Kiểm tra quy tắc 84 ngày & Ngày hiến máu thực tế** | `Controllers/DangKyController.cs` (dòng 23–176, 276–290, 412–424, 545–560), `Views/DangKy/Create.cshtml` (dòng 14–15, 42–60, 432–445), `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 14–33, 163–189) | Tính đúng 84 ngày từ `KetQuaHienMau.NgayCapNhat` của lần `"Hiến máu thành công"` gần nhất; **tuyệt đối không dùng `NgayNop` hoặc `NgayXuLy`**. Đã chạy 5 bài test tự động chuyên sâu (`REG-84D-01` đến `REG-84D-05`) trên assembly biên dịch: **5/5 PASS**. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F07** | **Kiểm tra điều kiện Tuổi (18–60) & Cân nặng (30–200 kg, `>= CanNangToiThieu`)** | `Controllers/NguoiHienMauController.cs` (dòng 133–163), `Controllers/DangKyController.cs` (dòng 433–449, 490–507) | Tính chính xác tuổi theo ngày sinh (`if (birthDate > today.AddYears(-age)) age--;`), chặn `< 18` hoặc `> 60` tuổi và ngày sinh tương lai; kiểm tra cân nặng bắt buộc, nằm trong `[30, 200]` kg và `>= dotHienMau.CanNangToiThieu`. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F08** | **Kiểm tra định dạng & chống trùng lặp Email giữa các tài khoản** | `Controllers/NguoiHienMauController.cs` (dòng 175–191), `Controllers/DangKyController.cs` (dòng 467–481) | Kiểm tra `EmailAddressAttribute().IsValid`, độ dài `<= 100` và truy vấn `_context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != maTaiKhoan && t.Email != null && t.Email.ToLower() == email.ToLower())` trước khi lưu và đồng bộ sang `TaiKhoan`. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F09** | **Chống đăng ký trùng & Chống Race Condition khi gửi đồng thời** | `Controllers/DangKyController.cs` (dòng 26, 292–304, 527–561) | Chặn tạo phiếu mới khi đã có phiếu đang xử lý trong cùng đợt (`TrangThai` khác `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"`). Bảo vệ đồng thời 2 lớp bằng `ConcurrentDictionary<int, SemaphoreSlim>` theo `MaNguoiHien` kết hợp Database Transaction mức `IsolationLevel.Serializable`. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | **LOW** *(Gợi ý thêm Unique Filtered Index ở DB khi nhóm mở quyền tạo Migration)* |
| **F10** | **Theo dõi phiếu & Hiển thị thông báo lý do từ chối / không đủ điều kiện** | `Controllers/DangKyController.cs` (dòng 617–713), `Views/DangKy/TheoDoi.cshtml` (dòng 228–260) | Lọc danh sách phiếu theo đúng người hiến đang đăng nhập; khi tra cứu `id`/`code` không hợp lệ hoặc của người khác (`explicitLookupFailed = true`), báo lỗi và giữ `selectedPhieu = null`. Khi phiếu ở trạng thái `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`, hiển thị rõ hộp thông báo màu đỏ kèm `Model.GhiChuXacNhan` (hoặc `Model.KetQuaHienMau?.NhanXet`) và `Model.NgayXuLy`. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F11** | **Hủy phiếu đăng ký hiến máu (`DangKyController.Huy [POST]`)** | `Controllers/DangKyController.cs` (dòng 716–797), `Views/DangKy/TheoDoi.cshtml` (dòng 150–160) | Kiểm tra chủ sở hữu phiếu (`phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien -> Forbid()`), chỉ cho phép hủy khi `TrangThai` là `"Chờ xác nhận"` hoặc `"Đã xác nhận"`, chưa có kết quả (`phieu.KetQuaHienMau == null`) **và** chưa có lịch khám sàng lọc đang hoạt động (`!phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy")`). | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F12** | **Phân quyền, chống IDOR, chống CSRF & bảo vệ dữ liệu cá nhân** | `Controllers/NguoiHienMauController.cs` (dòng 15, 37–48, 80–116, 249–260), `Controllers/DangKyController.cs` (dòng 17, 198–231, 316–361, 619–705, 716–750) | Cả 2 Controller gắn `[Authorize]`; toàn bộ 3 POST Actions gắn `[ValidateAntiForgeryToken]`; định danh người dùng luôn lấy từ `ClaimTypes.NameIdentifier` (không nhận `MaNguoiHien`/`MaTaiKhoan` từ client). Đã chạy test tự động `REG-SEC-01` đạt `PASS`. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F13** | **Liên kết từ giao diện quản trị đến trang quản lý phiếu đăng ký** | `Views/Admin/NguoiHienMau.cshtml` (dòng 547), `Views/Shared/_AdminLayout.cshtml` (dòng 164), `Controllers/AdminController.cs` (dòng 104) | Nút *"Xem các phiếu đăng ký của người này"* tại `Views/Admin/NguoiHienMau.cshtml` (dòng 547) đã trỏ đúng tới `asp-controller="Admin" asp-action="PhieuDangKyHienMau" asp-route-tenNguoiHien="@donor.HoTen"`, khớp 100% với `AdminController.PhieuDangKyHienMau(string? tenNguoiHien, ...)`. Liên kết menu bên trái tại `_AdminLayout.cshtml` (dòng 164) cũng trỏ đúng tới `/Admin/PhieuDangKyHienMau`. | **`PASS`** | **`BLOCKED`** *(chặn kết nối LocalDB trong sandbox)* | Không |
| **F14** | **Đồng bộ cách tính 84 ngày & số lần hiến trên trang Quản trị (`AdminController` / `Views/Admin/NguoiHienMau.cshtml` — Ngoài phạm vi sửa của Module 3)** | `Controllers/AdminController.cs` (dòng 815–820, 898–934), `Views/Admin/NguoiHienMau.cshtml` (dòng 62, 142, 481) | Tại trang quản trị danh sách người hiến máu (Module 4), công thức thống kê KPI vẫn đang dùng `p.TrangThai == "Hiến máu thành công" \|\| p.KetQuaHienMau != null` (tính nhầm cả kết quả `"Không đủ điều kiện hiến"`) và lấy ngày `p.NgayXuLy ?? p.NgayNop` thay vì `KetQuaHienMau.NgayCapNhat`; đồng thời nút `"Thêm Hồ Sơ Mới"` ở dòng 62 trỏ sang `DangKy/Create`. *(Chưa can thiệp do tuân thủ quy tắc không tự ý sửa code của Module 4)*. | **`FAIL`** *(Tại Module 4 / Admin)* | **`BLOCKED`** | **MEDIUM** |
| **F15** | **Cập nhật `PhieuDangKyHienMau.NgayXuLy` trong `Module5Service` (Ngoài phạm vi sửa của Module 3)** | `Services/Module5Service.cs` (dòng 97, 223, 262) | Khi chuyển trạng thái phiếu qua `Module5Service` (`HoanThanhKhamAsync`, `GhiKetQuaAsync`), `phieu.NgayXuLy` không được gán `clock.Now`. *(Module 3 đã xử lý tương thích trên `TheoDoi.cshtml` và `DangKyController` bằng cách đọc `KetQuaHienMau.NgayCapNhat`)*. | **`FAIL`** *(Tại Module 5 Service)* | **`BLOCKED`** | **LOW** |

---

## 3. KIỂM TRA CHI TIẾT CÁC QUY TẮC TRỌNG TÂM (MỤC 3 & MỤC 4 ĐỀ BÀI)

### 3.1. Tính đúng đắn của quy tắc 84 ngày & Ngày hiến máu thực tế
- **Vị trí triển khai**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`:
    - Hằng số `SoNgayChoToiThieuGiuaHaiLanHien = 84` (dòng 23).
    - Phương thức `IsConfirmedSuccessfulDonation(PhieuDangKyHienMau? phieu)` (dòng 44–62).
    - Phương thức `GetActualDonationDate(PhieuDangKyHienMau? phieu)` (dòng 68–80).
    - Phương thức `EvaluateRecentDonationEligibility(IEnumerable<PhieuDangKyHienMau>? lichSuPhieu, DateTime referenceDate)` (dòng 85–176).
    - Áp dụng trong `Create [HttpGet]` (dòng 276–290), đầu `Create [HttpPost]` (dòng 412–424) và bên trong `Serializable` transaction của `Create [HttpPost]` (dòng 545–560).
- **Đánh giá chi tiết**:
  1. **Không dùng `NgayNop` hoặc `NgayXuLy`**: `GetActualDonationDate` chỉ trả về `phieu.KetQuaHienMau.NgayCapNhat` khi bản ghi kết quả xác nhận `"Hiến máu thành công"`.
  2. **Lọc đúng trạng thái hiến máu thành công**: Các phiếu `"Chờ xác nhận"`, `"Đã xác nhận"`, `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`, `"Từ chối"`, `"Đã hủy"`, `"Không đủ điều kiện hiến"` đều trả về `false` trong `IsConfirmedSuccessfulDonation`.
  3. **Sắp xếp đúng lần gần nhất**: Khi có nhiều lần hiến máu thành công, danh sách được sắp xếp giảm dần theo `ActualDate` (`OrderByDescending(x => x.ActualDate!.Value)`), đảm bảo lấy đúng lần hiến thực tế gần nhất.
  4. **Xử lý an toàn khi thiếu ngày hiến thực tế trong dữ liệu**: Nếu có phiếu mang trạng thái `"Hiến máu thành công"` nhưng `KetQuaHienMau == null` hoặc `KetQuaHienMau.NgayCapNhat == null`, hàm trả về `IsEligible = false` và `HasMissingActualDonationDate = true`, tuyệt đối không lấy `NgayNop` thay thế.
  5. **Hiển thị rõ ngày đủ điều kiện đăng ký lại**: Thông báo lỗi trả về cả ở Server lẫn UI (`Views/DangKy/Create.cshtml` dòng 42–60, `Views/NguoiHienMau/HoSoCaNhan.cshtml` dòng 177–180) đều hiển thị chính xác `NextEligibleDate` (`lastActualDate.AddDays(84)` theo định dạng `dd/MM/yyyy`).

### 3.2. Kiểm tra điều kiện tuổi và cân nặng
- **Điều kiện độ tuổi (`18–60` tuổi)**:
  - Triển khai tại `Controllers/NguoiHienMauController.cs` (dòng 133–152) và `Controllers/DangKyController.cs` (dòng 433–449).
  - Kiểm tra ngày sinh không lớn hơn `DateTime.Today`, tính tuổi chính xác theo ngày sinh nhật trong năm (`int age = today.Year - birthDate.Year; if (birthDate > today.AddYears(-age)) age--;`) và từ chối nếu `age < 18 || age > 60`.
- **Điều kiện cân nặng (`30–200 kg` và `>= dotHienMau.CanNangToiThieu`)**:
  - Triển khai tại `Controllers/NguoiHienMauController.cs` (dòng 155–162) và `Controllers/DangKyController.cs` (dòng 490–507).
  - Kiểm tra bắt buộc có cân nặng (`> 0`), nằm trong khoảng hợp lý y tế (`30.0 <= canNang <= 200.0`) và đạt ngưỡng tối thiểu của đợt hiến máu (`finalWeight.Value >= dotHienMau.CanNangToiThieu`).

### 3.3. Kiểm tra Email và chống trùng lặp Email
- Triển khai tại `Controllers/NguoiHienMauController.cs` (dòng 175–191) và `Controllers/DangKyController.cs` (dòng 467–481).
- Kiểm tra độ dài `<= 100` ký tự, định dạng hợp lệ qua `new EmailAddressAttribute().IsValid(...)` và kiểm tra không trùng với bất kỳ tài khoản nào khác trong bảng `TaiKhoans` (`t.MaTaiKhoan != maTaiKhoan && t.Email != null && t.Email.ToLower() == email.ToLower()`) trước khi cho phép lưu và đồng bộ giữa `NguoiHienMau.Email` và `TaiKhoan.Email`.

### 3.4. Thông báo lý do từ chối / không đủ điều kiện hiến máu
- Triển khai tại `Views/DangKy/TheoDoi.cshtml` (dòng 242–260).
- Khi phiếu ở trạng thái `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`, giao diện hiển thị khối cảnh báo riêng nêu rõ tiêu đề trạng thái, lý do từ bộ phận y tế (`Model.GhiChuXacNhan` hoặc fallback sang `Model.KetQuaHienMau?.NhanXet`) và thời gian xử lý (`Model.NgayXuLy`).
- Kết hợp với kiểm tra quyền sở hữu tại `DangKyController.TheoDoi` (dòng 640–705), chỉ chủ sở hữu hợp lệ của phiếu (hoặc nhân viên y tế/Admin) mới xem được thông tin này.

### 3.5. Kiểm tra liên kết từ giao diện quản trị đến trang quản lý phiếu
- Tại `Views/Admin/NguoiHienMau.cshtml` (dòng 547):
  ```html
  <a asp-controller="Admin" asp-action="PhieuDangKyHienMau" asp-route-tenNguoiHien="@donor.HoTen" ... title="Xem các phiếu đăng ký của người này">
  ```
  Liên kết đã trỏ chính xác đến Action `AdminController.PhieuDangKyHienMau(string? tenNguoiHien, ...)` (`Controllers/AdminController.cs` dòng 104–140), truyền đúng tên tham số `tenNguoiHien` để lọc danh sách phiếu đăng ký của người hiến máu đó.
- Tại `Views/Shared/_AdminLayout.cshtml` (dòng 164):
  ```html
  <a asp-controller="Admin" asp-action="PhieuDangKyHienMau" ...>
  ```
  Menu điều hướng của cổng quản trị cũng trỏ đúng về `/Admin/PhieuDangKyHienMau`.

---

## 4. KẾT QUẢ BUILD VÀ KIỂM THỬ TỰ ĐỘNG

### 4.1. Kết quả Build dự án
- **Lệnh thực thi**:
  ```powershell
  dotnet build "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --no-restore
  ```
- **Kết quả**:
  - **Exit code**: `0` (`Build succeeded`)
  - **Số lỗi (`Error`)**: `0`
  - **Số cảnh báo thuộc Module 3 (`Warning`)**: `0` (27 cảnh báo `CS8602` còn lại nằm ở các file thuộc Module 5 và Module 6).

### 4.2. Kết quả chạy bộ kiểm thử hồi quy tự động trên Assembly biên dịch (`.dll` .NET 10)
Đã thực thi bộ kiểm thử tự động `Module3RuleTests` gọi trực tiếp vào `QuanLyHienMau_UNETI2_TI17A5HN.dll` với kết quả **7/7 bài kiểm thử đạt `PASS`**:

| Mã Test | Nội dung kiểm thử tự động | Kết quả | Chi tiết đầu ra thực tế |
|---|---|---|---|
| `REG-ENTITY-01` | Kiểm tra Reflection đầy đủ 13 thuộc tính `NguoiHienMau` và 9 thuộc tính `PhieuDangKyHienMau` | **`PASS`** | `NguoiHienMau(13/13)=True, PhieuDangKyHienMau(9/9)=True` |
| `REG-SEC-01` | Kiểm tra `[Authorize]` cấp Controller và `[ValidateAntiForgeryToken]` trên toàn bộ 3 POST Actions (`HoSoCaNhan`, `Create`, `Huy`) | **`PASS`** | `NguoiHienMauController[Authorize]=True, DangKyController[Authorize]=True, PostActionsWithCSRF=3/3` |
| `REG-84D-01` | Chưa từng hiến máu thành công (0 phiếu hoặc chỉ có phiếu ở 4 trạng thái chờ xử lý/khám sàng lọc) | **`PASS`** | `Empty.IsEligible=True, Pending4States.IsEligible=True` |
| `REG-84D-02` | Kiểm tra giá trị biên thời gian chờ: `83 ngày` (chặn, còn 1 ngày), `84 ngày` (cho phép), `85 ngày` (cho phép) | **`PASS`** | `83d: IsEligible=False (Remaining=1d, Next=29/10/2026); 84d: IsEligible=True; 85d: IsEligible=True` |
| `REG-84D-03` | Phiếu `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"` không bị tính là lần hiến máu thành công | **`PASS`** | `IsEligible=True, TotalSuccessfulDonations=0` |
| `REG-84D-04` | Nhiều lần hiến máu thành công: lấy đúng phiếu có `KetQuaHienMau.NgayCapNhat` gần nhất (không lấy theo `NgayNop` hay `MaPhieu`) | **`PASS`** | `PickedPhieu=#502, LastActual=08/10/2026, NextEligible=31/12/2026` |
| `REG-84D-05` | Bản ghi `"Hiến máu thành công"` thiếu `NgayCapNhat`: không lấy `NgayNop`/`NgayXuLy` thay thế, chặn đăng ký an toàn | **`PASS`** | `IsEligible=False, HasMissingDate=True` |

---

## 5. CÁC VẤN ĐỀ CÒN TỒN TẠI NGOÀI PHẠM VI MODULE 3 & ĐỀ XUẤT KHẮC PHỤC

Để đảm bảo tính nhất quán trên toàn bộ hệ thống khi tích hợp với các module khác, dưới đây là 2 điểm thuộc Module 4 và Module 5 được phát hiện trong quá trình kiểm tra hồi quy (chưa tự ý sửa để tuân thủ nguyên tắc không can thiệp module khác):

1. **[Mức độ: `MEDIUM`] Đồng bộ công thức tính 84 ngày và số lần hiến trên trang Quản trị người hiến máu (`AdminController` & `Views/Admin/NguoiHienMau.cshtml`)**:
   - **Vị trí**: `Controllers/AdminController.cs` (dòng 815–820, 898–934) và `Views/Admin/NguoiHienMau.cshtml` (dòng 62, 142, 481).
   - **Hiện trạng**: Đang đếm `p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null` (khiến phiếu có `KetQuaHienMau.KetQua == "Không đủ điều kiện hiến"` cũng bị đếm là đã hiến máu) và lấy ngày `p.NgayXuLy ?? p.NgayNop` thay vì `KetQuaHienMau.NgayCapNhat`. Кроме đó, nút `"Thêm Hồ Sơ Mới"` tại dòng 62 của `Views/Admin/NguoiHienMau.cshtml` đang trỏ tới `asp-controller="DangKy" asp-action="Create"`.
   - **Cách khắc phục đề xuất**: Trong `AdminController.NguoiHienMau` và `Views/Admin/NguoiHienMau.cshtml`, gọi trực tiếp `DangKyController.EvaluateRecentDonationEligibility(n.PhieuDangKyHienMaus, DateTime.Today)` để đồng bộ 100% cách đếm số lần hiến thành công và thời gian chờ 84 ngày giữa cổng người hiến máu (Module 3) và cổng quản trị (Module 4).
2. **[Mức độ: `LOW`] Cập nhật `PhieuDangKyHienMau.NgayXuLy` trong `Services/Module5Service.cs`**:
   - **Vị trí**: `Services/Module5Service.cs` (`LuuLichAsync`, `HuyLichAsync`, `HoanThanhKhamAsync`, `GhiKetQuaAsync` – dòng 148–269).
   - **Hiện trạng**: Khi nhân viên y tế thao tác qua `LichKhamSangLocController` / `KetQuaHienMauController` (sử dụng `Module5Service`), trường `phieu.NgayXuLy` không được cập nhật sang `clock.Now` (trong khi thao tác qua `AdminController` thì có cập nhật `phieu.NgayXuLy = DateTime.Now`).
   - **Cách khắc phục đề xuất**: Thành viên phụ trách Module 5 bổ sung `phieu.NgayXuLy = clock.Now;` khi thay đổi `phieu.TrangThai` trong `Module5Service.cs`.
