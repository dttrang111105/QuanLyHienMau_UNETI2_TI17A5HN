# PHẦN VI (MỤC 1, 2, 3, 4): TÓM TẮT 50 CÂU HỎI QUAN TRỌNG NHẤT, 20 CÂU DỄ TRẢ LỜI SAI, 15 CÂU CHỈ CODE TRỰC TIẾP VÀ 10 ĐIỂM CỐT LÕI

> **Mục đích tài liệu:** Đây là "Tờ phao ôn tập tốc hành" (Quick-Revision Sheet) giúp sinh viên rà soát nhanh toàn bộ kiến thức cốt lõi của Module 3 trong 30 phút trước khi bước vào phòng bảo vệ.

---

## 1. TOP 50 CÂU HỎI QUAN TRỌNG NHẤT PHẢI HỌC THUỘC Ý (KÈM ĐÁP ÁN NGẮN)

### A. Nhóm Tổng quan & Kiến trúc Module 3 (Câu 1 – 10)
1. **Module 3 tên là gì và giải quyết bài toán gì?**
   -> *Quản lý người hiến máu và Phiếu đăng ký hiến máu*: Quản lý hồ sơ sức khỏe cá nhân, tiếp nhận phiếu đăng ký theo đợt, sàng lọc điều kiện y tế & chu kỳ 84 ngày, theo dõi tiến trình phiếu 4 bước và hủy phiếu.
2. **Module 3 gồm những Controller nào?**
   -> 2 Controller chính là `DangKyController.cs` và `NguoiHienMauController.cs` (cùng phần quản lý người hiến và duyệt phiếu trong `AdminController.cs`).
3. **Module 3 làm việc với những bảng CSDL nào?**
   -> 2 bảng chính là `NguoiHienMau` và `PhieuDangKyHienMau`; liên kết trực tiếp với `TaiKhoan` (Module 1), `DotHienMau` (Module 2), `LichKhamSangLoc` (Module 4) và `KetQuaHienMau` (Module 5).
4. **Các file View chính của Module 3 là gì?**
   -> `Views/DangKy/Create.cshtml`, `Views/DangKy/TheoDoi.cshtml`, `Views/NguoiHienMau/HoSoCaNhan.cshtml` và `Views/Admin/NguoiHienMau.cshtml`.
5. **Những tác nhân (Actor) nào sử dụng Module 3?**
   -> (1) Người hiến máu đã đăng nhập, (2) Khách vãng lai chưa đăng nhập (chỉ ở `DangKy/Create`), và (3) Nhân viên y tế / Admin (`Admin`, `NhanVienYTe`).
6. **Dự án sử dụng nền tảng công nghệ gì?**
   -> ASP.NET Core MVC trên `.NET 10.0` (`net10.0`), Entity Framework Core `10.0.5` với SQL Server, xác thực bằng Cookie Authentication.
7. **Dự án trên nhánh `DamThuTrang_23103100243` có tách lớp `Service` hay `Repository` riêng cho Module 3 không?**
   -> Không. Các Controller nhận trực tiếp `ApplicationDbContext` qua Dependency Injection và dùng chung các hàm nghiệp vụ tĩnh trong `DangKyController`.
8. **Nếu bỏ Module 3 thì hệ thống bị ảnh hưởng thế nào?**
   -> Đứt gãy hoàn toàn: Module 2 có đợt hiến máu nhưng không ai đăng ký được; Module 4 (Lịch khám) và Module 5 (Kết quả) mất khóa ngoại `MaPhieu` nên không thể hoạt động.
9. **Khi đăng ký tài khoản mới ở Module 1 (`AccountController.Register`), hồ sơ `NguoiHienMau` được tạo lúc nào?**
   -> Được tự động tạo ngay trong `AccountController.Register` (dòng 150–160) gắn với `MaTaiKhoan` mới; ngoài ra nếu tài khoản cũ chưa có bản ghi `NguoiHienMau`, `NguoiHienMauController` và `DangKyController` cũng tự khởi tạo khi người dùng cập nhật hồ sơ hoặc đăng ký hiến máu.
10. **Những tính năng nào trên giao diện Module 3 chỉ là mô phỏng (UI Mock)?**
    -> Ô nhập Chiều cao trên `Create.cshtml`; các nút Tải Chứng Nhận, Xuất PDF, Lưu VNeID, Chia sẻ và tab Xét nghiệm chi tiết trên `HoSoCaNhan.cshtml`; nút Xuất CSV/Excel và Gọi máu khẩn cấp trên `Admin/NguoiHienMau.cshtml`.

