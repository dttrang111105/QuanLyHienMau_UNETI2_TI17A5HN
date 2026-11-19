# BÁO CÁO SỬA LỖI KIỂM TRA ĐIỀU KIỆN ĐĂNG KÝ HIẾN MÁU LẠI — MODULE 3

- **Dự án**: `QuanLyHienMau_UNETI2_TI17A5HN`
- **Phạm vi**: Module 3 – Quản lý người hiến máu và phiếu đăng ký hiến máu
- **Nhánh làm việc**: `integration`
- **Ngày thực hiện**: 28/10/2026

---

## 1. TỔNG QUAN PHÂN TÍCH CẤU TRÚC DỮ LIỆU & LOGIC HIỆN TẠI

### 1.1. Bảng và trường dữ liệu liên quan trong cơ sở dữ liệu
Qua rà soát trực tiếp các Entity, `ApplicationDbContext`, Migration và các Service/Controller trong dự án:

| Thành phần / Bảng | File định nghĩa | Trường liên quan | Ý nghĩa thực tế trong hệ thống |
|---|---|---|---|
| **Bảng phiếu đăng ký** (`PhieuDangKyHienMau`) | `Models/PhieuDangKyHienMau.cs` | `MaPhieu`, `MaNguoiHien`, `MaDot`, `NgayNop`, `TrangThai`, `NgayXuLy`, `GhiChuXacNhan` | Lưu thông tin mỗi lần người hiến nộp đơn đăng ký tham gia một đợt hiến máu. |
| **Trường ngày tạo phiếu** (`PhieuDangKyHienMau.NgayNop`) | `Models/PhieuDangKyHienMau.cs` | `DateTime NgayNop` | Ngày người hiến gửi form đăng ký trực tuyến (**KHÔNG PHẢI ngày hiến máu thực tế**). |
| **Trường ngày xử lý phiếu** (`PhieuDangKyHienMau.NgayXuLy`) | `Models/PhieuDangKyHienMau.cs` | `DateTime? NgayXuLy` | Ngày nhân viên/admin duyệt phiếu (`"Đã xác nhận"`), từ chối (`"Từ chối"`), người dùng hủy (`"Đã hủy"`), hoặc hoàn thành khám sàng lọc (`"Đã khám sàng lọc"`). Đặc biệt, trong `Services/Module5Service.cs` (`GhiKetQuaAsync`), khi ghi kết quả hiến máu thành công, `phieu.NgayXuLy` **không được cập nhật** mà vẫn giữ nguyên mốc duyệt phiếu trước đó (**KHÔNG ĐƯỢC dùng làm ngày hiến máu thực tế**). |
| **Trường trạng thái xác nhận hiến máu thành công** | `Models/PhieuDangKyHienMau.cs`, `Models/KetQuaHienMau.cs`, `Models/Module5TrangThai.cs` | `PhieuDangKyHienMau.TrangThai == "Hiến máu thành công"` và `KetQuaHienMau.KetQua == "Hiến máu thành công"` | Trạng thái xác nhận người hiến đã hoàn tất hiến máu thực tế. Các trạng thái khác (`"Chờ xác nhận"`, `"Đã xác nhận"`, `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`, `"Từ chối"`, `"Đã hủy"`, `"Không đủ điều kiện hiến"`) đều **chưa phải** là lần hiến máu thành công. |
| **Bảng kết quả hiến máu & trường ngày hoàn tất hiến máu thực tế** (`KetQuaHienMau`) | `Models/KetQuaHienMau.cs` | `KetQua` (`nvarchar(100)`), `NgayCapNhat` (`DateTime?`) | Quan hệ `1 - 0..1` với `PhieuDangKyHienMau` qua khóa ngoại duy nhất `MaPhieu`. Khi ghi nhận kết quả `"Hiến máu thành công"` (tại `Module5Service.GhiKetQuaAsync` và `AdminController.TaoKetQua` / `SuaKetQua`), hệ thống lưu thời điểm hoàn tất kết quả vào `KetQuaHienMau.NgayCapNhat`. |

