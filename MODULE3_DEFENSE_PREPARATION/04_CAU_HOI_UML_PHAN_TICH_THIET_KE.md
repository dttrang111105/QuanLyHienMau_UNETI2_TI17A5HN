# PHẦN II (NHÓM G): CÂU HỎI VỀ PHÂN TÍCH THIẾT KẾ HỆ THỐNG VÀ BIỂU ĐỒ UML CỦA MODULE 3

> **Lưu ý xác minh tài liệu trong Repository:**
> Qua khảo sát toàn bộ cây thư mục của repository `QuanLyHienMau_UNETI2_TI17A5HN`, dự án hiện **không lưu trữ các file biểu đồ UML gốc** (như `.drawio`, `.asta`, `.vpp`, `.puml`, `.docx`). Do đó, toàn bộ các biểu đồ UML dưới đây (**Use Case Diagram, Activity Diagrams, Sequence Diagrams, Class Diagram**) là **Biểu đồ tham khảo chuẩn xác 100% được trích xuất ngược (Reverse-Engineered) trực tiếp từ mã nguồn thực tế** trên nhánh `DamThuTrang_23103100243`. Sinh viên có thể sử dụng trực tiếp bộ biểu đồ này để đưa vào slide/báo cáo hoặc trả lời câu hỏi bảo vệ môn Phân tích thiết kế hệ thống.

---

## 1. XÁC ĐỊNH TÁC NHÂN (ACTORS) VÀ CA SỬ DỤNG (USE CASES) CỦA MODULE 3

### 1.1. Bảng phân tích các Tác nhân (Actors) tương tác với Module 3

| STT | Tác nhân (Actor) | Vai trò trong code (`VaiTro` / Auth) | Các Use Case thực hiện trong Module 3 |
|---|---|---|---|
| 1 | **Người hiến máu (Registered Donor)** | Đã đăng nhập (`[Authorize]`), thông thường `VaiTro == "NguoiHienMau"` | - UC01: Xem và cập nhật hồ sơ cá nhân (`NguoiHienMau/HoSoCaNhan`)<br>- UC02: Xem thống kê chu kỳ 84 ngày & lịch sử hiến máu (`HoSoCaNhan`, `TheoDoi`)<br>- UC03: Tạo phiếu đăng ký hiến máu (`DangKy/Create`)<br>- UC04: Theo dõi tiến trình phiếu đăng ký (`DangKy/TheoDoi`)<br>- UC05: Hủy phiếu đăng ký hiến máu (`DangKy/Huy`) |
| 2 | **Khách vãng lai (Guest Donor)** | Chưa đăng nhập (`!User.Identity.IsAuthenticated`) | - UC03: Tạo phiếu đăng ký hiến máu trực tuyến qua Số điện thoại (`DangKy/Create`) |
| 3 | **Nhân viên y tế / Quản trị viên (Admin / Medical Staff)** | Đã đăng nhập với `[Authorize(Roles = "Admin,NhanVienYTe")]` | - UC06: Tra cứu & lọc danh sách người hiến máu (`Admin/NguoiHienMau`)<br>- UC07: Quản lý & xem chi tiết phiếu đăng ký (`Admin/PhieuDangKyHienMau`, `ChiTietPhieuDangKy`)<br>- UC08: Duyệt / Từ chối / Chuyển phiếu sang chờ khám (`XacNhanPhieu`, `TuChoiPhieu`, `ChuyenChoKham`) |

---

### 1.2. Biểu đồ Ca sử dụng (Use Case Diagram) của Module 3

```mermaid
flowchart LR
    Guest["👤 Khách vãng lai\n(Chưa đăng nhập)"]
    Donor["🩸 Người hiến máu\n(Đã đăng nhập)"]
    Staff["🏥 Nhân viên y tế / Admin\n(Role: Admin, NhanVienYTe)"]

    subgraph System["HỆ THỐNG QUẢN LÝ HIẾN MÁU — MODULE 3"]
        UC1(["UC01: Xem & Cập nhật\nhồ sơ cá nhân"])
        UC2(["UC02: Xem lịch sử hiến máu\n& chu kỳ 84 ngày"])
        UC3(["UC03: Tạo phiếu đăng ký\nhiến máu theo đợt"])
        UC3_Sub1(["«include»\nKiểm tra điều kiện 84 ngày\n& sàng lọc ban đầu"])
        UC4(["UC04: Theo dõi tiến trình\nphiếu đăng ký (4 bước)"])
        UC5(["UC05: Hủy phiếu đăng ký\n(Chờ xác nhận / Đã xác nhận)"])
        UC6(["UC06: Tra cứu & lọc\ndanh sách người hiến máu"])
        UC7(["UC07: Xem danh sách &\nchi tiết phiếu đăng ký"])
        UC8(["UC08: Xác nhận / Từ chối /\nChuyển chờ khám phiếu"])
    end

    Guest --> UC3
    Donor --> UC1
    Donor --> UC2
    Donor --> UC3
    Donor --> UC4
    Donor --> UC5

    UC3 -. "«include»" .-> UC3_Sub1
    UC5 -. "«extend»\n(Khi phiếu hợp lệ)" .-> UC4

    Staff --> UC6
    Staff --> UC7
    Staff --> UC8
```

