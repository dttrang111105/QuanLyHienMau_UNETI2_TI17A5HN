# PHẦN II (NHÓM J, K) & PHẦN V: CÂU HỎI PHẢN BIỆN TÌNH HUỐNG, LỖI / HẠN CHẾ / HƯỚNG CẢI TIẾN VÀ 5 BỘ ĐỀ MÔ PHỎNG BẢO VỆ

> **Mục tiêu tài liệu:**
> - **Nhóm J:** 32 tình huống phản biện trực diện khi giảng viên vừa nhìn giao diện vừa vặn hỏi mã nguồn.
> - **Nhóm K:** Phân tích trung thực, khách quan 4 nhóm: (1) Lỗi/Sự thiếu đồng bộ đã xác minh trong code, (2) Rủi ro cần kiểm tra, (3) Hạn chế thiết kế có thật, và (4) Đề xuất cải tiến kèm giải pháp code cụ thể.
> - **Phần V:** 5 Bộ đề mô phỏng buổi bảo vệ đồ án (**Đề 1 Cơ bản 15 câu, Đề 2 Kỹ thuật 15 câu, Đề 3 Phân tích thiết kế 15 câu, Đề 4 Phản biện 20 câu, Đề 5 Chỉ code trực tiếp 20 câu — Tổng cộng 85 câu hỏi kèm đáp án ngắn gọn**).

---

## NHÓM J — 32 TÌNH HUỐNG PHẢN BIỆN TRỰC TIẾP TỪ GIẢNG VIÊN

### Tình huống J1: "Em nói người dùng phải chờ 84 ngày mới được đăng ký lại. Chỗ nào trong code kiểm tra điều đó? Mở đúng dòng code cho cô xem!"
- **Cách trả lời & Chỉ code:**
  Dạ em mời cô xem file `Controllers/DangKyController.cs`:
  - Dòng 23 khai báo hằng số `public const int SoNgayChoToiThieuGiuaHaiLanHien = 84;`.
  - Dòng 94–178 là phương thức `EvaluateRecentDonationEligibility`. Cụ thể ở dòng 128–136:
    ```csharp
    var lastActualDate = latestDated.ActualDate.Date;
    var nextEligibleDate = lastActualDate.AddDays(SoNgayChoToiThieuGiuaHaiLanHien);
    if (refDay < nextEligibleDate) { ... return new RecentDonationEligibilityResult(false, ...); }
    ```
  - Và phương thức này được gọi ở cả `[HttpGet] Create` (dòng 284) lẫn bên trong Transaction của `[HttpPost] Create` (dòng 543).

---

### Tình huống J2: "Tại sao hệ thống biết lần trước người đó đã hiến máu thật sự chứ không phải chỉ mới nộp đơn trên mạng?"
- **Cách trả lời & Chỉ code:**
  Dạ tại hàm `IsConfirmedSuccessfulDonation` (`DangKyController.cs` dòng 41–60) và `GetActualDonationDate` (dòng 68–86). Hệ thống loại trừ toàn bộ các phiếu đang chờ hoặc bị hủy/từ chối/không đủ điều kiện, và chỉ công nhận khi `phieu.KetQuaHienMau.KetQua == "Hiến máu thành công"` (do nhân viên y tế xác nhận ở Module 5 sau khi đã lấy máu xong).

---

### Tình huống J3: "Nếu một người chỉ mới tạo phiếu đăng ký hôm qua (trạng thái `'Chờ xác nhận'`), hôm nay họ hủy phiếu đó đi rồi đăng ký lại ngay lập tức thì có bị chặn 84 ngày không?"
- **Cách trả lời & Chỉ code:**
  Dạ **không bị chặn 84 ngày** ạ! Vì khi phiếu bị hủy, `TrangThai` chuyển thành `"Đã hủy"`, hàm `IsConfirmedSuccessfulDonation` (dòng 44 `DangKyController.cs`) trả về `false`. Do đó `successfulDonations.Count == 0`, người đó được phép đăng ký lại ngay lập tức.

---

### Tình huống J4: "Nếu người dùng đi khám sàng lọc nhưng bác sĩ kết luận huyết áp thấp không đủ điều kiện hiến (`'Không đủ điều kiện hiến'`), vậy họ có phải chờ 84 ngày mới được đăng ký đợt sau không?"
- **Cách trả lời & Chỉ code:**
  Dạ **không phải chờ 84 ngày** ạ! Tại dòng 46 và dòng 54 của `DangKyController.cs`:
  - Nếu `phieu.TrangThai == "Không đủ điều kiện hiến"` -> `IsConfirmedSuccessfulDonation` trả về `false`.
  - Nếu `phieu.KetQuaHienMau != null` thì kiểm tra `KetQua == "Hiến máu thành công"`, nên kết quả `"Không đủ điều kiện hiến"` cũng trả về `false`. Người đó chưa mất máu nên hoàn toàn có thể đăng ký ở đợt tiếp theo khi sức khỏe đã ổn định.

---

### Tình huống J5: "Nếu phiếu có kết quả `'Hiến máu thành công'` trong bảng `PhieuDangKyHienMau`, nhưng vì lỗi nhập liệu mà bên bảng `KetQuaHienMau` chưa có ngày cập nhật (`NgayCapNhat == null`), hệ thống của em sẽ lấy `NgayNop` để cộng 84 ngày phải không?"
- **Cách trả lời & Chỉ code:**
  Dạ **tuyệt đối không lấy `NgayNop`** ạ! Tại dòng 109–113 và 156–169 của `DangKyController.cs`, những phiếu thành công nhưng `GetActualDonationDate(p) == null` sẽ được đưa vào danh sách `undatedSuccessful`. Khi `undatedSuccessful.Count > 0`, hệ thống bật cờ `HasMissingActualDonationDate = true` và trả về `IsEligible = false` để **chặn an toàn**, đồng thời hiển thị thông báo yêu cầu liên hệ nhân viên y tế bổ sung ngày kết quả hiến máu thực tế.

---

### Tình huống J6: "Nếu trên trình duyệt cô nhấn F12 (Inspect Element), cô xóa thuộc tính `disabled` của nút `'Gửi phiếu đăng ký'` khi đang trong thời gian chờ 84 ngày rồi bấm Submit thì có tạo được phiếu không?"
- **Cách trả lời & Chỉ code:**
  Dạ **không thể tạo được phiếu** ạ! Vì việc khóa nút trên `Create.cshtml` chỉ là lớp giao diện. Khi request `POST /DangKy/Create` gửi lên Server, tại dòng 540–550 của `DangKyController.cs`, Server truy vấn lại lịch sử từ CSDL và chạy lại `EvaluateRecentDonationEligibility(lichSuPhieuInTx)` ngay trong Transaction. Nếu chưa đủ 84 ngày, Server lập tức `RollbackAsync()` và trả về `TempData["Error"]`.

---

### Tình huống J7: "Nếu cô không mở trình duyệt mà dùng Postman gửi thẳng một request `POST /DangKy/Create` thì sao?"
- **Cách trả lời & Chỉ code:**
  Dạ sẽ bị chặn ngay từ vòng ngoài bởi thuộc tính `[ValidateAntiForgeryToken]` ở dòng 313 của `DangKyController.cs` (trả về lỗi HTTP `400 Bad Request` do thiếu cặp Anti-Forgery Token hợp lệ giữa Cookie và Form). Và kể cả khi gửi kèm token từ phiên trình duyệt, toàn bộ logic kiểm tra tuổi, cân nặng, câu hỏi sức khỏe và 84 ngày ở Server vẫn chạy đầy đủ.

---

### Tình huống J8: "Nếu một người mở 2 tab trình duyệt, cùng điền form và bấm nút Submit ở cả 2 tab cùng một tích tắc (Double Submit) thì có lọt 2 phiếu vào CSDL không?"
- **Cách trả lời & Chỉ code:**
  Dạ **không lọt được** ạ. Tại dòng 26 và dòng 532–616 của `DangKyController.cs`, em đã dùng `ConcurrentDictionary<int, SemaphoreSlim> _submissionLocks` khóa theo từng `MaNguoiHien`, kết hợp với `BeginTransactionAsync(IsolationLevel.Serializable)`. Tab thứ nhất lấy được khóa `SemaphoreSlim` và tạo phiếu xong thì Tab thứ hai mới được vào vùng khóa; lúc này Tab thứ hai chạy câu lệnh kiểm tra `existingPhieu` (dòng 553) sẽ thấy phiếu của Tab thứ nhất vừa tạo nên sẽ tự động `RollbackAsync()` và báo lỗi trùng phiếu.

---

### Tình huống J9: "Nếu người dùng nhập cân nặng là `40` kg hoặc nhập chữ `'abc'` vào ô cân nặng thì chuyện gì xảy ra?"
- **Cách trả lời & Chỉ code:**
  Dạ:
  1. Nếu nhập `'abc'`: Ở Client, `<input type="number">` không cho nhập chữ; nếu gửi qua POST, Model Binder không parse được sang `double?` nên `canNang` thành `null`. Nếu trong hồ sơ cũ cũng chưa có cân nặng thì dòng 496 `DangKyController.cs` chặn lại với lỗi *"Vui lòng nhập cân nặng hợp lệ"*.
  2. Nếu nhập `40` kg: Dòng 505 của `DangKyController.cs` kiểm tra `if (finalWeight.Value < dotHienMau.CanNangToiThieu)` (mặc định của đợt là `45` kg) và từ chối ngay với thông báo: *"Cân nặng hiện tại (40 kg) chưa đạt mức tối thiểu (45 kg) của đợt hiến máu này."*

