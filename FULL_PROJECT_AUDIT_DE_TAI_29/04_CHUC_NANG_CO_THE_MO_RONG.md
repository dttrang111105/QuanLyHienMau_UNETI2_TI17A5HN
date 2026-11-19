# 04. ĐÁNH GIÁ & ĐỀ XUẤT CÁC CHỨC NĂNG CÓ THỂ MỞ RỘNG (MỤC 19 ĐỀ BÀI)

Tài liệu này phân tích toàn bộ các tính năng nâng cao được giảng viên gợi ý tại **Mục 19 (Tính năng nâng cao không bắt buộc – Điểm cộng)** của đề bài chính thức cùng một số tính năng mở rộng sát với nghiệp vụ hiến máu nhân đạo, đánh giá theo 8 tiêu chí thực tế của codebase hiện tại.

---

## 1. Bảng tổng hợp đánh giá 10 chức năng mở rộng tiềm năng

| STT | Tên chức năng mở rộng | Nguồn | Mức độ phù hợp đề tài | Giá trị điểm cộng khi bảo vệ | Độ phức tạp | Thời gian ước tính | Rủi ro ảnh hưởng code cũ | Khuyến nghị |
|---:|---|---|---|---|---|---|---|---|
| 1 | **Biểu đồ trực quan trên trang Thống kê & Dashboard** (Chart.js) | Mục 19.3 Đề bài | Rất cao | **Rất cao** | Thấp | 1 – 2 giờ | Không ảnh hưởng (chỉ đọc dữ liệu từ `ThongKeViewModel` sẵn có) | **Nên làm** |
| 2 | **Xuất danh sách Phiếu đăng ký / Kết quả / Thống kê ra file CSV hoặc Excel** | Mục 19.2 Đề bài | Rất cao | **Rất cao** | Thấp | 1.5 – 2 giờ | Không ảnh hưởng (thêm Action `ExportCsv` độc lập) | **Nên làm** |
| 3 | **Gợi ý đợt hiến máu phù hợp với Nhóm máu & Cân nặng của người đang đăng nhập** | Mục 19.4 Đề bài | Rất cao | **Cao** | Thấp | 1 – 1.5 giờ | Rất thấp (chỉ bổ sung query gợi ý trên `DotHienMau/Index`) | **Nên làm** |
| 4 | **Đếm ngược số ngày đủ điều kiện hiến máu tiếp theo (Chu kỳ 84 ngày)** | Mục 19.5 Đề bài | Rất cao | **Rất cao** | Rất thấp | 30 – 45 phút | Không ảnh hưởng (`NguoiHienMau/HoSo.cshtml` và `DangKy/TheoDoi.cshtml` đã có sẵn một phần, chỉ cần hoàn thiện đồng bộ) | **Nên làm** |
| 5 | **Cảnh báo lịch khám sàng lọc sắp diễn ra (trong vòng 24h–48h tới) cho Người hiến máu & Nhân viên y tế** | Mục 19.6 Đề bài | Rất cao | **Cao** | Thấp | 1 giờ | Không ảnh hưởng | **Nên làm** |
| 6 | **In Giấy chứng nhận hiến máu tình nguyện (Bản in HTML/PDF chuẩn) cho phiếu `"Hiến máu thành công"`** | Nghiệp vụ thực tế sát đề tài | Cao | **Cao** | Thấp | 1 – 1.5 giờ | Không ảnh hưởng (thêm 1 View bản in từ `KetQuaHienMau/Details`) | **Có thể làm** |
| 7 | **Tự động đóng đợt hoặc cảnh báo nổi bật khi số ca hiến máu thành công đạt 100% chỉ tiêu** | Mục 8.4 & 19.7 Đề bài | Rất cao | **Cao** | Thấp | 1 giờ | Đã có sẵn 80% trong codebase (`DotQuotaAlert`), chỉ cần hiển thị rõ hơn | **Có thể làm** |
| 8 | **Gửi thông báo nội bộ trên giao diện (In-app Notification Dropdown) thay cho `alert()` cứng trên Topbar** | Mục 19.1 Đề bài | Khá | Trung bình | Trung bình | 2 – 3 giờ | Trung bình (phải sửa `_AdminLayout.cshtml` hoặc dùng ViewComponent) | **Có thể làm** |
| 9 | **Gửi Email thật qua SMTP khi xác nhận phiếu / lập lịch khám / công bố kết quả** | Mục 19.1 Đề bài | Trung bình | Trung bình | Cao | 3 – 4 giờ | **Cao** (Phụ thuộc mạng internet, cấu hình App Password Gmail dễ lỗi khi chạy trên máy giảng viên) | **Không nên làm** |
| 10 | **Xây dựng đầy đủ Backend cho Kho lưu trữ máu, Kiểm định xét nghiệm, Tiêu hủy túi máu, Cảm biến IoT kho lạnh** | Các màn hình Mock sẵn có trong `AdminController` | **Vượt phạm vi đề tài (Mục 3)** | Không có (Dễ bị hỏi vặn) | Rất cao | > 15 giờ | **Rất cao** (Phải tạo thêm 4–5 bảng Database ngoài 7 bảng chuẩn của đề bài) | **Không nên làm** |