---

## 2. BIỂU ĐỒ HOẠT ĐỘNG (ACTIVITY DIAGRAMS) CỦA MODULE 3

### 2.1. Biểu đồ Hoạt động: Tạo phiếu đăng ký hiến máu (`DangKyController.Create`)

```mermaid
flowchart TD
    Start([Bắt đầu]) --> OpenForm["Người dùng mở trang GET /DangKy/Create?maDot=..."]
    OpenForm --> CheckCampGET{"Đợt hiến máu còn mở\nvà chưa đầy chỉ tiêu?"}
    CheckCampGET -- Không --> DisableUI["Hiển thị cảnh báo đỏ\n& Khóa nút Submit trên View"]
    CheckCampGET -- Có --> CheckAuthGET{"Đã đăng nhập?"}
    CheckAuthGET -- Có --> Check84GET["Truy vấn lịch sử phiếu +\nEvaluateRecentDonationEligibility (84 ngày)\n+ Kiểm tra trùng phiếu trong đợt"]
    Check84GET --> EligibleGET{"Đủ điều kiện?"}
    EligibleGET -- Không --> DisableUI
    EligibleGET -- Có --> FillForm["Tự động điền thông tin từ NguoiHienMau/TaiKhoan"]
    CheckAuthGET -- Không --> FillForm

    FillForm --> SubmitForm["Người dùng nhập thông tin, chọn dung tích, ca khám,\ntrả lời health_q1, health_q2 và gửi POST /DangKy/Create"]
    SubmitForm --> CheckCampPOST{"[Server] Đợt còn mở,\nchưa hết hạn & còn chỉ tiêu?"}
    CheckCampPOST -- Không --> ErrRedirect["Gán TempData['Error'] &\nRedirect về Create"]

    CheckCampPOST -- Có --> ValidateInput{"[Server] Validate Họ tên, SĐT,\nTuổi 18-60, Email, Cân nặng,\nhealth_q1=='none', health_q2=='good'?"}
    ValidateInput -- Sai --> ErrRedirect

    ValidateInput -- Đúng --> ResolveDonor["Tìm hoặc Tạo mới NguoiHienMau\n(SaveChangesAsync để có MaNguoiHien)"]
    ResolveDonor --> AcquireLock["Lấy khóa SemaphoreSlim theo MaNguoiHien\n& Mở Transaction IsolationLevel.Serializable"]
    AcquireLock --> ReCheckTx{"Kiểm tra lại trong Transaction:\n1. Đủ 84 ngày từ KetQuaHienMau.NgayCapNhat?\n2. Không thiếu ngày thực tế?\n3. Chưa có phiếu đang xử lý trong đợt?"}

    ReCheckTx -- Vi phạm --> RollbackTx["RollbackAsync() Transaction\n& Release SemaphoreSlim"]
    RollbackTx --> ErrRedirect

    ReCheckTx -- Hợp lệ --> SaveTicket["Cập nhật NguoiHienMau +\nTạo PhieuDangKyHienMau (TrangThai = 'Chờ xác nhận')\n+ SaveChangesAsync() & CommitAsync()"]
    SaveTicket --> ReleaseLock["Release SemaphoreSlim"]
    ReleaseLock --> RedirectSuccess["Gán TempData['Success'] &\nRedirect sang /DangKy/TheoDoi (hoặc DotHienMau/Details)"]
    RedirectSuccess --> End([Kết thúc])
    DisableUI --> End
    ErrRedirect --> End
```

---

### 2.2. Biểu đồ Hoạt động: Hủy phiếu đăng ký hiến máu (`DangKyController.Huy`)