### 1.2. Vấn đề về trường ngày hiến máu thực tế trong Schema và giải pháp tương thích
- **Phát hiện về Schema**: Cả bảng `PhieuDangKyHienMau` lẫn `KetQuaHienMau` không có cột mang tên riêng là `NgayHienMauThucTe` mà sử dụng `KetQuaHienMau.NgayCapNhat` (`DateTime?`, cho phép `null`) để lưu mốc thời gian ghi nhận kết quả hiến máu.
- **Sai sót của logic cũ trước khi sửa**:
  1. **Tại `Controllers/DangKyController.cs` (`Create [GET]` và `Create [POST]`)**:
     - Chỉ kiểm tra trùng phiếu đang hoạt động trong **cùng một đợt** (`p.MaDot == maDot && p.TrangThai != "Đã hủy" && p.TrangThai != "Từ chối"`).
     - **Hoàn toàn thiếu** bước kiểm tra khoảng thời gian chờ tối thiểu (`84 ngày`) kể từ lần hiến máu thành công gần nhất của người hiến khi người dùng đăng ký sang đợt hiến máu khác (hoặc gửi POST trực tiếp).
  2. **Tại `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 14–29 trước khi sửa)**:
     - Sắp xếp danh sách phiếu theo `OrderByDescending(p => p.NgayNop)` rồi lấy `FirstOrDefault()`, dẫn đến nếu có nhiều lần hiến máu thành công thì lấy theo ngày nộp phiếu (`NgayNop`) thay vì ngày hoàn tất hiến máu thực tế.
     - Sử dụng biểu thức fallback:
       `var lastSuccessDate = latestSuccessful?.KetQuaHienMau?.NgayCapNhat ?? latestSuccessful?.NgayXuLy ?? latestSuccessful?.NgayNop;`
       Khi `KetQuaHienMau.NgayCapNhat` bị trống (`null`), code cũ tự động lấy `NgayXuLy` (ngày duyệt phiếu) hoặc `NgayNop` (ngày tạo phiếu đăng ký) để tính 84 ngày chờ.
     - Hiển thị `"Đợt hiến gần nhất"` bằng `latestDonation?.NgayNop` kể cả khi phiếu đó mới chỉ đăng ký hoặc đã bị hủy.
  3. **Ghi nhận ngoài phạm vi Module 3 (không tự ý sửa theo nguyên tắc bảo vệ mã nguồn)**:
     - Tại `Controllers/AdminController.cs` (dòng 815–820, 927–931) và `Views/Admin/NguoiHienMau.cshtml` (dòng 142, 481), logic thống kê của Admin vẫn đang dùng `p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null` (tính nhầm cả kết quả `"Không đủ điều kiện hiến"`) và fallback `p.NgayXuLy ?? p.NgayNop`. Do quy định không tự ý sửa các module khác nếu không cần thiết, vấn đề này được ghi nhận rõ trong báo cáo để nhóm phụ trách Admin/Module 4–5 cập nhật đồng bộ.

---

## 2. DANH SÁCH FILE ĐÃ SỬA TRONG MODULE 3

Chỉ sửa 3 file thuộc đúng phạm vi **Module 3**, không thay đổi cấu trúc database và không chạm vào Module 1, 2, 4, 5, 6:

1. `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`
2. `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml`
3. `QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSoCaNhan.cshtml`

---

## 3. SO SÁNH LOGIC TRƯỚC VÀ SAU KHI SỬA

### 3.1. Xác định lần hiến máu thành công & ngày hiến thực tế (`DangKyController.cs`)
- **Trước khi sửa**:
  - Chưa có hàm kiểm tra thời gian chờ giữa các lần hiến máu trong `DangKyController`.
  - Ở `HoSoCaNhan.cshtml`, lấy `KetQuaHienMau?.NgayCapNhat ?? NgayXuLy ?? NgayNop` và sắp xếp theo `NgayNop`.
- **Sau khi sửa**:
  - Bổ sung hằng số chuẩn y tế của dự án: `public const int SoNgayChoToiThieuGiuaHaiLanHien = 84;` (84 ngày = 12 tuần / ~3 tháng).
  - Bổ sung phương thức `IsConfirmedSuccessfulDonation(PhieuDangKyHienMau? phieu)`:
    - Loại trừ tuyệt đối các phiếu có `TrangThai` là `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"` hoặc các trạng thái chưa hoàn tất hiến máu (`"Chờ xác nhận"`, `"Đã xác nhận"`, `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`).
    - Nếu có bản ghi `KetQuaHienMau`, bắt buộc `KetQuaHienMau.KetQua == "Hiến máu thành công"` (nếu `KetQua == "Không đủ điều kiện hiến"` sẽ trả về `false`).
  - Bổ sung phương thức `GetActualDonationDate(PhieuDangKyHienMau? phieu)`:
    - Chỉ lấy `phieu.KetQuaHienMau.NgayCapNhat` khi bản ghi kết quả xác nhận `"Hiến máu thành công"` và `NgayCapNhat.HasValue`.
    - **Tuyệt đối không lấy `NgayNop` (ngày tạo phiếu) hoặc `NgayXuLy` (ngày xử lý/duyệt phiếu) làm ngày hiến máu thực tế.**
  - Bổ sung phương thức `EvaluateRecentDonationEligibility(IEnumerable<PhieuDangKyHienMau>? lichSuPhieu, DateTime referenceDate)`:
    1. **Chưa từng hiến máu thành công** (`successfulDonations.Count == 0`): trả về `IsEligible = true`, không áp dụng thời gian chờ.
    2. **Có 1 hoặc nhiều lần hiến máu thành công có ngày hiến thực tế**: sắp xếp giảm dần theo `ActualDate` (`KetQuaHienMau.NgayCapNhat`), lấy lần gần nhất (`lastActualDate`), tính `nextEligibleDate = lastActualDate.Date.AddDays(84)`.
       - Nếu `referenceDate.Date < nextEligibleDate`: từ chối đăng ký (`IsEligible = false`) và trả về thông báo nêu rõ ngày hiến thành công gần nhất, mã phiếu `#DKHM-...`, và **ngày đủ điều kiện đăng ký lại (`nextEligibleDate:dd/MM/yyyy`)**.
       - Nếu `referenceDate.Date >= nextEligibleDate` (và không có bản ghi hiến thành công nào bị khuyết ngày hiến thực tế): cho phép tiếp tục đăng ký (`IsEligible = true`).
    3. **Có bản ghi xác nhận `"Hiến máu thành công"` nhưng thiếu ngày hiến thực tế (`KetQuaHienMau == null` hoặc `KetQuaHienMau.NgayCapNhat == null`)**:
       - Không tự ý lấy `NgayNop` hay `NgayXuLy` để tính 84 ngày.
       - Chặn đăng ký an toàn (`IsEligible = false`, `HasMissingActualDonationDate = true`) và hiển thị thông báo rõ mã phiếu `#DKHM-...` đang thiếu ngày hoàn tất hiến máu thực tế trong cơ sở dữ liệu, yêu cầu liên hệ nhân viên y tế cập nhật trước khi đăng ký lại.

