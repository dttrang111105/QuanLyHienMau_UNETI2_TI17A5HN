# BÁO CÁO KẾT QUẢ SỬA LỖI MODULE 3 — QUẢN LÝ NGƯỜI HIẾN MÁU VÀ PHIẾU ĐĂNG KÝ HIẾN MÁU

---

## 1. TỔNG QUAN

- **Thời gian thực hiện**: `2026-10-28 21:37:45+07:00` – `2026-10-28 21:53:00+07:00`.
- **Thư mục dự án**: `e:\InformationTechnology\visual tím\QuanLyHienMau_UNETI2_TI17A5HN` (Project: `QuanLyHienMau_UNETI2_TI17A5HN\QuanLyHienMau_UNETI2_TI17A5HN.csproj`).
- **Nhánh Git hiện tại**: `integration` (tracking `origin/integration`).
- **Trạng thái Git ban đầu trước khi sửa**:
  - Có sẵn 15 file ở trạng thái `Modified (M)` từ trước trong working tree và 1 file báo cáo audit `MODULE3_AUDIT_REPORT.md` (`??`).
  - Toàn bộ thay đổi sẵn có của người dùng và các thành viên khác trong nhóm được bảo toàn tuyệt đối; không reset, không ghi đè và không can thiệp vào logic của các module khác.
- **Kết quả build trước khi sửa**:
  - Lệnh: `dotnet build "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --no-restore`
  - Kết quả: **Build succeeded** (`Exit code 0`, `0 Error(s)`, `27 Warning(s)` thuộc Module 5 & 6).

---

## 2. DANH SÁCH LỖI ĐÃ XÁC MINH VÀ XỬ LÝ

Dưới đây là chi tiết từng lỗi được đối chiếu từ `MODULE3_AUDIT_REPORT.md`, nguyên nhân gốc, vị trí sửa và lý do giải pháp an toàn:

### LỖI 1 — Kiểm tra dữ liệu khi nộp phiếu đăng ký chưa chặt chẽ & Giao diện nộp phiếu tự điền sẵn dữ liệu giả
- **Mã lỗi**: `M3-BUG-01` (Tương ứng Lỗi số 1 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **HIGH**
- **Nguyên nhân gốc**:
  1. Trong `Views/DangKy/Create.cshtml` (dòng 203, 217, 224, 230, 286), khi hồ sơ người hiến máu chưa có dữ liệu (`NgaySinh`, `SoDienThoai`, `Email`, `NgheNghiep`, `CanNang` bằng `null`), giao diện tự động điền các giá trị mẫu giả định (`"2003-08-14"`, `"0988 123 456"`, `"sinhvien.uneti@gmail.com"`, `"Lớp DHTI17A5HN - Khoa CNTT"`, `64` kg) vào thuộc tính `value` của các ô input, đồng thời các ô `NgaySinh`, `GioiTinh`, `NgheNghiep` thiếu thuộc tính `name`.
  2. Trong `Controllers/DangKyController.cs` (`Create [POST]`, dòng 195–266), server chỉ kiểm tra `HoTen`, `SoDienThoai`, `CanNang` khác rỗng và `CanNang >= dotHienMau.CanNangToiThieu`, nhưng **không** kiểm tra định dạng số điện thoại (`^0[0-9]{9,10}$`), định dạng Email (`EmailAddressAttribute`), trùng lặp Email với tài khoản khác, độ tuổi hợp lệ (`18–60` tuổi), giới hạn cân nặng (`30–200 kg`) và giới hạn độ dài chuỗi (`StringLength`).
  3. Ngoài ra, nếu gọi trực tiếp `POST /DangKy/Create` khi tài khoản chưa có bản ghi `NguoiHienMau`, hệ thống trả về `NotFound` (HTTP 404) thay vì từ chối tạo phiếu và điều hướng người dùng về trang cập nhật hồ sơ kèm thông báo.
- **File / phương thức đã sửa**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml` (dòng 200–236, 280–291)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (`Create [GET]` và `Create [POST]`, dòng 34–335)
- **Mô tả thay đổi**:
  1. Loại bỏ toàn bộ các giá trị mẫu giả định trên `Views/DangKy/Create.cshtml`, chỉ hiển thị dữ liệu thực tế từ hồ sơ `NguoiHienMau` của tài khoản đang đăng nhập; bổ sung đầy đủ thuộc tính `name="ngaySinh"`, `name="gioiTinh"`, `name="ngheNghiep"`.
  2. Trong `DangKyController.Create [POST]`:
     - Xác định người hiến máu từ `ClaimTypes.NameIdentifier` (`maTaiKhoan`), kiểm tra tài khoản và hồ sơ người hiến máu tồn tại và đều ở trạng thái `"Hoạt động"`. Nếu chưa có hồ sơ `NguoiHienMau`, từ chối tạo phiếu, gán `TempData["ErrorMessage"]` và chuyển hướng về `NguoiHienMau/HoSoCaNhan`.
     - Kiểm tra đầy đủ ở phía server: `HoTen` bắt buộc (`<= 100` ký tự); `NgaySinh` hợp lệ (không lớn hơn ngày hiện tại, độ tuổi từ `18` đến `60` tuổi); `SoDienThoai` bắt buộc và khớp Regex `^0[0-9]{9,10}$`; `Email` đúng định dạng (`<= 100` ký tự) và không trùng với tài khoản khác; `CanNang` bắt buộc, nằm trong khoảng `30–200 kg` và đạt `>= dotHienMau.CanNangToiThieu`; kiểm tra câu hỏi tự sàng lọc `health_q1`, `health_q2` và độ dài `LyDoHienMau`, `GhiChu` (`<= 500` ký tự).
     - Giữ nguyên việc thiết lập `NgayNop = DateTime.Now` và `TrangThai = "Chờ xác nhận"` từ phía server.
- **Vì sao giải pháp an toàn**: Chỉ sửa trong `DangKyController.Create` và `Views/DangKy/Create.cshtml` của Module 3; áp dụng đúng quy ước trạng thái và quy tắc validation hiện có của dự án, không thay đổi schema CSDL hay ảnh hưởng đến module khác.
- **Trạng thái**: **`FIXED`**

---

### LỖI 2 — Quy tắc hủy phiếu đăng ký chưa chặt chẽ
- **Mã lỗi**: `M3-BUG-02` (Tương ứng Lỗi số 2 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **HIGH**
- **Nguyên nhân gốc**:
  1. Trong `Controllers/DangKyController.cs` (`Huy [POST]`), hệ thống cho phép hủy phiếu ở trạng thái `"Chờ xác nhận"` và `"Đã xác nhận"`, nhưng **không kiểm tra điều kiện phiếu đã có lịch khám sàng lọc đang hoạt động hay chưa** (`phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy")`). Thay vào đó, code cũ tự động chuyển toàn bộ lịch khám sàng lọc chưa hoàn thành sang `"Đã hủy"`.
  2. Trên giao diện `Views/DangKy/TheoDoi.cshtml`, nút `"Hủy phiếu đăng ký"` vẫn hiển thị khi phiếu ở trạng thái `"Đã xác nhận"` dù đã được xếp lịch khám sàng lọc.
  3. Ngoài ra, Action `DangKyController.Huy` (thuộc cổng người hiến máu Module 3) cần đảm bảo chỉ chủ sở hữu hợp lệ của phiếu (`phieu.MaNguoiHien == nguoiHienMau.MaNguoiHien`) mới được phép thực hiện hủy phiếu của chính mình.
- **File / phương thức đã sửa**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (`Huy [POST]`, dòng 485–570)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml` (dòng 150–160)
- **Mô tả thay đổi**:
  1. Trong `DangKyController.Huy [POST]`:
     - Kiểm tra trạng thái hoạt động của `TaiKhoan` đang đăng nhập.
     - Kiểm tra quyền sở hữu nghiêm ngặt ở phía server: `if (nguoiHienMau == null || phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien) return Forbid();`.
     - Chỉ cho phép hủy khi `phieu.TrangThai` là `"Chờ xác nhận"` hoặc `"Đã xác nhận"`, **VÀ** chưa có kết quả hiến máu (`phieu.KetQuaHienMau == null`), **VÀ** chưa có lịch khám sàng lọc đang hoạt động (`!phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy")`).
     - Nếu phiếu đã có lịch khám sàng lọc đang hoạt động, từ chối yêu cầu hủy và trả về thông báo rõ ràng qua `TempData["ErrorMessage"]`.
  2. Trong `Views/DangKy/TheoDoi.cshtml`: Chỉ hiển thị nút `"Hủy phiếu đăng ký"` khi `canCancelTicket == true` (`(trangThai == "Chờ xác nhận" || trangThai == "Đã xác nhận") && !hasActiveScreening && Model.KetQuaHienMau == null`).
- **Vì sao giải pháp an toàn**: Chỉ điều chỉnh trong `DangKyController.Huy` và `Views/DangKy/TheoDoi.cshtml` của Module 3; giữ nguyên toàn bộ các Action duyệt phiếu, từ chối phiếu và hủy lịch khám sàng lọc của cán bộ/quản trị viên trong `AdminController` và `Module5Service`.
- **Trạng thái**: **`FIXED`**

---

### LỖI 3 — Ngăn chặn đăng ký trùng khi có nhiều request đồng thời (Race Condition / Concurrency)
- **Mã lỗi**: `M3-BUG-03` (Tương ứng Lỗi số 3 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **HIGH**
- **Nguyên nhân gốc**: Trong `DangKyController.Create [POST]`, bước kiểm tra phiếu trùng (`FirstOrDefaultAsync`) và bước lưu phiếu mới (`SaveChangesAsync`) trước đây thực thi độc lập, không có cơ chế khóa đồng bộ hoặc giao dịch mức cô lập `Serializable`. Khi hai request được gửi gần như đồng thời cho cùng một người hiến và cùng một đợt hiến máu, cả hai có thể cùng đọc thấy `existingPhieu == null` và tạo ra 2 phiếu trùng.
- **File / phương thức đã sửa**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (`_submissionLocks` và `Create [POST]`, dòng 22, 337–399)
- **Mô tả thay đổi**:
  - Triển khai **cơ chế bảo vệ 2 lớp mà không cần thay đổi cấu trúc database dùng chung hay tạo migration mới**:
    1. **Lớp khóa đồng bộ theo người hiến tại tầng ứng dụng (`ConcurrentDictionary<int, SemaphoreSlim>`)**: Mỗi `MaNguoiHien` có một `SemaphoreSlim(1, 1)` riêng (`await userLock.WaitAsync()`), đảm bảo các request nộp phiếu đồng thời từ cùng một người hiến máu được xếp hàng tuần tự.
    2. **Lớp giao dịch cơ sở dữ liệu mức cô lập cao nhất (`IsolationLevel.Serializable`)**: Đặt toàn bộ khối truy vấn kiểm tra phiếu trùng (`p.MaNguoiHien == nguoiHienMau.MaNguoiHien && p.MaDot == maDot && p.TrangThai != "Đã hủy" && p.TrangThai != "Từ chối"`) và lệnh `_context.SaveChangesAsync()` bên trong `await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable)`. Trong SQL Server, `Serializable` giữ khóa dải (Key-Range Lock) cho đến khi transaction commit, ngăn chặn triệt để việc chèn bản ghi trùng từ bất kỳ kết nối đồng thời nào, đồng thời vẫn cho phép người hiến đăng ký lại bình thường nếu phiếu trước đó đã chuyển sang trạng thái `"Đã hủy"` hoặc `"Từ chối"`.
- **Vì sao giải pháp an toàn**: Không sửa đổi `ApplicationDbContext`, không tạo migration mới, không xóa hay thay đổi bất kỳ dữ liệu hiện có nào trong CSDL dùng chung.
- **Trạng thái**: **`FIXED`**

---

### LỖI 4 — Đồng bộ Email giữa `NguoiHienMau` và `TaiKhoan` thiếu kiểm tra trùng lặp
- **Mã lỗi**: `M3-BUG-04` (Tương ứng Lỗi số 4 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **MEDIUM**
- **Nguyên nhân gốc**: Khi người hiến máu cập nhật `Email` trong `NguoiHienMauController.HoSoCaNhan [POST]` (hoặc cập nhật nhanh khi nộp phiếu trong `DangKyController.Create [POST]`), hệ thống đồng bộ `Email` sang bảng `TaiKhoan` mà không kiểm tra xem địa chỉ email đó đã được sử dụng bởi một tài khoản khác (`MaTaiKhoan != maTaiKhoan`) hay chưa.
- **File / phương thức đã sửa**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs` (`HoSoCaNhan [POST]`, dòng 175–191)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (`Create [POST]`, dòng 285–299, 369–376)
- **Mô tả thay đổi**:
  - Kiểm tra `await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != maTaiKhoan && t.Email != null && t.Email.ToLower() == email.ToLower())` trước khi cho phép lưu và đồng bộ Email giữa `NguoiHienMau` và `TaiKhoan`. Nếu email đã thuộc về tài khoản khác, hệ thống từ chối lưu và trả về thông báo lỗi rõ ràng.
- **Vì sao giải pháp an toàn**: Chỉ bổ sung truy vấn đọc kiểm tra tính duy nhất của Email theo đúng quy tắc của `AccountController.Register`, bảo vệ tính nhất quán giữa Module 1 (`TaiKhoan`) và Module 3 (`NguoiHienMau`).
- **Trạng thái**: **`FIXED`**

---

### LỖI 5 — Trang theo dõi phiếu (`TheoDoi.cshtml`) không hiển thị lý do từ chối / không đủ điều kiện và hành vi fallback khi tra cứu mã phiếu không hợp lệ
- **Mã lỗi**: `M3-BUG-05` (Tương ứng Lỗi số 5 & Lỗi số 6 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **MEDIUM**
- **Nguyên nhân gốc**:
  1. Trong `Views/DangKy/TheoDoi.cshtml` (dòng 224–233), giao diện chỉ kiểm tra `@if (trangThai == "Đã hủy")` để hiển thị `Model.GhiChuXacNhan`. Khi phiếu ở trạng thái `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`, `stepNumber` bằng `-1` làm xám toàn bộ 5 bước và ẩn mất lý do từ chối (`GhiChuXacNhan`) do cán bộ y tế nhập.
  2. Trong `DangKyController.TheoDoi [GET]`, khi người dùng truyền `?id=999` (phiếu của người khác hoặc không tồn tại) hoặc `?code=DKHM-99999`, hệ thống vẫn chạy lệnh fallback `selectedPhieu = userPhieuList.FirstOrDefault()`, khiến trang vừa hiển thị thông báo lỗi (hoặc không báo lỗi với `code`) vừa hiển thị phiếu đầu tiên của người dùng bên dưới.
- **File / phương thức đã sửa**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml` (dòng 190–265, 459–545)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (`TheoDoi [GET]`, dòng 435–482)
- **Mô tả thay đổi**:
  1. Trong `Views/DangKy/TheoDoi.cshtml`: Bổ sung khối hiển thị thông báo cho trạng thái `"Từ chối"` và `"Không đủ điều kiện hiến"`, hiển thị rõ `Model.GhiChuXacNhan` (hoặc `Model.KetQuaHienMau?.NhanXet`) và thời điểm cập nhật `Model.NgayXuLy` cho chủ sở hữu hợp lệ của phiếu; đồng thời loại bỏ các giá trị hiển thị giả định (`"O Rh(+)"`, `64 kg`).
  2. Trong `DangKyController.TheoDoi [GET]`: Khi người dùng chủ động tra cứu theo `id` hoặc `code` nhưng phiếu không tồn tại hoặc thuộc về tài khoản khác (`explicitLookupFailed = true`), hệ thống gán `TempData["ErrorMessage"]` và giữ `selectedPhieu = null` (không hiển thị nhầm sang phiếu khác).
- **Vì sao giải pháp an toàn**: Chỉ thay đổi cách hiển thị trên View và Controller của Module 3, đảm bảo quyền riêng tư (chỉ chủ sở hữu hợp lệ mới xem được phiếu và lý do từ chối của mình).
- **Trạng thái**: **`FIXED`**

---

### LỖI 6 — Sai tên Action liên kết quản lý phiếu đăng ký trên trang danh sách người hiến máu
- **Mã lỗi**: `M3-BUG-06` (Tương ứng Lỗi số 8 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **MEDIUM**
- **Nguyên nhân gốc**: Trong `Views/Admin/NguoiHienMau.cshtml` (dòng 547), nút `"Xem các phiếu đăng ký của người này"` sử dụng `asp-controller="Admin" asp-action="PhieuDangKy" asp-route-search="@donor.HoTen"`, dẫn tới lỗi HTTP 404 vì Action thực tế trong `AdminController.cs` (dòng 396) có tên là `PhieuDangKyHienMau(string? tenNguoiHien, ...)`.
- **File / phương thức đã sửa**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/NguoiHienMau.cshtml` (dòng 547)
- **Mô tả thay đổi**: Sửa đúng 1 dòng liên kết thành `asp-controller="Admin" asp-action="PhieuDangKyHienMau" asp-route-tenNguoiHien="@donor.HoTen"`.
- **Vì sao giải pháp an toàn**: Không sửa `AdminController.cs`, giữ nguyên phân quyền của Admin/Nhân viên y tế, chỉ trỏ đúng về route Action sẵn có.
- **Trạng thái**: **`FIXED`**

---

### CÁC LỖI KHÁC TRONG BÁO CÁO AUDIT THUỘC MODULE 3

#### LỖI 7 — Kiểm tra trạng thái hoạt động của `TaiKhoan` và `NguoiHienMau` khi cập nhật hồ sơ cá nhân
- **Mã lỗi**: `M3-BUG-07` (Tương ứng Lỗi số 7 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **MEDIUM**
- **Nguyên nhân gốc**: `NguoiHienMauController.HoSoCaNhan [POST]` chưa kiểm tra `TaiKhoan.TrangThai == "Hoạt động"` và `nguoiHienMau.TrangThai == "Hoạt động"` trước khi cho phép cập nhật hồ sơ; đồng thời nếu gọi POST khi chưa có bản ghi `NguoiHienMau` thì trả về `NotFound`.
- **File / phương thức đã sửa**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs` (`HoSoCaNhan [GET/POST]`, dòng 43–118)
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` (`Create [GET/POST]`, `Huy [POST]`)
- **Mô tả thay đổi**: Bổ sung kiểm tra `taiKhoan.TrangThai == "Hoạt động"` và `nguoiHienMau.TrangThai == "Hoạt động"` ở phía server trước khi thực hiện các thao tác nghiệp vụ.
- **Vì sao giải pháp an toàn**: Tăng cường bảo mật phía server trong nội bộ 2 Controller của Module 3, không thay đổi cấu hình Cookie hay Authentication chung của toàn hệ thống.
- **Trạng thái**: **`FIXED`**

#### LỖI 8 — Số liệu thống kê số lần hiến máu và chu kỳ phục hồi 84 ngày trên `HoSoCaNhan.cshtml` bị tính sai / hardcode
- **Mã lỗi**: `M3-BUG-08` (Tương ứng Lỗi số 9 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **LOW**
- **Nguyên nhân gốc**: Trong `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 16), biến `totalCount` lấy `phieuList.Count(p => p.TrangThai != "Đã hủy")` khi `successfulDonations.Count == 0`, làm cho người mới nộp phiếu `"Chờ xác nhận"` hoặc bị `"Từ chối"` vẫn được tính là đã hiến máu; đồng thời hộp chu kỳ phục hồi 84 ngày (dòng 151–167) và thông tin fallback trên chứng nhận điện tử (dòng 514–520) bị hardcode.
- **File / phương thức đã sửa**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 14–37, 132–184, 523–544)
- **Mô tả thay đổi**: Sửa `var totalCount = successfulDonations.Count;`, tính toán động số ngày phục hồi `daysSinceLastSuccess` và phần trăm `recoveryPercent` trên chu kỳ 84 ngày kể từ lần hiến máu thành công gần nhất, và loại bỏ các thông tin cá nhân mẫu giả định trên thẻ chứng nhận.
- **Vì sao giải pháp an toàn**: Chỉ sửa logic hiển thị trên `Views/NguoiHienMau/HoSoCaNhan.cshtml`.
- **Trạng thái**: **`FIXED`**

#### LỖI 9 — Lặp thông báo lỗi validation trong `NguoiHienMauController.HoSoCaNhan [POST]`
- **Mã lỗi**: `M3-BUG-09` (Tương ứng Lỗi số 10 phần 1 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **LOW**
- **Nguyên nhân gốc**: Do model `NguoiHienMau` đã có các DataAnnotations (`[Required]`, `[Range]`, `[RegularExpression]`, `[EmailAddress]`), khi model binding thất bại và `HoSoCaNhan [POST]` lại gọi thêm `ModelState.AddModelError` cho cùng các trường đó, mỗi trường bị hiển thị lặp 2 câu báo lỗi.
- **File / phương thức đã sửa**:
  - `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs` (`HoSoCaNhan [POST]`, dòng 119–215)
- **Mô tả thay đổi**: Gọi `ModelState.Clear()` trước khi thực hiện bộ quy tắc kiểm tra server-side đầy đủ trong `HoSoCaNhan [POST]` (bao gồm cả kiểm tra độ dài chuỗi `StringLength` cho toàn bộ các trường).
- **Vì sao giải pháp an toàn**: Chỉ tác động tới `ModelState` của Action `HoSoCaNhan [POST]`, không sửa đổi `Models/NguoiHienMau.cs`.
- **Trạng thái**: **`FIXED`**

#### LỖI 10 — Thiếu cập nhật `NgayXuLy` trong `Services/Module5Service.cs`
- **Mã lỗi**: `M3-BUG-10` (Tương ứng Lỗi số 10 phần 2 trong Báo cáo Audit)
- **Mức độ nghiêm trọng**: **LOW**
- **Nguyên nhân gốc**: `Services/Module5Service.cs` thuộc phạm vi Module 5 (Lịch khám sàng lọc & Kết quả hiến máu) và hiện đang có thay đổi chưa commit (`M`) của thành viên phụ trách Module 5 trong working tree.
- **File / phương thức liên quan**: `QuanLyHienMau_UNETI2_TI17A5HN/Services/Module5Service.cs`
- **Lý do không can thiệp**: Tuân thủ nghiêm ngặt Nguyên tắc bảo vệ mã nguồn số 3 và số 7 (*"Không ghi đè, xóa hoặc hoàn tác các thay đổi chưa commit của người dùng hoặc thành viên khác"*, *"Không sửa trực tiếp chức năng của Module 1, Module 2, Module 4, Module 5, Module 6"*). Trên giao diện `Views/DangKy/TheoDoi.cshtml` (dòng 400), Module 3 đã xử lý an toàn bằng cách đọc `Model.KetQuaHienMau?.NgayCapNhat ?? Model.NgayXuLy`.
- **Trạng thái**: **`NOT APPLICABLE` (Ngoài phạm vi sửa của Module 3 – Đã xử lý hiển thị tương thích tại View Module 3)**

---

## 3. KIỂM THỬ (TC01 – TC22)

> **Lưu ý minh bạch về môi trường kiểm thử**:
> - Toàn bộ 22 trường hợp kiểm thử (`TC01`–`TC22`) đã được đối chiếu và xác minh trực tiếp trên mã nguồn sau khi sửa, kết hợp biên dịch toàn bộ dự án bằng `dotnet build` (`0 Error(s)`).
> - Do môi trường sandbox cô lập tiến trình chặn kết nối named-pipe tới SQL Server LocalDB (`(localdb)\mssqllocaldb` báo `Login timeout expired`) và giải pháp hiện không có project Unit Test riêng, đồng thời tuân thủ nghiêm ngặt quy tắc **không tuyên bố `PASS` nếu chưa thực thi runtime trên database và không tự tạo/xóa database dùng chung**, các trường hợp cần truy vấn/ghi CSDL runtime được ghi nhận trung thực là **`BLOCKED` (ở bước kết nối LocalDB trong sandbox)** kèm kết quả xác minh logic code đã sửa.

| Mã test | Mục tiêu | Kết quả mong đợi | Kết quả thực tế (Sau khi sửa) | Trạng thái | Ghi chú / Vị trí code xác minh |
|---|---|---|---|---|---|
| **TC01** | Người chưa đăng nhập nộp phiếu | Bị yêu cầu đăng nhập hoặc bị từ chối | `DangKyController` có `[Authorize]`; `Create [GET/POST]` kiểm tra `ClaimTypes.NameIdentifier` và trả về `Challenge()` / chuyển hướng `/Account/Login`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 15, 38–42, 162–166). |
| **TC02** | Người hiến chưa có hồ sơ nộp phiếu (`POST /DangKy/Create`) | Không tạo phiếu, thông báo và hướng dẫn cập nhật hồ sơ | Khi `nguoiHienMau == null`, `Create [POST]` không tạo phiếu, gán `TempData["ErrorMessage"]` và chuyển hướng về `NguoiHienMau/HoSoCaNhan`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 179–183). |
| **TC03** | Hồ sơ thiếu trường bắt buộc (`HoTen`, `SoDienThoai`, `CanNang`) | Không tạo phiếu và trả về thông báo lỗi | `Views/DangKy/Create.cshtml` đã xóa toàn bộ dữ liệu mẫu giả định; `Create [POST]` kiểm tra `finalHoTen`, `finalPhone`, `finalWeight` và từ chối nếu thiếu. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Views/DangKy/Create.cshtml` (dòng 217, 224, 286), `Controllers/DangKyController.cs` (dòng 240–310). |
| **TC04** | Đợt hiến máu không tồn tại | Không tạo phiếu, báo lỗi | `Create [GET/POST]` kiểm tra `if (dotHienMau == null)`, gán `TempData["ErrorMessage"]` và chuyển về `DotHienMau/Index`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 92–96, 196–200). |
| **TC05** | Đợt hiến máu đã đóng (`TrangThai != "Đang mở đăng ký"`) | Không tạo phiếu, báo lỗi | `Create [GET/POST]` kiểm tra `dotHienMau.TrangThai != "Đang mở đăng ký"` và từ chối. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 99–103, 203–207). |
| **TC06** | Ngoài thời gian đăng ký (`< NgayBatDauDangKy` hoặc `> HanDangKy`) | Không tạo phiếu, báo lỗi | `Create [GET/POST]` kiểm tra `now.Date < dotHienMau.NgayBatDauDangKy.Date` và `now.Date > dotHienMau.HanDangKy.Date` ở phía server. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 106–116, 210–220). |
| **TC07** | Cân nặng dưới mức tối thiểu của đợt (`< CanNangToiThieu`) hoặc âm / `> 200 kg` | Không tạo phiếu, báo lỗi | `Create [POST]` kiểm tra `!finalWeight.HasValue || finalWeight.Value <= 0`, `< 30 || > 200` và `< dotHienMau.CanNangToiThieu`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 307–322). |
| **TC08** | Nộp phiếu hợp lệ | Tạo đúng một phiếu với `NgayNop = DateTime.Now` và `TrangThai = "Chờ xác nhận"` | Lưu phiếu trong Transaction, commit và chuyển hướng sang `/DangKy/TheoDoi/{MaPhieu}` kèm thông báo thành công. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 378–399). |
| **TC09** | Nộp phiếu trùng khi đã có phiếu đang xử lý | Bị từ chối theo quy tắc nghiệp vụ | Truy vấn kiểm tra `p.MaNguoiHien == nguoiHienMau.MaNguoiHien && p.MaDot == maDot && p.TrangThai != "Đã hủy" && p.TrangThai != "Từ chối"`, rollback và chuyển sang trang `TheoDoi` phiếu hiện có. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 119–129, 344–356). |
| **TC10** | Hai request nộp phiếu đồng thời (Concurrency) | Không tạo phiếu trùng trái quy tắc | Được bảo vệ bởi 2 lớp: `SemaphoreSlim` theo `MaNguoiHien` (`_submissionLocks`) + Database Transaction với `IsolationLevel.Serializable`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 22, 338–404). |
| **TC11** | Người hiến sửa hồ sơ của chính mình với dữ liệu hợp lệ | Thành công và đồng bộ `HoTen`, `Email` sang `TaiKhoan` | `NguoiHienMauController.HoSoCaNhan [POST]` kiểm tra hợp lệ và lưu các trường trong whitelist. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/NguoiHienMauController.cs` (dòng 82–275). |
| **TC12** | Người hiến sửa hồ sơ của người khác (thay đổi `MaNguoiHien` / `MaTaiKhoan` trong form) | Bị từ chối / Bỏ qua ID giả mạo | Server lấy `maTaiKhoan` từ `ClaimTypes.NameIdentifier` và chỉ cập nhật bản ghi `NguoiHienMau` của chính tài khoản đang đăng nhập. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/NguoiHienMauController.cs` (dòng 84–100, 249–260). |
| **TC13** | Truy cập phiếu của người khác qua URL (`/DangKy/TheoDoi?id=...` hoặc `?code=...`) | Bị từ chối và không hiển thị dữ liệu phiếu của người khác | `TheoDoi [GET]` chỉ tìm trong `userPhieuList` của người đang đăng nhập; khi không tìm thấy sẽ gán `TempData["ErrorMessage"]` và giữ `selectedPhieu = null`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 424–482). |
| **TC14** | Hủy phiếu đang ở trạng thái được phép (`"Chờ xác nhận"` hoặc `"Đã xác nhận"` chưa có lịch khám) | Hủy thành công, chuyển sang `"Đã hủy"` | Cập nhật `phieu.TrangThai = "Đã hủy"`, `phieu.NgayXuLy = DateTime.Now` và lưu DB. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 520–569). |
| **TC15** | Hủy phiếu đã có lịch khám sàng lọc hoặc đã qua khám sàng lọc | Bị từ chối ở phía server | Chặn cả khi `phieu.TrangThai` là `"Chờ khám sàng lọc"`/`"Đã khám sàng lọc"` lẫn khi phiếu `"Đã xác nhận"` nhưng `phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy")`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 520–558). |
| **TC16** | Hủy phiếu đã có kết quả cuối cùng (`"Hiến máu thành công"` / `"Không đủ điều kiện hiến"` / `KetQuaHienMau != null`) | Bị từ chối ở phía server | Chặn theo `phieu.TrangThai` và `if (phieu.KetQuaHienMau != null)` trước khi lưu DB. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 531–551). |
| **TC17** | Hủy phiếu của người khác (`POST /DangKy/Huy`) | Bị từ chối (`Forbid()` / HTTP 403) | Kiểm tra `if (nguoiHienMau == null || phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien) return Forbid();`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 514–518). |
| **TC18** | Hủy phiếu đã hủy trước đó | Không được hủy lại, báo lỗi | Khi `phieu.TrangThai == "Đã hủy"`, từ chối và báo lỗi `"Phiếu đăng ký này đã được hủy trước đó."`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 523–526). |
| **TC19** | Cập nhật Email trùng với tài khoản khác | Bị từ chối ở phía server | Kiểm tra `AnyAsync(t => t.MaTaiKhoan != maTaiKhoan && t.Email != null && t.Email.ToLower() == email.ToLower())` ở cả `HoSoCaNhan [POST]` và `DangKy/Create [POST]`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/NguoiHienMauController.cs` (dòng 183–189), `Controllers/DangKyController.cs` (dòng 292–298). |
| **TC20** | Liên kết quản lý phiếu từ danh sách người hiến máu | Route hợp lệ (`/Admin/PhieuDangKyHienMau?tenNguoiHien=...`), đúng quyền | Đã sửa `asp-action="PhieuDangKyHienMau" asp-route-tenNguoiHien="@donor.HoTen"` trong `Views/Admin/NguoiHienMau.cshtml` khớp với `AdminController.PhieuDangKyHienMau`. | `PASS (Static Route & Build Verification)` | `Views/Admin/NguoiHienMau.cshtml` (dòng 547), `Controllers/AdminController.cs` (dòng 396). |
| **TC21** | Theo dõi phiếu bình thường | Hiển thị đúng dữ liệu của chủ sở hữu | Truy vấn lọc theo `p.MaNguoiHien == nguoiHienMau.MaNguoiHien`, hiển thị đúng mã phiếu, đợt, đơn vị tổ chức, lịch khám và kết quả. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Controllers/DangKyController.cs` (dòng 424–432), `Views/DangKy/TheoDoi.cshtml`. |
| **TC22** | Xem lý do từ chối hoặc không đủ điều kiện | Hiển thị đúng phạm vi quyền hạn cho chủ sở hữu phiếu | Khi `trangThai == "Từ chối"` hoặc `"Không đủ điều kiện hiến"`, `Views/DangKy/TheoDoi.cshtml` hiển thị hộp thông báo kèm `Model.GhiChuXacNhan` và `Model.NgayXuLy`. | `BLOCKED` *(Đạt logic code & build; chặn kết nối LocalDB trong sandbox)* | `Views/DangKy/TheoDoi.cshtml` (dòng 238–256). |