```mermaid
flowchart TD
    Start([Bắt đầu]) --> ViewTheoDoi["Người hiến máu mở trang GET /DangKy/TheoDoi"]
    ViewTheoDoi --> ClickCancel["Bấm nút 'Hủy đăng ký' trên phiếu đang hoạt động\nvà xác nhận trong Modal"]
    ClickCancel --> PostHuy["Gửi POST /DangKy/Huy (id = MaPhieu, + AntiForgeryToken)"]
    PostHuy --> CheckCookie{"Có Claim MaTaiKhoan\nvà tồn tại NguoiHienMau?"}
    CheckCookie -- Không --> RedirectLogin["Redirect về /Account/Login hoặc /DangKy/TheoDoi"]

    CheckCookie -- Có --> QueryPhieu["Truy vấn PhieuDangKyHienMau kèm LichKhamSangLocs, KetQuaHienMau\ntheo (MaPhieu == id && MaNguoiHien == nguoiHienMau.MaNguoiHien)"]
    QueryPhieu --> CheckOwner{"Tìm thấy phiếu\nthuộc sở hữu?"}
    CheckOwner -- Không --> ErrOwner["TempData['Error']: Không tìm thấy hoặc không có quyền hủy"]

    CheckOwner -- Có --> CheckStatus{"TrangThai thuộc\n{'Chờ xác nhận', 'Đã xác nhận'}?"}
    CheckStatus -- Không --> ErrStatus["TempData['Error']: Không thể hủy ở trạng thái hiện tại"]

    CheckStatus -- Có --> CheckLichKham{"Có LichKhamSangLoc nào\nTrangThai != 'Đã hủy'?"}
    CheckLichKham -- Có --> ErrLich["TempData['Error']: Đã xếp lịch khám sàng lọc, không thể tự hủy"]

    CheckLichKham -- Không --> CheckKetQua{"KetQuaHienMau != null?"}
    CheckKetQua -- Có --> ErrKQ["TempData['Error']: Đã có kết quả hiến máu, không thể hủy"]

    CheckKetQua -- Không --> UpdateCancel["Cập nhật phieu.TrangThai = 'Đã hủy'\nphieu.NgayXuLy = DateTime.Now\nawait _context.SaveChangesAsync()"]
    UpdateCancel --> SuccessMsg["TempData['Success']: Đã hủy phiếu thành công"]

    ErrOwner --> RedirectBack["Redirect về /DangKy/TheoDoi"]
    ErrStatus --> RedirectBack
    ErrLich --> RedirectBack
    ErrKQ --> RedirectBack
    SuccessMsg --> RedirectBack
    RedirectLogin --> End([Kết thúc])
    RedirectBack --> End
```

---

### 2.3. Biểu đồ Hoạt động: Cập nhật Hồ sơ cá nhân (`NguoiHienMauController.HoSoCaNhan`)

```mermaid
flowchart TD
    Start([Bắt đầu]) --> GetProfile["Người hiến máu truy cập GET /NguoiHienMau/HoSoCaNhan"]
    GetProfile --> LoadDB["Đọc MaTaiKhoan từ Cookie -> Truy vấn NguoiHienMau kèm TaiKhoan, PhieuDangKyHienMaus\n(Nếu chưa có trong DB thì khởi tạo tạm từ TaiKhoan)"]
    LoadDB --> CalcStats["Gọi DangKyController.EvaluateRecentDonationEligibility\nđể tính thống kê hiến máu & chu kỳ 84 ngày"]
    CalcStats --> EditForm["Người dùng chỉnh sửa thông tin cá nhân / y tế\nvà bấm Lưu (POST /NguoiHienMau/HoSoCaNhan)"]
    EditForm --> Normalize["Chuẩn hóa chuỗi (Trim) & làm sạch SĐT (NormalizePhone)"]
    Normalize --> Validate{"Kiểm tra hợp lệ:\n- Họ tên <= 100\n- SĐT ^0[0-9]{9,10}$\n- Ngày sinh & Tuổi 18-60\n- Giới tính Nam/Nữ/Khác\n- Email & Địa chỉ\n- Cân nặng 30-200kg?"}
    Validate -- Có lỗi (errors.Count > 0) --> ReturnView["Gán TempData['Error'] = string.Join(' ', errors)\nNạp lại ViewBag thống kê & return View(model)"]
    Validate -- Hợp lệ (errors.Count == 0) --> CheckExist{"Đã có bản ghi NguoiHienMau\ntrong DB?"}
    CheckExist -- Chưa có --> InsertNew["Khởi tạo NguoiHienMau mới (MaTaiKhoan, TrangThai='Hoạt động')\n_context.Add(nguoiHienMau)"]
    CheckExist -- Đã có --> UpdateTracked["Cập nhật 10 trường vào thực thể NguoiHienMau\n+ Đồng bộ nguoiHienMau.TaiKhoan.HoTen = model.HoTen"]
    InsertNew --> SaveDB["await _context.SaveChangesAsync()\nTempData['Success'] + RedirectToAction('HoSoCaNhan')"]
    UpdateTracked --> SaveDB
    ReturnView --> End([Kết thúc])
    SaveDB --> End
```