### 3.2. Kiểm tra tại Server (`DangKyController.Create [GET]` & `Create [POST]`) và Giao diện (`Views/DangKy/Create.cshtml`, `Views/NguoiHienMau/HoSoCaNhan.cshtml`)
- **Tại `DangKyController.Create [GET]`**:
  - Xác định người dùng từ `ClaimTypes.NameIdentifier` (`maTaiKhoan` -> `NguoiHienMau`).
  - Truy vấn lịch sử phiếu của chính người hiến đó kèm `KetQuaHienMau` và gọi `EvaluateRecentDonationEligibility`.
  - Nếu chưa đủ điều kiện (`!recentEligibility.IsEligible`), gán `ViewBag.IsCooldownBlocked = true` và `ViewBag.ErrorMessage = recentEligibility.ErrorMessage` để cảnh báo rõ ràng trên trang đăng ký và khóa nút submit trên giao diện.
- **Tại `DangKyController.Create [POST]`**:
  - Xác định người hiến từ `ClaimTypes.NameIdentifier` (không nhận `MaNguoiHien` hay `MaTaiKhoan` từ request).
  - Kiểm tra `EvaluateRecentDonationEligibility` ngay ở đầu xử lý POST **và** kiểm tra lại một lần nữa bên trong khóa đồng bộ `SemaphoreSlim` + `IsolationLevel.Serializable` transaction trước khi thêm `PhieuDangKyHienMau`.
  - Nếu không đủ điều kiện, rollback transaction (nếu đang trong transaction) và trả về `ReturnInvalidForm(eligibility.ErrorMessage!)` — chặn triệt để mọi hành vi sửa HTML/JS hoặc gửi POST trực tiếp.
