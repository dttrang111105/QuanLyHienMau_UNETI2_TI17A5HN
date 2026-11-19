# PHẦN II (NHÓM L) & PHẦN VI (MỤC 6, 7): LIÊN HỆ GIAO DIỆN – CODE, KỊCH BẢN THUYẾT TRÌNH (3–5 PHÚT) VÀ KỊCH BẢN DEMO WEBSITE (5–10 PHÚT)

> **Mục tiêu tài liệu:**
> 1. **Nhóm L (Liên hệ trực tiếp giữa Giao diện và Code):** Giúp sinh viên ở bất kỳ màn hình nào của Module 3 cũng chỉ ra được ngay: URL nào -> Controller/Action nào -> View nào -> Model/Bảng nào -> Kiểm tra điều kiện gì -> Xử lý sự cố ra sao.
> 2. **Phần VI — Mục 6:** Kịch bản thuyết trình miệng 3–5 phút mạch lạc, đúng trọng tâm.
> 3. **Phần VI — Mục 7:** Kịch bản thao tác Demo trực tiếp trên Website trong 5–10 phút theo từng bước cụ thể.

---

## PHẦN II — NHÓM L: BẢNG ÁNH XẠ CHI TIẾT TỪNG MÀN HÌNH GIAO DIỆN VỚI CODE & CSDL