---

### Tình huống J10: "Nếu người dùng nhập số điện thoại có dấu cách hoặc dấu chấm như `0912.345.678` hoặc nhập `12345` thì hệ thống xử lý thế nào?"
- **Cách trả lời & Chỉ code:**
  Dạ tại hàm `NormalizePhone` và `IsValidPhone` (`DangKyController.cs` dòng 180–191):
  - Nếu nhập `0912.345.678`: Hàm `NormalizePhone` tự động xóa dấu chấm thành `0912345678`, khớp với Regex `^0[0-9]{9,10}$` nên **hợp lệ và được lưu sạch sẽ** vào DB.
  - Nếu nhập `12345` (không bắt đầu bằng số `0` và chỉ có 5 chữ số): Không khớp Regex `^0[0-9]{9,10}$`, bị dòng 434 chặn lại.

---

### Tình huống J11: "Một bạn sinh ngày `30/10/2008`, hôm nay là ngày `29/10/2026` (tức là còn 1 ngày nữa mới sinh nhật 18 tuổi). Hệ thống của em tính là 17 tuổi hay 18 tuổi?"
- **Cách trả lời & Chỉ code:**
  Dạ hệ thống tính chính xác là **17 tuổi** và **chặn không cho đăng ký** ạ! Tại hàm `IsValidAge` (`DangKyController.cs` dòng 193–205):
  ```csharp
  int age = today.Year - ngaySinh.Year; // 2026 - 2008 = 18
  if (ngaySinh.Date > today.AddYears(-age)) // 30/10/2008 > 29/10/2008 -> true
  {
      age--; // Giảm xuống còn 17 tuổi!
  }
  return age >= 18 && age <= 60; // false
  ```

---

### Tình huống J12: "Khi phiếu đăng ký đã được nhân viên y tế chuyển sang trạng thái `'Chờ khám sàng lọc'`, người hiến máu có bấm nút Hủy phiếu được nữa không?"
- **Cách trả lời & Chỉ code:**
  Dạ **không hủy được nữa** ạ. Tại dòng 757 của `DangKyController.cs`:
  ```csharp
  var trangThaiChoPhep = new[] { "Chờ xác nhận", "Đã xác nhận" };
  if (!trangThaiChoPhep.Contains(phieu.TrangThai)) { ... }
  ```
  Chỉ khi phiếu đang ở `"Chờ xác nhận"` hoặc `"Đã xác nhận"` (và chưa có lịch khám hoạt động) thì người hiến mới được tự hủy. Khi đã sang `"Chờ khám sàng lọc"`, nút Hủy trên `TheoDoi.cshtml` (dòng 356) cũng ẩn đi và Backend cũng chặn hoàn toàn.

---

### Tình huống J13: "Nếu phiếu vẫn đang ở trạng thái `'Đã xác nhận'`, nhưng bác sĩ đã tạo một bản ghi trong bảng `LichKhamSangLoc` rồi (mà quên đổi trạng thái phiếu), người dùng có hủy được phiếu không?"
- **Cách trả lời & Chỉ code:**
  Dạ **vẫn bị chặn an toàn** ạ! Vì ở dòng 765 của `DangKyController.cs`, ngoài việc kiểm tra `phieu.TrangThai`, em còn kiểm tra trực tiếp bảng con:
  ```csharp
  if (phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy"))
  {
      TempData["Error"] = "Phiếu đã được xếp lịch khám sàng lọc, không thể tự hủy...";
      return RedirectToAction("TheoDoi");
  }
  ```

---

### Tình huống J14: "Nếu người dùng A đang đăng nhập, họ mở F12 sửa `<input name="id" value="10">` trong form Hủy phiếu thành mã phiếu của người dùng B rồi bấm Submit thì phiếu của B có bị hủy không?"
- **Cách trả lời & Chỉ code:**
  Dạ **không bị hủy** ạ. Ở dòng 743–745 của `DangKyController.cs`, câu truy vấn tìm phiếu cần hủy luôn gắn chặt với `nguoiHienMau.MaNguoiHien` lấy từ Cookie của người đang đăng nhập:
  `p.MaPhieu == id && p.MaNguoiHien == nguoiHienMau.MaNguoiHien`. Vì phiếu số 10 không thuộc về `MaNguoiHien` của A nên kết quả trả về `null` và hệ thống báo lỗi không có quyền hủy.

---

### Tình huống J15: "Trên form `DangKy/Create`, cô thấy có chỗ chọn Dung tích hiến (`250ml`, `350ml`, `450ml`) và Khung giờ hẹn (`Ca 1`, `Ca 2`), nhưng trong bảng `PhieuDangKyHienMau` cô không thấy cột `Volume` hay `TimeSlot`. Vậy dữ liệu đó đi đâu?"
- **Cách trả lời & Chỉ code:**
  Dạ tại dòng 580–596 của `DangKyController.cs`, hệ thống nhận 2 tham số `string? volume` và `string? timeSlot` từ form, sau đó ghép chuỗi:
  - `volume` được ghép vào đầu cột `LyDoHienMau` theo định dạng `"Dung tích dự kiến: 350ml | ..."`.
  - `timeSlot` được ghép vào đầu cột `GhiChu` theo định dạng `"Khung giờ hẹn: Ca 1 (07:30 - 09:30) | ..."`.
  Khi hiển thị lại trên trang `TheoDoi.cshtml` (dòng 329–344) và `ChiTietPhieuDangKy.cshtml`, giao diện đọc từ `LyDoHienMau` và `GhiChu` ra để hiển thị cho người dùng và nhân viên y tế.

---

### Tình huống J16: "Còn ô nhập `'Chiều cao (cm)'` ở dòng 315 file `Views/DangKy/Create.cshtml` thì lưu vào đâu?"
- **Cách trả lời & Chỉ code:**
  Dạ em xin trả lời trung thực là ô `Chiều cao (cm)` trên `Create.cshtml` hiện tại không có thuộc tính `name` và trong bảng `NguoiHienMau` không có cột `ChieuCao`, nên ô này chỉ là trường giao diện tham khảo phía Client chứ không lưu xuống CSDL.

---

### Tình huống J17: "Trên trang `HoSoCaNhan.cshtml`, khi cô bấm nút `'Tải Chứng Nhận'`, `'Lưu VNeID'` hoặc xem tab `'Hồ sơ xét nghiệm'` (HIV, Viêm gan B) thì dữ liệu đó lấy từ bảng nào?"
- **Cách trả lời & Chỉ code:**
  Dạ các nút `'Tải Chứng Nhận'`, `'Xuất PDF'`, `'Lưu VNeID'` (dòng 832–854 `HoSoCaNhan.cshtml`) và nội dung xét nghiệm chi tiết HIV/HBV/HCV (dòng 565–705) hiện là **mô phỏng giao diện (UI Mock)** bằng JavaScript và HTML tĩnh khi `totalCount > 0`. Còn những dữ liệu thật 100% lấy từ CSDL trên trang `HoSoCaNhan.cshtml` là: Thông tin cá nhân/sức khỏe của `NguoiHienMau`, Số lần hiến máu thành công (`ViewBag.ConfirmedSuccessfulCount`), Ngày hiến gần nhất, Đếm ngược 84 ngày và Bảng lịch sử các đợt đăng ký hiến máu.

---

### Tình huống J18: "Tại sao ở trang `HoSoCaNhan.cshtml`, tổng lượng máu đã hiến lại hiển thị là `350 ml` khi mới hiến 1 lần dù lúc đăng ký cô chọn túi `250 ml`?"
- **Cách trả lời & Chỉ code:**
  Dạ tại dòng 17 của `Views/NguoiHienMau/HoSoCaNhan.cshtml`, biến `totalVolume` đang được tính ước lượng bằng công thức `int totalVolume = totalCount * 350;` (lấy mức trung bình 350ml cho mỗi lần hiến thành công), bởi vì bảng `KetQuaHienMau` hiện chưa có cột `TheTichMauThucTe` kiểu số nguyên. Em đã đưa điểm này vào danh sách đề xuất cải tiến CSDL.

---

### Tình huống J19: "Khi người dùng đổi `'Họ và tên'` ở trang `HoSoCaNhan` rồi bấm Lưu, tại sao tên ở trong form thì đổi ngay nhưng tên hiển thị ở góc phải thanh menu (`_Layout.cshtml`) vẫn là tên cũ?"
- **Cách trả lời & Chỉ code:**
  Dạ tại dòng 249 của `NguoiHienMauController.cs`, code đã đồng bộ `nguoiHienMau.TaiKhoan.HoTen = model.HoTen` xuống CSDL. Tuy nhiên, thanh menu ở `Views/Shared/_Layout.cshtml` (dòng 150) lại đọc tên từ `@User.Identity?.Name` — tức là đọc từ `ClaimTypes.Name` lưu trong Cookie lúc đăng nhập. Vì sau khi lưu hồ sơ, controller chưa gọi lại `HttpContext.SignInAsync` để cấp lại Cookie mới nên phải đợi lần đăng nhập sau tên trên thanh menu mới cập nhật.

---