- **Tại `Views/DangKy/Create.cshtml` & `Views/NguoiHienMau/HoSoCaNhan.cshtml`**:
  - Hiển thị rõ ngày hiến máu thành công gần nhất (`dd/MM/yyyy`) và **ngày đủ điều kiện đăng ký lại (`dd/MM/yyyy`)** hoặc cảnh báo thiếu ngày hiến thực tế.

---

## 4. KẾT QUẢ KIỂM THỬ BẮT BUỘC (7 TRƯỜNG HỢP)

Đã thực thi kiểm thử tự động trực tiếp trên mã biên dịch của `DangKyController` (chạy trên .NET 10 SDK) cho toàn bộ 7 kịch bản bắt buộc (mốc ngày kiểm thử tham chiếu: `28/10/2026`):

| Mã TC | Trường hợp kiểm thử | Dữ liệu đầu vào | Kết quả thực tế | Đánh giá |
|---|---|---|---|---|
| **TC01** | Chưa từng hiến máu thành công | Người dùng chưa có phiếu nào, hoặc có 2 phiếu ở trạng thái `"Chờ xác nhận"` (nộp 3 ngày trước) và `"Đã khám sàng lọc"` (nộp 5 ngày trước). | `IsEligible = True`, `TotalSuccessfulDonations = 0`, `ErrorMessage = null`. Cho phép tiếp tục kiểm tra các điều kiện khác. | **PASS** |
| **TC02** | Đã hiến máu thành công nhưng chưa đủ thời gian chờ | Phiếu `#201` có `NgayNop = 30/06/2026` (120 ngày trước), `NgayXuLy = 05/07/2026` (115 ngày trước), nhưng `KetQuaHienMau.KetQua = "Hiến máu thành công"` có `NgayCapNhat = 28/09/2026` (30 ngày trước < 84 ngày). | Không dùng `NgayNop`/`NgayXuLy`. Trả về `IsEligible = False`, `LastActualDonationDate = 28/09/2026`, `NextEligibleDate = 21/12/2026`, `RemainingCooldownDays = 54`. Thông báo rõ: *"Ngày đủ điều kiện đăng ký hiến máu lại là từ ngày 21/12/2026 (còn 54 ngày)."* | **PASS** |
| **TC03** | Đã hiến máu thành công và đã đủ thời gian chờ | Phiếu `#301` có `KetQua = "Hiến máu thành công"`, `NgayCapNhat = 05/08/2026` (đúng 84 ngày trước). | `IsEligible = True`, `DaysSinceLastDonation = 84`, `RemainingCooldownDays = 0`, `ErrorMessage = null`. Cho phép đăng ký. | **PASS** |
| **TC04** | Có phiếu đăng ký bị hủy / từ chối / không đủ điều kiện hiến nhưng chưa từng hiến máu thành công | Phiếu `#401` (`"Đã hủy"`, 2 ngày trước), `#402` (`"Từ chối"`, 7 ngày trước), `#403` (`"Không đủ điều kiện hiến"`, `KetQua = "Không đủ điều kiện hiến"`, 10 ngày trước). | Không phiếu nào bị tính là lần hiến máu thực tế: `IsEligible = True`, `TotalSuccessfulDonations = 0`, `ErrorMessage = null`. | **PASS** |
| **TC05** | Có nhiều lần hiến máu thành công, phải lấy lần gần nhất | Phiếu `#501` (`NgayCapNhat` 180 ngày trước), Phiếu `#502` (`NgayNop` 40 ngày trước, `NgayCapNhat` **20 ngày trước** — `08/10/2026`), Phiếu `#503` (`MaPhieu` lớn hơn và `NgayNop` 35 ngày trước mới hơn `#502`, nhưng `NgayCapNhat` 100 ngày trước), Phiếu `#504` (`"Đã hủy"` hôm qua). | Lấy đúng phiếu `#502` theo `NgayCapNhat` gần nhất (`08/10/2026`), trả về `IsEligible = False`, `NextEligibleDate = 31/12/2026`, `RemainingCooldownDays = 64`. | **PASS** |
| **TC06** | Người dùng sửa ID hoặc gửi POST trực tiếp để vượt qua điều kiện | Kiểm tra signature và luồng xử lý của `DangKyController.Create [POST]`. | Action `[HttpPost] Create` có `[Authorize]`, `[ValidateAntiForgeryToken]`, không nhận `MaNguoiHien`/`MaTaiKhoan` từ client mà lấy từ `ClaimTypes.NameIdentifier`, và gọi `EvaluateRecentDonationEligibility` cả trước khi validate lẫn bên trong `Serializable` transaction để chặn POST trực tiếp. | **PASS** |
| **TC07** | Không có ngày hiến máu thực tế trong dữ liệu | Phiếu `#701` (`TrangThai = "Hiến máu thành công"`, `KetQuaHienMau = null`, `NgayNop` 150 ngày trước, `NgayXuLy` 140 ngày trước) và Phiếu `#702` (`KetQua = "Hiến máu thành công"`, `NgayCapNhat = null`, `NgayNop` 150 ngày trước). | Không lấy `NgayNop`/`NgayXuLy` thay thế. Trả về `IsEligible = False`, `HasMissingActualDonationDate = True`, `LastActualDonationDate = null`, chặn đăng ký và thông báo rõ mã phiếu `#DKHM-00702` thiếu `KetQuaHienMau.NgayCapNhat`. | **PASS** |

---

## 5. KẾT QUẢ BUILD & GIỚI HẠN MÔI TRƯỜNG KIỂM THỬ

- **Kết quả `dotnet build`**:
  - Lệnh: `dotnet build "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --no-restore`
  - Kết quả: **Build succeeded — `0 Error(s)`**, `0 Warning(s)` tại toàn bộ các file thuộc Module 3 (27 cảnh báo `CS8602` còn lại thuộc Module 5 và Module 6 không nằm trong phạm vi được phép chỉnh sửa).
- **Giới hạn môi trường chưa xác minh runtime trực tiếp trên trình duyệt + SQL Server LocalDB**:
  - Môi trường sandbox hiện tại chặn kết nối Named Pipes tới `(localdb)\mssqllocaldb`, do đó chưa thực hiện gửi request HTTP trực tiếp vào instance SQL Server LocalDB đang chạy. Toàn bộ logic điều kiện tại `DangKyController` đã được kiểm thử tự động trên assembly biên dịch thực tế (`.dll` .NET 10) với đầy đủ 7 kịch bản dữ liệu đầu vào.