| Màn hình / Thao tác trên Web | URL & HTTP Method | Controller & Action xử lý | File View (`.cshtml`) | Entity / Bảng CSDL đọc & ghi | Dữ liệu kiểm tra (Validation / Rule) | Cách giải thích ngắn gọn khi đang mở màn hình này |
|---|---|---|---|---|---|---|
| **1. Xem Hồ sơ cá nhân & Sổ tay sức khỏe** | `GET /NguoiHienMau/HoSoCaNhan` | `NguoiHienMauController.HoSoCaNhan()` (dòng 48–101) | `Views/NguoiHienMau/HoSoCaNhan.cshtml` | **Đọc:** `NguoiHienMau`, `TaiKhoan`, `PhieuDangKyHienMau`, `DotHienMau`, `KetQuaHienMau`, `LichKhamSangLoc` | Kiểm tra `[Authorize]` và Claim `"MaTaiKhoan"`. Tính thống kê qua `DangKyController.EvaluateRecentDonationEligibility`. | *"Thưa cô, màn hình này đọc `MaTaiKhoan` từ Cookie, nạp hồ sơ `NguoiHienMau` kèm lịch sử phiếu và gọi hàm `EvaluateRecentDonationEligibility` để hiển thị số lần hiến thành công thực tế và đếm ngược chu kỳ 84 ngày."* |
| **2. Lưu cập nhật Hồ sơ cá nhân** | `POST /NguoiHienMau/HoSoCaNhan` | `NguoiHienMauController.HoSoCaNhan(NguoiHienMau model)` (dòng 103–282) | `Views/NguoiHienMau/HoSoCaNhan.cshtml` | **Ghi (INSERT / UPDATE):** `NguoiHienMau` và đồng bộ `HoTen` sang `TaiKhoan` | Họ tên (`<=100`), SĐT (`^0[0-9]{9,10}$`), Ngày sinh & Tuổi (`18–60`), Giới tính, Email, Địa chỉ, Cân nặng (`30–200kg`). | *"Khi em bấm Lưu, dữ liệu gửi qua POST kèm Anti-Forgery Token. Controller làm sạch SĐT bằng `NormalizePhone`, kiểm tra tuổi 18–60 và cân nặng 30–200kg rồi mới cập nhật vào bảng `NguoiHienMau` và đồng bộ `HoTen` sang bảng `TaiKhoan`."* |
| **3. Mở trang Đăng ký hiến máu theo đợt** | `GET /DangKy/Create?maDot=...` | `DangKyController.Create(int? maDot)` (dòng 207–310) | `Views/DangKy/Create.cshtml` | **Đọc:** `DotHienMau`, `DonViToChuc`, `NguoiHienMau`, `TaiKhoan`, `PhieuDangKyHienMau`, `KetQuaHienMau` | Kiểm tra đợt còn mở/còn chỉ tiêu, kiểm tra trước quy tắc 84 ngày và kiểm tra trùng phiếu trong đợt để gán `ViewBag.CoTheDangKy`. | *"Khi mở trang đăng ký, hàm GET tự động điền sẵn thông tin từ hồ sơ của em, đồng thời kiểm tra trước xem đợt còn chỉ tiêu không và em đã đủ 84 ngày từ lần hiến thành công gần nhất chưa để bật hoặc khóa nút Gửi phiếu."* |
| **4. Gửi phiếu đăng ký hiến máu** | `POST /DangKy/Create` | `DangKyController.Create(...)` (dòng 312–681) | `Views/DangKy/Create.cshtml` -> Redirect `/DangKy/TheoDoi` | **Ghi:** Thêm/sửa `NguoiHienMau` + Thêm mới `PhieuDangKyHienMau` (`TrangThai = "Chờ xác nhận"`) | Đợt hợp lệ, còn chỗ; Tuổi `18–60`, SĐT, Email, Cân nặng `>= CanNangToiThieu`; `health_q1 == "none"`, `health_q2 == "good"`; Đủ 84 ngày & không trùng phiếu trong Transaction `Serializable`. | *"Khi bấm Gửi phiếu, Server kiểm tra lại toàn bộ điều kiện y tế, khóa luồng theo `MaNguoiHien` bằng `SemaphoreSlim` và mở Transaction `Serializable` để kiểm tra lại quy tắc 84 ngày trước khi lưu phiếu mới với trạng thái `'Chờ xác nhận'`."* |
| **5. Theo dõi tiến trình phiếu & Lịch sử** | `GET /DangKy/TheoDoi` | `DangKyController.TheoDoi()` (dòng 685–721) | `Views/DangKy/TheoDoi.cshtml` | **Đọc:** `NguoiHienMau`, `PhieuDangKyHienMau`, `DotHienMau`, `LichKhamSangLoc`, `KetQuaHienMau` | Kiểm tra `[Authorize]` và Claim `"MaTaiKhoan"`. Lọc `activePhieu` vẽ Stepper 4 bước và bảng lịch sử. | *"Trang Theo dõi hiển thị phiếu đang hoạt động mới nhất của em trên thanh tiến trình 4 bước (Gửi phiếu -> Xác nhận & Lịch khám -> Khám sàng lọc -> Kết quả), đồng thời liệt kê toàn bộ lịch sử các phiếu cũ bên dưới."* |
| **6. Hủy phiếu đăng ký hiến máu** | `POST /DangKy/Huy` | `DangKyController.Huy(int id)` (dòng 725–781) | `Views/DangKy/TheoDoi.cshtml` (Modal xác nhận hủy) | **Ghi (UPDATE):** `PhieuDangKyHienMau` (`TrangThai = "Đã hủy"`, `NgayXuLy = DateTime.Now`) | Kiểm tra chủ sở hữu (`MaNguoiHien`), trạng thái thuộc `{"Chờ xác nhận", "Đã xác nhận"}`, chưa có lịch khám hoạt động, chưa có kết quả. | *"Em chỉ hủy được phiếu của chính mình khi đang ở trạng thái `'Chờ xác nhận'` hoặc `'Đã xác nhận'` và chưa có lịch khám sàng lọc. Khi hủy, hệ thống cập nhật trạng thái thành `'Đã hủy'` chứ không xóa mất bản ghi."* |
| **7. Quản lý danh sách Người hiến máu (Admin)** | `GET /Admin/NguoiHienMau` | `AdminController.NguoiHienMau(...)` (dòng 638–779) | `Views/Admin/NguoiHienMau.cshtml` | **Đọc:** `NguoiHienMau`, `TaiKhoan`, `PhieuDangKyHienMau`, `KetQuaHienMau` | Yêu cầu `[Authorize(Roles = "Admin,NhanVienYTe")]`. Hỗ trợ tìm kiếm, lọc nhóm máu, lọc trạng thái và phân trang. | *"Đây là giao diện dành cho Admin và Nhân viên y tế để tra cứu hồ sơ người hiến máu toàn hệ thống, lọc theo nhóm máu cần huy động và bấm xem lịch sử phiếu đăng ký của từng người."* |
| **8. Duyệt / Từ chối / Xử lý phiếu (Admin)** | `GET /Admin/PhieuDangKyHienMau`<br>`POST /Admin/XacNhanPhieu`<br>`POST /Admin/TuChoiPhieu` | `AdminController` (dòng 356–523) | `Views/Admin/PhieuDangKyHienMau.cshtml` & `ChiTietPhieuDangKy.cshtml` | **Ghi (UPDATE):** `PhieuDangKyHienMau` (`TrangThai`, `NgayXuLy`, `GhiChuXacNhan`) | Kiểm tra Role `Admin`/`NhanVienYTe` và kiểm tra luồng chuyển trạng thái hợp lệ. | *"Khi nhân viên y tế duyệt hoặc từ chối phiếu ở đây, trạng thái của `PhieuDangKyHienMau` được cập nhật và phản ánh ngay lập tức lên thanh tiến trình 4 bước bên trang `TheoDoi` của người hiến máu."* |