### Tình huống J20: "Nếu một đợt hiến máu có `SoLuongToiDa = 100`, hiện đã có 100 phiếu đăng ký nhưng trong đó có 5 phiếu `'Đã hủy'` và 5 phiếu `'Từ chối'`, vậy đợt đó còn nhận đăng ký được không?"
- **Cách trả lời & Chỉ code:**
  Dạ **vẫn còn nhận thêm được 10 người nữa** ạ! Bởi vì tại dòng 239–241 và 350–351 của `DangKyController.cs`, khi đếm `soLuongDaDangKy`, hệ thống chỉ đếm các phiếu hợp lệ:
  ```csharp
  int soLuongDaDangKy = dotHienMau.PhieuDangKyHienMaus.Count(
      p => p.TrangThai != "Đã hủy" && p.TrangThai != "Từ chối" && p.TrangThai != "Không đủ điều kiện hiến");
  ```
  Những phiếu đã hủy, bị từ chối hoặc khám không đạt sẽ tự động trả lại suất trống cho đợt hiến máu đó.

---

### Tình huống J21: "Khách vãng lai chưa đăng nhập, họ vào `DangKy/Create` nhập SĐT `0912345678` (SĐT này trùng với một người vừa hiến máu thành công cách đây 10 ngày). Hệ thống có phát hiện ra để chặn 84 ngày không?"
- **Cách trả lời & Chỉ code:**
  Dạ **phát hiện và chặn được hoàn toàn** ạ! Tại dòng 404–413 của `DangKyController.cs`, khi người dùng chưa đăng nhập, hệ thống chuẩn hóa SĐT và tìm trong bảng `NguoiHienMau`:
  `nguoiHienMau = (await _context.NguoiHienMaus.Where(n => n.SoDienThoai != null).ToListAsync()).FirstOrDefault(n => NormalizePhone(n.SoDienThoai) == normalizedInputPhone);`.
  Khi tìm thấy hồ sơ cũ có SĐT `0912345678`, hệ thống lấy `MaNguoiHien` đó để truy vấn lịch sử phiếu ở dòng 540 và chặn ngay vì mới hiến cách đây 10 ngày (< 84 ngày).

---

### Tình huống J22: "Nếu khách vãng lai ở câu trên sau đó vào trang Đăng ký tài khoản (`Account/Register`) tạo tài khoản mới với cùng SĐT `0912345678` thì chuyện gì xảy ra?"
- **Cách trả lời & Chỉ code:**
  Dạ đây là một tình huống rất hay mà em đã phân tích kỹ trong code:
  - Ở `AccountController.Register` (dòng 150–160), khi đăng ký tài khoản mới, `AccountController` hiện tại luôn `new NguoiHienMau` mới gắn với `MaTaiKhoan` mới (chưa kiểm tra xem SĐT đó đã có bản ghi `NguoiHienMau` khách vãng lai `MaTaiKhoan == null` hay chưa). Khi đó trong DB sẽ xuất hiện 2 bản ghi `NguoiHienMau` có cùng SĐT (1 bản ghi cũ của khách vãng lai có lịch sử hiến máu và `MaTaiKhoan == null`, 1 bản ghi mới của tài khoản mới chưa có lịch sử).
  - Đây là một rủi ro nghiệp vụ liên-module giữa Module 1 và Module 3 mà em đã ghi nhận trong mục **Hạn chế & Hướng cải tiến**: ở `AccountController.Register` nên kiểm tra nếu đã có `NguoiHienMau` trùng SĐT và `MaTaiKhoan == null` thì gắn `MaTaiKhoan` vào bản ghi đó thay vì tạo bản ghi trùng.

---

### Tình huống J23: "Tại sao trên trang `Admin/NguoiHienMau`, số người `'Đang chờ (Chưa đủ 84N)'` đôi khi lại khác với kết quả kiểm tra của `DangKyController`?"
- **Cách trả lời & Chỉ code:**
  Dạ em xin chỉ rõ nguyên nhân trong code:
  - Ở `DangKyController` và `NguoiHienMauController`, nhóm đã chuẩn hóa việc tính 84 ngày thông qua hàm `EvaluateRecentDonationEligibility` (chỉ tính phiếu có kết quả `"Hiến máu thành công"` và lấy ngày `KetQuaHienMau.NgayCapNhat`).
  - Tuy nhiên, ở `AdminController.NguoiHienMau` (dòng 656–665 và 739–775) và `Views/Admin/NguoiHienMau.cshtml` (dòng 391–404), đoạn code thống kê phía Admin vẫn đang dùng công thức cũ: kiểm tra `p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null` (vô tình tính cả phiếu có kết quả `"Không đủ điều kiện hiến"`) và lấy mốc ngày là `p.NgayXuLy ?? p.NgayNop` thay vì `KetQuaHienMau.NgayCapNhat`. Nếu đồng bộ `AdminController.NguoiHienMau` gọi sang `DangKyController.EvaluateRecentDonationEligibility` thì hai bên sẽ khớp nhau 100%.

---

### Tình huống J24: "Khi nhân viên y tế vào sửa lại ghi chú của một kết quả hiến máu (`AdminController.SuaKetQua`), nó ảnh hưởng thế nào đến Module 3?"
- **Cách trả lời & Chỉ code:**
  Dạ tại dòng 1131 của `Controllers/AdminController.cs`, trong hàm `SuaKetQua`, có câu lệnh `kq.NgayCapNhat = DateTime.Now;`. Vì Module 3 đang lấy `KetQuaHienMau.NgayCapNhat` làm ngày hiến máu thực tế, nên nếu 1 tháng sau nhân viên y tế vào sửa lại ghi chú của kết quả đó, `NgayCapNhat` sẽ bị cập nhật thành ngày hôm nay và làm người hiến máu phải đếm lại 84 ngày từ đầu. Cách sửa rất đơn giản là bỏ dòng gán lại `kq.NgayCapNhat = DateTime.Now` khi chỉ sửa ghi chú, hoặc tách riêng cột `NgayHienMauThucTe`.

---

### Tình huống J25 – J32: Các câu hỏi nhanh "Sửa nóng tại chỗ" (Live Coding Modifications)

| STT | Yêu cầu sửa nóng của Giảng viên | Vị trí sửa chính xác trong Code | Đoạn code cần sửa |
|---|---|---|---|
| **J25** | *"Đổi quy tắc chờ từ 84 ngày sang 90 ngày cho cô!"* | `Controllers/DangKyController.cs` (dòng 23) | Đổi `public const int SoNgayChoToiThieuGiuaHaiLanHien = 84;` thành `= 90;`. |
| **J26** | *"Đổi quy tắc chờ từ 84 ngày sang đúng 3 tháng theo lịch dương!"* | `Controllers/DangKyController.cs` (dòng 129) | Đổi `var nextEligibleDate = lastActualDate.AddDays(SoNgayChoToiThieuGiuaHaiLanHien);` thành `var nextEligibleDate = lastActualDate.AddMonths(3);`. |
| **J27** | *"Đổi độ tuổi được phép hiến máu từ 18–60 thành 18–55 tuổi!"* | `DangKyController.cs` (dòng 204, 456) & `NguoiHienMauController.cs` (dòng 44, 152) | Trong hàm `IsValidAge`: đổi `return age >= 18 && age <= 60;` thành `return age >= 18 && age <= 55;`. |
| **J28** | *"Bắt buộc phải đăng nhập mới được vào trang Đăng ký hiến máu (`DangKy/Create`)!"* | `Controllers/DangKyController.cs` (dòng 207 & 312) | Thêm `[Authorize]` lên phía trên `[HttpGet] Create` và `[HttpPost] Create` (hoặc đặt `[Authorize]` ở đầu class `DangKyController`). |
| **J29** | *"Cho phép người hiến máu tự hủy phiếu cả khi đang ở trạng thái `'Chờ khám sàng lọc'` (nếu chưa có lịch khám)!"* | `Controllers/DangKyController.cs` (dòng 757) & `Views/DangKy/TheoDoi.cshtml` (dòng 356) | Đổi mảng `var trangThaiChoPhep = new[] { "Chờ xác nhận", "Đã xác nhận", "Chờ khám sàng lọc" };`. |
| **J30** | *"Chặn không cho một người có 2 phiếu đang chờ xử lý ở 2 đợt hiến máu khác nhau cùng lúc!"* | `Controllers/DangKyController.cs` (dòng 294 & 554) | Trong điều kiện tìm `existingPhieu`, xóa điều kiện `p.MaDot == maDot &&` để kiểm tra trên toàn bộ các đợt. |
| **J31** | *"Bắt buộc cân nặng tối thiểu của mọi đợt ít nhất phải từ 42kg trở lên!"* | `Controllers/DangKyController.cs` (dòng 505) | Đổi `double minWeight = Math.Max(42.0, dotHienMau.CanNangToiThieu);` rồi kiểm tra `if (finalWeight.Value < minWeight)`. |
| **J32** | *"Sửa trang `Admin/NguoiHienMau` để đếm đúng số lần hiến máu thành công giống `DangKyController`!"* | `Views/Admin/NguoiHienMau.cshtml` (dòng 391) | Đổi điều kiện lọc `successfulDonations` trên View thành `Model.PhieuDangKyHienMaus.Where(QuanLyHienMau_UNETI2_TI17A5HN.Controllers.DangKyController.IsConfirmedSuccessfulDonation)`. |

---

## NHÓM K — TỔNG HỢP LỖI, RỦI RO, HẠN CHẾ VÀ HƯỚNG CẢI TIẾN CỦA MODULE 3

Để giúp sinh viên trả lời phần **"Hạn chế của đề tài và Hướng phát triển"** một cách thuyết phục, chuẩn xác theo đúng code trên nhánh `DamThuTrang_23103100243`, dưới đây là bảng phân loại 4 nhóm:

### 1. Lỗi / Sự thiếu đồng bộ đã xác minh trong code hiện tại (Verified Discrepancies)
1. **Sự lệch công thức tính lần hiến thành công và 84 ngày tại `AdminController.NguoiHienMau`:**
   - *Hiện trạng:* `DangKyController` và `NguoiHienMauController` dùng `IsConfirmedSuccessfulDonation` + `GetActualDonationDate` (`KetQuaHienMau.NgayCapNhat`). Nhưng `AdminController.NguoiHienMau` (dòng 656–665, 739–775) và `Views/Admin/NguoiHienMau.cshtml` (dòng 391–404) lại dùng điều kiện `p.TrangThai == "Hiến máu thành công" || p.KetQuaHienMau != null` (đếm nhầm cả phiếu có kết quả `"Không đủ điều kiện hiến"`) và tính 84 ngày theo `p.NgayXuLy ?? p.NgayNop`.
2. **Ghi đè `KetQuaHienMau.NgayCapNhat = DateTime.Now` khi sửa kết quả (`AdminController.SuaKetQua` dòng 1131):**
   - *Hiện trạng:* Khi nhân viên y tế chỉnh sửa ghi chú của một kết quả hiến máu cũ, `NgayCapNhat` bị cập nhật thành thời điểm sửa, làm dịch chuyển mốc tính 84 ngày của người hiến máu.

### 2. Rủi ro kỹ thuật / nghiệp vụ cần lưu ý (Potential Risks)
1. **Rủi ro tạo trùng bản ghi `NguoiHienMau` khi khách vãng lai đăng ký tài khoản sau (`AccountController.Register` dòng 150):**
   - Khách vãng lai đăng ký hiến máu trước (tạo bản ghi `NguoiHienMau` với `MaTaiKhoan = null`), sau đó mới vào `Account/Register` tạo tài khoản với cùng SĐT -> `AccountController.Register` tạo thêm 1 bản ghi `NguoiHienMau` mới thay vì gắn `MaTaiKhoan` vào bản ghi cũ.
2. **Rủi ro đăng ký song song ở 2 chiến dịch khác nhau:**
   - Câu lệnh kiểm tra `existingPhieu` tại `DangKyController.cs` (dòng 294, 554) hiện đang lọc theo `p.MaDot == maDot`. Do đó một người có thể gửi 2 phiếu `"Chờ xác nhận"` ở 2 chiến dịch khác nhau đang mở cùng lúc.
3. **Rủi ro bộ nhớ nhỏ với `_submissionLocks`:**
   - Từ điển tĩnh `ConcurrentDictionary<int, SemaphoreSlim> _submissionLocks` giữ các đối tượng `SemaphoreSlim` theo `MaNguoiHien` suốt vòng đời ứng dụng mà chưa có cơ chế dọn dẹp định kỳ (tuy nhiên với quy mô đồ án vài nghìn người hiến thì chỉ tốn vài chục KB RAM).

### 3. Hạn chế thiết kế có thật (Design Limitations)
1. **Chưa tách cột riêng cho `DungTichDuKien` (`volume`), `KhungGioHen` (`timeSlot`) trong `PhieuDangKyHienMau` và `TheTichMauThucTe`, `NgayHienMauThucTe` trong `KetQuaHienMau`.**
2. **Chưa làm mới Cookie Claims (`SignInAsync`) ngay sau khi đổi `HoTen` ở `NguoiHienMauController.HoSoCaNhan`.**
3. **Một số nút chức năng nâng cao trên giao diện (`Tải Chứng Nhận PDF`, `Lưu VNeID`, `Xuất Excel`, `Gọi máu khẩn cấp SMS`) mới dừng ở mức mô phỏng giao diện (`alert` JavaScript).**

### 4. Đề xuất hướng cải tiến (Future Improvements)
1. **Về CSDL:** Bổ sung cột `NgayHienMauThucTe` (`DateTime`) và `TheTichMauHien` (`int`) vào bảng `KetQuaHienMau`; bổ sung cột `DungTichDangKy` (`int`) và `KhungGioHen` (`nvarchar(100)`) vào bảng `PhieuDangKyHienMau`; đánh Index cho `NguoiHienMau.SoDienThoai`.
2. **Về Kiến trúc Code:** Tách logic nghiệp vụ trong `DangKyController` và `NguoiHienMauController` sang một lớp `IDonorRegistrationService` / `DonorRegistrationService` riêng để cả `DangKyController`, `NguoiHienMauController` và `AdminController` cùng gọi chung, đảm bảo tính nhất quán 100%.
3. **Về Tính năng:** Tích hợp thư viện `QuestPDF` hoặc `iText7` để xuất Giấy chứng nhận hiến máu tình nguyện ra file PDF thật và thư viện `EPPlus` / `ClosedXML` để xuất danh sách người hiến máu ra file Excel.

---

## PHẦN V — 5 BỘ ĐỀ MÔ PHỎNG BUỔI BẢO VỆ ĐỒ ÁN (TỔNG CỘNG 85 CÂU HỎI KÈM GỢI Ý TRẢ LỜI)

---

### 📋 ĐỀ SỐ 1: BỘ ĐỀ CƠ BẢN (15 CÂU — Kiểm tra nắm vững nghiệp vụ & luồng chức năng Module 3)

1. **Câu 1.1:** Em phụ trách module nào trong nhóm? Tên đầy đủ của module là gì?
   - *Đáp án:* Em phụ trách Module 3: **Quản lý người hiến máu và Phiếu đăng ký hiến máu**.
2. **Câu 1.2:** Module 3 làm việc chính với những bảng nào trong cơ sở dữ liệu?
   - *Đáp án:* Làm việc trực tiếp với 2 bảng chính là `NguoiHienMau` và `PhieuDangKyHienMau`, đồng thời đọc dữ liệu liên kết từ `TaiKhoan`, `DotHienMau`, `LichKhamSangLoc` và `KetQuaHienMau`.
3. **Câu 1.3:** Kể tên các Controller và các View thuộc phạm vi Module 3 của em?
   - *Đáp án:* 2 Controller chính là `DangKyController.cs` và `NguoiHienMauController.cs` (cùng phần quản lý người hiến/phiếu trong `AdminController.cs`). Các View gồm `Views/DangKy/Create.cshtml`, `Views/DangKy/TheoDoi.cshtml`, `Views/NguoiHienMau/HoSoCaNhan.cshtml` và `Views/Admin/NguoiHienMau.cshtml`.
4. **Câu 1.4:** Khi người dùng gửi phiếu đăng ký hiến máu mới, trạng thái ban đầu của phiếu là gì?
   - *Đáp án:* Trạng thái mặc định luôn là `"Chờ xác nhận"` (dòng 606 `DangKyController.cs`).
5. **Câu 1.5:** Độ tuổi hợp lệ để đăng ký hiến máu và cập nhật hồ sơ trong hệ thống là bao nhiêu?
   - *Đáp án:* Từ đủ **18 tuổi đến 60 tuổi** (hàm `IsValidAge`).
6. **Câu 1.6:** Số điện thoại của người hiến máu được kiểm tra theo định dạng nào?
   - *Đáp án:* Sau khi loại bỏ khoảng trắng, dấu gạch ngang và dấu chấm, phải bắt đầu bằng số `0` và có độ dài 10 hoặc 11 chữ số (`^0[0-9]{9,10}$`).
7. **Câu 1.7:** Cân nặng của người hiến máu phải thỏa mãn điều kiện gì khi đăng ký?
   - *Đáp án:* Phải nằm trong khoảng `30–200` kg và lớn hơn hoặc bằng mức cân nặng tối thiểu của đợt hiến máu đó (`canNang >= dotHienMau.CanNangToiThieu`, mặc định là 45kg).
8. **Câu 1.8:** Thời gian chờ tối thiểu giữa hai lần hiến máu thành công là bao nhiêu ngày?
   - *Đáp án:* **84 ngày** (12 tuần), khai báo ở hằng số `SoNgayChoToiThieuGiuaHaiLanHien = 84`.
9. **Câu 1.9:** Người hiến máu có thể theo dõi tiến trình phiếu đăng ký của mình ở màn hình nào và gồm mấy bước?
   - *Đáp án:* Tại màn hình `/DangKy/TheoDoi` (`Views/DangKy/TheoDoi.cshtml`), hiển thị qua thanh tiến trình (Stepper) **4 bước**: (1) Gửi phiếu đăng ký -> (2) Xác nhận & Lịch khám -> (3) Khám sàng lọc -> (4) Kết quả hiến máu.
10. **Câu 1.10:** Người hiến máu được phép tự hủy phiếu đăng ký ở những trạng thái nào?
    - *Đáp án:* Chỉ được hủy ở 2 trạng thái là `"Chờ xác nhận"` hoặc `"Đã xác nhận"`, với điều kiện chưa có lịch khám sàng lọc đang hoạt động và chưa có kết quả hiến máu.
11. **Câu 1.11:** Sau khi người hiến máu hủy phiếu thành công, bản ghi phiếu đó có bị xóa khỏi CSDL không?
    - *Đáp án:* Không bị xóa vật lý, mà cột `TrangThai` được cập nhật thành `"Đã hủy"` và `NgayXuLy = DateTime.Now` để lưu lịch sử.
12. **Câu 1.12:** Trên form đăng ký hiến máu có mấy câu hỏi khảo sát sức khỏe bắt buộc?
    - *Đáp án:* Có **2 câu hỏi** (`health_q1` về tiền sử bệnh truyền nhiễm phải chọn `"none"`, và `health_q2` về sức khỏe 3 tháng gần đây phải chọn `"good"`).