---

## 4. KẾT QUẢ BUILD

- **Lệnh đã chạy**:
  ```powershell
  dotnet build "QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN.csproj" --no-restore
  ```
- **Trạng thái build**: **THÀNH CÔNG (`Build succeeded` – Exit code `0`)**.
- **Bảng so sánh trước và sau khi sửa**:

| Chỉ số | Trước khi sửa | Sau khi sửa | Đánh giá |
|---|---|---|---|
| Số lỗi biên dịch (`Error`) | `0` | `0` | Không có lỗi biên dịch. |
| Số cảnh báo toàn dự án (`Warning`) | `27` | `27` | Giữ nguyên 27 cảnh báo `CS8602` sẵn có ở Module 5 & 6. |
| Số cảnh báo thuộc Module 3 | `0` | `0` | **0 lỗi, 0 cảnh báo** trên toàn bộ các file thuộc Module 3. |

---

## 5. ẢNH HƯỞNG ĐẾN MODULE KHÁC

1. **Các file dùng chung đã thay đổi**:
   - **Không thay đổi** bất kỳ file dùng chung nào (`ApplicationDbContext.cs`, `Program.cs`, `NguoiHienMau.cs`, `PhieuDangKyHienMau.cs`, `Module5TrangThai.cs`, `Migrations/*` đều được giữ nguyên 100%).
   - Chỉ có 1 dòng sửa liên kết bị lỗi 404 tại `Views/Admin/NguoiHienMau.cshtml` (dòng 547: `asp-action="PhieuDangKy"` → `asp-action="PhieuDangKyHienMau" asp-route-tenNguoiHien="@donor.HoTen"`).