---

## 3. BIỂU ĐỒ TUẦN TỰ (SEQUENCE DIAGRAMS) CỦA MODULE 3

### 3.1. Biểu đồ Tuần tự: Theo dõi phiếu đăng ký (`GET /DangKy/TheoDoi`)

```mermaid
sequenceDiagram
    actor Donor as Người hiến máu
    participant Browser as Trình duyệt (TheoDoi.cshtml)
    participant Ctrl as DangKyController
    participant Helper as EvaluateRecentDonationEligibility
    participant DB as ApplicationDbContext (SQL Server)

    Donor->>Browser: Bấm menu "Theo dõi đăng ký"
    Browser->>Ctrl: GET /DangKy/TheoDoi (kèm Auth Cookie)
    Ctrl->>Ctrl: User.FindFirst("MaTaiKhoan")?.Value
    Ctrl->>DB: NguoiHienMaus.FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan)
    DB-->>Ctrl: Trả về nguoiHienMau
    Ctrl->>DB: PhieuDangKyHienMaus.Include(DotHienMau, LichKhamSangLocs, KetQuaHienMau)<br/>.Where(p => p.MaNguoiHien == nguoiHienMau.MaNguoiHien)<br/>.OrderByDescending(NgayNop).ThenByDescending(MaPhieu).ToListAsync()
    DB-->>Ctrl: Trả về danhSachPhieu
    Ctrl->>Helper: EvaluateRecentDonationEligibility(danhSachPhieu)
    Helper-->>Ctrl: Trả về RecentDonationEligibilityResult
    Ctrl->>Ctrl: ViewBag.EligibilityResult = eligibility
    Ctrl-->>Browser: Render Views/DangKy/TheoDoi.cshtml (activePhieu + Stepper 4 bước + Lịch sử)
    Browser-->>Donor: Hiển thị tiến trình phiếu & trạng thái chu kỳ 84 ngày
```

---

### 3.2. Biểu đồ Tuần tự: Hủy phiếu đăng ký (`POST /DangKy/Huy`)

```mermaid
sequenceDiagram
    actor Donor as Người hiến máu
    participant View as Views/DangKy/TheoDoi.cshtml
    participant Ctrl as DangKyController
    participant DB as ApplicationDbContext (SQL Server)

    Donor->>View: Bấm "Hủy đăng ký" -> Xác nhận trong Modal
    View->>Ctrl: POST /DangKy/Huy (id = MaPhieu, __RequestVerificationToken)
    Ctrl->>Ctrl: Đọc Claim "MaTaiKhoan" từ Cookie
    Ctrl->>DB: NguoiHienMaus.FirstOrDefaultAsync(n => n.MaTaiKhoan == maTaiKhoan)
    DB-->>Ctrl: Trả về nguoiHienMau
    Ctrl->>DB: PhieuDangKyHienMaus.Include(LichKhamSangLocs).Include(KetQuaHienMau)<br/>.FirstOrDefaultAsync(p => p.MaPhieu == id && p.MaNguoiHien == nguoiHienMau.MaNguoiHien)
    DB-->>Ctrl: Trả về phieu
    Ctrl->>Ctrl: Kiểm tra: TrangThai in {"Chờ xác nhận", "Đã xác nhận"}<br/>&& !LichKhamSangLocs.Any(TrangThai != "Đã hủy")<br/>&& KetQuaHienMau == null
    alt Không thỏa mãn điều kiện hủy
        Ctrl-->>View: TempData["Error"] + RedirectToAction("TheoDoi")
    else Đủ điều kiện hủy
        Ctrl->>Ctrl: phieu.TrangThai = "Đã hủy"<br/>phieu.NgayXuLy = DateTime.Now
        Ctrl->>DB: await _context.SaveChangesAsync()
        DB-->>Ctrl: Lưu thành công (1 row affected)
        Ctrl-->>View: TempData["Success"] + RedirectToAction("TheoDoi")
    end
```