13. **Câu 1.13:** Khách chưa đăng nhập tài khoản có vào điền form `DangKy/Create` được không?
    - *Đáp án:* Được, vì action `Create` không gắn `[Authorize]`. Sau khi gửi phiếu thành công, khách chưa đăng nhập sẽ được chuyển hướng về trang chi tiết đợt hiến máu (`DotHienMau/Details`), còn người đã đăng nhập được chuyển hướng về trang `DangKy/TheoDoi`.
14. **Câu 1.14:** Khi người dùng cập nhật Họ tên ở trang Hồ sơ cá nhân (`HoSoCaNhan`), thông tin đó có được cập nhật sang bảng `TaiKhoan` không?
    - *Đáp án:* Có, tại dòng 249 `NguoiHienMauController.cs`, hệ thống đồng bộ `nguoiHienMau.TaiKhoan.HoTen = model.HoTen`.
15. **Câu 1.15:** Những đợt hiến máu ở trạng thái nào thì được hiển thị trong danh sách cho phép đăng ký ở `DangKy/Create`?
    - *Đáp án:* Các đợt có `TrangThai == "Đang mở"` hoặc `"Sắp diễn ra"` và có ngày `KetThuc >= DateTime.Now` (dòng 212 `DangKyController.cs`).

---

### 💻 ĐỀ SỐ 2: BỘ ĐỀ KỸ THUẬT ASP.NET CORE MVC & EF CORE (15 CÂU)

1. **Câu 2.1:** Dự án đang sử dụng phiên bản .NET nào và ORM nào để truy cập CSDL?
   - *Đáp án:* Sử dụng **.NET 10.0 (`net10.0`)** với **Entity Framework Core (`Microsoft.EntityFrameworkCore.SqlServer` 10.0.5)**.
2. **Câu 2.2:** `ApplicationDbContext` được tiêm vào `DangKyController` và `NguoiHienMauController` bằng cơ chế gì?
   - *Đáp án:* Cơ chế **Constructor Dependency Injection (DI)**, được đăng ký qua `builder.Services.AddDbContext<ApplicationDbContext>(...)` trong `Program.cs`.
3. **Câu 2.3:** Làm thế nào Controller lấy được mã tài khoản của người dùng đang đăng nhập?
   - *Đáp án:* Đọc từ Claim `"MaTaiKhoan"` trong Cookie xác thực bằng câu lệnh `User.FindFirst("MaTaiKhoan")?.Value` và ép kiểu `int.TryParse(...)`.
4. **Câu 2.4:** Giải thích sự khác nhau giữa `.Include()` và `.ThenInclude()` trong truy vấn lịch sử hiến máu tại `NguoiHienMauController.HoSoCaNhan`?
   - *Đáp án:* `.Include(n => n.PhieuDangKyHienMaus)` nạp danh sách phiếu đăng ký (quan hệ cấp 1 từ `NguoiHienMau`), còn `.ThenInclude(p => p.KetQuaHienMau)` nạp tiếp đối tượng kết quả hiến máu của từng phiếu đó (quan hệ cấp 2 từ `PhieuDangKyHienMau`).
5. **Câu 2.5:** Tại sao trong `DangKyController.Create [POST]`, sau khi lưu phiếu thành công lại dùng `RedirectToAction` kết hợp `TempData["Success"]` mà không dùng `ViewBag` và `return View()`?
   - *Đáp án:* Để áp dụng mẫu **PRG (Post-Redirect-Get)** chống lỗi gửi lặp form khi người dùng nhấn F5. Vì `RedirectToAction` tạo request GET mới nên phải dùng `TempData` (lưu qua phiên) chứ `ViewBag` sẽ bị mất dữ liệu khi redirect.
6. **Câu 2.6:** Tại sao các hàm `IsConfirmedSuccessfulDonation`, `GetActualDonationDate`, `EvaluateRecentDonationEligibility` lại trả về kết quả thông qua một `record RecentDonationEligibilityResult`?
   - *Đáp án:* Kiểu `record` trong C# là kiểu dữ liệu bất biến (Immutable Value-like type) rất gọn nhẹ, giúp đóng gói 7 thông tin đầu ra (`IsEligible`, `LastSuccessfulPhieu`, `LastActualDonationDate`, `NextEligibleDonationDate`, `RemainingDays`, `HasMissingActualDonationDate`, `ReasonMessage`) để truyền sang cả Controller lẫn View một cách an toàn, tường minh.
7. **Câu 2.7:** Thuộc tính `[ValidateAntiForgeryToken]` đặt trên các hàm `[HttpPost]` có tác dụng gì?
   - *Đáp án:* Kiểm tra mã `__RequestVerificationToken` ẩn trong form khớp với Cookie để chống tấn công giả mạo yêu cầu liên trang (**CSRF**).
8. **Câu 2.8:** Trong `ApplicationDbContext.cs`, dòng lệnh `.HasIndex(n => n.MaTaiKhoan).IsUnique().HasFilter("[MaTaiKhoan] IS NOT NULL")` giải quyết bài toán kỹ thuật gì trong SQL Server?
   - *Đáp án:* Tạo **Filtered Unique Index** trên SQL Server để vừa đảm bảo quan hệ `1 - 1` khi `MaTaiKhoan` có giá trị, vừa cho phép lưu nhiều bản ghi khách vãng lai có `MaTaiKhoan = NULL`.
9. **Câu 2.9:** Trong `DangKyController.Create [POST]`, tại sao phải dùng `IsolationLevel.Serializable` khi mở Transaction tạo phiếu?
   - *Đáp án:* `Serializable` là mức cô lập giao dịch cao nhất, giúp khóa phạm vi đọc-ghi để ngăn chặn hoàn toàn hiện tượng Phantom Read / Race Condition khi kiểm tra điều kiện 84 ngày và kiểm tra trùng phiếu trước khi `INSERT`.
10. **Câu 2.10:** Vai trò của `ConcurrentDictionary<int, SemaphoreSlim> _submissionLocks` trong `DangKyController` là gì?
    - *Đáp án:* Tạo hàng đợi đồng bộ hóa theo từng `MaNguoiHien` ngay trong bộ nhớ ứng dụng (`SemaphoreSlim(1, 1)`), đảm bảo tại một thời điểm mỗi người hiến máu chỉ có tối đa 1 luồng được đi vào khối Transaction tạo phiếu.
11. **Câu 2.11:** Tại sao phải đặt `userLock.Release()` ở trong khối `finally` (dòng 665 `DangKyController.cs`)?
    - *Đáp án:* Để đảm bảo dù trong khối `try` có xảy ra `return` sớm hay ném ra ngoại lệ (`Exception`), khóa `SemaphoreSlim` vẫn luôn được giải phóng, không bao giờ gây treo khóa (Deadlock).
12. **Câu 2.12:** Trong `NguoiHienMauController.HoSoCaNhan [POST]`, tại sao em không gọi `_context.Update(model)` trực tiếp mà lại truy vấn `nguoiHienMau` từ DB lên rồi gán từng thuộc tính?
    - *Đáp án:* Để chống lỗ hổng **Mass Assignment (Over-posting)** và đảm bảo chỉ cập nhật đúng bản ghi `NguoiHienMau` gắn với `MaTaiKhoan` trong Cookie, không cho phép người dùng giả mạo `MaNguoiHien`, `MaTaiKhoan` hay `TrangThai`.
13. **Câu 2.13:** Phân trang danh sách người hiến máu ở `AdminController.NguoiHienMau` được viết bằng toán tử LINQ nào?
    - *Đáp án:* Sử dụng `.CountAsync()` để tính tổng số bản ghi, sau đó dùng `.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync()` với `pageSize = 6` (dòng 729–732 `AdminController.cs`).
14. **Câu 2.14:** Làm thế nào dropdown chọn chiến dịch trên `Views/DangKy/Create.cshtml` chuyển đổi được thông tin đợt hiến máu khi người dùng chọn đợt khác?
    - *Đáp án:* Sự kiện `onchange="changeCampaign(this.value)"` gọi hàm JavaScript `window.location.href = '/DangKy/Create?maDot=' + maDot;` (dòng 429–431 `Create.cshtml`) để tải lại trang với `maDot` mới.
15. **Câu 2.15:** Nếu trong câu truy vấn `lichSuPhieu` ở `DangKyController.Create` quên viết `.Include(p => p.KetQuaHienMau)` thì điều gì xảy ra?
    - *Đáp án:* Navigation property `p.KetQuaHienMau` sẽ bị `null` (do EF Core mặc định không bật Lazy Loading). Khi đó hàm `GetActualDonationDate` trả về `null`, làm các phiếu `"Hiến máu thành công"` bị báo lỗi thiếu ngày hiến thực tế (`HasMissingActualDonationDate = true`).

---

### 📐 ĐỀ SỐ 3: BỘ ĐỀ PHÂN TÍCH THIẾT KẾ HỆ THỐNG & CSDL (15 CÂU)

1. **Câu 3.1:** Hãy kể tên tất cả các Tác nhân (Actors) tham gia vào Module 3?
   - *Đáp án:* Gồm 3 tác nhân: (1) Người hiến máu đã đăng nhập, (2) Khách vãng lai chưa đăng nhập, và (3) Nhân viên y tế / Quản trị viên (`Admin`, `NhanVienYTe`).
2. **Câu 3.2:** Trình bày các Use Case chính của Module 3?
   - *Đáp án:* Xem & cập nhật hồ sơ cá nhân, Xem thống kê & lịch sử hiến máu, Tạo phiếu đăng ký hiến máu, Theo dõi tiến trình phiếu, Hủy phiếu đăng ký, Tra cứu danh sách người hiến máu (Admin), và Duyệt/xử lý phiếu đăng ký (Admin).
