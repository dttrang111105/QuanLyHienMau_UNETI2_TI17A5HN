# PHẦN II (NHÓM A, B, C) & PHẦN III: NGÂN HÀNG CÂU HỎI TỔNG QUAN, NGHIỆP VỤ, QUY TẮC ĐĂNG KÝ HIẾN MÁU VÀ PHÂN TÍCH ĐỘ ƯU TIÊN ÔN TẬP

> **Hướng dẫn sử dụng:** Tài liệu này bao gồm toàn bộ câu hỏi và phân tích chuyên sâu thuộc **Nhóm A (Tổng quan Module 3)**, **Nhóm B (Phân tích 11 nghiệp vụ cốt lõi theo 10 tiêu chí/nghiệp vụ + câu hỏi bảo vệ)**, **Nhóm C (Quy tắc đăng ký hiến máu & 12 câu hỏi phản biện chuyên sâu)** và **Phần III (Ma trận phân loại độ ưu tiên ôn tập Mức 1 đến Mức 4)**.

---

## NHÓM A — CÂU HỎI TỔNG QUAN MODULE 3

### Câu A1: Module 3 giải quyết bài toán gì trong hệ thống quản lý hiến máu vì cộng đồng?
- **Mức độ:** Cơ bản
- **Câu trả lời mẫu:**
  Thưa cô, Module 3 có tên là **Quản lý người hiến máu và Phiếu đăng ký hiến máu**. Module này giải quyết bài toán số hóa toàn bộ quy trình từ lúc người tình nguyện thiết lập hồ sơ cá nhân, đăng ký tham gia một đợt hiến máu trực tuyến, hệ thống tự động sàng lọc các điều kiện ban đầu (độ tuổi, cân nặng, thời gian chờ tối thiểu 84 ngày giữa hai lần hiến máu thành công), cho đến việc theo dõi tiến trình xử lý phiếu và tra cứu lịch sử hiến máu. Nhờ có Module 3, ban tổ chức giảm tải được việc nhập liệu giấy tờ thủ công và ngăn chặn sớm các trường hợp đăng ký trùng lặp hoặc chưa đủ ngày nghỉ hồi phục sức khỏe.
- **Bằng chứng trong dự án:**
  - `Controllers/DangKyController.cs` (các action `Create`, `TheoDoi`, `Huy`)
  - `Controllers/NguoiHienMauController.cs` (action `HoSoCaNhan`)
  - `Models/NguoiHienMau.cs`, `Models/PhieuDangKyHienMau.cs`
- **Ví dụ thực tế:** Một sinh viên UNETI muốn đăng ký hiến máu trong đợt "Ngày Chủ Nhật Đỏ". Thay vì xếp hàng điền giấy tại bàn tiếp đón, bạn đó đăng nhập vào hệ thống, cập nhật cân nặng, chọn ca khám và gửi phiếu đăng ký trực tuyến.
- **Câu hỏi phụ:** *Nếu một người chưa có tài khoản trên hệ thống thì có gửi phiếu đăng ký ở màn hình `DangKy/Create` được không?*
- **Trả lời câu hỏi phụ:** Dạ theo code hiện tại ở `DangKyController.cs`, action `Create` (cả GET và POST) **không gắn `[Authorize]`**, nên khách vãng lai chưa đăng nhập vẫn điền form đăng ký được; hệ thống sẽ tìm `NguoiHienMau` theo `SoDienThoai` (dòng 404–413) hoặc tạo mới bản ghi `NguoiHienMau` với `MaTaiKhoan = null`. Tuy nhiên, để sử dụng chức năng `TheoDoi`, `Huy` hoặc `HoSoCaNhan` thì bắt buộc phải đăng nhập vì các action đó có `[Authorize]`.
- **Lưu ý xác minh:** Đã xác minh 100% trong code `DangKyController.cs` và `NguoiHienMauController.cs`.

---

### Câu A2: Người dùng chính (Actor) của Module 3 là những ai? Quyền hạn của từng đối tượng như thế nào?
- **Mức độ:** Cơ bản
- **Câu trả lời mẫu:**
  Thưa cô, Module 3 phục vụ 3 nhóm người dùng chính:
  1. **Người hiến máu đã có tài khoản (`VaiTro = "NguoiHienMau"`):** Được xem và cập nhật hồ sơ cá nhân (`NguoiHienMau/HoSoCaNhan`), tạo phiếu đăng ký (`DangKy/Create`), theo dõi trạng thái phiếu (`DangKy/TheoDoi`) và hủy phiếu khi phiếu đang ở trạng thái `"Chờ xác nhận"` hoặc `"Đã xác nhận"` (`DangKy/Huy`).
  2. **Khách vãng lai (chưa đăng nhập):** Có thể truy cập form `DangKy/Create` để gửi phiếu đăng ký hiến máu dựa trên số điện thoại và thông tin cá nhân nhập trực tiếp.
  3. **Quản trị viên và Nhân viên y tế (`VaiTro = "Admin"` hoặc `"NhanVienYTe"`):** Truy cập trang quản trị để tra cứu danh sách người hiến máu (`Admin/NguoiHienMau`), xem danh sách phiếu đăng ký (`Admin/PhieuDangKyHienMau`), xem chi tiết phiếu (`Admin/ChiTietPhieuDangKy`), xác nhận phiếu (`XacNhanPhieu`), từ chối phiếu (`TuChoiPhieu`) hoặc chuyển phiếu sang chờ khám sàng lọc (`ChuyenChoKham`).
- **Bằng chứng trong dự án:**
  - `Controllers/NguoiHienMauController.cs` (dòng 10: `[Authorize]`)
  - `Controllers/DangKyController.cs` (dòng 685, 725: `[Authorize]` trên `TheoDoi` và `Huy`)
  - `Controllers/AdminController.cs` (dòng 11: `[Authorize(Roles = "Admin,NhanVienYTe")]`)
- **Câu hỏi phụ:** *Admin có thể vào trang `DangKy/Create` để tạo phiếu không?*
- **Trả lời câu hỏi phụ:** Dạ được, vì `DangKyController.Create` không giới hạn Role. Thậm chí trên trang `Views/Admin/NguoiHienMau.cshtml` (dòng 62), nút "Thêm Hồ Sơ Mới" cũng đang điều hướng tới `/DangKy/Create`.
- **Lưu ý xác minh:** Đã xác minh trong code.

---

### Câu A3: Module 3 bao gồm những chức năng cụ thể nào?
- **Mức độ:** Cơ bản
- **Câu trả lời mẫu:**
  Thưa cô, Module 3 có 6 nhóm chức năng cụ thể đã được triển khai trong code:
  1. **Xem và cập nhật hồ sơ cá nhân người hiến máu** (`GET/POST /NguoiHienMau/HoSoCaNhan`).
  2. **Thống kê lịch sử hiến máu cá nhân và tính chu kỳ hồi phục 84 ngày** (hiển thị trên cả `HoSoCaNhan`, `DangKy/Create` và `DangKy/TheoDoi`).
  3. **Tạo phiếu đăng ký hiến máu theo chiến dịch** (`GET/POST /DangKy/Create`) kèm kiểm tra điều kiện tuổi, cân nặng, SĐT, khảo sát sức khỏe và thời gian chờ 84 ngày.
  4. **Theo dõi tiến trình phiếu đăng ký theo 4 bước và xem lịch sử phiếu** (`GET /DangKy/TheoDoi`).
  5. **Hủy phiếu đăng ký phía người hiến máu** (`POST /DangKy/Huy`).
  6. **Tra cứu người hiến máu và duyệt/xử lý phiếu đăng ký phía Admin/Nhân viên y tế** (`AdminController.NguoiHienMau`, `PhieuDangKyHienMau`, `ChiTietPhieuDangKy`, `XacNhanPhieu`, `TuChoiPhieu`, `ChuyenChoKham`).
- **Bằng chứng trong dự án:** `Controllers/DangKyController.cs`, `Controllers/NguoiHienMauController.cs`, `Controllers/AdminController.cs`.
- **Câu hỏi phụ:** *Trong các chức năng trên, chức năng nào có nhiều ràng buộc nghiệp vụ nhất?*
- **Trả lời câu hỏi phụ:** Dạ đó là chức năng **Tạo phiếu đăng ký hiến máu (`DangKyController.Create`)**, vì nó phải kiểm tra đồng thời điều kiện của đợt hiến máu, dữ liệu cá nhân, khảo sát y tế, phiếu trùng trong đợt, quy tắc 84 ngày từ lần hiến thành công thực tế và chống gửi trùng request đồng thời.
- **Lưu ý xác minh:** Đã xác minh trong code.

---

### Câu A4: Đầu vào (Input) và Đầu ra (Output) của Module 3 là gì?
- **Mức độ:** Cơ bản
- **Câu trả lời mẫu:**
  Thưa cô:
  - **Đầu vào (Input) của Module 3 gồm:**
    1. Thông tin định danh từ phiên đăng nhập (`Claim("MaTaiKhoan")` trong Cookie).
    2. Thông tin chiến dịch hiến máu (`MaDot`, thời gian, chỉ tiêu `SoLuongToiDa`, `CanNangToiThieu`) từ bảng `DotHienMau` (Module 2).
    3. Dữ liệu do người hiến máu nhập trên form: Họ tên, Ngày sinh, Giới tính, SĐT, Email, Địa chỉ, Nhóm máu, Nghề nghiệp, Cân nặng, Ghi chú sức khỏe, Dung tích dự kiến (`volume`), Khung giờ hẹn (`timeSlot`), và 2 câu trả lời sàng lọc (`health_q1`, `health_q2`).
    4. Dữ liệu lịch khám (`LichKhamSangLoc`) từ Module 4 và Kết quả hiến máu (`KetQuaHienMau.KetQua`, `KetQuaHienMau.NgayCapNhat`) từ Module 5 để kiểm tra điều kiện hủy phiếu và điều kiện đăng ký lại sau 84 ngày.
  - **Đầu ra (Output) của Module 3 gồm:**
    1. Bản ghi hồ sơ người hiến máu được thêm mới hoặc cập nhật trong bảng `NguoiHienMau` (và đồng bộ `HoTen` sang `TaiKhoan`).
    2. Bản ghi phiếu đăng ký trong bảng `PhieuDangKyHienMau` với trạng thái khởi tạo `"Chờ xác nhận"`, cung cấp đầu vào cho Module 4 (xếp lịch khám) và Module 5 (ghi nhận kết quả).
    3. Các thông tin hiển thị trực quan trên giao diện: trạng thái phiếu, thanh tiến trình 4 bước, số ngày còn phải chờ trong chu kỳ 84 ngày, và thông báo lỗi/thành công qua `TempData`.
- **Bằng chứng trong dự án:** `DangKyController.cs` (dòng 314–332), `NguoiHienMauController.cs` (dòng 104), `Models/NguoiHienMau.cs`, `Models/PhieuDangKyHienMau.cs`.
- **Câu hỏi phụ:** *Hai trường `volume` (dung tích máu) và `timeSlot` (khung giờ hẹn) trên form đăng ký được lưu vào cột nào trong CSDL?*
- **Trả lời câu hỏi phụ:** Dạ trong bảng `PhieuDangKyHienMau` không có cột riêng cho `volume` và `timeSlot`. Khi xử lý ở `DangKyController.Create [POST]` (dòng 580–596), hệ thống nối chuỗi `volume` vào cột `LyDoHienMau` (ví dụ: `"Dung tích dự kiến: 350ml"`) và nối chuỗi `timeSlot` vào cột `GhiChu` (ví dụ: `"Khung giờ hẹn: Ca 1 (07:30 - 09:30)"`).
- **Lưu ý xác minh:** Đã xác minh trực tiếp tại dòng 580–596 của `DangKyController.cs`.

---