---

## 4. BIỂU ĐỒ LỚP (CLASS DIAGRAM) CỦA MODULE 3 VÀ CÁC LỚP LIÊN QUAN

```mermaid
classDiagram
    class NguoiHienMau {
        +int MaNguoiHien
        +int? MaTaiKhoan
        +string HoTen
        +DateTime? NgaySinh
        +string? GioiTinh
        +string? SoDienThoai
        +string? Email
        +string? DiaChi
        +string? NhomMau
        +string? NgheNghiep
        +double? CanNang
        +string? GhiChuSucKhoe
        +string TrangThai
        +TaiKhoan? TaiKhoan
        +ICollection~PhieuDangKyHienMau~ PhieuDangKyHienMaus
    }

    class PhieuDangKyHienMau {
        +int MaPhieu
        +int MaNguoiHien
        +int MaDot
        +DateTime NgayNop
        +string? LyDoHienMau
        +string? GhiChu
        +string TrangThai
        +DateTime? NgayXuLy
        +string? GhiChuXacNhan
        +NguoiHienMau? NguoiHienMau
        +DotHienMau? DotHienMau
        +ICollection~LichKhamSangLoc~ LichKhamSangLocs
        +KetQuaHienMau? KetQuaHienMau
    }

    class TaiKhoan {
        +int MaTaiKhoan
        +string TenDangNhap
        +string MatKhau
        +string HoTen
        +string? Email
        +string? SoDienThoai
        +string VaiTro
        +bool TrangThai
        +NguoiHienMau? NguoiHienMau
    }

    class DotHienMau {
        +int MaDot
        +int? MaDonVi
        +string TenDot
        +DateTime BatDau
        +DateTime KetThuc
        +string DiaDiem
        +int SoLuongToiDa
        +double CanNangToiThieu
        +int DoTuoiToiThieu
        +int DoTuoiToiDa
        +string TrangThai
    }

    class LichKhamSangLoc {
        +int MaLichKham
        +int MaPhieu
        +DateTime ThoiGianKham
        +string? DiaDiemKham
        +string? BacSiPhuTrach
        +string KetQuaSangLoc
        +string TrangThai
    }

    class KetQuaHienMau {
        +int MaKetQua
        +int MaPhieu
        +string KetQua
        +DateTime? NgayCapNhat
        +string? NhanVienXacNhan
        +string? GhiChu
    }

    class DangKyController {
        +const int SoNgayChoToiThieuGiuaHaiLanHien = 84
        -ApplicationDbContext _context
        -ConcurrentDictionary~int, SemaphoreSlim~ _submissionLocks$
        +IsConfirmedSuccessfulDonation(PhieuDangKyHienMau phieu)$ bool
        +GetActualDonationDate(PhieuDangKyHienMau phieu)$ DateTime?
        +EvaluateRecentDonationEligibility(IEnumerable~PhieuDangKyHienMau~ lichSuPhieu, DateTime? referenceDate)$ RecentDonationEligibilityResult
        +Create(int? maDot) Task~IActionResult~
        +Create(int maDot, string hoTen, ...) Task~IActionResult~
        +TheoDoi() Task~IActionResult~
        +Huy(int id) Task~IActionResult~
    }

    class NguoiHienMauController {
        -ApplicationDbContext _context
        +HoSoCaNhan() Task~IActionResult~
        +HoSoCaNhan(NguoiHienMau model) Task~IActionResult~
    }

    TaiKhoan "1" --> "0..1" NguoiHienMau : liên kết
    NguoiHienMau "1" --> "0..*" PhieuDangKyHienMau : đăng ký
    DotHienMau "1" --> "0..*" PhieuDangKyHienMau : thuộc đợt
    PhieuDangKyHienMau "1" --> "0..*" LichKhamSangLoc : có lịch khám
    PhieuDangKyHienMau "1" --> "0..1" KetQuaHienMau : có kết quả
    DangKyController ..> PhieuDangKyHienMau : quản lý
    DangKyController ..> NguoiHienMau : cập nhật
    NguoiHienMauController ..> NguoiHienMau : quản lý
    NguoiHienMauController ..> DangKyController : gọi hàm 84 ngày
```

---

## 5. CÂU HỎI BẢO VỆ VỀ PHÂN TÍCH THIẾT KẾ & UML (NHÓM G)