---

## PHẦN VI (MỤC 6) — KỊCH BẢN THUYẾT TRÌNH MODULE 3 TRONG 3–5 PHÚT

> **Lời khuyên:** Sinh viên hãy đọc trôi chảy kịch bản dưới đây (tốc độ vừa phải, khoảng 450–500 từ, tương đương đúng **3,5 – 4 phút** nói trước hội đồng):

### 🎙️ BÀI THUYẾT TRÌNH MẪU (3–5 PHÚT)

1. **Mở đầu & Giới thiệu vai trò (30 giây):**
   "Em chào cô và các thầy cô trong hội đồng chấm đồ án ạ. Em là **Đàm Thu Trang — Mã sinh viên 23103100243**. Trong đề tài *Xây dựng hệ thống Quản lý hiến máu vì cộng đồng* bằng ASP.NET Core MVC, em chịu trách nhiệm phân tích thiết kế và lập trình **Module 3: Quản lý người hiến máu và Phiếu đăng ký hiến máu**."

2. **Vị trí của Module 3 trong kiến trúc tổng thể (45 giây):**
   "Thưa cô, Module 3 đóng vai trò là cầu nối nghiệp vụ trung tâm của toàn bộ dự án:
   - Module của em nhận thông tin tài khoản đăng nhập từ **Module 1 (`TaiKhoan`)** và thông tin các chiến dịch hiến máu từ **Module 2 (`DotHienMau`)**.
   - Từ đó, Module 3 quản lý hồ sơ sức khỏe của người tình nguyện trong bảng **`NguoiHienMau`** và tiếp nhận đơn đăng ký tham gia chiến dịch trong bảng **`PhieuDangKyHienMau`**.
   - Tờ phiếu đăng ký do Module 3 tạo ra chính là dữ liệu đầu vào bắt buộc để **Module 4 (`LichKhamSangLoc`)** xếp lịch khám sàng lọc và **Module 5 (`KetQuaHienMau`)** ghi nhận kết quả hiến máu thực tế."