### Câu A5: Phạm vi chức năng và giới hạn của Module 3 là gì? Những gì trên giao diện chỉ là mô phỏng (UI Mock)?
- **Mức độ:** Trung bình
- **Câu trả lời mẫu:**
  Thưa cô, về phạm vi thực tế trong CSDL, Module 3 chịu trách nhiệm quản lý 2 bảng chính là `NguoiHienMau` và `PhieuDangKyHienMau`.
  Em cũng xin trình bày rõ những giới hạn và các phần trên giao diện hiện đang là **mô phỏng UI (Frontend Mock)** chứ chưa lưu hoặc xử lý ở tầng Backend để hội đồng nắm rõ:
  1. Ở trang `Views/DangKy/Create.cshtml`: Ô nhập **Chiều cao (cm)** (dòng 315) chỉ có trên giao diện, không có thuộc tính `name` và không lưu vào bảng `NguoiHienMau`.
  2. Ở trang `Views/NguoiHienMau/HoSoCaNhan.cshtml`:
     - Các nút **"Tải Chứng Nhận"**, **"Xuất PDF"**, **"Lưu VNeID"**, **"Chia sẻ"** sử dụng hàm JavaScript `alert(...)` mô phỏng (dòng 832–854).
     - Tab **"Hành trình giọt máu"** và Tab **"Hồ sơ xét nghiệm"** (HIV, HBV, HCV, Giang mai, Sốt rét) hiển thị nội dung tĩnh minh họa khi `totalCount > 0`, chứ trong CSDL không có bảng lưu chi tiết từng chỉ số xét nghiệm máu.
     - Tổng lượng máu đã hiến (`totalVolume`) đang được ước tính bằng công thức `totalCount * 350` ml (dòng 17).
  3. Ở trang `Views/Admin/NguoiHienMau.cshtml`: Nút **"Xuất danh sách (CSV/Excel)"**, **"Gọi máu khẩn cấp (Emergency Callout)"**, và nút gọi điện/nhắn tin SMS đang dùng hàm JavaScript `alert(...)` mô phỏng.