### B. Nhóm Quy tắc 84 ngày & Điều kiện Đăng ký (Câu 11 – 25)
11. **Khoảng thời gian chờ tối thiểu giữa 2 lần hiến máu thành công là bao nhiêu ngày?**
    -> **84 ngày** (tương đương 12 tuần theo chuẩn Bộ Y tế), khai báo tại `DangKyController.SoNgayChoToiThieuGiuaHaiLanHien = 84`.
12. **Làm sao hệ thống xác định một phiếu là "lần hiến máu thành công thực sự"?**
    -> Hàm `IsConfirmedSuccessfulDonation` loại trừ `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"` và kiểm tra `phieu.KetQuaHienMau.KetQua == "Hiến máu thành công"` (hoặc `phieu.TrangThai == "Hiến máu thành công"` nếu chưa nạp object kết quả).
13. **Hệ thống lấy ngày nào để cộng 84 ngày?**
    -> Lấy duy nhất từ **`phieu.KetQuaHienMau.NgayCapNhat`** thông qua hàm `GetActualDonationDate(phieu)`. Tuyệt đối không lấy `PhieuDangKyHienMau.NgayNop` hay `NgayXuLy`.
14. **Tại sao không lấy ngày nộp phiếu (`NgayNop`) để tính 84 ngày?**
    -> Vì `NgayNop` chỉ là ngày đăng ký online trước đợt hiến máu, không phải ngày lấy máu thực tế.
15. **Nếu phiếu có trạng thái `"Đã hủy"`, `"Từ chối"` hoặc `"Không đủ điều kiện hiến"` thì có bị tính 84 ngày không?**
    -> **Không!** Người dùng được phép đăng ký lại mà không cần chờ 84 ngày.
16. **Nếu phiếu có trạng thái `"Hiến máu thành công"` nhưng bên bảng `KetQuaHienMau` bị thiếu ngày (`NgayCapNhat == null`) thì hệ thống làm gì?**
    -> Bật cờ `HasMissingActualDonationDate = true` và trả về `IsEligible = false` để **chặn an toàn**, yêu cầu liên hệ nhân viên y tế cập nhật ngày hiến thực tế chứ không lấy `NgayNop` thay thế.
17. **Nếu một người có nhiều lần hiến máu thành công trong lịch sử, hệ thống lấy lần nào để tính 84 ngày?**
    -> Sắp xếp giảm dần `.OrderByDescending(x => x.ActualDate).ThenByDescending(x => x.Phieu.MaPhieu)` để lấy lần hiến thành công có ngày thực tế **mới nhất** (`latestDated`).
18. **Điều kiện độ tuổi hợp lệ để hiến máu là bao nhiêu?**
    -> Từ đủ **18 đến 60 tuổi** (tính chính xác đến từng ngày sinh nhật trong hàm `IsValidAge`).
19. **Điều kiện cân nặng hợp lệ khi đăng ký hiến máu là gì?**
    -> Phải từ **30 kg đến 200 kg** và đạt mức tối thiểu của đợt hiến máu đó (`canNang >= dotHienMau.CanNangToiThieu`, mặc định là 45kg).
20. **Điều kiện Số điện thoại hợp lệ là gì?**
    -> Sau khi gọi `NormalizePhone` (xóa khoảng trắng, `-`, `.`), phải khớp Regex `^0[0-9]{9,10}$` (bắt đầu bằng số `0`, dài 10 hoặc 11 chữ số).
21. **Hai câu hỏi khảo sát sức khỏe trên form `DangKy/Create` được kiểm tra ở Server như thế nào?**
    -> Kiểm tra `if (health_q1 != "none" || health_q2 != "good")` tại dòng 513 `DangKyController.cs`. Nếu chọn bất kỳ phương án bệnh lý nào khác sẽ bị chặn ngay.
22. **Khi nào một đợt hiến máu không nhận thêm phiếu đăng ký nữa?**
    -> Khi `TrangThai` khác `"Đang mở"` / `"Sắp diễn ra"`, hoặc `KetThuc < DateTime.Now`, hoặc số phiếu hợp lệ đã đạt chỉ tiêu (`soLuongDaDangKy >= dotHienMau.SoLuongToiDa`).
