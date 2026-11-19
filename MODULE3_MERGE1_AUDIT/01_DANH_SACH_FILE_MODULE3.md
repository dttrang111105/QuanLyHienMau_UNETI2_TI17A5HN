# 01. DANH SÁCH TOÀN BỘ FILE THUỘC MODULE 3 (NHÁNH `merge1`)

Dưới đây là danh sách toàn bộ các file thuộc phạm vi **Module 3 (Quản lý Người hiến máu & Phiếu đăng ký hiến máu phía Người hiến máu + Quản lý Người hiến máu phía Admin)** được xác định trực tiếp từ cấu trúc source code thực tế tại commit `0b0d7d6926094f398cdbfd88e18658570650be63` (`origin/merge1`).

---

## 1. Nhóm File Cốt lõi của Module 3 (Core Module 3 Files)

### 1.1. Controllers (Tầng điều khiển nghiệp vụ)
| STT | Đường dẫn File | Số dòng | Vai trò & Nghiệp vụ trong Module 3 |
| :---: | :--- | :---: | :--- |
| 1 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs` | 261 | Quản lý hồ sơ cá nhân người hiến máu (`GET /NguoiHienMau`, `GET /NguoiHienMau/HoSoCaNhan`, `POST /NguoiHienMau/HoSoCaNhan`), tự động khởi tạo hồ sơ lần đầu, kiểm tra dữ liệu đầu vào, chống overposting và đồng bộ `HoTen`, `Email` với bảng `TaiKhoan`. |
| 2 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs` | 760 | Quản lý toàn bộ luồng đăng ký hiến máu của người hiến máu: hiển thị form đăng ký (`GET /DangKy/Create`), xử lý nộp phiếu (`POST /DangKy/Create`), kiểm tra điều kiện đợt hiến máu, tuổi (18–60), cân nặng tối thiểu, câu hỏi sức khỏe, thời gian chờ 84 ngày kể từ lần hiến máu thành công gần nhất (`EvaluateRecentDonationEligibility`), chống đăng ký trùng đợt với `SemaphoreSlim` + `Serializable` transaction, theo dõi phiếu (`GET /DangKy/TheoDoi`) và hủy phiếu (`POST /DangKy/Huy`). |
| 3 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs` *(Action `NguoiHienMau`, dòng 648–832)* | 185 *(trên tổng 1648)* | Quản lý danh sách người hiến máu phía Admin/Medical Staff (`GET /Admin/NguoiHienMau`): tìm kiếm đa tiêu chí, lọc theo nhóm máu ABO, yếu tố Rh, trạng thái hiến máu (`eligible`/`waiting`), mức độ đóng góp (`new`/`regular`/`frequent`), phân trang và tính toán 4 chỉ số KPI người hiến máu. |

### 1.2. Repositories & Interfaces (Tầng truy xuất dữ liệu của Module 3)
| STT | Đường dẫn File | Số dòng | Vai trò & Nghiệp vụ trong Module 3 |
| :---: | :--- | :---: | :--- |
| 4 | `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/IRepository.cs` | 27 | Interface Generic Repository (`QuanLyHienMau_UNETI2_TI17A5HN.Repositories.Interfaces.IRepository<T>`) định nghĩa các phương thức CRUD nền tảng cho Module 3. |
| 5 | `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/INguoiHienMauRepository.cs` | 46 | Interface Repository chuyên biệt cho thực thể `NguoiHienMau`: truy vấn hồ sơ theo `MaTaiKhoan`, nạp kèm lịch sử `PhieuDangKyHienMaus` -> `DotHienMau` / `KetQuaHienMau`, kiểm tra tài khoản và kiểm tra trùng lặp Email. |
| 6 | `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/IPhieuDangKyRepository.cs` | 57 | Interface Repository chuyên biệt cho thực thể `PhieuDangKyHienMau` (cổng người hiến máu): lấy đợt hiến máu đang mở, lấy lịch sử phiếu của người hiến máu, nạp chi tiết theo dõi phiếu, lấy phiếu để hủy và quản lý Transaction `Serializable`. |
| 7 | `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Repository.cs` | 74 | Lớp triển khai Generic Repository (`Repository<T>`) sử dụng `ApplicationDbContext`. |
| 8 | `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/NguoiHienMauRepository.cs` | 77 | Lớp triển khai `INguoiHienMauRepository` phục vụ `NguoiHienMauController` và `DangKyController`. |
| 9 | `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs` | 96 | Lớp triển khai `IPhieuDangKyRepository` phục vụ `DangKyController`. |

### 1.3. Models / Entities (Tầng thực thể dữ liệu của Module 3)
| STT | Đường dẫn File | Số dòng | Vai trò & Nghiệp vụ trong Module 3 |
| :---: | :--- | :---: | :--- |
| 10 | `QuanLyHienMau_UNETI2_TI17A5HN/Models/NguoiHienMau.cs` | 65 | Entity ánh xạ bảng `NguoiHienMau` (PK: `MaNguoiHien`, FK 1–1 tùy chọn: `MaTaiKhoan`, thông tin cá nhân, nhóm máu, cân nặng, trạng thái và tập hợp `PhieuDangKyHienMaus`). |
| 11 | `QuanLyHienMau_UNETI2_TI17A5HN/Models/PhieuDangKyHienMau.cs` | 55 | Entity ánh xạ bảng `PhieuDangKyHienMau` (PK: `MaPhieu`, FK: `MaNguoiHien`, `MaDot`, `NgayNop`, `TrangThai`, `LyDoHienMau`, `GhiChu`, `NgayXuLy`, `GhiChuXacNhan`, liên kết tới `LichKhamSangLocs` và `KetQuaHienMau`). |

### 1.4. Views (Tầng giao diện Razor của Module 3)
| STT | Đường dẫn File | Số dòng | Vai trò & Nghiệp vụ trong Module 3 |
| :---: | :--- | :---: | :--- |
| 12 | `QuanLyHienMau_UNETI2_TI17A5HN/Views/NguoiHienMau/HoSoCaNhan.cshtml` | 866 | Giao diện Hồ sơ cá nhân người hiến máu: hiển thị thẻ người hiến máu điện tử, KPI cá nhân (tổng lần hiến, thể tích máu, đếm ngược 84 ngày), form cập nhật thông tin cá nhân và bảng lịch sử đăng ký/hiến máu. |
| 13 | `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/Create.cshtml` | 495 | Giao diện Nộp phiếu đăng ký hiến máu: hiển thị thông tin đợt hiến máu, cảnh báo thời gian chờ 84 ngày (nếu có), form xác nhận thông tin cá nhân, chọn nhóm máu, dung tích, khung giờ và khảo sát tự sàng lọc sức khỏe (`health_q1`, `health_q2`). |
| 14 | `QuanLyHienMau_UNETI2_TI17A5HN/Views/DangKy/TheoDoi.cshtml` | 655 | Giao diện Theo dõi trạng thái phiếu đăng ký hiến máu: tra cứu theo mã phiếu `#DKHM-xxxxx`, thanh tiến trình 4 bước, chi tiết lịch khám sàng lọc, kết quả lâm sàng, nút hủy phiếu trực tuyến và danh sách toàn bộ phiếu của người hiến máu. |
| 15 | `QuanLyHienMau_UNETI2_TI17A5HN/Views/Admin/NguoiHienMau.cshtml` | 745 | Giao diện Quản lý danh sách người hiến máu phía Admin: hiển thị 4 thẻ KPI thống kê, bộ lọc nâng cao (Từ khóa, Nhóm máu ABO, Yếu tố Rh, Trạng thái đủ điều kiện/chờ 84 ngày, Mức độ đóng góp), bảng danh sách người hiến máu và phân trang. |