3. **Câu 3.3:** Mối quan hệ giữa bảng `TaiKhoan` và `NguoiHienMau` là quan hệ gì? Vì sao không gộp chung thành 1 bảng?
   - *Đáp án:* Quan hệ `1 - 0..1` qua khóa ngoại `MaTaiKhoan`. Không gộp chung vì: (1) Admin/Nhân viên y tế có tài khoản nhưng không nhất thiết là người hiến máu, (2) Khách vãng lai là người hiến máu nhưng chưa có tài khoản (`MaTaiKhoan = null`), và (3) Tách biệt thông tin bảo mật đăng nhập (Module 1) với hồ sơ y tế (Module 3).
4. **Câu 3.4:** Mối quan hệ giữa `NguoiHienMau` và `PhieuDangKyHienMau` là quan hệ gì? Khóa ngoại nằm ở đâu?
   - *Đáp án:* Quan hệ `1 - n` (một người hiến máu có thể đăng ký nhiều phiếu ở nhiều đợt). Khóa ngoại `MaNguoiHien` nằm ở bảng `PhieuDangKyHienMau`.
5. **Câu 3.5:** Bảng `PhieuDangKyHienMau` đóng vai trò gì giữa bảng `NguoiHienMau` và bảng `DotHienMau`?
   - *Đáp án:* Về mặt mô hình dữ liệu quan hệ, một người hiến máu có thể tham gia nhiều đợt hiến máu và một đợt hiến máu có nhiều người tham gia (`n - n`). Bảng `PhieuDangKyHienMau` chính là **Thực thể kết hợp (Associative Entity)** phân rã quan hệ `n - n` giữa `NguoiHienMau` và `DotHienMau` thành hai quan hệ `1 - n`.
6. **Câu 3.6:** Tại sao hành vi xóa (`DeleteBehavior`) giữa `TaiKhoan` và `NguoiHienMau` lại đặt là `SetNull`?
   - *Đáp án:* Để khi xóa một tài khoản đăng nhập (`TaiKhoan`), bản ghi hồ sơ y tế `NguoiHienMau` chỉ bị gán `MaTaiKhoan = NULL` chứ không bị xóa mất lịch sử các lần hiến máu của người đó.
7. **Câu 3.7:** Tại sao hành vi xóa giữa `NguoiHienMau` / `DotHienMau` với `PhieuDangKyHienMau` lại đặt là `Restrict`, còn giữa `PhieuDangKyHienMau` với `LichKhamSangLoc` / `KetQuaHienMau` lại là `Cascade`?
   - *Đáp án:* Dùng `Restrict` để cấm xóa người hiến máu hoặc đợt hiến máu đã có phát sinh phiếu đăng ký; còn dùng `Cascade` cho `LichKhamSangLoc` và `KetQuaHienMau` vì lịch khám và kết quả là thành phần phụ thuộc hoàn toàn vào tờ phiếu đăng ký đó.
8. **Câu 3.8:** Kể tên đầy đủ 8 trạng thái trong vòng đời của `PhieuDangKyHienMau.TrangThai`?
   - *Đáp án:* (1) `"Chờ xác nhận"`, (2) `"Đã xác nhận"`, (3) `"Từ chối"`, (4) `"Đã hủy"`, (5) `"Chờ khám sàng lọc"`, (6) `"Đã khám sàng lọc"`, (7) `"Hiến máu thành công"`, (8) `"Không đủ điều kiện hiến"`.
9. **Câu 3.9:** Trong 8 trạng thái trên, những trạng thái nào là trạng thái kết thúc (Terminal States) của một tờ phiếu?
   - *Đáp án:* Gồm 4 trạng thái kết thúc: `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"`, và `"Hiến máu thành công"`.
10. **Câu 3.10:** Trạng thái nào là trạng thái duy nhất kích hoạt bộ đếm thời gian nghỉ 84 ngày?
    - *Đáp án:* Chỉ trạng thái/kết quả `"Hiến máu thành công"` (kèm ngày thực tế tại `KetQuaHienMau.NgayCapNhat`).
11. **Câu 3.11:** Trên Biểu đồ Tuần tự (Sequence Diagram) của chức năng Tạo phiếu đăng ký, thông điệp tự gọi (Self-Message) quan trọng nhất tại `DangKyController` là gì?
    - *Đáp án:* Là lời gọi hàm `EvaluateRecentDonationEligibility(lichSuPhieu)` (bên trong gọi tiếp `IsConfirmedSuccessfulDonation` và `GetActualDonationDate`) để thẩm định quy tắc 84 ngày.
12. **Câu 3.12:** Tại sao bảng `KetQuaHienMau` lại đánh `Unique Index` trên cột khóa ngoại `MaPhieu`?
    - *Đáp án:* Để ép buộc ràng buộc mức cơ sở dữ liệu rằng mỗi phiếu đăng ký (`MaPhieu`) chỉ có tối đa **1 kết quả hiến máu duy nhất** (quan hệ `1 - 0..1`).
13. **Câu 3.13:** Cột `NgayXuLy` trong bảng `PhieuDangKyHienMau` lưu thông tin gì và vì sao nó cho phép `NULL`?
    - *Đáp án:* Lưu thời điểm tờ phiếu được tác động xử lý (khi Admin xác nhận/từ chối/chuyển khám/ghi kết quả hoặc khi người dùng tự hủy). Khi mới nộp phiếu (`"Chờ xác nhận"`), phiếu chưa được xử lý nên `NgayXuLy` mang giá trị `NULL`.
14. **Câu 3.14:** Nếu thiết kế lại bảng `PhieuDangKyHienMau` và `KetQuaHienMau` để đạt chuẩn 3NF tốt hơn, em sẽ tách thêm những cột nào?
    - *Đáp án:* Tách cột `DungTichDuKien` (`int`) và `KhungGioHen` (`nvarchar(100)`) ra khỏi chuỗi ghép trong `LyDoHienMau`/`GhiChu` của `PhieuDangKyHienMau`; và thêm cột `NgayHienMauThucTe` (`DateTime`) cùng `TheTichMauThucTe` (`int`) vào `KetQuaHienMau`.
15. **Câu 3.15:** Hãy giải thích cách ánh xạ từ Biểu đồ Lớp (Class Diagram) sang cấu trúc thư mục của project ASP.NET Core MVC?
    - *Đáp án:* Các lớp Entity ánh xạ vào thư mục `Models/` và cấu hình quan hệ trong `Data/ApplicationDbContext.cs`; các lớp Control ánh xạ vào `Controllers/`; các lớp Boundary giao diện ánh xạ vào `Views/`.

---

### 🔥 ĐỀ SỐ 4: BỘ ĐỀ PHẢN BIỆN CHUYÊN SÂU (20 CÂU — Dành cho giảng viên hỏi xoáy & bẫy)

1. **Câu 4.1:** Tại sao không lấy `PhieuDangKyHienMau.NgayNop` để cộng 84 ngày cho đơn giản mà phải JOIN sang `KetQuaHienMau.NgayCapNhat`?
   - *Đáp án:* Vì `NgayNop` chỉ là ngày đăng ký online trước đợt hiến máu, chưa chắc người đó đã đi hiến. Chu kỳ 84 ngày phục hồi máu chỉ được tính từ ngày thực tế hiến máu thành công (`KetQuaHienMau.NgayCapNhat`).
2. **Câu 4.2:** Vậy `KetQuaHienMau.NgayCapNhat` đã thực sự là ngày lấy máu ngoài đời trong mọi trường hợp chưa?
   - *Đáp án:* Dạ chưa hoàn toàn tuyệt đối: nếu nhân viên y tế nhập kết quả lên phần mềm muộn hơn ngày lấy máu vài ngày, hoặc vào `SuaKetQua` sửa lại ghi chú (dòng 1131 `AdminController.cs` gán lại `NgayCapNhat = DateTime.Now`), thì `NgayCapNhat` sẽ bị lệch sang ngày nhập/sửa. Em đề xuất tách riêng cột `NgayHienMauThucTe`.
3. **Câu 4.3:** Tại sao quy tắc trong code là 84 ngày mà trên giao diện một số chỗ ghi chú là "84 ngày (12 tuần)" hoặc "3 tháng"? 84 ngày có bằng 3 tháng không?
   - *Đáp án:* 84 ngày chính xác bằng **12 tuần** ($12 \times 7 = 84$) theo chuẩn Thông tư 26/2013/TT-BYT của Bộ Y tế, còn 3 tháng dương lịch dài 89–92 ngày. Dùng 84 ngày đảm bảo đúng chuẩn y tế và công bằng cho mọi tháng.
4. **Câu 4.4:** Nếu một người có 2 lần hiến máu thành công: lần 1 cách đây 200 ngày (có đủ `NgayCapNhat`), lần 2 mới tuần trước nhưng bị lỗi thiếu `NgayCapNhat == null`. Hệ thống của em sẽ lấy lần 1 (200 ngày > 84 ngày) và cho họ đăng ký tiếp đúng không?
   - *Đáp án:* **Không!** Tại `EvaluateRecentDonationEligibility` (`DangKyController.cs` dòng 156–169), hệ thống kiểm tra cả `undatedSuccessful`. Dù `datedSuccessful` đã qua 200 ngày, nhưng hễ có bất kỳ phiếu thành công nào bị thiếu ngày (`hasMissingDate == true`), hệ thống sẽ chặn lại ngay ở bước kiểm tra `if (hasMissingDate)` chứ không bỏ sót lần 2!