3. **Các chức năng chính đã hoàn thiện trong code (60 giây):**
   "Về mặt chức năng, em đã triển khai đầy đủ trên 2 Controller chính là `NguoiHienMauController` và `DangKyController` (cùng phần tra cứu quản trị trong `AdminController`):
   - **Thứ nhất — Quản lý hồ sơ cá nhân & Sổ tay sức khỏe (`HoSoCaNhan`):** Cho phép người hiến máu xem và cập nhật thông tin cá nhân, nhóm máu, cân nặng, tiền sử sức khỏe. Hệ thống tự động chuẩn hóa số điện thoại, kiểm tra độ tuổi từ 18 đến 60, cân nặng từ 30 đến 200kg và đồng bộ họ tên với bảng `TaiKhoan`.
   - **Thứ hai — Tạo phiếu đăng ký hiến máu trực tuyến (`DangKy/Create`):** Tự động điền thông tin từ hồ sơ, kiểm tra đợt hiến máu còn mở và còn chỉ tiêu, kiểm tra cân nặng đạt chuẩn tối thiểu của đợt và bảng câu hỏi sàng lọc sức khỏe ban đầu.
   - **Thứ ba — Theo dõi tiến trình 4 bước và Hủy phiếu (`DangKy/TheoDoi` & `DangKy/Huy`):** Hiển thị trực quan tiến trình xử lý phiếu qua 4 bước từ lúc gửi phiếu đến khi có kết quả, và cho phép người hiến tự hủy phiếu khi đang ở trạng thái `'Chờ xác nhận'` hoặc `'Đã xác nhận'` (chưa có lịch khám và chưa có kết quả)."

4. **Điểm nhấn kỹ thuật & nghiệp vụ cốt lõi nhất của Module 3 (60 giây):**
   "Thưa cô, có 2 điểm kỹ thuật quan trọng nhất mà em đã tập trung xử lý sâu trong Module 3:
   - **Một là quy tắc kiểm tra thời gian chờ 84 ngày (12 tuần) giữa 2 lần hiến máu:** Thay vì lấy ngày nộp phiếu đăng ký (`NgayNop`), em xây dựng bộ 3 phương thức `IsConfirmedSuccessfulDonation`, `GetActualDonationDate` và `EvaluateRecentDonationEligibility` trong `DangKyController`. Hệ thống chỉ tính những phiếu đã có kết quả `'Hiến máu thành công'` và lấy ngày thực tế tại cột `KetQuaHienMau.NgayCapNhat` để cộng 84 ngày. Những phiếu bị hủy, bị từ chối hoặc khám sàng lọc không đủ điều kiện đều không bị khóa 84 ngày. Đặc biệt, nếu dữ liệu lịch sử có phiếu thành công nhưng bị khuyết ngày kết quả, hệ thống sẽ chặn an toàn chứ không tự ý lấy `NgayNop`.
   - **Hai là cơ chế chống gửi trùng phiếu đồng thời (Concurrency Control):** Em kết hợp khóa `SemaphoreSlim` theo từng `MaNguoiHien` (`_submissionLocks`) với Database Transaction ở mức cô lập `IsolationLevel.Serializable` để đảm bảo dù người dùng bấm đúp chuột gửi 2 request cùng lúc cũng chỉ tạo ra đúng 1 phiếu duy nhất."

5. **Kết luận & Chuyển sang phần Demo (15 giây):**
   "Sau đây, em xin phép được thao tác demo trực tiếp trên giao diện website và mã nguồn để minh họa rõ toàn bộ luồng nghiệp vụ của Module 3 ạ."

---

## PHẦN VI (MỤC 7) — KỊCH BẢN DEMO WEBSITE TRỰC TIẾP TRONG 5–10 PHÚT

### 🛠️ Chuẩn bị trước khi bước lên bàn Demo (2 phút trước giờ gọi tên)
1. Mở sẵn **Visual Studio** (hoặc VS Code) tại nhánh `DamThuTrang_23103100243`, mở sẵn 3 tab code:
   - Tab 1: `Controllers/DangKyController.cs` (cuộn sẵn tới dòng 23–178).
   - Tab 2: `Controllers/NguoiHienMauController.cs`.
   - Tab 3: `Data/ApplicationDbContext.cs`.