2. **Các module có thể bị ảnh hưởng**:
   - Module 1 (`AccountController`): Được bảo vệ tốt hơn nhờ kiểm tra không cho phép đổi Email trong hồ sơ người hiến máu trùng với Email của tài khoản khác.
   - Module 2 (`DotHienMauController`), Module 4 (`AdminController`), Module 5 (`LichKhamSangLocController`, `KetQuaHienMauController`, `Module5Service`), Module 6 (`ThongKeController`): Hoàn toàn không bị ảnh hưởng về mặt cấu trúc dữ liệu, route hay phương thức.
3. **Kiểm thử hồi quy đã thực hiện**:
   - Biên dịch toàn bộ Solution/Project (`dotnet build`) xác nhận tất cả Controller, Service, ViewModel và Razor Views của cả 6 module đều biên dịch thành công.
   - Kiểm tra đối chiếu các chuỗi trạng thái (`"Chờ xác nhận"`, `"Đã xác nhận"`, `"Từ chối"`, `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`, `"Hiến máu thành công"`, `"Không đủ điều kiện hiến"`, `"Đã hủy"`) đảm bảo khớp 100% với `Module5TrangThai.Phieu` và `AdminController`.
   - Kiểm tra `git diff --check` đảm bảo không có lỗi khoảng trắng hay xung đột cú pháp.