- **Bằng chứng trong dự án:**
  - `Views/DangKy/Create.cshtml` (dòng 315)
  - `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 17, 509–707, 832–854)
  - `Views/Admin/NguoiHienMau.cshtml` (dòng 54–58, 543, 550, 655–710)
- **Câu hỏi phụ:** *Tại sao nhóm lại để các phần mô phỏng đó trên giao diện?*
- **Trả lời câu hỏi phụ:** Dạ nhóm thiết kế giao diện theo hướng cổng thông tin hiến máu hiện đại để minh họa tầm nhìn mở rộng của sản phẩm, còn phần Backend và CSDL tập trung giải quyết trọn vẹn luồng nghiệp vụ cốt lõi theo yêu cầu đồ án là quản lý hồ sơ, đăng ký, khám sàng lọc và ghi nhận kết quả hiến máu.
- **Lưu ý xác minh:** Đã xác minh 100% trong các file `.cshtml`. Sự trung thực này sẽ giúp sinh viên ghi điểm rất cao khi bảo vệ.

---

### Câu A6: Vì sao trong thiết kế hệ thống lại gộp "Quản lý người hiến máu" và "Quản lý phiếu đăng ký hiến máu" vào cùng Module 3, nhưng trong CSDL lại tách thành 2 bảng `NguoiHienMau` và `PhieuDangKyHienMau`?
- **Mức độ:** Trung bình
- **Câu trả lời mẫu:**
  Thưa cô, có 2 lý do rất rõ ràng về mặt nghiệp vụ và thiết kế CSDL:
  - **Thứ nhất, lý do gộp chung vào Module 3 về mặt chức năng:** Khi một người đăng ký hiến máu, hệ thống luôn phải kiểm tra và cập nhật thông tin hồ sơ cá nhân/sức khỏe của người đó (như cân nặng, SĐT, nhóm máu, lịch sử các lần hiến trước) rồi mới sinh ra phiếu đăng ký. Hai nghiệp vụ này gắn liền với trải nghiệm của cùng một tác nhân là Người hiến máu.
  - **Thứ hai, lý do bắt buộc phải tách thành 2 bảng `NguoiHienMau` và `PhieuDangKyHienMau` trong CSDL:** Một người hiến máu có thể tham gia hiến máu **nhiều lần ở nhiều đợt khác nhau** (quan hệ `1 - n`). Nếu gộp chung vào 1 bảng thì mỗi lần đăng ký lại phải lưu lặp lại toàn bộ họ tên, ngày sinh, địa chỉ, nhóm máu gây dư thừa dữ liệu (vi phạm dạng chuẩn 2NF/3NF) và không thể quản lý được một hồ sơ người hiến máu duy nhất xuyên suốt các đợt.
- **Bằng chứng trong dự án:** `Data/ApplicationDbContext.cs` (dòng 52–56: cấu hình quan hệ `1 - n` giữa `NguoiHienMau` và `PhieuDangKyHienMau` qua khóa ngoại `MaNguoiHien`).
- **Câu hỏi phụ:** *Quan hệ xóa (`DeleteBehavior`) giữa `NguoiHienMau` và `PhieuDangKyHienMau` đang đặt là gì và vì sao?*
- **Trả lời câu hỏi phụ:** Dạ trong `ApplicationDbContext.cs` (dòng 55), quan hệ này được cấu hình là `.OnDelete(DeleteBehavior.Restrict)`. Nghĩa là nếu một người hiến máu đã từng có phiếu đăng ký trong hệ thống thì không được phép xóa bản ghi `NguoiHienMau` đó, nhằm bảo toàn lịch sử y tế và hồ sơ hiến máu.
- **Lưu ý xác minh:** Đã xác minh trong `ApplicationDbContext.cs`.

---

### Câu A7: Nếu bỏ Module 3 thì quy trình hiến máu của toàn hệ thống sẽ bị đứt gãy như thế nào?
- **Mức độ:** Cơ bản
- **Câu trả lời mẫu:**
  Thưa cô, nếu bỏ Module 3 thì toàn bộ quy trình nghiệp vụ từ Module 2 sang Module 4 và Module 5 sẽ bị đứt gãy hoàn toàn:
  1. **Module 2 (`DotHienMau`)** tạo ra các chiến dịch hiến máu nhưng sẽ không có ai đăng ký tham gia được vì không có bảng `PhieuDangKyHienMau`.
  2. **Module 4 (`LichKhamSangLoc`)** và **Module 5 (`KetQuaHienMau`)** đều lấy `MaPhieu` của `PhieuDangKyHienMau` làm khóa ngoại bắt buộc (`Required`). Không có phiếu đăng ký của Module 3 thì bác sĩ không thể xếp lịch khám sàng lọc và nhân viên y tế không thể ghi nhận kết quả hiến máu cho bất kỳ ai.
  3. Hệ thống cũng mất hoàn toàn thông tin y tế của người hiến (`NhomMau`, `CanNang`, `GhiChuSucKhoe`) vốn nằm ở bảng `NguoiHienMau` (do bảng `TaiKhoan` của Module 1 chỉ lưu thông tin đăng nhập cơ bản).
- **Bằng chứng trong dự án:** `Models/LichKhamSangLoc.cs` (dòng 14: `public int MaPhieu { get; set; }`), `Models/KetQuaHienMau.cs` (dòng 14: `public int MaPhieu { get; set; }`).
- **Câu hỏi phụ:** *Vì sao bảng `LichKhamSangLoc` và `KetQuaHienMau` lại nối vào `PhieuDangKyHienMau` (`MaPhieu`) chứ không nối trực tiếp vào `NguoiHienMau` (`MaNguoiHien`)?*
- **Trả lời câu hỏi phụ:** Dạ bởi vì mỗi lần khám sàng lọc và mỗi kết quả hiến máu phải gắn với **một lần đăng ký của một đợt hiến máu cụ thể** (`PhieuDangKyHienMau`), chứ nếu nối thẳng vào `NguoiHienMau` thì sẽ không biết kết quả khám hay kết quả hiến máu đó thuộc về chiến dịch nào và lần đăng ký nào.

---

### Câu A8: Khó khăn kỹ thuật và nghiệp vụ đáng chú ý nhất khi triển khai Module 3 là gì? Phần nào là quan trọng nhất?
- **Mức độ:** Trung bình
- **Câu trả lời mẫu:**
  Thưa cô, phần quan trọng nhất và cũng là thách thức lớn nhất khi triển khai Module 3 gồm 2 vấn đề:
  1. **Về nghiệp vụ:** Xây dựng quy tắc kiểm tra thời gian chờ **84 ngày** giữa hai lần hiến máu. Khó khăn nằm ở chỗ không được phép lấy ngày nộp phiếu (`NgayNop`) để tính 84 ngày, vì nộp phiếu chưa chắc đã đi hiến hoặc có thể khám sàng lọc không đạt. Hệ thống phải truy ngược sang bảng `KetQuaHienMau` (Module 5), chỉ tính những phiếu có kết quả `"Hiến máu thành công"` và lấy ngày thực tế tại `KetQuaHienMau.NgayCapNhat`. Ngoài ra còn phải xử lý an toàn trường hợp dữ liệu lịch sử bị khuyết ngày (`HasMissingActualDonationDate`) để không tính sai.
  2. **Về kỹ thuật:** Chống lỗi **Race Condition (gửi 2 request đăng ký đồng thời)** khi người dùng bấm đúp chuột hoặc dùng công cụ gửi nhiều HTTP POST cùng lúc. Em đã giải quyết bằng cách kết hợp khóa đồng bộ trong bộ nhớ `SemaphoreSlim` theo từng `MaNguoiHien` (`_submissionLocks`) với Database Transaction ở mức cô lập cao nhất `IsolationLevel.Serializable` để kiểm tra lại điều kiện 84 ngày và phiếu trùng ngay bên trong transaction trước khi lưu.
- **Bằng chứng trong dự án:** `Controllers/DangKyController.cs` (dòng 23–178 cho thuật toán 84 ngày, và dòng 532–616 cho `SemaphoreSlim` + `IsolationLevel.Serializable`).
- **Câu hỏi phụ:** *Nếu chỉ dùng kiểm tra `if (existingPhieu != null)` bình thường mà không có `SemaphoreSlim` và Transaction `Serializable` thì điều gì xảy ra?*
- **Trả lời câu hỏi phụ:** Dạ nếu 2 request đến cùng một mili-giây, cả 2 luồng đều chạy câu lệnh `SELECT` kiểm tra `existingPhieu` trước khi luồng kia kịp `INSERT`, dẫn đến cả 2 đều thấy `existingPhieu == null` và cùng `INSERT` tạo ra 2 phiếu đăng ký trùng nhau cho cùng 1 người trong 1 đợt.

---

## NHÓM B — PHÂN TÍCH CHI TIẾT 11 NGHIỆP VỤ CỦA MODULE 3 (10 TIÊU CHÍ / NGHIỆP VỤ)

Dưới đây là phân tích chi tiết từng nghiệp vụ thực tế tồn tại trong code của Module 3 theo đúng 10 câu hỏi chuẩn:

---

### Nghiệp vụ B1: Xem và cập nhật hồ sơ cá nhân người hiến máu (`HoSoCaNhan`)
1. **Ai được phép thực hiện?** Người dùng đã đăng nhập (bất kỳ Role nào có phiên đăng nhập hợp lệ vì `NguoiHienMauController` gắn `[Authorize]`, thực tế đối tượng chính là người hiến máu).
2. **Điều kiện đầu vào là gì?** Phải có Cookie xác thực chứa claim `"MaTaiKhoan"` parse được sang kiểu `int` (`int.TryParse(maTaiKhoanClaim, out int maTaiKhoan)`).
3. **Hệ thống xử lý theo trình tự nào?**
   - **Ở `[HttpGet] HoSoCaNhan()`:**
     - Bước 1: Đọc `MaTaiKhoan` từ `User.FindFirst("MaTaiKhoan")?.Value`. Nếu không có -> `RedirectToAction("Login", "Account")`.
     - Bước 2: Truy vấn `NguoiHienMau` kèm `TaiKhoan`, `PhieuDangKyHienMaus` -> `DotHienMau`, `KetQuaHienMau`, `LichKhamSangLocs`.
     - Bước 3: Nếu chưa có bản ghi `NguoiHienMau` trong DB, đọc bảng `TaiKhoan` và khởi tạo một đối tượng `new NguoiHienMau` tạm trên RAM (chưa `SaveChanges`) để đổ sẵn `HoTen`, `Email`, `SoDienThoai`, `TrangThai = "Hoạt động"` lên View.
     - Bước 4: Gọi `DangKyController.EvaluateRecentDonationEligibility(lichSuPhieu)` và `IsConfirmedSuccessfulDonation` để tính số lần hiến thành công thực tế, ngày hiến gần nhất, ngày đủ điều kiện tiếp theo và số ngày còn phải chờ, gán vào `ViewBag`.
   - **Ở `[HttpPost] HoSoCaNhan(NguoiHienMau model)`:**
     - Bước 1: Đọc `MaTaiKhoan` từ Cookie.
     - Bước 2: Chuẩn hóa dữ liệu (`Trim()` các chuỗi, `NormalizePhone` loại bỏ khoảng trắng, dấu gạch ngang, dấu chấm).
     - Bước 3: Kiểm tra hợp lệ dữ liệu (Họ tên, SĐT, Ngày sinh, Giới tính, Email, Địa chỉ, Cân nặng).
     - Bước 4: Tìm `nguoiHienMau` trong DB theo `MaTaiKhoan`. Nếu chưa có thì `new NguoiHienMau` và `_context.Add(nguoiHienMau)`. Nếu đã có thì cập nhật từng trường vào thực thể đang được EF Core theo dõi (tracked entity) và đồng bộ `nguoiHienMau.TaiKhoan.HoTen = model.HoTen`.
     - Bước 5: Gọi `await _context.SaveChangesAsync()` và chuyển hướng về lại `HoSoCaNhan`.
4. **Điều kiện thành công là gì?** Tất cả các kiểm tra dữ liệu đầu vào không phát sinh lỗi (`errors.Count == 0`) và `SaveChangesAsync()` thực thi thành công.
5. **Những trường hợp nào bị từ chối?**
   - Chưa đăng nhập hoặc Cookie không có `MaTaiKhoan`.
   - Họ tên rỗng hoặc `> 100` ký tự.
   - SĐT rỗng hoặc không khớp biểu thức chính quy `^0[0-9]{9,10}$`.
   - Ngày sinh bị bỏ trống, lớn hơn ngày hiện tại, hoặc tuổi tính theo ngày sinh `< 18` hoặc `> 60`.
   - Giới tính không thuộc `{"Nam", "Nữ", "Khác"}`.
   - Email rỗng, `> 100` ký tự hoặc sai định dạng `EmailAddressAttribute`.
   - Địa chỉ rỗng hoặc `> 255` ký tự.
   - Cân nặng bị bỏ trống hoặc `< 30` kg hoặc `> 200` kg.
   - Nhóm máu `> 20` ký tự, Nghề nghiệp `> 100` ký tự, Ghi chú sức khỏe `> 500` ký tự.
6. **Dữ liệu nào được thêm, sửa hoặc đọc?**
   - **Đọc:** `NguoiHienMau`, `TaiKhoan`, `PhieuDangKyHienMau`, `DotHienMau`, `KetQuaHienMau`, `LichKhamSangLoc`.
   - **Thêm/Sửa:** Thêm mới hoặc cập nhật bản ghi `NguoiHienMau`; cập nhật cột `HoTen` của bảng `TaiKhoan`.
7. **Hệ thống phản hồi thế nào khi lỗi?** Gộp tất cả các thông báo lỗi bằng `string.Join(" ", errors)` gán vào `TempData["Error"]`, nạp lại dữ liệu thống kê vào `ViewBag` và trả về `View(model)` kèm các giá trị người dùng vừa nhập để họ không phải gõ lại.
8. **Có tình huống ngoại lệ nào cần xử lý?** Trường hợp tài khoản được tạo trực tiếp trong DB (hoặc seed data) mà chưa có bản ghi `NguoiHienMau` tương ứng: code đã xử lý an toàn ở cả `GET` (tạo object tạm) và `POST` (`if (nguoiHienMau == null)` thì tạo mới và `_context.Add`).
9. **Code nào thực hiện?** `Controllers/NguoiHienMauController.cs` (dòng 48–282) và `Views/NguoiHienMau/HoSoCaNhan.cshtml`.
10. **Có cần phối hợp với module khác không?** Có: phối hợp với Module 1 (`TaiKhoan`), Module 2 (`DotHienMau`), Module 4 (`LichKhamSangLoc`) và Module 5 (`KetQuaHienMau`).

---

### Nghiệp vụ B2: Kiểm tra dữ liệu hồ sơ người hiến máu (Data Validation)
1. **Ai được phép thực hiện?** Hệ thống tự động thực hiện ở cả Client-side (HTML5/JS) và Server-side (Controller + Model Data Annotations) khi người hiến máu lưu hồ sơ hoặc gửi phiếu đăng ký.
2. **Điều kiện đầu vào là gì?** Các trường thông tin cá nhân và chỉ số cơ thể gửi lên từ form `HoSoCaNhan` hoặc `DangKy/Create`.
3. **Hệ thống xử lý theo trình tự nào?**
   - Bước 1: Chuẩn hóa chuỗi (`Trim()`) và chuẩn hóa số điện thoại bằng `NormalizePhone()` (xóa khoảng trắng, `-`, `.`).
   - Bước 2: Kiểm tra tính bắt buộc (`string.IsNullOrWhiteSpace`, `.HasValue`).
   - Bước 3: Kiểm tra định dạng và miền giá trị (`IsValidPhone`, `IsValidAge`, `EmailAddressAttribute`, khoảng cân nặng `30..200`).
4. **Điều kiện thành công là gì?** Mọi trường bắt buộc đều có giá trị và nằm trong miền hợp lệ.
5. **Những trường hợp nào bị từ chối?**
   - SĐT có chữ cái, không bắt đầu bằng số `0`, hoặc độ dài khác 10–11 chữ số.
   - Tuổi tính chính xác theo ngày/tháng/năm sinh dưới 18 tuổi hoặc trên 60 tuổi.
   - Cân nặng ngoài khoảng `30–200` kg.
6. **Dữ liệu nào được thêm, sửa hoặc đọc?** Chỉ đọc dữ liệu từ request form/model trên bộ nhớ trước khi cho phép ghi xuống DB.
7. **Hệ thống phản hồi thế nào khi lỗi?** Ghi thông báo tiếng Việt cụ thể vào `TempData["Error"]` và chặn thao tác `SaveChangesAsync()`.
8. **Có tình huống ngoại lệ nào cần xử lý?** Người dùng nhập SĐT có dấu cách hoặc dấu chấm (ví dụ `0912.345.678` hoặc `0912 345 678`): hàm `NormalizePhone` sẽ tự động làm sạch thành `0912345678` trước khi kiểm tra Regex, giúp trải nghiệm người dùng thân thiện hơn.
9. **Code nào thực hiện?**
   - `Controllers/NguoiHienMauController.cs` (hàm `NormalizePhone`, `IsValidPhone`, `IsValidAge` dòng 20–45 và logic validate dòng 126–198).
   - `Controllers/DangKyController.cs` (dòng 180–205 và 422–523).
   - `Models/NguoiHienMau.cs` (Data Annotations dòng 16–53).
10. **Có cần phối hợp với module khác không?** Khi kiểm tra ở `DangKyController.Create`, điều kiện cân nặng còn phải phối hợp với `DotHienMau.CanNangToiThieu` của Module 2.

---

### Nghiệp vụ B3: Tạo phiếu đăng ký hiến máu (`DangKyController.Create`)
1. **Ai được phép thực hiện?** Người hiến máu đã đăng nhập hoặc khách vãng lai truy cập trang đăng ký.
2. **Điều kiện đầu vào là gì?** Mã đợt hiến máu `maDot`, thông tin cá nhân, cân nặng, câu trả lời sàng lọc `health_q1`, `health_q2`, cùng tuỳ chọn `volume`, `timeSlot`.
3. **Hệ thống xử lý theo trình tự nào?**
   - Bước 1: Kiểm tra sự tồn tại và trạng thái của `DotHienMau` (`"Đang mở"` hoặc `"Sắp diễn ra"`, `KetThuc >= DateTime.Now`, số phiếu hợp lệ `< SoLuongToiDa`).
   - Bước 2: Tìm hồ sơ `NguoiHienMau` hiện có (theo `MaTaiKhoan` nếu đã đăng nhập, hoặc theo `SoDienThoai` nếu chưa đăng nhập).
   - Bước 3: Kiểm tra toàn bộ thông tin cá nhân, tuổi 18–60, SĐT, email, cân nặng (`30–200` kg và `>= dotHienMau.CanNangToiThieu`), và 2 câu hỏi sức khỏe (`health_q1 == "none"`, `health_q2 == "good"`).
   - Bước 4: Tạo mới hoặc cập nhật bản ghi `NguoiHienMau` và gọi `SaveChangesAsync()` để đảm bảo có `MaNguoiHien`.
   - Bước 5: Lấy khóa `SemaphoreSlim` theo `MaNguoiHien` và mở Transaction `IsolationLevel.Serializable`.
   - Bước 6: Trong Transaction, truy vấn lại toàn bộ lịch sử phiếu của `MaNguoiHien`, kiểm tra quy tắc 84 ngày (`EvaluateRecentDonationEligibility`) và kiểm tra xem đã có phiếu đang hoạt động trong cùng `maDot` chưa (`existingPhieu`).
   - Bước 7: Tạo đối tượng `PhieuDangKyHienMau` với `TrangThai = "Chờ xác nhận"`, `NgayNop = DateTime.Now`, ghép `volume` vào `LyDoHienMau`, ghép `timeSlot` vào `GhiChu`, lưu vào DB và `CommitAsync()`.
4. **Điều kiện thành công là gì?** Đợt hiến máu hợp lệ, thông tin cá nhân/sức khỏe đạt chuẩn, đủ 84 ngày kể từ lần hiến thành công gần nhất, chưa có phiếu đang hoạt động trong cùng đợt, và transaction commit thành công.
5. **Những trường hợp nào bị từ chối?**
   - Đợt hiến máu không tồn tại, đã đóng/hủy/kết thúc, hoặc đã đủ `SoLuongToiDa`.
   - Thông tin cá nhân không hợp lệ, tuổi ngoài 18–60, cân nặng `< dotHienMau.CanNangToiThieu`.
   - Khai báo có bệnh truyền nhiễm (`health_q1 != "none"`) hoặc sức khỏe 3 tháng gần đây không tốt (`health_q2 != "good"`).
   - Chưa đủ 84 ngày kể từ ngày có kết quả `"Hiến máu thành công"` gần nhất, hoặc có phiếu `"Hiến máu thành công"` nhưng bị thiếu ngày kết quả thực tế.
   - Đã có một phiếu đang chờ xử lý (`Chờ xác nhận`, `Đã xác nhận`, `Chờ khám sàng lọc`, `Đã khám sàng lọc`) trong cùng đợt hiến máu đó.
6. **Dữ liệu nào được thêm, sửa hoặc đọc?**
   - **Đọc:** `DotHienMau`, `NguoiHienMau`, `TaiKhoan`, `PhieuDangKyHienMau`, `KetQuaHienMau`.
   - **Thêm/Sửa:** Thêm mới hoặc cập nhật `NguoiHienMau`; thêm mới 1 bản ghi `PhieuDangKyHienMau`.
7. **Hệ thống phản hồi thế nào khi lỗi?** Gán lý do từ chối chi tiết vào `TempData["Error"]` và `RedirectToAction("Create", new { maDot })`.
8. **Có tình huống ngoại lệ nào cần xử lý?** Tình huống 2 request gửi đồng thời hoặc ngoại lệ CSDL trong lúc tạo phiếu: khối `try ... catch ... finally` đảm bảo luôn `RollbackAsync()` transaction và `userLock.Release()` để không bao giờ gây treo khóa (deadlock).
9. **Code nào thực hiện?** `Controllers/DangKyController.cs` (GET dòng 207–310, POST dòng 312–681) và `Views/DangKy/Create.cshtml`.
10. **Có cần phối hợp với module khác không?** Phối hợp chặt chẽ với Module 1 (`TaiKhoan`), Module 2 (`DotHienMau`) và Module 5 (`KetQuaHienMau`).

---

### Nghiệp vụ B4: Kiểm tra điều kiện trước khi đăng ký (Pre-registration Eligibility Check)
1. **Ai được phép thực hiện?** Hệ thống tự động thực hiện ở cả `[HttpGet] Create` (để hiển thị cảnh báo sớm) và `[HttpPost] Create` (để chặn thực tế).
2. **Điều kiện đầu vào là gì?** `MaDot`, `MaNguoiHien`, danh sách `PhieuDangKyHienMau` lịch sử kèm `KetQuaHienMau`.
3. **Hệ thống xử lý theo trình tự nào?**
   - Bước 1: Đếm số lượng người đã đăng ký hợp lệ của đợt (`p.TrangThai != "Đã hủy" && p.TrangThai != "Từ chối" && p.TrangThai != "Không đủ điều kiện hiến"`). Nếu `>= dotHienMau.SoLuongToiDa` thì đánh dấu hết chỉ tiêu.
   - Bước 2: Gọi `EvaluateRecentDonationEligibility(lichSuPhieu)` để kiểm tra quy tắc 84 ngày.
   - Bước 3: Kiểm tra `existingPhieu` trong cùng `maDot`.
4. **Điều kiện thành công là gì?** `coTheDangKy == true` (đợt còn chỗ, đủ 84 ngày, chưa có phiếu đang hoạt động trong đợt).
5. **Những trường hợp nào bị từ chối?** Đợt đầy chỗ, vi phạm 84 ngày, thiếu ngày hiến thực tế, hoặc đã có phiếu trong đợt.
6. **Dữ liệu nào được thêm, sửa hoặc đọc?** Chỉ đọc từ `DotHienMau`, `PhieuDangKyHienMau`, `KetQuaHienMau`.
7. **Hệ thống phản hồi thế nào khi lỗi?**
   - Ở `GET`: Gán `ViewBag.CoTheDangKy = false` và `ViewBag.LyDoKhongTheDangKy`, giao diện `Create.cshtml` hiển thị khung cảnh báo màu đỏ và vô hiệu hóa (`disabled`) nút gửi phiếu.
   - Ở `POST`: Chặn thẳng và trả về `TempData["Error"]`.
8. **Có tình huống ngoại lệ nào cần xử lý?** Người dùng đã hiến máu thành công ở một đợt và muốn đăng ký lại trong khi vẫn còn dưới 84 ngày: hệ thống tính chính xác số ngày còn lại (`remainingDays`) và ngày cụ thể được phép đăng ký lại (`nextEligibleDate.ToString("dd/MM/yyyy")`).
9. **Code nào thực hiện?** `DangKyController.cs` (dòng 94–178, 250–308, 536–578).
10. **Có cần phối hợp với module khác không?** Phối hợp với Module 2 (`DotHienMau`) và Module 5 (`KetQuaHienMau`).

---

### Nghiệp vụ B5: Theo dõi phiếu đăng ký (`DangKyController.TheoDoi`)
1. **Ai được phép thực hiện?** Người hiến máu đã đăng nhập (`[Authorize]`).
2. **Điều kiện đầu vào là gì?** Claim `"MaTaiKhoan"` trong Cookie đăng nhập.
3. **Hệ thống xử lý theo trình tự nào?**
   - Bước 1: Lấy `MaTaiKhoan` từ Cookie. Nếu không hợp lệ -> chuyển hướng sang `Account/Login`.
   - Bước 2: Tìm bản ghi `NguoiHienMau` theo `MaTaiKhoan`. Nếu chưa có -> trả về danh sách rỗng `new List<PhieuDangKyHienMau>()`.
   - Bước 3: Truy vấn danh sách `PhieuDangKyHienMau` của `MaNguoiHien` kèm `DotHienMau`, `LichKhamSangLocs`, `KetQuaHienMau`, sắp xếp giảm dần theo `NgayNop` rồi đến `MaPhieu`.
   - Bước 4: Gọi `EvaluateRecentDonationEligibility(danhSachPhieu)` và truyền kết quả qua `ViewBag.EligibilityResult`.
4. **Điều kiện thành công là gì?** Hiển thị thành công trang `TheoDoi.cshtml` với phiếu đang hoạt động (nếu có) trên thanh tiến trình 4 bước và bảng lịch sử bên dưới.
5. **Những trường hợp nào bị từ chối?** Người dùng chưa đăng nhập sẽ bị中间件 Cookie Authentication tự động chuyển hướng về `/Account/Login`.
6. **Dữ liệu nào được thêm, sửa hoặc đọc?** Chỉ đọc từ `NguoiHienMau`, `PhieuDangKyHienMau`, `DotHienMau`, `LichKhamSangLoc`, `KetQuaHienMau`.
7. **Hệ thống phản hồi thế nào khi lỗi?** Nếu người dùng chưa từng có phiếu nào, giao diện hiển thị màn hình trạng thái trống ("Bạn chưa có phiếu đăng ký hiến máu nào đang chờ xử lý") kèm nút "Đăng ký hiến máu ngay".
8. **Có tình huống ngoại lệ nào cần xử lý?** Cách chọn `activePhieu` trên `TheoDoi.cshtml` (dòng 12–16): Hệ thống chỉ chọn phiếu mới nhất có trạng thái khác `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"` và `"Hiến máu thành công"` để hiển thị ở khu vực "Phiếu đang hoạt động". Khi phiếu đã có kết quả cuối cùng, nó sẽ chuyển xuống bảng Lịch sử.
9. **Code nào thực hiện?** `Controllers/DangKyController.cs` (dòng 685–721) và `Views/DangKy/TheoDoi.cshtml`.
10. **Có cần phối hợp với module khác không?** Hiển thị trực tiếp dữ liệu lịch khám của Module 4 (`LichKhamSangLocs`) và kết quả của Module 5 (`KetQuaHienMau`).