2. Khởi chạy ứng dụng (`dotnet run` hoặc nhấn `F5` trong Visual Studio). Khi `Program.cs` chạy, `DbInitializer.Initialize(context)` sẽ tự động khởi tạo CSDL và nạp dữ liệu mẫu (nếu DB trống).
3. Mở trình duyệt với **2 cửa sổ** (1 cửa sổ thường đăng nhập tài khoản **Người hiến máu**, 1 cửa sổ ẩn danh/Incognito đăng nhập sẵn tài khoản **Admin** để phối hợp duyệt phiếu/ghi kết quả cực nhanh).

---

### 🎬 Trình tự 6 Bước Demo Mượt Mà (5–10 Phút)

#### Bước 1: Xem & Cập nhật Hồ sơ cá nhân + Kiểm tra Validation (1,5 phút)
1. **Thao tác:** Trên cửa sổ Người hiến máu, bấm vào menu **"Hồ sơ cá nhân"** (`/NguoiHienMau/HoSoCaNhan`).
2. **Chỉ trên màn hình:** Chỉ cho giảng viên thấy thẻ thông tin người hiến, nhóm máu, số lần hiến máu thành công thực tế và trạng thái chu kỳ 84 ngày.
3. **Thử nhập sai để chứng minh Validation:**
   - Sửa cân nặng thành `25` kg hoặc sửa ngày sinh thành năm `2015` (chưa đủ 18 tuổi) rồi bấm **"Lưu thay đổi"**.
   - Chỉ ra thông báo lỗi kiểm tra hợp lệ.
4. **Nhập đúng và Lưu:**
   - Nhập cân nặng `58` kg, ngày sinh `15/05/2003`, SĐT `0912.345.678` (có dấu chấm để minh họa hàm `NormalizePhone`), bấm **"Lưu thay đổi"**.
   - Chỉ ra thông báo xanh *"Cập nhật hồ sơ cá nhân và sổ tay sức khỏe thành công!"* và SĐT đã được tự động làm sạch thành `0912345678`.

#### Bước 2: Tạo phiếu đăng ký hiến máu & Giải thích Auto-fill (1,5 phút)
1. **Thao tác:** Bấm vào menu **"Đợt hiến máu"** -> chọn một đợt đang mở -> bấm **"Đăng ký tham gia"** để vào `/DangKy/Create?maDot=...`.
2. **Chỉ trên màn hình:**
   - Thông tin chiến dịch, địa điểm, thanh tiến độ số lượng đã đăng ký (`soLuongDaDangKy / SoLuongToiDa`) và cân nặng tối thiểu (`45 kg`).
   - Toàn bộ Họ tên, Ngày sinh, SĐT, Email, Địa chỉ, Cân nặng vừa lưu ở Bước 1 đã được tự động điền sẵn vào form.
3. **Điền thông tin phiếu:**
   - Chọn dung tích hiến dự kiến: **`350ml`**.
   - Chọn khung giờ hẹn: **`Ca 1 (07:30 - 09:30)`**.
   - Ở mục Khảo sát sức khỏe: Chọn **`Không mắc bệnh nào kể trên`** (`health_q1 = "none"`) và **`Hoàn toàn khỏe mạnh`** (`health_q2 = "good"`).
   - Bấm **"Gửi phiếu đăng ký"**.

#### Bước 3: Theo dõi tiến trình phiếu & Thử đăng ký trùng trong cùng đợt (1,5 phút)
1. **Thao tác:** Sau khi gửi phiếu, hệ thống tự động chuyển hướng sang `/DangKy/TheoDoi`.
2. **Chỉ trên màn hình:**
   - Mã phiếu `#DKHM-xxxxx` vừa tạo nằm ở khung **"Phiếu đăng ký đang hoạt động"** với trạng thái **`"Chờ xác nhận"`** (Bước 1 trên thanh Stepper 4 bước sáng lên).
   - Dung tích dự kiến `350ml` và khung giờ `Ca 1` hiển thị rõ ràng.
3. **Chứng minh chặn đăng ký trùng trong đợt:**
   - Quay lại `/DangKy/Create` của chính đợt đó: Chỉ cho giảng viên thấy hộp cảnh báo đỏ *"Bạn đã có phiếu đăng ký (#DKHM-xxxxx) ở trạng thái 'Chờ xác nhận' cho đợt hiến máu này"* và nút Submit đã bị khóa (`disabled`).