4. **Những ảnh hưởng chưa thể xác minh**:
   - Chưa chạy kiểm thử tương tác giao diện trình duyệt kết nối trực tiếp vào tiến trình SQL Server LocalDB do giới hạn sandbox cô lập named-pipe.

---

## 6. CÁC VẤN ĐỀ CÒN TỒN TẠI & GIỚI HẠN

1. **Về đồng bộ trạng thái phiếu trong `Services/Module5Service.cs` (`M3-BUG-10` - mức `LOW`)**:
   - **Nguyên nhân chưa sửa trực tiếp**: File `Services/Module5Service.cs` thuộc phạm vi Module 5 và đang có thay đổi chưa commit của thành viên khác. Để tuân thủ nguyên tắc không chạm vào code của module khác, Module 3 đã xử lý hiển thị dự phòng tại `Views/DangKy/TheoDoi.cshtml` (`Model.KetQuaHienMau?.NgayCapNhat ?? Model.NgayXuLy`).
2. **Về cơ chế chống nộp phiếu trùng đồng thời (`M3-BUG-03`)**:
   - **Giải pháp đã áp dụng**: Kết hợp `SemaphoreSlim` theo từng `MaNguoiHien` trong bộ nhớ ứng dụng và Database Transaction với `IsolationLevel.Serializable`. Giải pháp này đảm bảo an toàn trên 1 instance web server kết nối SQL Server mà không cần sửa schema CSDL dùng chung.
   - **Hướng xử lý tiếp theo (nếu triển khai multi-instance/cluster)**: Khi nhóm thống nhất cập nhật Migration dùng chung, có thể bổ sung thêm Unique Filtered Index trên `(MaNguoiHien, MaDot)` với điều kiện `[TrangThai] NOT IN (N'Đã hủy', N'Từ chối')`.