---

### Nghiệp vụ B6: Hủy phiếu đăng ký (`DangKyController.Huy`)
1. **Ai được phép thực hiện?** Chính người hiến máu sở hữu phiếu đó (`[Authorize]`).
2. **Điều kiện đầu vào là gì?** `int id` (tức `MaPhieu`) gửi qua `POST /DangKy/Huy` kèm Anti-Forgery Token và Cookie đăng nhập.
3. **Hệ thống xử lý theo trình tự nào?**
   - Bước 1: Kiểm tra `MaTaiKhoan` từ Cookie và tìm `NguoiHienMau` tương ứng.
   - Bước 2: Truy vấn `PhieuDangKyHienMau` kèm `LichKhamSangLocs` và `KetQuaHienMau` với điều kiện kép: `p.MaPhieu == id && p.MaNguoiHien == nguoiHienMau.MaNguoiHien`.
   - Bước 3: Kiểm tra `phieu.TrangThai` có nằm trong mảng `{"Chờ xác nhận", "Đã xác nhận"}` hay không.
   - Bước 4: Kiểm tra `phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy")` và `phieu.KetQuaHienMau != null`.
   - Bước 5: Cập nhật `phieu.TrangThai = "Đã hủy"`, `phieu.NgayXuLy = DateTime.Now` và lưu `SaveChangesAsync()`.
4. **Điều kiện thành công là gì?** Phiếu tồn tại, thuộc quyền sở hữu của người đang đăng nhập, đang ở trạng thái `"Chờ xác nhận"` hoặc `"Đã xác nhận"`, chưa có lịch khám sàng lọc đang hoạt động và chưa có kết quả hiến máu.
5. **Những trường hợp nào bị từ chối?**
   - Phiếu không tồn tại hoặc thuộc về người hiến máu khác (chống tấn công IDOR).
   - Phiếu đã chuyển sang `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`, `"Hiến máu thành công"`, `"Không đủ điều kiện hiến"`, `"Từ chối"` hoặc `"Đã hủy"`.
   - Phiếu đã được xếp lịch khám sàng lọc (`TrangThai != "Đã hủy"`) hoặc đã có bản ghi `KetQuaHienMau`.
6. **Dữ liệu nào được thêm, sửa hoặc đọc?**
   - **Đọc:** `NguoiHienMau`, `PhieuDangKyHienMau`, `LichKhamSangLoc`, `KetQuaHienMau`.
   - **Sửa:** Cập nhật `TrangThai = "Đã hủy"` và `NgayXuLy = DateTime.Now` trên bảng `PhieuDangKyHienMau`.
7. **Hệ thống phản hồi thế nào khi lỗi?** Gán thông báo lỗi cụ thể vào `TempData["Error"]` và `RedirectToAction("TheoDoi")`.
8. **Có tình huống ngoại lệ nào cần xử lý?** Trường hợp phiếu vẫn mang trạng thái `"Đã xác nhận"` nhưng nhân viên y tế đã tạo bản ghi `LichKhamSangLoc`: nhờ có câu lệnh kiểm tra `phieu.LichKhamSangLocs.Any(l => l.TrangThai != "Đã hủy")` (dòng 765), hệ thống vẫn chặn hủy an toàn.
9. **Code nào thực hiện?** `Controllers/DangKyController.cs` (dòng 725–781) và `Views/DangKy/TheoDoi.cshtml` (dòng 356–367, 569–611).
10. **Có cần phối hợp với module khác không?** Phối hợp với Module 4 (`LichKhamSangLoc`) và Module 5 (`KetQuaHienMau`) để kiểm tra điều kiện chặn hủy.

---

### Nghiệp vụ B7: Xem lịch sử hiến máu (Donation History)
1. **Ai được phép thực hiện?** Người hiến máu đã đăng nhập (xem lịch sử của chính mình tại `TheoDoi` và `HoSoCaNhan`) hoặc Admin/Nhân viên y tế (xem tại `Admin/NguoiHienMau` và `Admin/PhieuDangKyHienMau`).
2. **Điều kiện đầu vào là gì?** `MaNguoiHien` được xác định từ tài khoản đăng nhập.
3. **Hệ thống xử lý theo trình tự nào?**
   - Truy vấn `PhieuDangKyHienMaus` kèm `.Include(p => p.DotHienMau)`, `.Include(p => p.KetQuaHienMau)`, `.Include(p => p.LichKhamSangLocs)`, sắp xếp giảm dần theo `NgayNop` và `MaPhieu`.
   - Tại `HoSoCaNhan.cshtml` và `TheoDoi.cshtml`, phân biệt rõ **Tổng số phiếu đã gửi** và **Số lần hiến máu thành công thực tế** (`IsConfirmedSuccessfulDonation`).
4. **Điều kiện thành công là gì?** Hiển thị đầy đủ danh sách các lần đăng ký kèm tên chiến dịch, ngày nộp phiếu, ngày hiến máu thực tế (nếu đã thành công) và nhãn trạng thái tương ứng.
5. **Những trường hợp nào bị từ chối?** Chưa đăng nhập.
6. **Dữ liệu nào được thêm, sửa hoặc đọc?** Chỉ đọc dữ liệu.
7. **Hệ thống phản hồi thế nào khi lỗi?** Nếu danh sách trống, hiển thị thông báo "Chưa có dữ liệu lịch sử hiến máu".
8. **Có tình huống ngoại lệ nào cần xử lý?** Phân biệt giữa ngày nộp phiếu (`NgayNop`) và ngày hiến máu thực tế (`KetQuaHienMau.NgayCapNhat`): Trên `HoSoCaNhan.cshtml` (dòng 484–493), chỉ khi `isSuccess && phieu.KetQuaHienMau?.NgayCapNhat != null` thì mới hiển thị dòng màu xanh `"Hiến ngày: dd/MM/yyyy"`.
9. **Code nào thực hiện?** `NguoiHienMauController.cs` (dòng 56–101), `DangKyController.cs` (dòng 703–716), `Views/NguoiHienMau/HoSoCaNhan.cshtml` (dòng 436–506), `Views/DangKy/TheoDoi.cshtml` (dòng 394–565).
10. **Có cần phối hợp với module khác không?** Có, đọc dữ liệu từ Module 2 (`DotHienMau`), Module 4 (`LichKhamSangLoc`) và Module 5 (`KetQuaHienMau`).