---

## 2. Nhóm File Dùng chung & Liên kết Tích hợp với Module 3 (Shared & Integrated Files)

Các file dưới đây thuộc hạ tầng dùng chung hoặc thuộc các Module 1, 2, 4, 5 nhưng có tương tác trực tiếp với dữ liệu và luồng nghiệp vụ của Module 3:

| STT | Đường dẫn File | Module liên quan | Mối liên hệ tích hợp với Module 3 |
| :---: | :--- | :---: | :--- |
| 1 | `QuanLyHienMau_UNETI2_TI17A5HN/Program.cs` | Hạ tầng chung | Đăng ký Dependency Injection cho `INguoiHienMauRepository`, `IPhieuDangKyRepository`; cấu hình Cookie Authentication kèm sự kiện `OnValidatePrincipal` kiểm tra khóa tài khoản tức thời; kích hoạt Migration và Seed dữ liệu. |
| 2 | `QuanLyHienMau_UNETI2_TI17A5HN/Data/ApplicationDbContext.cs` | Hạ tầng Database | Cấu hình Fluent API cho `NguoiHienMau` (1–1 với `TaiKhoan`, `DeleteBehavior.SetNull`), `PhieuDangKyHienMau` (`DeleteBehavior.Restrict` với `NguoiHienMau` và `DotHienMau`). |
| 3 | `QuanLyHienMau_UNETI2_TI17A5HN/Data/DbInitializer.cs` | Hạ tầng Database | Khởi tạo tài khoản mẫu (`donor`), hồ sơ `NguoiHienMau` mẫu và 8 phiếu đăng ký mẫu (`SeedTestData`). |
| 4 | `QuanLyHienMau_UNETI2_TI17A5HN/Migrations/20260405134900_InitialCreate.cs` | Hạ tầng Database | Migration khởi tạo cấu trúc bảng `NguoiHienMau`, `PhieuDangKyHienMau` và các ràng buộc khóa chính/khóa ngoại. |
| 5 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AccountController.cs` | Module 1 (Tài khoản) | Khi đăng ký tài khoản mới (`POST /Account/Register`), tự động tạo luôn bản ghi `NguoiHienMau` gắn với `TaiKhoan` vừa tạo. |
| 6 | `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/TaiKhoanRepository.cs` | Module 1 (Tài khoản) | Phương thức `ToggleStatusAsync` (dòng 164–179) khi khóa/mở khóa tài khoản sẽ tự động đồng bộ `NguoiHienMau.TrangThai = newStatus`. |
| 7 | `QuanLyHienMau_UNETI2_TI17A5HN/Controllers/AdminController.cs` (`NhanSu`, `KhoaMoKhoaTaiKhoan`, `Dashboard`) | Module 1 & 4 | Khóa/mở khóa tài khoản & hồ sơ người hiến máu (`POST /Admin/KhoaMoKhoaTaiKhoan`), xử lý duyệt/từ chối phiếu đăng ký và hiển thị tổng số người hiến máu trên Dashboard. |
| 8 | `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/IPhieuDangKyRepository.cs` & `EfPhieuDangKyRepository.cs` | Module 4 (Admin duyệt phiếu) | Cặp Repository thứ hai cho `PhieuDangKyHienMau` được Module 4 sử dụng trong `AdminController` (tồn tại song song với `Repositories/Interfaces/IPhieuDangKyRepository.cs` của Module 3). |
| 9 | `QuanLyHienMau_UNETI2_TI17A5HN/Views/DotHienMau/Index.cshtml` & `Details.cshtml` | Module 2 (Đợt hiến máu) | Chứa các nút `"Đăng ký hiến máu"` điều hướng sang `/DangKy/Create?maDot=...` và thanh điều hướng sang `/DangKy/TheoDoi`, `/NguoiHienMau/HoSoCaNhan`. |
| 10 | `QuanLyHienMau_UNETI2_TI17A5HN/Views/Shared/_Layout.cshtml` & `_AdminLayout.cshtml` | Giao diện chung | Thanh điều hướng (Navbar/Sidebar) liên kết trực tiếp tới các chức năng của Module 3. |
