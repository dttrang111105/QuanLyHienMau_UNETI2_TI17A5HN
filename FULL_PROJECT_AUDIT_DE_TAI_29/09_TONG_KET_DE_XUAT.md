# 09. TỔNG KẾT KIỂM TOÁN & ĐỀ XUẤT QUYẾT ĐỊNH CHO NHÓM (EXECUTIVE SUMMARY)

---

## 1. Đánh giá tổng thể tỷ lệ hoàn thành của Project hiện tại

- **Tỷ lệ hoàn thành các yêu cầu bắt buộc của Đề tài 29:** **~86.5%**
  - **Về cấu trúc Database & 7 Thực thể bắt buộc:** Đạt **100%**
  - **Về chức năng cốt lõi của 5 Module:** Đạt **90%**
  - **Về dữ liệu mẫu (`DbInitializer.cs`) theo Mục 16:** Đạt **35%** (Thiếu số lượng bản ghi ở hầu hết các bảng, chưa có bản ghi mẫu cho `LichKhamSangLoc` và `KetQuaHienMau`)
  - **Về quy định ghi chú Họ tên + MSSV trên đầu file theo Mục 18:** Đạt **40%** (Mới có Module 1 và Module 2 ghi đầy đủ)

---

## 2. Module mạnh nhất & Module yếu nhất hiện tại

- **Module mạnh nhất:**
  1. **Module 2 (Quản lý Đợt hiến máu – Nguyễn Thị Phương Thảo):** Kiến trúc Repository Pattern rất sạch, tách biệt rõ truy vấn `IQueryable`, tìm kiếm, 5 bộ lọc, 6 tiêu chí sắp xếp, phân trang `Skip/Take` và kiểm tra ràng buộc `IValidatableObject` chuẩn mực.
  2. **Module 3 (Quản lý Người hiến máu & Đăng ký hiến máu):** Kiểm tra 7 điều kiện nghiệp vụ hiến máu (đặc biệt là quy tắc 84 ngày đối chiếu chéo với cả lịch sử kết quả hiến máu thật trong DB và khóa giao dịch `Serializable`) rất sâu sắc và chặt chẽ.
- **Module rủi ro nhất (cần chỉnh sửa gấp nhất):**
  - **Module 5 & Sự chồng chéo trong `AdminController`:** Mặc dù thành viên Module 5 đã viết bộ `LichKhamSangLocController`, `KetQuaHienMauController`, `ThongKeController` và `KhamSangLocService` rất chuẩn, nhưng `AdminController` lại viết lặp lại các chức năng này với phiên bản thiếu kiểm tra trùng lịch khám và ngày quá khứ, đồng thời Sidebar `_AdminLayout.cshtml` lại đang trỏ vào phiên bản lỗi trong `AdminController`.

---

## 3. Top 10 việc cần sửa ưu tiên cao nhất (Theo thứ tự từ 1 đến 10)

1. **[P0 - Critical]** Sửa `AdminController.TaoLichKham` & `SuaLichKham` để gọi qua `KhamSangLocService` (hoặc điều hướng menu sang `LichKhamSangLocController`) nhằm áp dụng đầy đủ kiểm tra: không lập lịch quá khứ, không trùng lịch người hiến, không trùng lịch bác sĩ (Mục 9.2).
2. **[P0 - Critical]** Mở rộng `DbInitializer.SeedAsync` để tạo đủ tối thiểu: 05+ Đơn vị tổ chức, 15+ Đợt hiến máu, 30+ Người hiến máu có tài khoản, 02 Admin, 03 Nhân viên y tế, 45+ Phiếu đăng ký, 15+ Lịch khám sàng lọc, và Kết quả hiến máu ở cả 2 trạng thái (Mục 16).
3. **[P0 - Critical]** Bổ sung comment `// Họ và tên: ... // Mã sinh viên: ... // Nội dung thực hiện: ...` ở đầu toàn bộ các file thuộc Module 3, Module 4, Module 5 (Mục 18).
4. **[P1 - High]** Ẩn 6 mục menu giao diện tĩnh ngoài phạm vi đề tài (`HoSoBenhSu`, `TieuHuy`, `GiamSatTuiMau`, `KhoLuuTruMau`, `RBAC`, `KhoLanhIoT`) khỏi `Views/Shared/_AdminLayout.cshtml` (Mục 3).
5. **[P1 - High]** Thêm `[Authorize(Roles = "Admin")]` vào `AdminController.KhoaMoKhoaNguoiHien` và ẩn các menu chỉ dành cho `Admin` (`Đơn vị tổ chức`, `Người hiến máu`, `Quản lý tài khoản`) khi đăng nhập bằng `Medical Staff` (Mục 5.1).
6. **[P1 - High]** Chặn việc chuyển trạng thái phiếu trực tiếp từ `"Đã xác nhận"` sang `"Chờ khám sàng lọc"` trong `AdminController.CapNhatTrangThaiPhieu` khi chưa lập lịch khám sàng lọc (Mục 8.3 & 9.1).
7. **[P2 - Medium]** Bổ sung fallback đọc `ClaimTypes.NameIdentifier` từ Cookie Auth trong `KiemTraQuyenAttribute`, `LichKhamSangLocController` và `KetQuaHienMauController` để không bị văng lỗi phân quyền khi Session hết hạn trước Cookie.
8. **[P2 - Medium]** Bổ sung phân trang `Skip/Take` thật cho danh sách người hiến máu tại `AdminController.NguoiHienMau` và `Views/Admin/NguoiHienMau.cshtml`.
9. **[P2 - Medium]** Gắn `<form method="get" asp-controller="Admin" asp-action="PhieuDangKyHienMau">` cho ô tìm kiếm trên thanh Topbar của `_AdminLayout.cshtml`.
10. **[P3 - Low]** Dọn dẹp hoặc chú thích rõ các file không sử dụng (`Data/CauHinhKhamSangLoc.cs`, `Services/ThongKeService.cs`).