23. **Khi đếm `soLuongDaDangKy` của một đợt hiến máu, những phiếu nào không bị tính vào chỉ tiêu?**
    -> Các phiếu có trạng thái `"Đã hủy"`, `"Từ chối"` và `"Không đủ điều kiện hiến"` không bị tính, giúp trả lại chỗ trống cho người khác đăng ký.
24. **Nếu người dùng đã có 1 phiếu `"Chờ xác nhận"` trong Đợt 1, họ vào lại Đợt 1 đăng ký lần nữa thì sao?**
    -> Bị biến `existingPhieu` phát hiện và chặn ở cả `GET` (khóa nút Submit) lẫn `POST` (Rollback Transaction).
25. **Trường `volume` (dung tích máu) và `timeSlot` (khung giờ hẹn) được lưu vào đâu trong bảng `PhieuDangKyHienMau`?**
    -> `volume` được ghép chuỗi vào cột `LyDoHienMau`, còn `timeSlot` được ghép chuỗi vào cột `GhiChu` (dòng 580–596 `DangKyController.cs`).

### C. Nhóm Vòng đời Phiếu, Theo dõi & Hủy phiếu (Câu 26 – 35)
26. **Kể tên 8 trạng thái của `PhieuDangKyHienMau.TrangThai`?**
    -> `"Chờ xác nhận"`, `"Đã xác nhận"`, `"Từ chối"`, `"Đã hủy"`, `"Chờ khám sàng lọc"`, `"Đã khám sàng lọc"`, `"Hiến máu thành công"`, `"Không đủ điều kiện hiến"`.
27. **Thanh tiến trình (Stepper) trên trang `Views/DangKy/TheoDoi.cshtml` gồm mấy bước?**
    -> Gồm **4 bước**: Bước 1 (Gửi phiếu đăng ký) -> Bước 2 (Xác nhận & Lịch khám) -> Bước 3 (Khám sàng lọc) -> Bước 4 (Kết quả hiến máu).
28. **Phiếu nào được hiển thị ở khu vực "Phiếu đang hoạt động" (Stepper 4 bước) trên trang `TheoDoi.cshtml`?**
    -> Phiếu mới nhất có trạng thái chưa kết thúc (tức khác `"Đã hủy"`, `"Từ chối"`, `"Không đủ điều kiện hiến"` và `"Hiến máu thành công"`).
29. **Người hiến máu chỉ được tự hủy phiếu (`POST /DangKy/Huy`) khi thỏa mãn những điều kiện nào?**
    -> (1) Là chủ sở hữu phiếu (`MaNguoiHien`), (2) `TrangThai` thuộc `{"Chờ xác nhận", "Đã xác nhận"}`, (3) Không có `LichKhamSangLoc` nào khác `"Đã hủy"`, và (4) `KetQuaHienMau == null`.
30. **Khi hủy phiếu thành công, dữ liệu trong CSDL thay đổi như thế nào?**
    -> Cột `TrangThai` đổi thành `"Đã hủy"` và cột `NgayXuLy` cập nhật thành `DateTime.Now` (không xóa bản ghi).
31. **Khi Admin bấm "Xác nhận phiếu" (`AdminController.XacNhanPhieu`), phiếu chuyển từ trạng thái nào sang trạng thái nào?**
    -> Chuyển từ `"Chờ xác nhận"` sang `"Đã xác nhận"`, cập nhật `NgayXuLy = DateTime.Now` và lưu `GhiChuXacNhan`.
32. **Khi Admin bấm "Từ chối phiếu" (`AdminController.TuChoiPhieu`), điều gì xảy ra với các lịch khám sàng lọc của phiếu đó?**
    -> Phiếu chuyển sang `"Từ chối"`, đồng thời tất cả các bản ghi `LichKhamSangLoc` chưa hoàn thành của phiếu đó tự động chuyển sang `"Đã hủy"`.
33. **Khi bác sĩ hoàn thành khám sàng lọc (`AdminController.HoanThanhKham`), trạng thái phiếu chuyển thành gì?**
    -> Chuyển thành `"Đã khám sàng lọc"`.
34. **Khi nhân viên y tế ghi nhận kết quả (`AdminController.TaoKetQua`), trạng thái phiếu được đồng bộ như thế nào?**
    -> Chỉ cho phép tạo kết quả khi phiếu đang ở `"Đã khám sàng lọc"`, và đồng bộ `phieu.TrangThai = ketQua` (`"Hiến máu thành công"` hoặc `"Không đủ điều kiện hiến"`).