#### Bước 4: Demo Hủy phiếu đăng ký và Tạo lại phiếu mới (1,5 phút)
1. **Thao tác:** Quay lại `/DangKy/TheoDoi`, bấm nút **"Hủy đăng ký"** -> Xác nhận hủy trong Modal.
2. **Chỉ trên màn hình:**
   - Thông báo *"Đã hủy phiếu đăng ký thành công"*.
   - Phiếu vừa hủy chuyển xuống bảng **Lịch sử đăng ký** bên dưới với nhãn **`"Đã hủy"`**.
3. **Chứng minh hủy phiếu không bị tính 84 ngày:**
   - Bấm lại vào `/DangKy/Create`, tạo lại 1 phiếu mới cho đợt đó -> **Tạo lại thành công ngay lập tức** vì phiếu `"Đã hủy"` không phải là lần hiến máu thành công!

#### Bước 5: Phối hợp với cửa sổ Admin để minh họa Vòng đời phiếu & Kích hoạt quy tắc 84 ngày (2 phút)
1. **Thao tác trên cửa sổ Admin (`/Admin/PhieuDangKyHienMau`):**
   - Tìm phiếu mới vừa tạo ở Bước 4 -> Bấm vào **"Chi tiết"** (`/Admin/ChiTietPhieuDangKy/{id}`).
   - Bấm **"Xác nhận phiếu"** -> trạng thái lên `"Đã xác nhận"`.
   - Tạo lịch khám sàng lọc và bấm **"Hoàn thành khám"** -> trạng thái lên `"Đã khám sàng lọc"` (trên trang `TheoDoi` của người hiến, Stepper nhảy sang Bước 3 và nút Hủy phiếu tự động biến mất!).
   - Bấm **"Ghi nhận kết quả"** -> chọn **`"Hiến máu thành công"`** và Lưu.
2. **Quay lại cửa sổ Người hiến máu (Điểm ăn điểm tuyệt đối):**
   - Tải lại trang `/DangKy/TheoDoi`: Phiếu đã hoàn tất với trạng thái **`"Hiến máu thành công"`**, đồng thời xuất hiện thông báo cảnh báo chu kỳ nghỉ **84 ngày** tính từ ngày hôm nay (`KetQuaHienMau.NgayCapNhat`).
   - Bấm sang `/DangKy/Create` (thử chọn bất kỳ đợt hiến máu nào khác đang mở): Hệ thống **chặn hoàn toàn** trên mọi đợt hiến máu vì mới hiến máu thành công hôm nay, còn phải chờ đủ **84 ngày**!

---

### 🛟 Cách xử lý nhanh nếu gặp sự cố bất ngờ khi đang Demo
1. **Sự cố 1: Bấm vào `/DangKy/Create` nhưng báo *"Hiện tại không có đợt hiến máu nào đang mở"*:**
   - *Nguyên nhân:* Dữ liệu mẫu của bảng `DotHienMau` có ngày `KetThuc` nhỏ hơn ngày hiện tại của máy tính.
   - *Cách xử lý trong 15 giây:* Chuyển sang cửa sổ **Admin** -> vào menu **"Quản lý đợt hiến máu"** -> tạo nhanh 1 đợt hiến máu mới có trạng thái `"Đang mở"` và ngày `KetThuc` ở tháng sau.
2. **Sự cố 2: Tài khoản mẫu đang đăng nhập đã bị khóa 84 ngày sẵn từ trước nên không bấm tạo phiếu được:**
   - *Cách xử lý trong 15 giây:* Bấm **"Đăng ký tài khoản mới"** (`/Account/Register`) tạo nhanh 1 tài khoản mới tinh để demo luồng người hiến máu mới, còn tài khoản cũ dùng luôn để chỉ cho giảng viên thấy giao diện chặn 84 ngày!