---

## 4. 5 Chức năng mở rộng NÊN LÀM để lấy điểm cộng (Mục 19 Đề bài)

1. **Biểu đồ thống kê trực quan (Chart.js)** trên trang `ThongKe/Index.cshtml` (Biểu đồ tròn tỷ lệ trạng thái phiếu & Biểu đồ cột số phiếu/lịch khám theo tháng).
2. **Xuất danh sách Phiếu đăng ký & Thống kê theo đợt ra file CSV/Excel (UTF-8 BOM)**.
3. **Nút lọc nhanh "Gợi ý đợt hiến máu phù hợp với Nhóm máu & Cân nặng của tôi"** trên trang `DotHienMau/Index.cshtml` dành cho người hiến máu đang đăng nhập.
4. **Banner cảnh báo Lịch khám sàng lọc sắp diễn ra (trong 48h tới)** trên trang `TheoDoi.cshtml` và `HoSo.cshtml`.
5. **Trang in Giấy chứng nhận hiến máu tình nguyện** cho các phiếu có kết quả `"Hiến máu thành công"`.

---

## 5. 5 Chức năng mở rộng KHÔNG NÊN LÀM

1. **Không làm chức năng gửi Email SMTP thật** (dễ lỗi kết nối mạng/App Password trên máy chấm thi).
2. **Không viết thêm Backend/Database cho Kho lưu trữ máu (`KhoLuuTruMau`)** (vượt phạm vi Đề tài 29).
3. **Không viết thêm Backend cho Kiểm định xét nghiệm & Tiêu hủy túi máu (`TieuHuy`, `GiamSatTuiMau`)** (vi phạm giới hạn tại Mục 3 Đề bài).
4. **Không viết thêm Backend cho Cảm biến nhiệt độ kho lạnh (`KhoLanhIoT`)** (ngoài phạm vi đề tài).
5. **Không xây dựng hệ thống phân quyền động theo bảng Permission (`RBAC`)** (phức tạp hóa không cần thiết, đề bài chỉ yêu cầu 3 vai trò cố định).

---

## 6. Các phần dư thừa có thể ẩn/bỏ an toàn

- **Ẩn khỏi Sidebar `_AdminLayout.cshtml`:** 6 link `HoSoBenhSu`, `TieuHuy`, `GiamSatTuiMau`, `KhoLuuTruMau`, `RBAC`, `KhoLanhIoT`.
- **Không nên xóa cứng các Controller riêng của thành viên (`TaiKhoanController`, `DonViToChucController`, `DotHienMauController`, `LichKhamSangLocController`, `KetQuaHienMauController`)** vì đó là nơi lưu trữ code và minh chứng đóng góp cá nhân của từng sinh viên khi giảng viên kiểm tra theo Mục 18 & Mục 20.

---

## 7. Đề xuất phạm vi chốt cuối cùng để đi Bảo vệ đạt điểm A/A+

Để đạt điểm tối đa (**9.0 – 10.0 / 100 điểm** theo thang chấm Mục 20), nhóm chỉ cần tập trung thực hiện **Giai đoạn 1 → Giai đoạn 4** (mất khoảng 1–2 giờ làm việc) để đạt **100% yêu cầu bắt buộc + 0 lỗi trừ điểm**, sau đó bổ sung **2–3 tính năng nâng cao nhẹ nhàng ở Giai đoạn 6** (Biểu đồ Chart.js + Xuất Excel/CSV + Gợi ý đợt hiến máu phù hợp) là dự án hoàn toàn vững vàng để bảo vệ trước giảng viên.