35. **Mã phiếu `#DKHM-00012` trên giao diện được tạo ra từ cột nào?**
    -> Từ khóa chính `MaPhieu` (`int`) được định dạng bằng `.ToString("D5")` với tiền tố `#DKHM-`.

### D. Nhóm Kỹ thuật MVC, CSDL, Concurrency & Bảo mật (Câu 36 – 50)
36. **Hệ thống chống gửi trùng 2 phiếu đồng thời (Double Submit / Race Condition) bằng cách nào?**
    -> Kết hợp khóa `SemaphoreSlim(1, 1)` theo `MaNguoiHien` trong `_submissionLocks` với Database Transaction `IsolationLevel.Serializable` (dòng 532–616 `DangKyController.cs`).
37. **Tại sao phải kiểm tra lại quy tắc 84 ngày và `existingPhieu` ở bên trong Transaction?**
    -> Để đảm bảo dữ liệu đọc lên không bị thay đổi bởi một luồng khác giữa thời điểm kiểm tra và thời điểm `INSERT` (đảm bảo tính ACID).
38. **Tại sao khóa ngoại `NguoiHienMau.MaTaiKhoan` lại dùng `DeleteBehavior.SetNull`?**
    -> Để nếu xóa tài khoản đăng nhập thì hồ sơ y tế và lịch sử hiến máu của người đó vẫn được giữ lại.
39. **Tại sao khóa ngoại `PhieuDangKyHienMau.MaNguoiHien` và `MaDot` lại dùng `DeleteBehavior.Restrict`?**
    -> Để ngăn chặn việc xóa nhầm một người hiến máu hoặc một đợt hiến máu đã có phiếu đăng ký phát sinh.
40. **Tại sao trên cột `NguoiHienMau.MaTaiKhoan` phải dùng Filtered Unique Index `WHERE [MaTaiKhoan] IS NOT NULL`?**
    -> Để vừa đảm bảo mỗi tài khoản chỉ gắn với 1 hồ sơ người hiến (`1 - 1`), vừa cho phép lưu nhiều hồ sơ khách vãng lai có `MaTaiKhoan = NULL`.
41. **Làm thế nào hệ thống hợp nhất lịch sử của khách vãng lai khi họ đăng nhập tài khoản để đăng ký hiến máu?**
    -> Tại dòng 367–376 `DangKyController.cs`, nếu tài khoản chưa có `NguoiHienMau`, hệ thống tìm bản ghi `NguoiHienMau` trùng SĐT có `MaTaiKhoan == null` và gán `MaTaiKhoan` vào bản ghi đó.
42. **Làm sao hệ thống xác định người đang đăng nhập trong Controller?**
    -> Đọc custom claim `"MaTaiKhoan"` từ Cookie xác thực: `User.FindFirst("MaTaiKhoan")?.Value`.
43. **Hệ thống chống lỗ hổng IDOR khi xem/sửa hồ sơ và hủy phiếu như thế nào?**
    -> Luôn truy vấn `NguoiHienMau` theo `MaTaiKhoan` lấy từ Cookie, và khi hủy phiếu luôn kiểm tra `p.MaPhieu == id && p.MaNguoiHien == nguoiHienMau.MaNguoiHien`.
44. **Hệ thống chống lỗ hổng Mass Assignment (Over-posting) ở `HoSoCaNhan [POST]` như thế nào?**
    -> Chỉ gán tường minh 10 trường thông tin cá nhân/y tế từ `model` sang thực thể `nguoiHienMau` lấy từ DB, không bao giờ gán `MaNguoiHien`, `MaTaiKhoan`, `TrangThai` từ form.
45. **Sự khác nhau giữa `ViewBag` và `TempData` trong Module 3 là gì?**
    -> `ViewBag` chỉ truyền dữ liệu trong cùng 1 request để render View; `TempData` lưu qua Cookie/Session để giữ thông báo `Success`/`Error` sau khi gọi `RedirectToAction`.
46. **Tại sao trong `HoSoCaNhan [GET]`, khi chưa có bản ghi `NguoiHienMau` dưới DB, hệ thống chỉ `new NguoiHienMau` trên RAM mà không `SaveChangesAsync()`?**
    -> Vì `HTTP GET` chỉ dùng để đọc và hiển thị dữ liệu (Safe & Idempotent), chỉ khi người dùng gửi `HTTP POST` mới ghi xuống DB.
