# 07. ĐÁNH GIÁ RỦI RO & KHẢ NĂNG BẢO VỆ TRƯỚC GIẢNG VIÊN (MỤC 20 & MỤC 21 ĐỀ BÀI)

Tài liệu này phân tích các rủi ro thực tế khi nhóm mang dự án `QuanLyHienMau_UNETI2_TI17A5HN` đi bảo vệ trước giảng viên, dựa trên **Mục 18 (Ghi chú tác giả)**, **Mục 20 (Hình thức đánh giá 100 điểm)** và **Mục 21 (Các lỗi bị trừ điểm)** của đề bài chính thức.

---

## 1. Đối chiếu trực tiếp với Danh sách 10 lỗi bị trừ điểm của Giảng viên (Mục 21 Đề bài)

| STT | Lỗi bị trừ điểm nêu trong Mục 21 Đề bài | Tình trạng hiện tại của Project | Mức độ rủi ro bị trừ điểm | Cách xử lý triệt để trước buổi bảo vệ |
|---:|---|---|---|---|
| 1 | **Project không build được hoặc lỗi cấu hình cơ bản** | Project build thành công (`0 Error(s)`), tự động chạy `MigrateAsync()` khi khởi động. | **Thấp (An toàn)** | Giữ nguyên cấu hình LocalDB và `MigrateAsync()`. |
| 2 | **Thiết kế thiếu bảng bắt buộc hoặc sai quan hệ khóa ngoại** | Có đủ 7 bảng bắt buộc + cấu hình khóa ngoại `Restrict` / `SetNull` và Unique Index chuẩn xác. | **Thấp (An toàn)** | Giữ nguyên cấu trúc 7 bảng chính. |
| 3 | **Form không có kiểm tra dữ liệu hợp lệ (Validation)** | Các form Đăng ký TK, Đơn vị, Đợt hiến máu, Hồ sơ, Đăng ký hiến máu, Lịch khám, Kết quả đều có Data Annotations + Server-side validation. | **Thấp (An toàn)** | Duy nhất form `AdminController.TaoLichKham` thiếu kiểm tra trùng lịch & ngày quá khứ → **Phải sửa**. |
| 4 | **Cho phép dữ liệu sai nghiệp vụ đi vào cơ sở dữ liệu** | - Module 3 đã chặn rất tốt (18–60 tuổi, cân nặng, 84 ngày, trùng đợt).<br>- **NHƯNG `AdminController.TaoLichKham` đang cho phép lập lịch trong quá khứ và trùng lịch bác sĩ/người hiến!** | **RẤT CAO (Nếu không sửa `AdminController.TaoLichKham`)** | Chuyển `AdminController` sang gọi `KhamSangLocService` hoặc trỏ menu sang `LichKhamSangLocController`. |
| 5 | **Xóa dữ liệu cha làm lỗi hoặc mất dữ liệu con** | Đã chặn bằng cả code (`HasDotHienMauAsync`, `HasPhieuDangKyAsync`) và DB (`DeleteBehavior.Restrict`). | **Thấp (An toàn)** | Đạt 100%. |
| 6 | **Không có phân quyền hoặc người hiến máu truy cập được trang quản trị** | Đã chặn người hiến máu vào trang quản trị. Tuy nhiên `Medical Staff` vẫn gọi được `AdminController.KhoaMoKhoaNguoiHien`. | **Trung bình** | Thêm `[Authorize(Roles = "Admin")]` vào `KhoaMoKhoaNguoiHien`. |
| 7 | **Controller ôm toàn bộ xử lý dữ liệu phức tạp, không tách hàm hoặc lớp hợp lý** | `AdminController.cs` dài **1.648 dòng**, ôm cả 5 Module và gọi trực tiếp `_context` ở nhiều nơi. | **CAO** | Giải thích rõ hoặc ủy quyền các hàm trong `AdminController` sang Repository/Service tương ứng đã có sẵn. |
| 8 | **Dữ liệu mẫu quá ít, không kiểm thử được tìm kiếm, lọc, sắp xếp và phân trang** | `DbInitializer.cs` hiện chỉ có **5 đợt hiến máu** (chỉ có 1 trang!), **0 lịch khám**, **0 kết quả hiến máu**. | **RẤT CAO (Chắc chắn bị phát hiện ngay khi mở app)** | **Bắt buộc nâng cấp `DbInitializer.cs`** đủ số lượng theo Mục 16 (tối thiểu 5 đơn vị, 15 đợt, 30 người hiến, 45 phiếu, 18 lịch khám, 12 kết quả). |
| 9 | **Không ghi họ tên và mã sinh viên trên đầu các file phụ trách** | Các file của Module 3, Module 4, Module 5 chưa ghi Họ tên và Mã sinh viên. | **RẤT CAO (Trừ điểm cá nhân trực tiếp)** | **Bắt buộc bổ sung comment Họ tên + MSSV** ở đầu tất cả các file của cả 5 thành viên. |
| 10 | **Thành viên không giải thích được đoạn code nằm trong module của mình** | Do `AdminController` viết trùng lặp lại code của Module 1, 2, 5 nên dễ xảy ra tình trạng thành viên Module 5 học thuộc `LichKhamSangLocController` nhưng giảng viên lại mở `AdminController.LichKhamSangLoc` để hỏi! | **CAO** | Thống nhất rõ file nào là file chính của từng thành viên và điều hướng menu đúng vào Controller của thành viên đó. |