---

## 2. Phân tích chi tiết 5 chức năng **"Nên làm"** (Đem lại điểm cộng cao nhất và an toàn nhất)

### 2.1. Biểu đồ thống kê trực quan trên Dashboard & Trang Thống kê (Mục 19.3)
- **Lý do nên làm:** Hiện tại [`ThongKeRepository.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/ThongKeRepository.cs) đã tính toán sẵn đầy đủ các tập dữ liệu: `TheoDonVi`, `TheoDot`, `TheoTrangThai`, `PhieuTheoThang`, `LichTheoThang`, nhưng [`Views/ThongKe/Index.cshtml`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/ThongKe/Index.cshtml) chỉ mới hiển thị dưới dạng bảng số liệu và thanh progress bar CSS.
- **Cách thực hiện an toàn:** Chỉ cần nhúng Chart.js và vẽ 2 biểu đồ trực tiếp từ `Model.TheoTrangThai` (Biểu đồ tròn/Doughnut) và `Model.PhieuTheoThang` (Biểu đồ cột/đường) ngay trong `Views/ThongKe/Index.cshtml`. Không cần sửa bất kỳ dòng code C# hay Database nào.

### 2.2. Xuất báo cáo Danh sách Phiếu đăng ký / Kết quả hiến máu ra file CSV/Excel (Mục 19.2)
- **Lý do nên làm:** Được nêu đích danh tại Mục 19.2 của đề bài. Rất dễ demo trực tiếp trước mặt giảng viên (bấm nút "Xuất Excel/CSV" → tải về file mở bằng Microsoft Excel có đầy đủ tiếng Việt UTF-8 with BOM).
- **Cách thực hiện an toàn:** Bổ sung 1 action `ExportCsv` trong `ThongKeController` hoặc `AdminController` trả về `File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray(), "text/csv", "BaoCaoHienMau.csv")` mà không cần cài thêm thư viện NuGet bên thứ ba (như EPPlus/ClosedXML) để tránh lỗi license hoặc lỗi restore package.

### 2.3. Gợi ý Đợt hiến máu phù hợp với Nhóm máu và Cân nặng của người đang đăng nhập (Mục 19.4)
- **Lý do nên làm:** Được nêu đích danh tại Mục 19.4. Hiện tại [`DotHienMauRepository.cs`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/DotHienMauRepository.cs) đã có sẵn hàm `ApplyFilterNhomMau` và `ApplyFilterCanNang`.
- **Cách thực hiện an toàn:** Khi người hiến máu đã đăng nhập truy cập `/DotHienMau/Index`, hệ thống đọc `NhomMau` và `CanNang` từ hồ sơ `NguoiHienMau` của họ và hiển thị một nút nhanh **"Lọc đợt phù hợp với hồ sơ của tôi (Nhóm máu ..., ... kg)"** hoặc gắn nhãn `"Phù hợp với bạn"` trên thẻ đợt hiến máu.

### 2.4. Đồng hồ đếm ngược 84 ngày đến lần hiến máu tiếp theo (Mục 19.5)
- **Lý do nên làm:** Hiện tại [`Views/NguoiHienMau/HoSo.cshtml`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSo.cshtml#L45-L60) và [`Views/DangKy/TheoDoi.cshtml`](file:///e:/InformationTechnology/visual%20tím/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml#L41-L65) đã có tính toán chu kỳ 84 ngày! Tuy nhiên perlu kiểm tra để đồng bộ: lấy ngày lớn nhất giữa `NguoiHienMau.NgayHienMauGanNhat` và ngày của phiếu có `TrangThai == "Hiến máu thành công"` (giống hệt logic trong `DangKyController.KiemTraDieuKienNghiepVu`).

### 2.5. Cảnh báo lịch khám sàng lọc sắp diễn ra (Mục 19.6)
- **Lý do nên làm:** Được gợi ý tại Mục 19.6. Khi người hiến máu vào trang `TheoDoi` hoặc `HoSo`, nếu có lịch khám sàng lọc ở trạng thái `"Đã lên lịch"` có `ThoiGianBatDau` trong vòng 48 giờ tới, hiển thị một banner nhắc nhở nổi bật (kèm thời gian, địa điểm, bác sĩ phụ trách).

---

## 3. Lý do **"Không nên làm"** các tính năng vượt phạm vi

1. **Không làm tính năng gửi Email SMTP thật:** Khi chấm bài trên máy giảng viên hoặc phòng máy trường, tường lửa hoặc chính sách bảo mật tài khoản Google thường chặn kết nối SMTP порт 587, dễ gây treo request hoặc văng Exception khi duyệt phiếu.
2. **Không phát triển tiếp 6 màn hình Kho máu / Tiêu hủy / Xét nghiệm / IoT:** Mục 3 Đề bài đã quy định rõ: *"Không đi sâu vào nghiệp vụ xét nghiệm y khoa phức tạp sau hiến máu"*. Việc giữ hoặc mở rộng các màn hình này vừa tốn công sức, vừa làm loãng 5 Module chính và tạo điểm yếu để giảng viên chất vấn.