---

### Nghiệp vụ B8: Hiển thị và chuyển đổi trạng thái phiếu đăng ký (Ticket Status Lifecycle)
1. **Ai được phép thực hiện?**
   - Người hiến máu: Tạo phiếu (`"Chờ xác nhận"`), Hủy phiếu (`"Đã hủy"`).
   - Admin / Nhân viên y tế: Xác nhận (`"Đã xác nhận"`), Từ chối (`"Từ chối"`), Chuyển chờ khám / Tạo lịch khám (`"Chờ khám sàng lọc"`), Hoàn thành khám (`"Đã khám sàng lọc"`), Ghi nhận kết quả (`"Hiến máu thành công"` hoặc `"Không đủ điều kiện hiến"`).
2. **Điều kiện đầu vào là gì?** `MaPhieu` và trạng thái hiện tại của phiếu.
3. **Hệ thống xử lý theo trình tự nào?** Mỗi action trong `AdminController` kiểm tra trạng thái hiện tại trước khi chuyển sang trạng thái tiếp theo và cập nhật `NgayXuLy = DateTime.Now`.
4. **Điều kiện thành công là gì?** Chuyển đổi đúng luồng trạng thái quy định:
   - `XacNhanPhieu`: chỉ nhận phiếu đang `"Chờ xác nhận"`.
   - `TuChoiPhieu`: nhận phiếu đang `"Chờ xác nhận"`, `"Đã xác nhận"`, hoặc `"Chờ khám sàng lọc"` (và chưa có kết quả).
   - `ChuyenChoKham`: chỉ nhận phiếu đang `"Đã xác nhận"`.
   - `TaoLichKham`: chỉ nhận phiếu đang `"Chờ xác nhận"`, `"Đã xác nhận"`, hoặc `"Chờ khám sàng lọc"`.
   - `HoanThanhKham`: chuyển phiếu sang `"Đã khám sàng lọc"`.
   - `TaoKetQua` / `SuaKetQua`: chỉ nhận phiếu đang `"Đã khám sàng lọc"` (khi tạo mới) và đồng bộ `phieu.TrangThai = ketQua` (`"Hiến máu thành công"` hoặc `"Không đủ điều kiện hiến"`).
5. **Những trường hợp nào bị từ chối?** Chuyển trạng thái nhảy cóc (ví dụ: phiếu chưa `"Đã khám sàng lọc"` đã bấm `TaoKetQua`, hoặc phiếu đã có kết quả mà bấm `TuChoiPhieu`).
6. **Dữ liệu nào được thêm, sửa hoặc đọc?** Sửa cột `TrangThai`, `NgayXuLy`, `GhiChuXacNhan` của `PhieuDangKyHienMau`.
7. **Hệ thống phản hồi thế nào khi lỗi?** Trả về `TempData["Error"]` với thông báo trạng thái không hợp lệ.
8. **Có tình huống ngoại lệ nào cần xử lý?** Khi Admin từ chối một phiếu đang ở trạng thái `"Chờ khám sàng lọc"` (`AdminController.TuChoiPhieu`, dòng 477–486), hệ thống tự động hủy luôn các lịch khám sàng lọc chưa hoàn thành của phiếu đó để dữ liệu nhất quán.
9. **Code nào thực hiện?** `AdminController.cs` (dòng 422–523, 811–980, 1029–1149) và `Views/DangKy/TheoDoi.cshtml` (Stepper 4 bước dòng 130–310).
10. **Có cần phối hợp với module khác không?** Đây chính là điểm giao thoa giữa Module 3, Module 4 và Module 5.

---

### Nghiệp vụ B9: Liên kết phiếu đăng ký và hồ sơ người hiến máu với tài khoản (`MaTaiKhoan` & `MaNguoiHien`)
1. **Ai được phép thực hiện?** Hệ thống tự động thực hiện khi đăng ký tài khoản (`AccountController.Register`), khi cập nhật hồ sơ (`NguoiHienMauController.HoSoCaNhan`) và khi tạo phiếu (`DangKyController.Create`).
2. **Điều kiện đầu vào là gì?** `MaTaiKhoan` (nếu đã đăng nhập) hoặc `SoDienThoai` (nếu khách vãng lai hoặc nối hồ sơ cũ).
3. **Hệ thống xử lý theo trình tự nào?**
   - **Trường hợp 1 (Đã đăng nhập):** Tìm `NguoiHienMau` theo `MaTaiKhoan`. Nếu chưa có bản ghi gắn `MaTaiKhoan`, hệ thống tìm tiếp xem có bản ghi `NguoiHienMau` nào trùng `SoDienThoai` mà `MaTaiKhoan == null` không (`DangKyController.cs` dòng 367–370). Nếu thấy thì tự động gắn `nguoiHienMau.MaTaiKhoan = maTaiKhoan` để hợp nhất lịch sử hiến máu trước đây của họ vào tài khoản mới!
   - **Trường hợp 2 (Chưa đăng nhập):** Tìm `NguoiHienMau` theo `SoDienThoai` đã chuẩn hóa (dòng 404–413). Nếu thấy thì tái sử dụng `MaNguoiHien` đó để kiểm tra 84 ngày và gắn phiếu mới vào; nếu không thấy mới tạo `NguoiHienMau` mới với `MaTaiKhoan = null`.
4. **Điều kiện thành công là gì?** Mỗi tài khoản chỉ liên kết tối đa với 1 hồ sơ `NguoiHienMau` (được bảo vệ bởi Unique Filtered Index `[MaTaiKhoan] IS NOT NULL` trong SQL Server), và mọi phiếu đăng ký đều có `MaNguoiHien` hợp lệ.
5. **Những trường hợp nào bị từ chối?** Hai tài khoản khác nhau cố gắng gắn vào cùng một `MaTaiKhoan` (bị chặn bởi Unique Index).
6. **Dữ liệu nào được thêm, sửa hoặc đọc?** Bảng `TaiKhoan`, `NguoiHienMau`, `PhieuDangKyHienMau`.
7. **Hệ thống phản hồi thế nào khi lỗi?** Tự động hợp nhất hoặc tạo mới trong code mà không làm gián đoạn người dùng.
8. **Có tình huống ngoại lệ nào cần xử lý?** Khách vãng lai từng đăng ký bằng SĐT `0988111222` (khi chưa có tài khoản), sau đó tạo tài khoản và đăng ký hiến máu tiếp: code tại dòng 367–376 của `DangKyController.cs` tự động nhận diện bản ghi `NguoiHienMau` có `MaTaiKhoan == null` và nối vào tài khoản mới.
9. **Code nào thực hiện?** `DangKyController.cs` (dòng 356–420), `AccountController.cs` (dòng 150–162), `ApplicationDbContext.cs` (dòng 24–30).
10. **Có cần phối hợp với module khác không?** Phối hợp trực tiếp với Module 1 (`TaiKhoan`).

---

### Nghiệp vụ B10: Xử lý dữ liệu phiếu chưa có kết quả hiến máu hoặc bị từ chối/hủy
1. **Ai được phép thực hiện?** Hệ thống tự động xử lý trong `DangKyController.IsConfirmedSuccessfulDonation` và `EvaluateRecentDonationEligibility`.
2. **Điều kiện đầu vào là gì?** Danh sách `PhieuDangKyHienMau` của người hiến máu.
3. **Hệ thống xử lý theo trình tự nào?**
   - Khi kiểm tra điều kiện 84 ngày: Hàm `IsConfirmedSuccessfulDonation(phieu)` loại bỏ hoàn toàn các phiếu có trạng thái `"Chờ xác nhận"`, `"Đã xác nhận"`, `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`, `"Đã hủy"`, `"Từ chối"` và `"Không đủ điều kiện hiến"`. Những phiếu này **không phải là lần hiến máu thành công** nên tuyệt đối không kích hoạt thời gian chờ 84 ngày.
   - Khi kiểm tra trùng phiếu trong cùng một đợt (`existingPhieu`): Hệ thống chặn nếu người dùng đang có phiếu ở trạng thái đang xử lý (`Chờ xác nhận`, `Đã xác nhận`, `Chờ khám sàng lọc`, `Đã khám sàng lọc`), nhưng **cho phép đăng ký lại** nếu phiếu trước đó trong đợt đã bị `"Đã hủy"`, `"Từ chối"` hoặc `"Không đủ điều kiện hiến"`.
4. **Điều kiện thành công là gì?** Người dùng có phiếu cũ bị hủy/từ chối/không đủ điều kiện khám sàng lọc không bị khóa 84 ngày oan; ngược lại người đang có phiếu chờ xử lý trong đợt thì không thể gửi thêm phiếu trùng.
5. **Những trường hợp nào bị từ chối?** Gửi phiếu thứ 2 vào cùng một đợt khi phiếu thứ 1 vẫn đang ở trạng thái `"Chờ xác nhận"` / `"Đã xác nhận"` / `"Chờ khám sàng lọc"` / `"Đã khám sàng lọc"`.
6. **Dữ liệu nào được thêm, sửa hoặc đọc?** Đọc `PhieuDangKyHienMau` và `KetQuaHienMau`.
7. **Hệ thống phản hồi thế nào khi lỗi?** Nếu trùng phiếu đang hoạt động trong đợt, thông báo: *"Bạn đã có phiếu đăng ký (#DKHM-xxxxx) ở trạng thái '...' cho đợt hiến máu này."*
8. **Có tình huống ngoại lệ nào cần xử lý?** Phiếu có `TrangThai == "Hiến máu thành công"` nhưng `KetQuaHienMau.KetQua == "Không đủ điều kiện hiến"` (dữ liệu lệch): hàm `IsConfirmedSuccessfulDonation` ưu tiên kiểm tra `phieu.KetQuaHienMau.KetQua` trước (dòng 52–55 `DangKyController.cs`).
9. **Code nào thực hiện?** `DangKyController.cs` (dòng 41–60, 293–307, 553–578).
10. **Có cần phối hợp với module khác không?** Phối hợp với Module 5 (`KetQuaHienMau`).

---

### Nghiệp vụ B11: Kiểm tra thời gian chờ 84 ngày trước lần đăng ký tiếp theo (`EvaluateRecentDonationEligibility`)
1. **Ai được phép thực hiện?** Hệ thống tự động thực thi thông qua phương thức `DangKyController.EvaluateRecentDonationEligibility`.
2. **Điều kiện đầu vào là gì?** Tập hợp `IEnumerable<PhieuDangKyHienMau> lichSuPhieu` (đã `.Include(p => p.KetQuaHienMau)`) và ngày tham chiếu `referenceDate` (mặc định là `DateTime.Today`).
3. **Hệ thống xử lý theo trình tự nào?**
   - Bước 1: Lọc ra tất cả các phiếu thỏa mãn `IsConfirmedSuccessfulDonation(p) == true`. Nếu không có phiếu nào -> trả về `IsEligible = true` (được phép đăng ký).
   - Bước 2: Tách danh sách phiếu thành công thành 2 nhóm dựa trên `GetActualDonationDate(p)` (tức `p.KetQuaHienMau?.NgayCapNhat`):
     - Nhóm có ngày thực tế (`datedSuccessful`)
     - Nhóm bị thiếu ngày thực tế (`undatedSuccessful`)
   - Bước 3: Nếu `datedSuccessful.Count > 0`, sắp xếp giảm dần theo `ActualDate` rồi đến `MaPhieu` để lấy lần hiến thành công gần nhất (`latestDated`). Tính:
     - `lastActualDate = latestDated.ActualDate.Date`
     - `nextEligibleDate = lastActualDate.AddDays(84)`
     - `daysElapsed = (refDay - lastActualDate).Days`
     - `remainingDays = (nextEligibleDate - refDay).Days`
   - Bước 4: Nếu `refDay < nextEligibleDate` (`daysElapsed < 84`), trả về `IsEligible = false` kèm thông báo chi tiết mã phiếu, ngày hiến gần nhất, ngày được đăng ký lại và số ngày còn phải chờ.
   - Bước 5: Nếu có bất kỳ phiếu nào trong `undatedSuccessful` (`HasMissingActualDonationDate == true`), trả về `IsEligible = false` để chặn an toàn (không lấy `NgayNop` thay thế).
   - Bước 6: Nếu đã đủ `>= 84` ngày và không có phiếu thiếu ngày -> trả về `IsEligible = true`.