47. **Điểm chưa đồng bộ giữa trang `Admin/NguoiHienMau` và `DangKyController` là gì?**
    -> `DangKyController` và `NguoiHienMauController` tính lần hiến thành công và 84 ngày theo `KetQuaHienMau.NgayCapNhat`, trong khi `AdminController.NguoiHienMau` vẫn đang dùng công thức cũ (`p.NgayXuLy ?? p.NgayNop` và `p.KetQuaHienMau != null`).
48. **Hạn chế của việc dùng cột `KetQuaHienMau.NgayCapNhat` làm ngày hiến máu thực tế là gì?**
    -> Nếu nhân viên nhập kết quả muộn hơn ngày lấy máu, hoặc vào `SuaKetQua` sửa lại ghi chú (bị gán lại `NgayCapNhat = DateTime.Now`), mốc 84 ngày sẽ bị trôi theo ngày nhập/sửa.
49. **Nếu giảng viên yêu cầu đổi quy tắc từ 84 ngày sang 3 tháng theo lịch dương thì sửa dòng nào?**
    -> Sửa dòng 129 trong `Controllers/DangKyController.cs` từ `lastActualDate.AddDays(SoNgayChoToiThieuGiuaHaiLanHien)` thành `lastActualDate.AddMonths(3)`.
50. **Kết quả biên dịch (`dotnet build`) trên nhánh `DamThuTrang_23103100243` hiện tại như thế nào?**
    -> Biên dịch thành công 100% (`Build succeeded: 0 Error(s), 0 C# Compiler Warning(s)`).

---

## 2. TOP 20 CÂU HỎI DỄ TRẢ LỜI SAI NHẤT (CÁC "BẪY" TRONG PHÒNG BẢO VỆ)