### Câu G1: Trong biểu đồ Use Case của Module 3, tại sao mối quan hệ giữa Use Case "Tạo phiếu đăng ký hiến máu" và "Kiểm tra điều kiện 84 ngày & sàng lọc ban đầu" lại là `«include»` chứ không phải `«extend»`?
- **Mức độ:** Trung bình
- **Câu trả lời mẫu:**
  Thưa cô, trong UML:
  - Quan hệ **`«include»`** biểu thị một hành vi **bắt buộc phải được thực hiện** mỗi khi Use Case cơ sở được kích hoạt. Trong code `DangKyController.Create`, bất kỳ lần tạo phiếu nào cũng bắt buộc phải chạy qua bước kiểm tra điều kiện 84 ngày (`EvaluateRecentDonationEligibility`) và sàng lọc tuổi/cân nặng/khảo sát sức khỏe. Không có ngoại lệ nào tạo được phiếu mà bỏ qua bước này, nên đây là `«include»`.
  - Ngược lại, quan hệ **`«extend»`** dùng cho hành vi **tùy chọn có điều kiện** (ví dụ: người dùng đang ở Use Case *"Theo dõi tiến trình phiếu đăng ký"*, chỉ khi người dùng có nhu cầu hủy và phiếu đang ở trạng thái `"Chờ xác nhận"` / `"Đã xác nhận"` thì mới kích hoạt Use Case mở rộng *"Hủy phiếu đăng ký"*).

---

### Câu G2: Từ Biểu đồ Tuần tự (Sequence Diagram) và Biểu đồ Lớp (Class Diagram), nhóm đã ánh xạ sang kiến trúc ASP.NET Core MVC như thế nào?
- **Mức độ:** Trung bình / Khó
- **Câu trả lời mẫu:**
  Thưa cô, theo phương pháp phân tích thiết kế hướng đối tượng (OOAD) sử dụng mẫu **Boundary – Control – Entity (BCE)**, nhóm ánh xạ sang ASP.NET Core MVC theo tỷ lệ 1–1 như sau:
  1. **Lớp Biên (Boundary / Interface Classes)** trên Sequence Diagram -> Ánh xạ thành các **Razor Views (`.cshtml`)** trong thư mục `Views/DangKy/` (`Create.cshtml`, `TheoDoi.cshtml`) và `Views/NguoiHienMau/` (`HoSoCaNhan.cshtml`).
  2. **Lớp Điều khiển (Control Classes)** chứa các thông điệp xử lý luồng nghiệp vụ -> Ánh xạ thành các **Controllers** (`DangKyController.cs`, `NguoiHienMauController.cs`, `AdminController.cs`).
  3. **Lớp Thực thể (Entity Classes)** trên Class Diagram và ERD -> Ánh xạ thành các **Model classes** trong thư mục `Models/` (`NguoiHienMau.cs`, `PhieuDangKyHienMau.cs`, `KetQuaHienMau.cs`, `LichKhamSangLoc.cs`, `DotHienMau.cs`, `TaiKhoan.cs`) được quản lý bởi `ApplicationDbContext`.

---

### Câu G3: Giữa Biểu đồ Lớp phân tích (Analysis Class Diagram) và Biểu đồ Thực thể Liên kết vật lý (Physical ERD) của Module 3 có điểm gì khác nhau?
- **Mức độ:** Khó
- **Câu trả lời mẫu:**
  Thưa cô:
  - Trên **Biểu đồ Lớp (Class Diagram)** ở mức hướng đối tượng C#, các mối quan hệ giữa `NguoiHienMau`, `PhieuDangKyHienMau`, `KetQuaHienMau` được biểu diễn thông qua các **Navigation Properties** (thuộc tính điều hướng đối tượng như `public ICollection<PhieuDangKyHienMau> PhieuDangKyHienMaus { get; set; }` và `public KetQuaHienMau? KetQuaHienMau { get; set; }`), đồng thời có thêm các lớp điều khiển như `DangKyController` chứa phương thức hành vi (`EvaluateRecentDonationEligibility`, `Create`, `Huy`).
  - Còn trên **Sơ đồ ERD vật lý trong SQL Server**, không có các phương thức hay `ICollection<T>`, mà các mối quan hệ được hiện thực hóa bằng các cột **Khóa chính (`PK`)**, **Khóa ngoại (`FK`: `MaTaiKhoan`, `MaNguoiHien`, `MaDot`, `MaPhieu`)**, kiểu dữ liệu vật lý (`nvarchar(100)`, `datetime2`, `float`) và các **Chỉ mục (`Unique Filtered Index`)**.