4. **Điều kiện thành công là gì?** `IsEligible == true`.
5. **Những trường hợp nào bị từ chối?** `daysElapsed < 84` hoặc `HasMissingActualDonationDate == true`.
6. **Dữ liệu nào được thêm, sửa hoặc đọc?** Chỉ đọc từ `PhieuDangKyHienMau` và `KetQuaHienMau`.
7. **Hệ thống phản hồi thế nào khi lỗi?** Hiển thị cảnh báo rõ ràng trên `Create.cshtml`, `TheoDoi.cshtml`, `HoSoCaNhan.cshtml` và chặn tạo phiếu ở `POST /DangKy/Create`.
8. **Có tình huống ngoại lệ nào cần xử lý?** Người hiến máu có nhiều lần hiến thành công trong quá khứ: thuật toán luôn sắp xếp `.OrderByDescending(x => x.ActualDate).ThenByDescending(x => x.Phieu.MaPhieu)` để lấy đúng lần hiến gần nhất.
9. **Code nào thực hiện?** `Controllers/DangKyController.cs` (dòng 23–178).
10. **Có cần phối hợp với module khác không?** Phụ thuộc trực tiếp vào dữ liệu `KetQuaHienMau` của Module 5.

---

## NHÓM C — CÂU HỎI CHUYÊN SÂU & PHẢN BIỆN VỀ QUY TẮC ĐĂNG KÝ HIẾN MÁU

### 1. Bảng tổng hợp chính xác các quy tắc kiểm tra trong code hiện tại (`DangKyController.cs`)

| Quy tắc kiểm tra | Giá trị / Điều kiện trong Code | Vị trí chính xác trong Code | Ghi chú kỹ thuật |
|---|---|---|---|
| **Độ tuổi** | Từ **18 đến 60 tuổi** (tính chính xác theo ngày sinh nhật so với `DateTime.Today`) | `DangKyController.IsValidAge` (dòng 193–205) & dòng 447–459 | Nếu `ngaySinh.Date > refDay.AddYears(-age)` thì `age--` để tính tuổi thực tế tròn năm. |
| **Cân nặng** | 1. Nằm trong khoảng **30.0 kg – 200.0 kg**<br>2. Đạt ngưỡng tối thiểu của đợt: **`canNang >= dotHienMau.CanNangToiThieu`** (mặc định trong `DotHienMau` là 45kg) | `DangKyController.cs` (dòng 494–511), `Models/NguoiHienMau.cs` (dòng 48), `Models/DotHienMau.cs` (dòng 35) | Nếu trên form không nhập `canNang`, hệ thống lấy `nguoiHienMau?.CanNang` (`finalWeight = canNang ?? nguoiHienMau?.CanNang`). |
| **Số điện thoại** | Bắt buộc, sau khi loại bỏ khoảng trắng, `-`, `.` phải khớp Regex: **`^0[0-9]{9,10}$`** (10 hoặc 11 chữ số, bắt đầu bằng `0`) | `DangKyController.NormalizePhone` & `IsValidPhone` (dòng 180–191), dòng 430–437 | Bảo vệ ở cả Controller và `[RegularExpression]` trên `Models/NguoiHienMau.cs` (dòng 30). |
| **Email** | Bắt buộc khi đăng ký, tối đa **100 ký tự**, đúng chuẩn `new EmailAddressAttribute().IsValid(email)` | `DangKyController.cs` (dòng 470–482) | Trên `Models/NguoiHienMau.cs` (dòng 36) có `[EmailAddress]`. |
| **Họ tên, Giới tính, Địa chỉ** | - Họ tên: bắt buộc, `<= 100` ký tự<br>- Giới tính: bắt buộc thuộc `{"Nam", "Nữ", "Khác"}`<br>- Địa chỉ: bắt buộc, `<= 255` ký tự | `DangKyController.cs` (dòng 422–492) | Được `Trim()` trước khi kiểm tra độ dài. |
| **Khảo sát sức khỏe** | - `health_q1 == "none"` (Không mắc bệnh truyền nhiễm)<br>- `health_q2 == "good"` (Sức khỏe 3 tháng gần đây tốt) | `DangKyController.cs` (dòng 513–517) & `Views/DangKy/Create.cshtml` (dòng 365–382) | Nếu chọn bất kỳ phương án bệnh lý nào khác sẽ bị chặn ngay ở Server. |
| **Xác định 1 lần hiến máu thành công** | 1. Loại trừ trạng thái `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"`<br>2. Nếu có `KetQuaHienMau` thì `KetQua == "Hiến máu thành công"`<br>3. Nếu chưa nạp `KetQuaHienMau` thì `phieu.TrangThai == "Hiến máu thành công"` | `DangKyController.IsConfirmedSuccessfulDonation` (dòng 41–60) | Ưu tiên kiểm tra bảng `KetQuaHienMau` trước. |
| **Xác định ngày hiến máu thực tế** | Lấy duy nhất từ **`phieu.KetQuaHienMau.NgayCapNhat`** của phiếu hiến máu thành công | `DangKyController.GetActualDonationDate` (dòng 68–86) | Tuyệt đối không dùng `PhieuDangKyHienMau.NgayNop` hay `NgayXuLy`. |
| **Thời gian chờ giữa 2 lần hiến** | Tối thiểu **84 ngày** (`SoNgayChoToiThieuGiuaHaiLanHien = 84`, tương đương 12 tuần) | `DangKyController.cs` (dòng 23, 128–153) | Công thức: `nextEligibleDate = lastActualDate.Date.AddDays(84)`. Chặn khi `DateTime.Today < nextEligibleDate`. |

---

### 2. Giải đáp 12 Câu hỏi Phản biện Chuyên sâu (Nhóm C)

#### Câu C1: Vì sao phải kiểm tra lại dữ liệu ở phía Server (`DangKyController`, `NguoiHienMauController`) khi trên View đã có validation bằng HTML5 và JavaScript?
- **Mức độ:** Phản biện
- **Câu trả lời mẫu:**
  Thưa cô, kiểm tra ở View (HTML5 `required`, `min`, `max` và JavaScript) chỉ chạy trên trình duyệt của người dùng nhằm mục đích phản hồi nhanh và cải thiện trải nghiệm người dùng (UX). Tuy nhiên, người dùng hoặc kẻ tấn công hoàn toàn có thể tắt JavaScript, sửa mã HTML bằng F12 (DevTools), hoặc dùng Postman/cURL gửi trực tiếp một HTTP POST request chứa dữ liệu sai lệch lên địa chỉ `/DangKy/Create`. Vì vậy, kiểm tra ở phía Server trong Controller là **chốt chặn bắt buộc cuối cùng** để bảo vệ tính toàn vẹn của cơ sở dữ liệu và quy tắc nghiệp vụ.
- **Bằng chứng trong dự án:**
  - Client-side validation: `Views/DangKy/Create.cshtml` (dòng 433–489).
  - Server-side validation: `Controllers/DangKyController.cs` (dòng 422–523).
- **Câu hỏi phụ:** *Em hãy thử nêu một ví dụ cụ thể trong form `Create.cshtml` nếu không có Server-side validation thì người dùng lách luật bằng F12 như thế nào?*
- **Trả lời câu hỏi phụ:** Dạ ví dụ ở câu hỏi sàng lọc sức khỏe `health_q1` (tiền sử viêm gan B/HIV), nếu chỉ chặn bằng JS ở trình duyệt, người dùng mở F12 xóa sự kiện `submit` listener rồi gửi `health_q1 = "hcv"` lên server. Nhờ ở `DangKyController.cs` dòng 513 có câu lệnh `if (health_q1 != "none" || health_q2 != "good")`, server sẽ lập tức từ chối và báo lỗi.

---

#### Câu C2: Vì sao không chỉ kiểm tra điều kiện đăng ký bằng JavaScript? Có điều kiện nào JavaScript hoàn toàn không thể tự kiểm tra được không?
- **Mức độ:** Phản biện
- **Câu trả lời mẫu:**
  Thưa cô, ngoài lý do bảo mật (JavaScript có thể bị vô hiệu hóa ở trình duyệt), có những điều kiện nghiệp vụ mà **JavaScript phía Client hoàn toàn không có dữ liệu để tự kiểm tra**, cụ thể là:
  1. Kiểm tra lịch sử hiến máu và quy tắc 84 ngày của một số điện thoại khi khách vãng lai nhập vào form (phải truy vấn bảng `NguoiHienMau`, `PhieuDangKyHienMau`, `KetQuaHienMau` trong SQL Server).
  2. Kiểm tra số lượng chỉ tiêu còn lại theo thời gian thực (`soLuongDaDangKy >= dotHienMau.SoLuongToiDa`) tại đúng thời điểm bấm nút Submit.
  3. Kiểm tra trùng phiếu đăng ký đồng thời trong Transaction CSDL.
- **Bằng chứng trong dự án:** `DangKyController.cs` (dòng 350–355, 532–600).

---

#### Câu C3: Vì sao hệ thống tuyệt đối không lấy ngày nộp phiếu (`PhieuDangKyHienMau.NgayNop`) làm ngày hiến máu để tính 84 ngày?
- **Mức độ:** Phản biện (Rất quan trọng)
- **Câu trả lời mẫu:**
  Thưa cô, ngày nộp phiếu (`NgayNop`) chỉ là thời điểm người dùng gửi đơn đăng ký trực tuyến trên web, thường diễn ra trước ngày tổ chức chiến dịch từ vài ngày đến vài tuần. Hơn nữa, một người nộp phiếu xong có thể bận việc nên hủy phiếu, hoặc đến ngày khám sàng lọc bị huyết áp thấp nên không được hiến máu.
  Chu kỳ nghỉ 84 ngày là thời gian để cơ thể tái tạo lượng hồng cầu đã hiến, nên về mặt y khoa và nghiệp vụ, nó **chỉ được tính từ ngày người đó thực sự hiến máu thành công**. Do đó, trong hàm `GetActualDonationDate` (`DangKyController.cs` dòng 68–86), hệ thống chỉ lấy ngày từ `phieu.KetQuaHienMau.NgayCapNhat` của những phiếu có kết quả `"Hiến máu thành công"`.
- **Bằng chứng trong dự án:** `Controllers/DangKyController.cs` (dòng 68–86: comment và code của `GetActualDonationDate`).
- **Ví dụ thực tế:** Người hiến nộp phiếu ngày `01/10/2026` cho đợt hiến máu tổ chức ngày `15/10/2026`. Nếu tính 84 ngày từ `01/10` thì họ được đăng ký sớm mất 14 ngày so với thực tế lấy máu ngày `15/10`.

---