| STT | Câu hỏi "Bẫy" của Giảng viên | ❌ Câu trả lời SAI thường gặp của sinh viên | ✅ Câu trả lời ĐÚNG theo Code thực tế nhánh `DamThuTrang_23103100243` |
|---|---|---|---|
| 1 | *"Hệ thống lấy ngày nộp phiếu (`NgayNop`) cộng thêm 84 ngày để chặn đăng ký đúng không?"* | "Dạ đúng ạ, lấy `NgayNop` cộng 84 ngày." | **"Dạ KHÔNG ạ! Hệ thống tuyệt đối không lấy `NgayNop`, mà chỉ lấy `KetQuaHienMau.NgayCapNhat` của phiếu có kết quả `'Hiến máu thành công'`."** |
| 2 | *"Trên form đăng ký có 4 câu hỏi khảo sát sức khỏe giống trong file báo cáo `.md` cũ đúng không?"* | "Dạ có 4 câu hỏi `q1, q2, q3, q4` ạ." | **"Dạ trên nhánh `DamThuTrang_23103100243` hiện tại, code thực tế có 2 câu hỏi là `health_q1` (bệnh truyền nhiễm) và `health_q2` (sức khỏe 3 tháng gần đây)."** |
| 3 | *"Người hiến máu được hủy phiếu ở cả trạng thái `'Chờ khám sàng lọc'` đúng không?"* | "Dạ được hủy ở 3 trạng thái gồm cả Chờ khám sàng lọc." | **"Dạ KHÔNG ạ! Trong `DangKyController.Huy` (dòng 757), mảng `trangThaiChoPhep` chỉ cho phép hủy ở đúng 2 trạng thái là `'Chờ xác nhận'` và `'Đã xác nhận'`."** |
| 4 | *"Dự án có file `Module5Service.cs` và `KetQuaHienMauController.cs` phải không?"* | "Dạ có ạ." | **"Dạ các file đó nằm ở nhánh `integration` cũ. Trên nhánh `DamThuTrang_23103100243` của em, toàn bộ chức năng quản lý lịch khám và kết quả được đặt trong `AdminController.cs`."** |
| 5 | *"Khi người dùng hủy phiếu thì bản ghi đó bị lệnh `Remove` xóa khỏi bảng `PhieuDangKyHienMau` đúng không?"* | "Dạ bị xóa khỏi bảng luôn ạ." | **"Dạ không xóa vật lý, mà chỉ cập nhật `phieu.TrangThai = "Đã hủy"` và `phieu.NgayXuLy = DateTime.Now` để giữ lại lịch sử."** |
| 6 | *"Nếu người dùng đi khám sàng lọc không đạt (`'Không đủ điều kiện hiến'`), họ phải chờ 84 ngày mới được đăng ký lại đúng không?"* | "Dạ phải chờ 84 ngày ạ." | **"Dạ KHÔNG phải chờ 84 ngày! Vì họ chưa hiến máu, hàm `IsConfirmedSuccessfulDonation` trả về `false` nên họ được đăng ký lại ở đợt sau."** |
| 7 | *"Nếu phiếu có trạng thái `'Hiến máu thành công'` nhưng `KetQuaHienMau.NgayCapNhat` bị `NULL`, hệ thống lấy tạm `NgayNop` để tính 84 ngày đúng không?"* | "Dạ nếu thiếu ngày kết quả thì lấy tạm `NgayNop`." | **"Dạ KHÔNG lấy `NgayNop`! Hệ thống bật cờ `HasMissingActualDonationDate = true` và chặn đăng ký an toàn cho đến khi nhân viên y tế cập nhật ngày thực tế."** |
| 8 | *"Bắt buộc phải đăng nhập tài khoản mới vào được trang `/DangKy/Create` đúng không?"* | "Dạ phải đăng nhập mới vào được trang tạo phiếu." | **"Dạ trong code hiện tại, action `Create` không gắn `[Authorize]` nên cả khách vãng lai chưa đăng nhập cũng gửi phiếu được; chỉ `TheoDoi`, `Huy` và `HoSoCaNhan` mới bắt buộc `[Authorize]`."** |
| 9 | *"Ô nhập Chiều cao (cm) trên trang `DangKy/Create` được lưu vào bảng `NguoiHienMau` đúng không?"* | "Dạ lưu vào cột `ChieuCao` trong bảng `NguoiHienMau`." | **"Dạ trong bảng `NguoiHienMau` không có cột `ChieuCao`, ô Chiều cao trên `Create.cshtml` chỉ là giao diện tham khảo phía Client."** |
| 10 | *"Dung tích máu (`volume`) và Khung giờ hẹn (`timeSlot`) có 2 cột riêng trong bảng `PhieuDangKyHienMau` đúng không?"* | "Dạ lưu vào cột `Volume` và `TimeSlot`." | **"Dạ không có cột riêng, mà `volume` được nối chuỗi vào cột `LyDoHienMau` và `timeSlot` được nối chuỗi vào cột `GhiChu`."** |
| 11 | *"Nút `'Tải Chứng Nhận'`, `'Xuất PDF'`, `'Lưu VNeID'` trên trang `HoSoCaNhan` sinh ra file PDF thật từ Server đúng không?"* | "Dạ Server xuất ra file PDF thật ạ." | **"Dạ các nút đó hiện đang là mô phỏng giao diện (UI Mock) bằng hàm `alert()` của JavaScript."** |
| 12 | *"Tab `'Hồ sơ xét nghiệm'` (HIV, Viêm gan B, Giang mai) lấy từ bảng `XetNghiem` trong CSDL đúng không?"* | "Dạ lấy từ CSDL ra ạ." | **"Dạ trong CSDL không có bảng xét nghiệm chi tiết, tab đó hiển thị nội dung minh họa tĩnh khi `totalCount > 0`."** |
| 13 | *"Tổng lượng máu đã hiến (`totalVolume`) trên trang `HoSoCaNhan` được cộng từ dung tích thực tế của từng lần hiến đúng không?"* | "Dạ cộng từ các túi 250ml, 350ml, 450ml." | **"Dạ tại dòng 17 file `HoSoCaNhan.cshtml`, biến `totalVolume` đang được tính ước lượng bằng công thức `totalCount * 350` ml."** |
| 14 | *"Khi đổi Họ tên ở `HoSoCaNhan`, tên ở góc phải thanh menu (`_Layout.cshtml`) cũng đổi ngay lập tức đúng không?"* | "Dạ đổi ngay lập tức ạ." | **"Dạ trong DB (`NguoiHienMau.HoTen` và `TaiKhoan.HoTen`) thì đổi ngay, nhưng thanh menu đọc từ Claim trong Cookie nên phải đăng nhập lại mới đổi tên trên thanh menu."** |
| 15 | *"Hàm tính tuổi chỉ lấy ` Năm hiện tại - Năm sinh ` thôi đúng không?"* | "Dạ lấy `DateTime.Today.Year - ngaySinh.Year`." | **"Dạ không chỉ trừ năm, mà hàm `IsValidAge` còn kiểm tra `if (ngaySinh.Date > today.AddYears(-age)) age--;` để tính chính xác đến từng ngày sinh nhật."** |
| 16 | *"Nếu người dùng đang có phiếu `'Chờ xác nhận'` ở Đợt 1, hệ thống chặn không cho họ đăng ký ở Đợt 2 luôn đúng không?"* | "Dạ chặn trên mọi đợt luôn ạ." | **"Dạ câu lệnh kiểm tra `existingPhieu` hiện có điều kiện `p.MaDot == maDot`, nên mới chặn trùng trong cùng 1 đợt. Nếu muốn chặn trên mọi đợt thì bỏ `p.MaDot == maDot`."** |
| 17 | *"Khi xóa một tài khoản `TaiKhoan` thì bản ghi `NguoiHienMau` và các phiếu đăng ký của họ cũng bị xóa theo (`Cascade`) đúng không?"* | "Dạ xóa tài khoản là xóa hết hồ sơ luôn." | **"Dạ KHÔNG! Quan hệ `TaiKhoan - NguoiHienMau` dùng `DeleteBehavior.SetNull` (chỉ gán `MaTaiKhoan = NULL`), và `NguoiHienMau - PhieuDangKyHienMau` dùng `DeleteBehavior.Restrict`."** |
| 18 | *"84 ngày chính là đúng 3 tháng theo lịch dương (`AddMonths(3)`) đúng không?"* | "Dạ 84 ngày với 3 tháng là một." | **"Dạ 84 ngày là đúng 12 tuần ($12 \times 7$), còn 3 tháng dương lịch dài 89–92 ngày. Code đang dùng `.AddDays(84)`."** |
| 19 | *"Mật khẩu trong bảng `TaiKhoan` đã được mã hóa MD5/BCrypt rồi đúng không?"* | "Dạ đã mã hóa BCrypt rồi ạ." | **"Dạ ở Module 1 (`AccountController`), mật khẩu hiện đang lưu dạng văn bản thuần (Plaintext) để phục vụ test dữ liệu mẫu của đồ án."** |
| 20 | *"Trang `Admin/NguoiHienMau` và `DangKyController` đang dùng chung một hàm tính 84 ngày giống hệt nhau đúng không?"* | "Dạ giống hệt nhau ạ." | **"Dạ `DangKyController` và `NguoiHienMauController` đã dùng chung `EvaluateRecentDonationEligibility` (`KetQuaHienMau.NgayCapNhat`), còn `AdminController.NguoiHienMau` vẫn đang dùng công thức cũ theo `NgayXuLy ?? NgayNop`."** |