---

## 2. Bộ câu hỏi Giảng viên rất dễ hỏi khi Vấn đáp (Kèm điểm yếu hiện tại trong Code)

### Câu hỏi 1 (Dành cho Module 2): *"Hãy demo chức năng phân trang Đợt hiến máu kết hợp lọc theo Nhóm máu O và sắp xếp theo Hạn đăng ký. Code phân trang nằm ở đâu?"*
- **Điểm mạnh:** Code nằm ở [`DotHienMauRepository.GetPagedAsync`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/DotHienMauRepository.cs#L222-L231) dùng `.Skip((page - 1) * pageSize).Take(pageSize)` rất chuẩn.
- **Rủi ro khi demo hiện tại:** Vì `DbInitializer` chỉ seed 5 đợt hiến máu mà `pageSize = 6`, nên trên giao diện chỉ có đúng **1 trang duy nhất**, không bấm sang trang 2 được! → **Cần seed >= 15 đợt hiến máu**.

### Câu hỏi 2 (Dành cho Module 3): *"Làm sao hệ thống biết một người vừa hiến máu thành công cách đây 50 ngày thì không được đăng ký đợt mới? Nếu người đó tự sửa ngày hiến máu gần nhất trong Hồ sơ cá nhân thành năm ngoái thì có lách luật được không?"*
- **Điểm rất mạnh trong code hiện tại:** Trong [`DangKyController.KiemTraDieuKienNghiepVu`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L371-L398), hệ thống lấy **ngày lớn nhất (mới nhất)** giữa `nguoiHienMau.NgayHienMauGanNhat` tự khai báo VÀ ngày thực tế của phiếu có `TrangThai == "Hiến máu thành công"` hoặc `KetQuaHienMau.KetQua == "Hiến máu thành công"` trong Database! Do đó người dùng dù có sửa hồ sơ cá nhân cũng không thể lách được quy tắc 84 ngày nếu đã có kết quả hiến máu thành công trong hệ thống. Đây là câu trả lời ăn điểm tuyệt đối khi vấn đáp!

### Câu hỏi 3 (Dành cho Module 4): *"Khi nhân viên y tế duyệt phiếu từ 'Chờ xác nhận' sang 'Đã xác nhận', nếu cùng lúc đó có 2 nhân viên bấm duyệt hoặc người hiến bấm hủy phiếu thì hệ thống xử lý thế nào?"*
- **Điểm mạnh:** Cả `DangKyController.HuyDangKy` và `AdminController.CapNhatTrangThaiPhieu` đều bọc trong Transaction mức cô lập cao nhất `IsolationLevel.Serializable` (`BeginTransactionAsync(IsolationLevel.Serializable)`), đảm bảo khóa đọc-ghi tuần tự và kiểm tra lại trạng thái hiện tại trước khi `CommitAsync`.

### Câu hỏi 4 (Dành cho Module 5): *"Hãy thử lập lịch khám sàng lọc cho 2 phiếu khác nhau nhưng cùng 1 bác sĩ phụ trách vào cùng khung giờ 08:00 – 09:00 ngày mai xem hệ thống có chặn không?"*
- **Rủi ro chí mạng hiện tại:**
  - Nếu thành viên Module 5 vào đường dẫn `/LichKhamSangLoc/Create` (`LichKhamSangLocController`), hàm [`LichKhamSangLocRepository.CoLichTrungBacSiAsync`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/LichKhamSangLocRepository.cs#L45-L50) sẽ chặn ngay với thông báo: `"Bác sĩ sàng lọc đã có lịch trùng khoảng thời gian này."`
  - Nhưng nếu bấm từ menu `"Lịch khám sàng lọc"` trên Sidebar (`_AdminLayout.cshtml`), hệ thống lại mở `/Admin/LichKhamSangLoc` và post về `AdminController.TaoLichKham` — nơi **không hề kiểm tra trùng lịch bác sĩ** và cho lưu thành công!

### Câu hỏi 5 (Dành cho cả nhóm): *"Các mục 'Kho lưu trữ máu', 'Kiểm định & Tiêu hủy túi máu', 'Theo dõi nhiệt độ kho máu IoT' trên thanh menu bên trái lấy dữ liệu từ bảng nào trong Database?"*
- **Rủi ro chí mạng hiện tại:** Không có bảng nào cả! Toàn bộ 6 màn hình đó là HTML tĩnh. Giảng viên sẽ hỏi tiếp: *"Tại sao làm giao diện giả đưa vào bài nộp?"*. Do đó **bắt buộc phải ẩn 6 menu ngoài phạm vi này** khỏi `_AdminLayout.cshtml`.