#### Câu C4: Nếu người dùng có phiếu cũ trong hệ thống nhưng chưa hiến máu (hoặc bị từ chối, đã hủy, không đủ điều kiện) thì hệ thống xử lý thế nào?
- **Mức độ:** Trung bình
- **Câu trả lời mẫu:**
  Thưa cô, hệ thống chia thành 2 trường hợp rõ ràng:
  1. **Nếu phiếu cũ đã kết thúc mà không hiến máu** (trạng thái `"Đã hủy"`, `"Từ chối"`, hoặc `"Không đủ điều kiện hiến"`): Hàm `IsConfirmedSuccessfulDonation` trả về `false` nên người đó **không bị tính thời gian chờ 84 ngày**, đồng thời câu truy vấn `existingPhieu` cũng loại trừ 3 trạng thái này nên người dùng **được phép đăng ký lại ngay** (kể cả đăng ký lại vào chính đợt hiến máu đó nếu đợt vẫn còn mở).
  2. **Nếu phiếu cũ vẫn đang trong quá trình xử lý chưa có kết quả** (`"Chờ xác nhận"`, `"Đã xác nhận"`, `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`):
     - Nếu người dùng đăng ký vào **cùng đợt hiến máu đó (`p.MaDot == maDot`)**: Hệ thống chặn lại với thông báo đã có phiếu đang hoạt động trong đợt này.
     - Phiếu này chưa có kết quả thành công nên chưa kích hoạt bộ đếm 84 ngày.
- **Bằng chứng trong dự án:** `DangKyController.cs` (dòng 41–60, 293–307, 553–578).
- **Câu hỏi phụ:** *Nếu một người đang có phiếu `"Chờ xác nhận"` ở Đợt 1, họ chuyển sang Đợt 2 (cũng đang mở) để đăng ký thì code hiện tại có chặn không?*
- **Trả lời câu hỏi phụ:** Dạ em xin trả lời trung thực theo code tại dòng 554 của `DangKyController.cs`: hiện tại điều kiện kiểm tra `existingPhieu` có lọc `p.MaDot == maDot` (chỉ kiểm tra trong cùng một đợt). Vì vậy, nếu phiếu ở Đợt 1 mới ở trạng thái `"Chờ xác nhận"` (chưa thành công), người đó vẫn có thể gửi thêm 1 phiếu ở Đợt 2. Đây là một điểm em đã ghi nhận vào mục *Cải tiến*: nếu muốn chặt chẽ hơn, chỉ cần bỏ điều kiện `p.MaDot == maDot` để mỗi thời điểm một người chỉ có tối đa 1 phiếu đang chờ xử lý trên toàn hệ thống.

---

#### Câu C5: Nếu trong CSDL có một phiếu mang trạng thái `"Hiến máu thành công"` nhưng lại chưa có bản ghi `KetQuaHienMau` hoặc `NgayCapNhat == null` thì hệ thống làm gì?
- **Mức độ:** Khó / Phản biện
- **Câu trả lời mẫu:**
  Thưa cô, đây là trường hợp dữ liệu lịch sử không đầy đủ (ví dụ do nhập liệu thủ công trực tiếp vào bảng `PhieuDangKyHienMau` mà quên tạo bản ghi bên bảng `KetQuaHienMau`).
  Trong `EvaluateRecentDonationEligibility` (`DangKyController.cs` dòng 109–113 và 156–169), hệ thống gom các phiếu này vào danh sách `undatedSuccessful`. Khi `undatedSuccessful.Count > 0`, hệ thống thực hiện **cơ chế Fail-Safe (chặn an toàn)**:
  - Đặt `HasMissingActualDonationDate = true` và `IsEligible = false`.
  - **Không tự ý lấy `NgayNop` hay `NgayXuLy` để cộng 84 ngày** (vì làm vậy có thể cho phép hiến máu sớm gây nguy hiểm sức khỏe).
  - Hiển thị thông báo yêu cầu liên hệ nhân viên y tế để cập nhật đầy đủ ngày kết quả hiến máu trước khi đăng ký mới.
- **Bằng chứng trong dự án:** `DangKyController.cs` (dòng 109–113, 156–169).

---

#### Câu C6: Kiểm tra điều kiện ở `[HttpGet] Create` và `[HttpPost] Create` có giống nhau không? Có thể vượt qua kiểm tra bằng cách gửi request POST trực tiếp không?
- **Mức độ:** Phản biện
- **Câu trả lời mẫu:**
  Thưa cô:
  - Về quy tắc kiểm tra chiến dịch (`DotHienMau`), quy tắc 84 ngày (`EvaluateRecentDonationEligibility`) và kiểm tra trùng phiếu trong đợt (`existingPhieu`), **cả GET và POST đều kiểm tra giống hệt nhau**. Thậm chí `POST` còn kiểm tra **chặt hơn** `GET` vì:
    1. `POST` kiểm tra thêm toàn bộ các trường nhập trên form (Họ tên, SĐT, Ngày sinh 18–60, Email, Cân nặng `>= CanNangToiThieu`, `health_q1`, `health_q2`).
    2. `POST` kiểm tra được lịch sử 84 ngày cho cả khách vãng lai thông qua số điện thoại họ vừa nhập (trong khi ở `GET`, khách vãng lai chưa nhập SĐT nên chưa tra cứu được).
    3. `POST` thực hiện kiểm tra lại lần nữa bên trong Transaction `IsolationLevel.Serializable`.
  - Do đó, người dùng **không thể** vượt qua kiểm tra bằng cách bỏ qua giao diện `GET` để gửi thẳng request `POST /DangKy/Create`.
- **Bằng chứng trong dự án:** `DangKyController.cs` (GET: dòng 207–310; POST: dòng 312–681).

---

#### Câu C7: Nếu người dùng sửa trạng thái phiếu hoặc gửi request giả mạo (Over-posting / Mass Assignment / IDOR) thì sao?
- **Mức độ:** Phản biện
- **Câu trả lời mẫu:**
  Thưa cô, hệ thống đã phòng chống cả 3 kịch bản giả mạo này:
  1. **Khi tạo phiếu (`POST /DangKy/Create`):** Action không nhận đối tượng `PhieuDangKyHienMau` từ form mà chỉ nhận các tham số rời (`int maDot, string hoTen, ...`) và tự khởi tạo `TrangThai = "Chờ xác nhận"` cứng ở dòng 606. Dù người dùng có chèn thêm field `TrangThai=Hiến máu thành công` vào HTTP POST thì server cũng hoàn toàn bỏ qua.
  2. **Khi cập nhật hồ sơ (`POST /NguoiHienMau/HoSoCaNhan`):** Mặc dù nhận `NguoiHienMau model`, nhưng ở dòng 234–245, controller chỉ gán tường minh 10 thuộc tính thông tin cá nhân/sức khỏe vào đối tượng `nguoiHienMau` lấy từ DB theo `MaTaiKhoan` của Cookie, tuyệt đối không gán `MaNguoiHien`, `MaTaiKhoan` hay `TrangThai` từ `model`.
  3. **Khi hủy phiếu (`POST /DangKy/Huy`):** Dòng 745 kiểm tra `p.MaPhieu == id && p.MaNguoiHien == nguoiHienMau.MaNguoiHien` (lấy từ Cookie đăng nhập), nên người dùng A không thể gửi `id` phiếu của người dùng B để hủy hộ (chống IDOR).
- **Bằng chứng trong dự án:** `DangKyController.cs` (dòng 314–332, 606, 745), `NguoiHienMauController.cs` (dòng 234–245).

---

#### Câu C8: Nếu hai yêu cầu đăng ký được gửi gần như đồng thời (Race Condition / Double Submit) thì có thể tạo ra hai phiếu không? Hệ thống đã chống như thế nào?
- **Mức độ:** Khó / Phản biện
- **Câu trả lời mẫu:**
  Thưa cô, trong code hiện tại của `DangKyController.cs`, nhóm đã triển khai cơ chế chống Race Condition 2 lớp cho cùng một người hiến máu (`MaNguoiHien`):
  - **Lớp 1 (Đồng bộ hóa luồng trong tiến trình ứng dụng):** Sử dụng `private static readonly ConcurrentDictionary<int, SemaphoreSlim> _submissionLocks` (dòng 26). Trước khi tạo phiếu, luồng xử lý phải lấy khóa `SemaphoreSlim(1, 1)` theo `nguoiHienMau.MaNguoiHien` (`await userLock.WaitAsync()`, dòng 533). Nếu 2 request của cùng 1 người gửi đến đồng thời, request thứ 2 buộc phải xếp hàng đợi request thứ 1 xử lý xong.
  - **Lớp 2 (Transaction CSDL mức cô lập cao nhất):** Mở `await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable)` (dòng 537), truy vấn lại lịch sử phiếu và kiểm tra lại cả quy tắc 84 ngày lẫn `existingPhieu` ngay bên trong transaction rồi mới `Add` và `CommitAsync()`.
  - **Lưu ý trung thực về giới hạn:** Cơ chế này chống triệt để việc 1 người hiến máu tạo 2 phiếu đồng thời trên 1 web server instance. Tuy nhiên, đối với tổng chỉ tiêu của đợt (`soLuongDaDangKy >= dotHienMau.SoLuongToiDa`), do phép đếm nằm ngoài khóa theo `MaDot` (dòng 350), nếu 2 người hiến máu khác nhau cùng tranh suất cuối cùng ở cùng một mili-giây thì lý thuyết vẫn có thể vượt 1 chỉ tiêu nếu không khóa theo `MaDot`.
- **Bằng chứng trong dự án:** `DangKyController.cs` (dòng 26, 350–355, 532–616).

---

#### Câu C9: Tại sao phải xem xét Transaction và tính nhất quán dữ liệu (Data Consistency) trong `DangKyController.Create`?
- **Mức độ:** Khó
- **Câu trả lời mẫu:**
  Thưa cô, trong `POST /DangKy/Create`, hệ thống thực hiện chuỗi thao tác đọc-kiểm tra-ghi (Read-Check-Write): đọc lịch sử phiếu -> kiểm tra 84 ngày và phiếu trùng -> ghi phiếu mới. Nếu không đặt chuỗi thao tác này trong một Transaction có mức cô lập `IsolationLevel.Serializable`, dữ liệu có thể bị thay đổi bởi một giao dịch khác xen vào giữa lúc vừa đọc xong nhưng chưa kịp ghi (hiện tượng Phantom Read / Non-repeatable Read), làm phá vỡ tính nhất quán dữ liệu (ACID).
- **Bằng chứng trong dự án:** `DangKyController.cs` (dòng 536–616).

---

#### Câu C10: Quy tắc 84 ngày có tương đương chính xác với "3 tháng theo lịch" không? Vì sao trong code lại dùng 84 ngày?
- **Mức độ:** Phản biện
- **Câu trả lời mẫu:**
  Thưa cô, **84 ngày không tương đương chính xác tuyệt đối với 3 tháng theo lịch dương** (`AddMonths(3)`):
  - **84 ngày** tương đương chính xác với **12 tuần** ($12 \times 7 = 84$ ngày). Đây là quy định chuẩn trong Thông tư 26/2013/TT-BYT của Bộ Y tế Việt Nam về hướng dẫn hoạt động truyền máu (thời gian tối thiểu giữa 2 lần hiến máu toàn phần là **12 tuần**, tức 84 ngày).
  - Trong khi đó, **3 tháng theo lịch dương** (`AddMonths(3)`) có độ dài dao động từ **89 đến 92 ngày** tùy vào tháng có 28, 29, 30 hay 31 ngày (ví dụ từ `01/05` đến `01/08` là $31 + 30 + 31 = 92$ ngày).
  - Trong dự án này, nhóm lựa chọn hằng số `SoNgayChoToiThieuGiuaHaiLanHien = 84` (`lastActualDate.AddDays(84)`) để bám sát quy định 12 tuần của Bộ Y tế, vừa đảm bảo mọi người hiến máu đều được tính công bằng đúng 84 ngày bất kể họ hiến vào tháng nào trong năm.
- **Bằng chứng trong dự án:** `DangKyController.cs` (dòng 23: `public const int SoNgayChoToiThieuGiuaHaiLanHien = 84;` và dòng 129: `lastActualDate.AddDays(SoNgayChoToiThieuGiuaHaiLanHien)`).

---