---

## 3. TOP 15 CÂU HỎI YÊU CẦU CHỈ RA CODE TRỰC TIẾP (BẢNG TRA CỨU NHANH DÒNG CODE)

1. **Hằng số 84 ngày:** `Controllers/DangKyController.cs` — **Dòng 23** (`SoNgayChoToiThieuGiuaHaiLanHien = 84`).
2. **Khóa đồng bộ chống gửi trùng request (`_submissionLocks`):** `Controllers/DangKyController.cs` — **Dòng 26** & **Dòng 532–533, 665**.
3. **Hàm kiểm tra lần hiến máu thành công (`IsConfirmedSuccessfulDonation`):** `Controllers/DangKyController.cs` — **Dòng 41–60**.
4. **Hàm lấy ngày hiến máu thực tế (`GetActualDonationDate`):** `Controllers/DangKyController.cs` — **Dòng 68–86**.
5. **Hàm đánh giá điều kiện 84 ngày (`EvaluateRecentDonationEligibility`):** `Controllers/DangKyController.cs` — **Dòng 94–178**.
6. **Hàm chuẩn hóa SĐT và tính tuổi 18–60 (`NormalizePhone`, `IsValidPhone`, `IsValidAge`):** `Controllers/DangKyController.cs` — **Dòng 180–205** (và `Controllers/NguoiHienMauController.cs` — **Dòng 20–45**).
7. **Kiểm tra chỉ tiêu đợt hiến máu (`soLuongDaDangKy >= dotHienMau.SoLuongToiDa`):** `Controllers/DangKyController.cs` — **Dòng 239–248 (GET)** & **Dòng 350–355 (POST)**.
8. **Hợp nhất hồ sơ khách vãng lai (`MaTaiKhoan == null`) vào tài khoản đăng nhập:** `Controllers/DangKyController.cs` — **Dòng 367–376**.
9. **Kiểm tra cân nặng (`30–200` kg và `>= CanNangToiThieu`) và 2 câu hỏi sức khỏe (`health_q1`, `health_q2`):** `Controllers/DangKyController.cs` — **Dòng 494–517**.
10. **Transaction `IsolationLevel.Serializable` tạo phiếu đăng ký:** `Controllers/DangKyController.cs` — **Dòng 536–616**.
11. **Ghép `volume` vào `LyDoHienMau` và `timeSlot` vào `GhiChu`:** `Controllers/DangKyController.cs` — **Dòng 580–596**.
12. **Action Theo dõi phiếu (`TheoDoi`) và Stepper 4 bước:** `Controllers/DangKyController.cs` — **Dòng 685–721** & `Views/DangKy/TheoDoi.cshtml` — **Dòng 12–103, 175–310**.
13. **Action Hủy phiếu (`Huy`) kiểm tra chủ sở hữu, 2 trạng thái cho phép, lịch khám và kết quả:** `Controllers/DangKyController.cs` — **Dòng 725–781**.
14. **Xem & Cập nhật hồ sơ cá nhân (`HoSoCaNhan` GET & POST) + đồng bộ `TaiKhoan.HoTen`:** `Controllers/NguoiHienMauController.cs` — **Dòng 48–282** (đồng bộ `HoTen` ở **Dòng 249**).
15. **Cấu hình Fluent API (`SetNull`, `Restrict`, `Cascade`, Filtered Unique Index):** `Data/ApplicationDbContext.cs` — **Dòng 20–84**.