5. **Câu 4.5:** Nếu một người đang có phiếu `"Chờ xác nhận"` ở Đợt A, họ mở Đợt B đăng ký tiếp thì code hiện tại có chặn không?
   - *Đáp án:* Code hiện tại ở dòng 554 `DangKyController.cs` lọc `p.MaDot == maDot` nên chỉ chặn trùng phiếu trong cùng Đợt A, chưa chặn sang Đợt B. Nếu muốn chặn toàn cục trên mọi đợt, chỉ cần bỏ điều kiện `p.MaDot == maDot`.
6. **Câu 4.6:** Khóa `_submissionLocks` (`ConcurrentDictionary<int, SemaphoreSlim>`) có tác dụng nếu ứng dụng được triển khai trên 2 máy chủ Web (Load Balancing 2 servers) không?
   - *Đáp án:* `SemaphoreSlim` chỉ khóa các luồng trong cùng 1 tiến trình (In-process). Nếu chạy nhiều máy chủ Web song song, lớp bảo vệ thứ 2 là Database Transaction `IsolationLevel.Serializable` dưới SQL Server sẽ phát huy tác dụng, hoặc có thể thêm Unique Filtered Index dưới SQL Server / dùng Redis Distributed Lock.
7. **Câu 4.7:** Tại sao khách vãng lai chưa đăng nhập vẫn tạo được phiếu ở `DangKy/Create`, nhưng lại không hủy được phiếu ở `DangKy/Huy`?
   - *Đáp án:* Vì `DangKy/Huy` có `[Authorize]` yêu cầu xác thực tài khoản qua Cookie để chứng minh quyền sở hữu phiếu (`MaTaiKhoan` -> `MaNguoiHien`). Khách vãng lai không có phiên đăng nhập nên nếu muốn hủy phải liên hệ nhân viên y tế hỗ trợ hoặc đăng ký/đăng nhập tài khoản.
8. **Câu 4.8:** Nếu khách vãng lai dùng SĐT của một người đã có tài khoản để gửi form ở `DangKy/Create` thì sao?
   - *Đáp án:* Ở dòng 409–412 `DangKyController.cs`, khi chưa đăng nhập, hệ thống tìm `NguoiHienMau` theo SĐT đó, cập nhật thông tin từ form vào `nguoiHienMau` và gắn phiếu vào `MaNguoiHien` đó. Đây là sự đánh đổi giữa tính tiện lợi (cho phép đăng ký nhanh không cần đăng nhập) và bảo mật (nếu muốn chống người khác nhập SĐT của mình thì nên bắt buộc `[Authorize]` cho `Create` hoặc xác thực OTP).
9. **Câu 4.9:** Tại sao ở `DangKyController.cs` dòng 409, khi tìm `NguoiHienMau` theo SĐT đã chuẩn hóa, em lại gọi `.Where(n => n.SoDienThoai != null).ToListAsync()` rồi mới `.FirstOrDefault(n => NormalizePhone(n.SoDienThoai) == normalizedInputPhone)`?
   - *Đáp án:* Bởi vì `NormalizePhone` là một hàm C# tùy chỉnh, EF Core không thể dịch hàm C# `NormalizePhone` sang câu lệnh SQL `WHERE`. Do đó code phải tải danh sách về RAM (`.ToListAsync()`) rồi mới so sánh qua `NormalizePhone`. Để tối ưu hiệu năng khi dữ liệu lớn, vì mọi nơi khi lưu SĐT đều đã gọi `NormalizePhone` trước khi lưu, ta có thể truy vấn thẳng dưới SQL: `.FirstOrDefaultAsync(n => n.SoDienThoai == normalizedInputPhone)`.
10. **Câu 4.10:** Nếu một đợt hiến máu chỉ còn đúng 1 suất cuối cùng (`99/100`), hai người hiến máu khác nhau (`MaNguoiHien = 5` và `MaNguoiHien = 6`) bấm đăng ký cùng một mili-giây thì sao?
    - *Đáp án:* Vì `_submissionLocks` đang khóa theo `MaNguoiHien` (2 người có 2 khóa khác nhau) và câu lệnh đếm `soLuongDaDangKy` nằm ở dòng 350 (trước Transaction), nên cả 2 người có thể cùng vượt qua bước kiểm tra `99 < 100`. Để chống tuyệt đối tranh chấp suất cuối của đợt, cần đưa bước đếm `soLuongDaDangKy` vào bên trong Transaction `Serializable` hoặc khóa theo `MaDot`.
11. **Câu 4.11:** Tại sao khi Admin từ chối một phiếu đăng ký (`AdminController.TuChoiPhieu`), code lại phải duyệt qua `phieu.LichKhamSangLocs` để đổi trạng thái lịch khám sang `"Đã hủy"`?
    - *Đáp án:* Vì `TuChoiPhieu` cho phép từ chối cả phiếu đang ở trạng thái `"Chờ khám sàng lọc"`. Nếu chỉ đổi trạng thái phiếu sang `"Từ chối"` mà để nguyên lịch khám ở trạng thái `"Đã lên lịch"`, danh sách lịch khám của bác sĩ ở Module 4 vẫn hiện ca khám của một phiếu đã bị từ chối, gây mâu thuẫn dữ liệu.
12. **Câu 4.12:** Ngược lại, khi người hiến máu tự hủy phiếu ở `DangKyController.Huy`, tại sao code không cần hủy các `LichKhamSangLoc`?
    - *Đáp án:* Vì ở `DangKyController.Huy` (dòng 765), hệ thống đã chặn trước: nếu có bất kỳ `LichKhamSangLoc` nào khác `"Đã hủy"` thì không cho phép người hiến tự hủy phiếu. Do đó những phiếu người hiến tự hủy được chắc chắn chưa có lịch khám nào đang hoạt động.
13. **Câu 4.13:** Nếu trong CSDL một phiếu có `phieu.TrangThai = "Đã khám sàng lọc"` nhưng bên bảng `KetQuaHienMau` lại đã có bản ghi với `KetQua = "Hiến máu thành công"`, hàm `IsConfirmedSuccessfulDonation` trả về `true` hay `false`?
    - *Đáp án:* Trả về **`true`**! Vì ở dòng 52–55 của `DangKyController.cs`, hàm kiểm tra `if (phieu.KetQuaHienMau != null) return phieu.KetQuaHienMau.KetQua == "Hiến máu thành công";` trước khi xét `phieu.TrangThai`.
14. **Câu 4.14:** Còn nếu `phieu.TrangThai = "Đã hủy"` nhưng do lỗi dữ liệu mà vẫn tồn tại `phieu.KetQuaHienMau` thì `IsConfirmedSuccessfulDonation` trả về gì?
    - *Đáp án:* Trả về **`false`**, vì dòng 44–49 kiểm tra loại trừ `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"` ngay đầu hàm.
15. **Câu 4.15:** Tại sao trong `NguoiHienMauController.HoSoCaNhan [POST]` (dòng 105), em không kiểm tra `if (!ModelState.IsValid)` mà dùng `if (errors.Count > 0)`?
    - *Đáp án:* Vì em thực hiện bước chuẩn hóa dữ liệu (`Trim`, `NormalizePhone`) ở dòng 117–124 sau khi Model Binding đã chạy xong, đồng thời kiểm tra các quy tắc nghiệp vụ tùy chỉnh (`IsValidAge`, `IsValidPhone`) gom vào danh sách `errors` để trả về thông báo lỗi tiếng Việt thống nhất qua `TempData["Error"]`.
16. **Câu 4.16:** Nếu người dùng nhập `LyDoHienMau` dài 490 ký tự trên form `DangKy/Create`, lại chọn thêm `volume = "350ml"`, khi ghép chuỗi `"Dung tích dự kiến: 350ml | "` vào đầu `LyDoHienMau` thì tổng độ dài vượt quá 500 ký tự của cột `nvarchar(500)`. Code xử lý thế nào?
    - *Đáp án:* Hiện tại dòng 520 kiểm tra `lyDoHienMau.Length > 500` trước khi ghép chuỗi `volume` ở dòng 583. Nếu người dùng nhập sát 490 ký tự rồi ghép thêm 27 ký tự của `volume` thì tổng độ dài sẽ vượt 500 ký tự và khi `SaveChangesAsync()` SQL Server sẽ báo lỗi cắt chuỗi (`String or binary data would be truncated`), bị bắt bởi khối `catch` ở dòng 660. Cách khắc phục là kiểm tra độ dài sau khi đã ghép chuỗi hoặc cắt `.Substring(0, 500)`.
17. **Câu 4.17:** Tại sao trên `Views/DangKy/TheoDoi.cshtml`, mã phiếu hiển thị là `#DKHM-00005` nhưng trong CSDL cột `MaPhieu` chỉ lưu số `5`?
    - *Đáp án:* Trong CSDL lưu kiểu số nguyên tự tăng `int` (`5`) để tối ưu tốc độ truy vấn và làm khóa ngoại, còn khi hiển thị trên giao diện thì định dạng bằng C# `phieu.MaPhieu.ToString("D5")` (padding đủ 5 chữ số với tiền tố `#DKHM-`).
18. **Câu 4.18:** Nếu một người hiến máu có tài khoản bị Admin khóa (`TaiKhoan.TrangThai = false`), họ đang mở sẵn trình duyệt từ trước thì có gửi phiếu ở `HoSoCaNhan` được nữa không?
    - *Đáp án:* `AccountController.Login` chỉ kiểm tra `TaiKhoan.TrangThai == true` lúc đăng nhập. Nếu họ đã có Cookie từ trước khi bị khóa tài khoản, Cookie đó vẫn còn hạn cho đến khi hết phiên. Muốn chặn tức thời cả phiên đang mở thì có thể kiểm tra thêm `nguoiHienMau.TaiKhoan?.TrangThai == true` hoặc `nguoiHienMau.TrangThai == "Hoạt động"` ngay trong Controller.