#### Câu C11: Nếu yêu cầu của giảng viên là tính theo đúng "3 tháng theo lịch" (hoặc 90 ngày) thì cần thống nhất nghiệp vụ và sửa code ở đâu?
- **Mức độ:** Phản biện
- **Câu trả lời mẫu:**
  Thưa cô, nhờ nhóm đã tập trung toàn bộ logic tính thời gian chờ vào một hàm dùng chung duy nhất là `DangKyController.EvaluateRecentDonationEligibility` (và `NguoiHienMauController` cũng gọi lại hàm này), nên nếu cô yêu cầu đổi quy tắc thì việc sửa code cực kỳ gọn gàng tại một chỗ duy nhất trong `Controllers/DangKyController.cs`:
  - **Trường hợp 1 (Nếu đổi sang 90 ngày cố định):** Chỉ cần sửa hằng số ở dòng 23 từ `public const int SoNgayChoToiThieuGiuaHaiLanHien = 84;` thành `= 90;`.
  - **Trường hợp 2 (Nếu đổi sang 3 tháng theo lịch dương):** Tại dòng 129 của `DangKyController.cs`, sửa câu lệnh:
    ```csharp
    // Hiện tại:
    var nextEligibleDate = lastActualDate.AddDays(SoNgayChoToiThieuGiuaHaiLanHien);
    // Sửa thành 3 tháng theo lịch:
    var nextEligibleDate = lastActualDate.AddMonths(3);
    ```
    Khi đó `remainingDays = (nextEligibleDate - refDay).Days` sẽ tự động tính số ngày còn lại cho đến đúng ngày cùng kỳ của 3 tháng sau!
- **Bằng chứng trong dự án:** `DangKyController.cs` (dòng 23 và dòng 128–132).

---

#### Câu C12: Dựa vào đâu để xác nhận ngày trong dữ liệu là ngày hiến máu thực tế? Nếu ngày nhân viên cập nhật kết quả (`KetQuaHienMau.NgayCapNhat`) không trùng với ngày lấy máu thực tế ngoài đời thì hệ thống có vấn đề gì?
- **Mức độ:** Phản biện (Câu hỏi phân loại sinh viên Xuất sắc)
- **Câu trả lời mẫu:**
  Thưa cô, đây là một câu hỏi rất sâu sắc về thiết kế cơ sở dữ liệu:
  1. **Trong cấu trúc bảng `KetQuaHienMau` hiện tại** (`Models/KetQuaHienMau.cs`), chỉ có 1 cột kiểu `DateTime?` duy nhất là `NgayCapNhat` (mặc định gán `DateTime.Now` khi nhân viên y tế lưu kết quả tại `AdminController.TaoKetQua` dòng 1072 hoặc `SuaKetQua` dòng 1131). Vì vậy, `DangKyController.GetActualDonationDate` đang lấy `KetQuaHienMau.NgayCapNhat` làm ngày hiến máu thực tế (tốt hơn rất nhiều so với việc lấy `NgayNop` của phiếu đăng ký).
  2. **Tuy nhiên, hạn chế có thật của thiết kế này là:**
     - Nếu người tình nguyện hiến máu vào Chủ Nhật (`10/10`), nhưng đến thứ Tư (`13/10`) nhân viên y tế mới nhập kết quả lên phần mềm, thì `NgayCapNhat` sẽ lưu là `13/10`, làm người hiến máu bị lùi ngày đăng ký tiếp theo thêm 3 ngày.
     - Nghiêm trọng hơn, trong `AdminController.SuaKetQua` (dòng 1131), mỗi khi nhân viên sửa lại ghi chú kết quả, code lại gán đè `kq.NgayCapNhat = DateTime.Now`, làm mốc 84 ngày bị dịch chuyển theo ngày sửa!
  3. **Hướng khắc phục triệt để em đề xuất:** Cần tách rõ trong bảng `KetQuaHienMau` thành 2 cột riêng biệt:
     - `NgayHienMau` (`DateTime`, cho phép nhân viên y tế chọn đúng ngày lấy máu thực tế trên form, mặc định gợi ý theo ngày diễn ra đợt hiến máu).
     - `NgayCapNhat` (`DateTime`, lưu vết thời điểm hệ thống tạo/sửa bản ghi kết quả).
- **Bằng chứng trong dự án:**
  - `Models/KetQuaHienMau.cs` (dòng 24: `public DateTime? NgayCapNhat { get; set; } = DateTime.Now;`)
  - `Controllers/DangKyController.cs` (dòng 82–85: `return phieu.KetQuaHienMau.NgayCapNhat.Value;`)
  - `Controllers/AdminController.cs` (dòng 1072 trong `TaoKetQua` và dòng 1131 trong `SuaKetQua`).

---

## PHẦN III — MA TRẬN PHÂN TÍCH ĐỘ ƯU TIÊN ÔN TẬP (MỨC 1 ĐẾN MỨC 4)

Để giúp sinh viên phân bổ thời gian ôn tập hiệu quả nhất trước buổi bảo vệ, toàn bộ kiến thức và câu hỏi của Module 3 được chia thành **4 mức độ ưu tiên**:

### 🔴 MỨC 1 — BẮT BUỘC PHẢI THUỘC LÒNG (Khả năng bị hỏi > 90%)
Đây là những câu hỏi bất kỳ giảng viên nào cũng sẽ hỏi đầu tiên khi mở đồ án và xem demo Module 3:
1. **Phạm vi và chức năng của Module 3:** Gồm những màn hình nào, Controller nào (`DangKyController`, `NguoiHienMauController`), bảng CSDL nào (`NguoiHienMau`, `PhieuDangKyHienMau`).
2. **Luồng tạo phiếu đăng ký (`GET` và `POST /DangKy/Create`):** Từ lúc chọn đợt hiến máu -> điền thông tin -> các bước kiểm tra điều kiện -> lưu `NguoiHienMau` -> lưu `PhieuDangKyHienMau` với trạng thái `"Chờ xác nhận"`.
3. **Quy tắc kiểm tra 84 ngày (`EvaluateRecentDonationEligibility`):**
   - Tại sao là 84 ngày (12 tuần)?
   - Lấy ngày nào để cộng 84 ngày? (Bắt buộc trả lời: Lấy `KetQuaHienMau.NgayCapNhat` của phiếu có kết quả `"Hiến máu thành công"`, **không** lấy `NgayNop`).
   - Chỉ ra ngay trên code hàm `IsConfirmedSuccessfulDonation`, `GetActualDonationDate` và `EvaluateRecentDonationEligibility` ở đầu file `DangKyController.cs` (dòng 23–178).
4. **Quy tắc hủy phiếu (`POST /DangKy/Huy`):** Khi nào người hiến được hủy phiếu? (Chỉ khi trạng thái là `"Chờ xác nhận"` hoặc `"Đã xác nhận"`, chưa có lịch khám sàng lọc đang hoạt động và chưa có kết quả hiến máu).
5. **Các điều kiện validation khi đăng ký và cập nhật hồ sơ:** Tuổi `18–60`, SĐT `^0[0-9]{9,10}$`, cân nặng `30–200` kg và `>= dotHienMau.CanNangToiThieu`, 2 câu hỏi sức khỏe `health_q1 == "none"`, `health_q2 == "good"`.

---

### 🟠 MỨC 2 — RẤT NÊN NẮM CHẮC (Khả năng bị hỏi 60% – 80%)
Dành cho các câu hỏi kiểm tra kỹ năng lập trình ASP.NET Core MVC và Cơ sở dữ liệu:
1. **Cấu trúc CSDL và khóa ngoại của Module 3 (`ApplicationDbContext.cs`):**
   - Quan hệ `TaiKhoan (1) — (0..1) NguoiHienMau` (`DeleteBehavior.SetNull`, Unique Filtered Index `[MaTaiKhoan] IS NOT NULL`).
   - Quan hệ `NguoiHienMau (1) — (n) PhieuDangKyHienMau` (`DeleteBehavior.Restrict`).
   - Quan hệ `DotHienMau (1) — (n) PhieuDangKyHienMau` (`DeleteBehavior.Restrict`).
   - Quan hệ `PhieuDangKyHienMau (1) — (n) LichKhamSangLoc` và `(1) — (0..1) KetQuaHienMau` (`DeleteBehavior.Cascade`).
2. **Kỹ thuật ASP.NET Core MVC trong dự án:**
   - Cách lấy `MaTaiKhoan` từ Cookie Claims (`User.FindFirst("MaTaiKhoan")?.Value`).
   - Sự khác nhau giữa `.Include()` / `.ThenInclude()` (Eager Loading) và truy vấn thường.
   - Sự khác nhau giữa `ViewBag` (dùng trong cùng 1 request render View) và `TempData` (dùng truyền thông báo qua `RedirectToAction`).
   - Cơ chế `[ValidateAntiForgeryToken]` chống CSRF.
3. **8 trạng thái của `PhieuDangKyHienMau.TrangThai`** và luồng chuyển trạng thái giữa Module 3, Module 4, Module 5.

---

### 🟡 MỨC 3 — CÂU HỎI PHÂN LOẠI KHÁ / GIỎI (Khả năng bị hỏi 30% – 50%)
Dành cho giảng viên muốn thử thách tư duy hệ thống và xử lý tình huống biên (Edge Cases):
1. **Chống gửi 2 request đăng ký đồng thời (Concurrency Control):** Giải thích sự kết hợp giữa `_submissionLocks` (`ConcurrentDictionary<int, SemaphoreSlim>`) và `BeginTransactionAsync(IsolationLevel.Serializable)` tại `DangKyController.cs` (dòng 532–616).
2. **Xử lý dữ liệu lịch sử bị khuyết ngày (`HasMissingActualDonationDate`):** Tại sao khi phiếu có trạng thái `"Hiến máu thành công"` nhưng `KetQuaHienMau.NgayCapNhat == null` thì hệ thống lại chặn đăng ký thay vì lấy `NgayNop`?
3. **Liên kết hồ sơ khách vãng lai với tài khoản mới:** Giải thích đoạn code tìm `NguoiHienMau` theo `SoDienThoai` có `MaTaiKhoan == null` và gán `MaTaiKhoan` vào khi người dùng đăng nhập (dòng 367–376 `DangKyController.cs`).
4. **Cách lưu `volume` (dung tích máu) và `timeSlot` (khung giờ hẹn):** Giải thích cách nối chuỗi vào `LyDoHienMau` và `GhiChu` (dòng 580–596 `DangKyController.cs`).

---

### 🟢 MỨC 4 — CÂU HỎI MỞ RỘNG & PHẢN BIỆN SÂU (Dành để lấy điểm 9 – 10)
Khi giảng viên xoáy sâu vào hạn chế của hệ thống và yêu cầu đề xuất cải tiến:
1. **Phân tích hạn chế của cột `KetQuaHienMau.NgayCapNhat`:** Sự lệch ngày nếu nhân viên nhập kết quả muộn hoặc sửa kết quả (`AdminController.SuaKetQua` ghi đè `NgayCapNhat = DateTime.Now`), và đề xuất tách cột `NgayHienMauThucTe`.
2. **Sự lệch công thức tính 84 ngày giữa trang `AdminController.NguoiHienMau` và `DangKyController`:** Chỉ ra rằng `DangKyController` và `NguoiHienMauController` đã dùng `KetQuaHienMau.NgayCapNhat`, còn `AdminController.NguoiHienMau` vẫn đang dùng `NgayXuLy ?? NgayNop`.
3. **So sánh `84 ngày` (12 tuần theo chuẩn Bộ Y tế) và `3 tháng theo lịch` (`AddMonths(3)`):** Chỉ ra ngay dòng code 129 trong `DangKyController.cs` nếu cô yêu cầu đổi sang `AddMonths(3)`.
4. **Phân biệt rạch ròi tính năng thật (lưu DB) và tính năng mô phỏng giao diện (UI Mock)** trên `HoSoCaNhan.cshtml` và `Admin/NguoiHienMau.cshtml`.