---

## 4. 10 ĐIỂM CỐT LÕI VỀ MODULE 3 SINH VIÊN CẦN NẮM THẬT CHẮC

1. **Hiểu rõ vai trò trung tâm của Module 3:** Nối `TaiKhoan` (Module 1) + `DotHienMau` (Module 2) -> tạo ra `NguoiHienMau` & `PhieuDangKyHienMau` (Module 3) -> cung cấp đầu vào cho `LichKhamSangLoc` (Module 4) & `KetQuaHienMau` (Module 5).
2. **Thuộc lòng nguyên lý 84 ngày:** Chỉ tính phiếu có kết quả `"Hiến máu thành công"`, lấy ngày tại `KetQuaHienMau.NgayCapNhat`, cộng 84 ngày (`AddDays(84)`), tuyệt đối không lấy `NgayNop`.
3. **Hiểu cơ chế Fail-Safe khi thiếu ngày thực tế:** Nếu phiếu mang trạng thái `"Hiến máu thành công"` nhưng `KetQuaHienMau.NgayCapNhat == null`, hệ thống bật `HasMissingActualDonationDate = true` để chặn an toàn chứ không đoán ngày.
4. **Nắm chắc điều kiện Hủy phiếu:** Chỉ người sở hữu phiếu mới hủy được, chỉ hủy ở 2 trạng thái `"Chờ xác nhận"` và `"Đã xác nhận"`, chưa có lịch khám hoạt động và chưa có kết quả; hủy bằng cách đổi `TrangThai = "Đã hủy"`.
5. **Giải thích được cơ chế chống Race Condition:** `SemaphoreSlim` theo `MaNguoiHien` + Transaction `IsolationLevel.Serializable`.
6. **Nắm vững 8 trạng thái của `PhieuDangKyHienMau.TrangThai`** và biết trạng thái nào do người hiến tạo/hủy, trạng thái nào do Admin/Nhân viên y tế chuyển đổi.
7. **Giải thích được cấu trúc khóa ngoại và Index trong `ApplicationDbContext.cs`:** Tại sao `MaTaiKhoan` cho phép `NULL` + `Filtered Unique Index` + `SetNull`, tại sao `MaNguoiHien` và `MaDot` dùng `Restrict`.
8. **Nắm rõ các quy tắc Validation ở Server:** Tuổi `18–60` (tính chính xác theo ngày sinh nhật), SĐT `^0[0-9]{9,10}$` (có `NormalizePhone`), cân nặng `30–200` kg và `>= dotHienMau.CanNangToiThieu`, khảo sát sức khỏe `health_q1 == "none" && health_q2 == "good"`.
9. **Thành thật và chủ động về phạm vi code thực tế:** Phân biệt rõ tính năng lưu CSDL thật với các nút mô phỏng giao diện (UI Mock) và sự khác biệt giữa nhánh `DamThuTrang_23103100243` với các file báo cáo cũ.
10. **Biết cách đề xuất cải tiến thông minh:** Tách cột `NgayHienMauThucTe`, `TheTichMauHien`, `DungTichDangKy`, `KhungGioHen` và đồng bộ công thức tính 84 ngày sang `AdminController.NguoiHienMau`.