19. **Câu 4.19:** Tại sao trong `Views/DangKy/TheoDoi.cshtml` (dòng 12–16), biến `activePhieu` chỉ lấy 1 phiếu (`FirstOrDefault`) để vẽ thanh tiến trình 4 bước?
    - *Đáp án:* Vì theo thiết kế giao diện, khu vực đầu trang hiển thị chi tiết tiến trình của **phiếu đang hoạt động mới nhất** (`NgayNop` mới nhất chưa kết thúc), còn tất cả các phiếu khác của người dùng đều được liệt kê đầy đủ trong bảng **Lịch sử đăng ký hiến máu** ngay phía dưới.
20. **Câu 4.20:** Giữa các file báo cáo `.md` cũ trong thư mục gốc của repo và code trên nhánh `DamThuTrang_23103100243` của em có sự khác biệt gì?
    - *Đáp án:* Các file báo cáo cũ được sinh trên nhánh `integration` (nơi tách `Module5Service`, `KetQuaHienMauController`), còn trên nhánh `DamThuTrang_23103100243` hiện tại của em, toàn bộ chức năng duyệt phiếu, lịch khám, kết quả phía quản trị được tích hợp trong `AdminController.cs`, form đăng ký có 2 câu hỏi sức khỏe (`health_q1`, `health_q2`), và hàm `Huy` chỉ cho phép hủy ở 2 trạng thái (`"Chờ xác nhận"`, `"Đã xác nhận"`).

---

### 🔍 ĐỀ SỐ 5: BỘ ĐỀ CHỈ CODE TRỰC TIẾP TRÊN MÁY (20 CÂU — "Mở file và chỉ đúng dòng code cho cô")

| Câu | Yêu cầu của Giảng viên | File cần mở | Dòng code chính xác & Nội dung chỉ cho cô xem |
|---|---|---|---|
| **5.1** | Chỉ cho cô hằng số quy định 84 ngày chờ giữa 2 lần hiến máu? | `Controllers/DangKyController.cs` | **Dòng 23:** `public const int SoNgayChoToiThieuGiuaHaiLanHien = 84;` |
| **5.2** | Chỉ cho cô hàm xác định một phiếu có phải là lần hiến máu thành công thực sự hay không? | `Controllers/DangKyController.cs` | **Dòng 41–60:** Phương thức `IsConfirmedSuccessfulDonation(PhieuDangKyHienMau phieu)` kiểm tra `phieu.KetQuaHienMau.KetQua == "Hiến máu thành công"`. |
| **5.3** | Chỉ cho cô dòng code lấy ngày hiến máu thực tế từ bảng `KetQuaHienMau`? | `Controllers/DangKyController.cs` | **Dòng 68–86 (cụ thể dòng 80–83):** Phương thức `GetActualDonationDate` trả về `phieu.KetQuaHienMau.NgayCapNhat.Value`. |
| **5.4** | Chỉ cho cô dòng code cộng 84 ngày để tính ngày được phép đăng ký tiếp theo? | `Controllers/DangKyController.cs` | **Dòng 129:** `var nextEligibleDate = lastActualDate.AddDays(SoNgayChoToiThieuGiuaHaiLanHien);` trong `EvaluateRecentDonationEligibility`. |
| **5.5** | Chỉ cho cô đoạn code chặn an toàn khi phiếu `"Hiến máu thành công"` nhưng bị thiếu ngày kết quả? | `Controllers/DangKyController.cs` | **Dòng 109–113 & 156–169:** Lọc `undatedSuccessful` và trả về `new RecentDonationEligibilityResult(false, ..., HasMissingActualDonationDate: true, ...)`. |
| **5.6** | Chỉ cho cô hàm tính tuổi từ 18 đến 60 tuổi và giải thích cách trừ tuổi khi chưa tới sinh nhật? | `Controllers/DangKyController.cs` | **Dòng 193–205:** Hàm `IsValidAge` với câu lệnh `if (ngaySinh.Date > today.AddYears(-age)) age--;`. |
| **5.7** | Chỉ cho cô chỗ kiểm tra số lượng người đã đăng ký không vượt quá chỉ tiêu tối đa của đợt? | `Controllers/DangKyController.cs` | **Dòng 239–248 (GET) & Dòng 350–355 (POST):** Đếm `soLuongDaDangKy` và kiểm tra `if (soLuongDaDangKy >= dotHienMau.SoLuongToiDa)`. |
| **5.8** | Chỉ cho cô đoạn code nối hồ sơ khách vãng lai (`MaTaiKhoan == null`) vào tài khoản khi người dùng đăng nhập? | `Controllers/DangKyController.cs` | **Dòng 367–376:** Tìm bản ghi trùng `SoDienThoai` mà `MaTaiKhoan == null` và gán `nguoiHienMau.MaTaiKhoan = maTaiKhoan;`. |
| **5.9** | Chỉ cho cô chỗ kiểm tra cân nặng của người đăng ký so với cân nặng tối thiểu của đợt hiến máu? | `Controllers/DangKyController.cs` | **Dòng 494–511:** Kiểm tra `finalWeight.Value < 30.0 || finalWeight.Value > 200.0` và `finalWeight.Value < dotHienMau.CanNangToiThieu`. |
| **5.10** | Chỉ cho cô chỗ kiểm tra 2 câu hỏi khảo sát sức khỏe trên Server? | `Controllers/DangKyController.cs` | **Dòng 513–517:** `if (health_q1 != "none" || health_q2 != "good")`. |
| **5.11** | Chỉ cho cô đoạn code dùng `SemaphoreSlim` và Transaction `Serializable` để chống tạo 2 phiếu trùng nhau? | `Controllers/DangKyController.cs` | **Dòng 26** (khai báo `_submissionLocks`) và **Dòng 532–616** (`userLock.WaitAsync()` + `BeginTransactionAsync(IsolationLevel.Serializable)`). |
| **5.12** | Chỉ cho cô đoạn code ghép Dung tích máu (`volume`) và Khung giờ hẹn (`timeSlot`) vào phiếu đăng ký? | `Controllers/DangKyController.cs` | **Dòng 580–596:** Ghép `volume` vào `finalLyDo` (`LyDoHienMau`) và ghép `timeSlot` vào `finalGhiChu` (`GhiChu`). |
| **5.13** | Chỉ cho cô chỗ kiểm tra điều kiện chỉ cho phép hủy phiếu ở 2 trạng thái `"Chờ xác nhận"` và `"Đã xác nhận"`? | `Controllers/DangKyController.cs` | **Dòng 757–776:** Mảng `trangThaiChoPhep`, kiểm tra `LichKhamSangLocs` và `KetQuaHienMau` trong action `Huy(int id)`. |
| **5.14** | Chỉ cho cô chỗ chống người dùng A hủy phiếu của người dùng B (chống IDOR)? | `Controllers/DangKyController.cs` | **Dòng 743–745:** `FirstOrDefaultAsync(p => p.MaPhieu == id && p.MaNguoiHien == nguoiHienMau.MaNguoiHien)`. |
| **5.15** | Chỉ cho cô đoạn code trong `NguoiHienMauController` gọi sang hàm tính 84 ngày của `DangKyController`? | `Controllers/NguoiHienMauController.cs` | **Dòng 89–99 (GET) & Dòng 208–215 (POST):** Gọi `DangKyController.EvaluateRecentDonationEligibility(lichSuPhieu)`. |
| **5.16** | Chỉ cho cô đoạn code cập nhật thông tin hồ sơ cá nhân và đồng bộ `HoTen` sang bảng `TaiKhoan`? | `Controllers/NguoiHienMauController.cs` | **Dòng 234–252:** Gán tường minh 10 thuộc tính của `nguoiHienMau` và `nguoiHienMau.TaiKhoan.HoTen = model.HoTen;`. |
| **5.17** | Chỉ cho cô cấu hình khóa ngoại và Unique Filtered Index của bảng `NguoiHienMau` và `PhieuDangKyHienMau`? | `Data/ApplicationDbContext.cs` | **Dòng 24–30** (`NguoiHienMau.MaTaiKhoan`) và **Dòng 52–62** (`PhieuDangKyHienMau` với `DeleteBehavior.Restrict`). |
| **5.18** | Chỉ cho cô đoạn code trên View `Create.cshtml` khóa nút Submit khi `coTheDangKy == false`? | `Views/DangKy/Create.cshtml` | **Dòng 95–107** (hiển thị hộp cảnh báo đỏ `lyDoKhongTheDangKy`) và **Dòng 390–406** (nút Submit `disabled` khi `!coTheDangKy`). |
| **5.19** | Chỉ cho cô đoạn code vẽ thanh tiến trình Stepper 4 bước trên trang `TheoDoi.cshtml`? | `Views/DangKy/TheoDoi.cshtml` | **Dòng 12–16** (lọc `activePhieu`), **Dòng 46–103** (tính biến `currentStep` từ 1 đến 4) và **Dòng 175–310** (HTML Stepper 4 bước). |
| **5.20** | Chỉ cho cô đoạn code Admin duyệt phiếu, từ chối phiếu và ghi nhận kết quả hiến máu? | `Controllers/AdminController.cs` | **Dòng 422–450** (`XacNhanPhieu`), **Dòng 452–495** (`TuChoiPhieu`), và **Dòng 1029–1089** (`TaoKetQua`). |