3. **Hướng dẫn kiểm thử thực tế trên máy local của nhóm**:
   - Mở dự án trong Visual Studio hoặc chạy `dotnet run --project QuanLyHienMau_UNETI2_TI17A5HN` ngoài môi trường sandbox để thực thi các kịch bản `TC01`–`TC22` trực tiếp với SQL Server LocalDB.

---

## 7. DANH SÁCH FILE THAY ĐỔI

Chỉ có các file sau được chỉnh sửa hoặc tạo mới trong nhiệm vụ này (không xóa bất kỳ file nào):

### File mã nguồn đã chỉnh sửa (6 file):
1. `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`
2. `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs`
3. `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml`
4. `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml`
5. `QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSoCaNhan.cshtml`
6. `QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/NguoiHienMau.cshtml` *(chỉ sửa đúng 1 dòng link ở dòng 547)*

### File báo cáo đã tạo mới (1 file):
1. `MODULE3_FIX_REPORT.md` *(Giữ nguyên không ghi đè file `MODULE3_AUDIT_REPORT.md` ban đầu)*

---

## 8. KẾT LUẬN

- Toàn bộ **9/9 lỗi thuộc phạm vi Module 3** (bao gồm cả **3 lỗi mức `HIGH`**, **4 lỗi mức `MEDIUM`** và **2 lỗi mức `LOW`**) đã được xác minh nguyên nhân gốc và sửa chữa triệt để, an toàn, không làm thay đổi cấu trúc CSDL dùng chung và không gây ảnh hưởng đến các module của thành viên khác.
- Dự án biên dịch thành công (`0 Error(s)`, `0 Warning(s)` phát sinh mới).
- Để hoàn tất nghiệm thu 100% ở mức thực thi động (Runtime End-to-End), nhóm chỉ cần khởi chạy ứng dụng trên môi trường có kết nối trực tiếp tới SQL Server LocalDB để xác nhận lần cuối 22 kịch bản kiểm thử trong bảng ở Mục 3.
