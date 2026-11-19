# BÁO CÁO FINAL AUDIT – KIỂM TRA ĐỘ AN TOÀN SAU KHI CHUYỂN MODULE 3 SANG REPOSITORY PATTERN

- **Dự án**: `QuanLyHienMau_UNETI2_TI17A5HN` (ASP.NET Core MVC .NET 10, Entity Framework Core 10, SQL Server / LocalDB)
- **Phạm vi kiểm tra**: Module 3 – Quản lý hồ sơ người hiến máu và Đăng ký hiến máu
- **Nhánh Git hiện tại**: `DamThuTrang_23103100243`
- **Phương pháp kiểm tra**: Đọc trực tiếp toàn bộ mã nguồn thực tế (6 file Repository/Interface, 2 Controller Module 3, `Program.cs`, bộ test `Module3RepoTests`), đối chiếu dòng-đối-dòng (`Compare-Object`) với bản sao lưu trước khi chuyển đổi (`backup_before_repo/*.bak`), và kiểm tra `git status` / `git diff`.

---

## 1. Kiểm tra Kiến trúc Repository Pattern

### 1.1. Đánh giá 6 file Repository và Interface

| STT | File | Số dòng | Vai trò & Đánh giá chi tiết |
| :--- | :--- | :---: | :--- |
| 1 | [IRepository.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/IRepository.cs#L1-L27) | 27 | Generic Interface `IRepository<T> where T : class`. Định nghĩa 9 phương thức chuẩn CRUD & truy vấn bất đồng bộ (`GetAllAsync`, `FindAsync`, `FirstOrDefaultAsync`, `AnyAsync`, `GetByIdAsync`, `AddAsync`, `Update`, `Remove`, `SaveChangesAsync`). |
| 2 | [Repository.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Repository.cs#L1-L74) | 74 | Generic Implementation `Repository<T> : IRepository<T>`. Giữ `protected readonly ApplicationDbContext _context` và `protected readonly DbSet<T> _dbSet` để các Specific Repository kế thừa trực tiếp. Toàn bộ thao tác I/O đều dùng `async/await` của EF Core. |
| 3 | [INguoiHienMauRepository.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/INguoiHienMauRepository.cs#L1-L46) | 46 | Specific Interface kế thừa `IRepository<NguoiHienMau>`. Định nghĩa 5 phương thức đặc thù cho nghiệp vụ hồ sơ người hiến máu và kiểm tra trùng thông tin tài khoản. |
| 4 | [NguoiHienMauRepository.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/NguoiHienMauRepository.cs#L1-L77) | 77 | Specific Implementation kế thừa `Repository<NguoiHienMau>`, đóng gói truy vấn `NguoiHienMaus`, `TaiKhoans`, và Explicit Loading lịch sử hiến máu. |
| 5 | [IPhieuDangKyRepository.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/IPhieuDangKyRepository.cs#L1-L57) | 57 | Specific Interface kế thừa `IRepository<PhieuDangKyHienMau>`. Định nghĩa 8 phương thức đặc thù cho đợt hiến máu, lịch sử phiếu, theo dõi phiếu và khởi tạo Transaction `Serializable`. |
| 6 | [PhieuDangKyRepository.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs#L1-L96) | 96 | Specific Implementation kế thừa `Repository<PhieuDangKyHienMau>`, đóng gói toàn bộ truy vấn `PhieuDangKyHienMaus`, `DotHienMaus` và quản lý Transaction. |

### 1.2. Kiểm tra sự tách biệt của Controller khỏi `ApplicationDbContext`
- **[NguoiHienMauController.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs#L1-L261)**:
  - **Không** còn `using QuanLyHienMau_UNETI2_TI17A5HN.Data;` hay `using Microsoft.EntityFrameworkCore;`.
  - Chỉ có duy nhất 1 field `private readonly INguoiHienMauRepository _nguoiHienMauRepo;` ([dòng 15](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs#L15)).
  - **0** tham chiếu tới `ApplicationDbContext` hay `_context`.
- **[DangKyController.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L1-L760)**:
  - **Không** còn `using QuanLyHienMau_UNETI2_TI17A5HN.Data;` hay `using Microsoft.EntityFrameworkCore;`.
  - Chỉ inject `INguoiHienMauRepository _nguoiHienMauRepo` và `IPhieuDangKyRepository _phieuDangKyRepo` ([dòng 18–25](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L18-L25)).
  - **0** tham chiếu tới `ApplicationDbContext` hay `_context`.

### 1.3. Đánh giá mức độ đóng gói truy vấn (Encapsulation vs. Copy-Paste)
Các Repository **thực sự đóng gói và tái sử dụng truy vấn miền nghiệp vụ (domain queries)** chứ không chỉ bê nguyên xi từng đoạn từ Controller sang:
1. **Tái sử dụng cây Eager Loading phức tạp qua `BuildTrackingQuery()`**:
   - Trước khi chuyển đổi, trong `DangKyController.cs` có 3 nơi lặp lại cụm `.Include(p => p.NguoiHienMau).Include(p => p.DotHienMau!).ThenInclude(d => d.DonViToChuc).Include(p => p.LichKhamSangLocs).Include(p => p.KetQuaHienMau)` (`TheoDoi` cho Donor, `TheoDoi` cho Admin/BacSi, và `ChiTietPhieu`).
   - Sau khi chuyển đổi, [PhieuDangKyRepository.cs](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs#L25-L33) tách riêng phương thức private `BuildTrackingQuery()` và tái sử dụng cho cả 3 phương thức:
     - `GetTrackingListByNguoiHienAsync` ([dòng 61–67](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs#L61-L67))
     - `GetRecentTrackingListForStaffAsync` ([dòng 69–75](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs#L69-L75))
     - `GetByIdWithTrackingDetailsAsync` ([dòng 77–81](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs#L77-L81))
2. **Hợp nhất truy vấn lịch sử hiến máu 84 ngày qua `GetHistoryByNguoiHienAsync()`**:
   - Trước khi chuyển đổi, truy vấn lấy lịch sử phiếu kèm `DotHienMau`, `LichKhamSangLocs`, `KetQuaHienMau` bị viết lặp lại 3 lần trong `DangKyController` (`Create [GET]`, `Create [POST]` bước kiểm tra sớm, và `Create [POST]` bên trong Transaction).
   - Sau khi chuyển đổi, cả 3 vị trí trong `DangKyController` ([dòng 268](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L268), [dòng 480](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L480), [dòng 517](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L517)) đều gọi chung 1 phương thức `GetHistoryByNguoiHienAsync(int maNguoiHien)` ([PhieuDangKyRepository.cs dòng 52–59](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs#L52-L59)).
3. **Đóng gói điều kiện lọc thông tin trùng**:
   - `IsEmailTakenByOtherAccountAsync` ([NguoiHienMauRepository.cs dòng 44–53](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/NguoiHienMauRepository.cs#L44-L53)) và `IsPhoneTakenByOtherDonorAsync` ([dòng 55–60](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/NguoiHienMauRepository.cs#L55-L60)) ẩn hoàn toàn biểu thức LINQ kiểm tra trên bảng `TaiKhoans` và `NguoiHienMaus`.

### 1.4. Kiểm tra Dependency Injection, Generic/Specific Repository, `Include`/`ThenInclude` & Explicit Loading
- **Dependency Injection ([Program.cs dòng 13–20](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Program.cs#L13-L20))**:
  ```csharp
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

  // Module 3: Repository Pattern
  builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
  builder.Services.AddScoped<INguoiHienMauRepository, NguoiHienMauRepository>();
  builder.Services.AddScoped<IPhieuDangKyRepository, PhieuDangKyRepository>();
  ```
  Tất cả đều đăng ký với vòng đời `Scoped`, khớp với vòng đời mặc định `Scoped` của `AddDbContext<ApplicationDbContext>`.
- **Bảo toàn `Include` / `ThenInclude` & Explicit Loading**:
  - **Explicit Loading** trong `NguoiHienMauController.EnsureNguoiHienMauAsync` đã được chuyển nguyên vẹn vào [`NguoiHienMauRepository.LoadDonationHistoryCollectionAsync`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/NguoiHienMauRepository.cs#L62-L75):
    ```csharp
    await _context.Entry(nguoiHienMau)
        .Collection(n => n.PhieuDangKyHienMaus)
        .Query()
        .Include(p => p.DotHienMau!)
            .ThenInclude(d => d.DonViToChuc)
        .Include(p => p.LichKhamSangLocs)
        .Include(p => p.KetQuaHienMau)
        .OrderByDescending(p => p.NgayDangKy)
        .LoadAsync();
    ```
  - Toàn bộ 7 cụm Eager Loading (`Include`/`ThenInclude`) trong `NguoiHienMauRepository` và `PhieuDangKyRepository` khớp 100% với truy vấn gốc.

---

## 2. Kiểm tra Transaction & Concurrency (`DangKyController.Create [POST]`)

Kiểm tra thực tế mã nguồn tại [DangKyController.cs dòng 319–592](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L319-L592):

### 2.1. Hai Repository có dùng chung `ApplicationDbContext` instance không?
- **CÓ (100% dùng chung)**.
- `ApplicationDbContext`, `INguoiHienMauRepository`, và `IPhieuDangKyRepository` đều được đăng ký `Scoped` trong `Program.cs`.
- Trong cùng một HTTP Request tới `DangKyController.Create [POST]`, DI Container khởi tạo **duy nhất 1 instance** `ApplicationDbContext` và truyền cùng tham chiếu đó vào constructor của cả `NguoiHienMauRepository` và `PhieuDangKyRepository`.
- Điều này cũng đã được kiểm chứng thực nghiệm bằng `ServiceProvider` thật trong test `ARCH-05` (`ReferenceEquals(ctx1, ctx2) == true`).

### 2.2. Các thao tác cập nhật `NguoiHienMau`, `TaiKhoan` và tạo `PhieuDangKyHienMau` có nằm trong cùng Transaction không?
- **CÓ**. Luồng thực thi cụ thể trong `Create [POST]`:
  1. **Dòng 334**: `var nguoiHienMau = await _nguoiHienMauRepo.GetByTaiKhoanWithAccountAsync(maTaiKhoan.Value);`
     - Truy vấn này không dùng `AsNoTracking()`, nên cả `nguoiHienMau` và navigation property `nguoiHienMau.TaiKhoan` đều được **Change Tracker** của `ApplicationDbContext` dùng chung theo dõi.
     - Nếu người dùng mới chưa có hồ sơ ([dòng 337–349](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L337-L349)), hồ sơ rỗng ban đầu được tạo và nạp lại trước khi kiểm tra điều kiện.
  2. **Dòng 356–509**: Thực hiện toàn bộ các bước kiểm tra dữ liệu đầu vào (Họ tên, SĐT, Email, Ngày sinh 18–60, Nhóm máu, Cân nặng, Đợt hiến máu, Khảo sát sức khỏe, Kiểm tra sớm 84 ngày & trùng phiếu). **Không có bất kỳ lệnh sửa đổi entity hay `SaveChangesAsync()` nào ở giai đoạn này**.
  3. **Dòng 511–512**: Lấy khóa `SemaphoreSlim` theo `MaNguoiHien` và chờ vào vùng găng (`await userLock.WaitAsync()`).
  4. **Dòng 515**: Mở Transaction mức `IsolationLevel.Serializable` trên `ApplicationDbContext` dùng chung thông qua:
     ```csharp
     await using var transaction = await _phieuDangKyRepo.BeginSerializableTransactionAsync();
     ```
  5. **Dòng 517–540**: Truy vấn lại lịch sử phiếu trong Transaction (`lichSuPhieuInTx`). Nếu vi phạm 84 ngày hoặc trùng phiếu hoạt động, gọi `await transaction.RollbackAsync();` và thoát ngay khi chưa sửa đổi bất kỳ dữ liệu nào.
  6. **Dòng 543–565**: Gán giá trị cập nhật cho `nguoiHienMau` (`HoTen`, `NgaySinh`, `GioiTinh`, `DienThoai`, `DiaChi`, `NgheNghiep`, `NhomMau`, `CanNang`) và đồng bộ `nguoiHienMau.TaiKhoan` (`HoTen`, `Email`). Vì hai thực thể này đang được track bởi cùng `ApplicationDbContext`, Change Tracker tự động đánh dấu trạng thái `Modified`.
  7. **Dòng 567–581**: Khởi tạo `PhieuDangKyHienMau` mới và gọi `await _phieuDangKyRepo.AddAsync(phieu);` (Change Tracker đánh dấu `Added`).
  8. **Dòng 582**: Gọi **duy nhất 1 lệnh** `await _phieuDangKyRepo.SaveChangesAsync();` — EF Core đẩy cả 3 thao tác (`UPDATE NguoiHienMau`, `UPDATE TaiKhoan`, `INSERT PhieuDangKyHienMau`) xuống SQL Server trong cùng một batch bên trong Transaction `Serializable`.
  9. **Dòng 583**: Gọi `await transaction.CommitAsync();`.

### 2.3. Khi xảy ra Exception, Transaction có Rollback đầy đủ không?
- **CÓ**.
- Biến `transaction` được khai báo bằng cú pháp `await using var transaction = ...` ([dòng 515](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L515)).
- Nếu xảy ra bất kỳ ngoại lệ nào (`DbUpdateException`, `SqlException`, lỗi kết nối mạng, timeout...) tại `AddAsync`, `SaveChangesAsync`, hoặc trong khối `try`, luồng thực thi sẽ thoát khỏi khối `try` trước khi chạm tới `transaction.CommitAsync()`.
- Khi thoát khỏi scope, `IDbContextTransaction.DisposeAsync()` được gọi tự động và **SQL Server tự động Rollback toàn bộ các lệnh `UPDATE NguoiHienMau`, `UPDATE TaiKhoan`, `INSERT PhieuDangKyHienMau` chưa commit**, đảm bảo không bao giờ xảy ra tình trạng cập nhật dở dang (partial update).
- Đồng thời khối `finally` ([dòng 588–591](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L588-L591)) luôn chạy `userLock.Release()` để giải phóng `SemaphoreSlim`, không gây treo khóa.

### 2.4. `IsolationLevel.Serializable` và `SemaphoreSlim` có còn hoạt động đúng không? Có nguy cơ Deadlock hoặc Race Condition không?
- **`IsolationLevel.Serializable`**: Được bảo toàn nguyên vẹn tại [`PhieuDangKyRepository.BeginSerializableTransactionAsync()`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs#L89-L94):
  ```csharp
  public async Task<IDbContextTransaction> BeginSerializableTransactionAsync()
  {
      return await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
  }
  ```
- **`SemaphoreSlim`**: Được bảo toàn nguyên vẹn tại [DangKyController.cs dòng 20–21](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L20-L21) và [dòng 511–591](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L511-L591) (`ConcurrentDictionary<int, SemaphoreSlim>`).
- **Đánh giá Deadlock & Race Condition**:
  - **Cùng một người hiến máu gửi nhiều request đồng thời (Double-submit / Spam click)**: Các request của cùng `MaNguoiHien` bị tuần tự hóa (serialize) ngay trên bộ nhớ ứng dụng bởi `SemaphoreSlim(1, 1)` **trước khi** mở Transaction `Serializable` dưới SQL Server. Nhờ đó, không bao giờ có 2 Transaction `Serializable` của cùng 1 `MaNguoiHien` tranh chấp Range Lock trên bảng `PhieuDangKyHienMaus` cùng lúc $\rightarrow$ **Triệt tiêu nguy cơ Self-Deadlock và Race Condition cho cùng một người hiến máu**.
  - **Nhiều người hiến máu khác nhau đăng ký đồng thời**: Mỗi `MaNguoiHien` có một `SemaphoreSlim` riêng nên không chặn lẫn nhau. Tuy nhiên, trong SQL Server, `IsolationLevel.Serializable` đặt Range Shared Lock (`RangeS-S`) trên index theo `MaNguoiHien` khi đọc `PhieuDangKyHienMaus` và giữ Exclusive Lock trên dòng `NguoiHienMau`/`TaiKhoan` tương ứng khi `SaveChangesAsync()`. Nếu bảng `PhieuDangKyHienMaus` có Index trên cột `MaNguoiHien` (Foreign Key mặc định của EF Core), Range Lock chỉ khóa phạm vi của từng `MaNguoiHien` riêng biệt. (Lưu ý nhỏ: `_submissionLocks` lưu `SemaphoreSlim` theo `MaNguoiHien` trong suốt vòng đời tiến trình; với quy mô dự án môn học/trường đại học thì số lượng entry rất nhỏ và hoàn toàn an toàn).

---

## 3. Xác minh và Phân loại chi tiết 21/21 Test (`Module3RepoTests`)

Đã đọc toàn bộ [Module3RepoTests.csproj](file:///C:/Users/ADMIN/.gemini/antigravity/brain/6d75e986-2b72-47bc-aa79-7c1c7a1366e0/scratch/Module3RepoTests/Module3RepoTests.csproj#L1-L12) và [Program.cs](file:///C:/Users/ADMIN/.gemini/antigravity/brain/6d75e986-2b72-47bc-aa79-7c1c7a1366e0/scratch/Module3RepoTests/Program.cs#L1-L639) (639 dòng code).

### 3.1. Tuyên bố minh bạch về phân loại Test
> [!IMPORTANT]
> **Xác nhận trung thực về môi trường thực thi của 21 test:**
> - **4 / 21 test (`ARCH-01` → `ARCH-04`)**: Là **Test kiểm tra cấu trúc mã nguồn / Reflection** trên Assembly đã biên dịch.
> - **1 / 21 test (`ARCH-05`)**: Là **Integration Test cho Dependency Injection Container** (`ServiceCollection` / `ServiceProvider` thật của ASP.NET Core & EF Core), kiểm chứng việc chia sẻ cùng một instance `ApplicationDbContext` trong 1 Scope (không mở kết nối vật lý tới SQL Server).
> - **16 / 21 test (`REG-01A` → `REG-12C`)**: Là **Controller Unit Test sử dụng In-Memory Fake Repository** (`FakeNguoiHienMauRepository`, `FakePhieuDangKyRepository`, `FakeDbContextTransaction`).
> - **0 / 21 test**: Sử dụng **Database SQL Server / LocalDB thật**. Do giới hạn cô lập Named Pipe của môi trường chạy dòng lệnh tự động đối với tiến trình SQL Server LocalDB, bộ test tự động **KHÔNG** thực thi câu lệnh SQL thật xuống database vật lý. Các truy vấn LINQ-to-Entities bên trong `NguoiHienMauRepository.cs` và `PhieuDangKyRepository.cs` được kiểm chứng thông qua đối chiếu mã nguồn 1:1 với code gốc và biên dịch kiểu tĩnh (`dotnet build`).

### 3.2. Bảng chi tiết từng Test trong 21/21 Test

| STT | Mã & Tên Test | Phân loại | Mục tiêu | Dữ liệu đầu vào | Cách thực thi | Kết quả thực tế |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: |
| 1 | `ARCH-01: NguoiHienMauController has no ApplicationDbContext field` | Reflection / Cấu trúc | Đảm bảo `NguoiHienMauController` không còn giữ field `ApplicationDbContext` | `typeof(NguoiHienMauController).GetFields(NonPublic \| Instance \| Public)` | Quét toàn bộ field bằng Reflection, assert không có field nào thuộc kiểu `ApplicationDbContext` | **PASS** |
| 2 | `ARCH-02: DangKyController has no ApplicationDbContext field` | Reflection / Cấu trúc | Đảm bảo `DangKyController` không còn giữ field `ApplicationDbContext` | `typeof(DangKyController).GetFields(...)` | Quét toàn bộ field bằng Reflection, assert không có field nào thuộc kiểu `ApplicationDbContext` | **PASS** |
| 3 | `ARCH-03: NguoiHienMauController constructor injects INguoiHienMauRepository` | Reflection / Cấu trúc | Kiểm tra constructor của `NguoiHienMauController` nhận đúng `INguoiHienMauRepository` | `typeof(NguoiHienMauController).GetConstructors()` | Kiểm tra mảng `GetParameters()` có đúng 1 tham số kiểu `INguoiHienMauRepository` | **PASS** |
| 4 | `ARCH-04: DangKyController constructor injects INguoiHienMauRepository & IPhieuDangKyRepository` | Reflection / Cấu trúc | Kiểm tra constructor của `DangKyController` nhận đúng 2 interface Repository | `typeof(DangKyController).GetConstructors()` | Kiểm tra mảng `GetParameters()` gồm 2 tham số `INguoiHienMauRepository` và `IPhieuDangKyRepository` | **PASS** |
| 5 | `ARCH-05: Scoped DI shares exact same ApplicationDbContext instance between both Repositories` | DI Integration Test | Chứng minh 2 Repository dùng chung 1 `ApplicationDbContext` trong cùng Scope | `ServiceCollection` đăng ký `AddDbContext<ApplicationDbContext>`, `INguoiHienMauRepository`, `IPhieuDangKyRepository` | Tạo scope từ `BuildServiceProvider()`, resolve 2 repo, đọc field `_context` qua Reflection và kiểm tra `ReferenceEquals(ctx1, ctx2)` | **PASS** |
| 6 | `REG-01A: Auto-creates NguoiHienMau profile from TaiKhoan when first visiting HoSoCaNhan` | Unit Test (Fake Repo) | Kiểm tra tự động tạo hồ sơ `NguoiHienMau` mới khi tài khoản lần đầu vào `HoSoCaNhan` | Claims `MaTaiKhoan = "10"`, `TaiKhoan` (`HoTen="Nguyễn Văn A"`, `Email="a@uneti.edu.vn"`), danh sách `NguoiHienMau` rỗng | Gọi `NguoiHienMauController.HoSoCaNhan()`, kiểm tra `repo.NguoiHienMaus.Count == 1`, `SaveChangesCount == 1`, `LoadCollectionCalled == true` | **PASS** |
| 7 | `REG-01B: Loads existing NguoiHienMau profile and explicit-loads donation history` | Unit Test (Fake Repo) | Kiểm tra nạp hồ sơ đã tồn tại và gọi Explicit Loading lịch sử hiến máu | Claims `MaTaiKhoan = "10"`, đã có 1 `NguoiHienMau` trong `FakeNguoiHienMauRepository` | Gọi `HoSoCaNhan()`, kiểm tra không tạo mới (`Count == 1`), `SaveChangesCount == 0`, `LoadCollectionCalled == true` | **PASS** |
| 8 | `REG-02A: Blocks profile update when Email or Phone is already used by another donor` | Unit Test (Fake Repo) | Chặn cập nhật hồ sơ khi Email và SĐT trùng với người dùng khác | Người dùng `MaTaiKhoan=1, MaNguoiHien=1` cập nhật sang Email `"taken@uneti.edu.vn"` và SĐT `"0988888888"` của người dùng số 2 | Gọi `CapNhatHoSo(...)`, kiểm tra `ModelState` có lỗi ở cả key `Email` và `DienThoai`, `SaveChangesCount == 0` | **PASS** |
| 9 | `REG-02B: Updates profile and syncs HoTen & Email to TaiKhoan when valid` | Unit Test (Fake Repo) | Cập nhật thành công hồ sơ và đồng bộ `HoTen`, `Email` sang `TaiKhoan` | Dữ liệu hợp lệ: `HoTen="Trần Thị Mới"`, `Email="new@uneti.edu.vn"`, `DienThoai="0912345678"`, `NgaySinh=2000-05-15`, `CanNang=55` | Gọi `CapNhatHoSo(...)`, kiểm tra `RedirectToAction("HoSoCaNhan")`, `donor.HoTen == tk.HoTen`, `tk.Email == "new@uneti.edu.vn"`, `SaveChangesCount == 1` | **PASS** |
| 10 | `REG-03: Blocks registration when donor is under 18 or over 60 years old` | Unit Test (Fake Repo) | Chặn đăng ký khi người hiến máu dưới 18 tuổi | `NgaySinh = DateTime.Today.AddYears(-17)` (17 tuổi), đợt hiến máu đang mở, sức khỏe hợp lệ | Gọi `DangKyController.Create [POST]`, kiểm tra trả về `ViewResult` có `TempData["Error"]` chứa `"từ 18 đến 60 tuổi"`, `BeginTxCalled == false` | **PASS** |
| 11 | `REG-04: Blocks registration when donor weight is below campaign minimum (CanNangToiThieu)` | Unit Test (Fake Repo) | Chặn đăng ký khi cân nặng thấp hơn `CanNangToiThieu` của đợt | Đợt hiến máu yêu cầu `CanNangToiThieu = 45`, người hiến máu nhập `CanNang = 43` | Gọi `Create [POST]`, kiểm tra trả về `ViewResult` có `TempData["Error"]` chứa `"45kg"`, `BeginTxCalled == false` | **PASS** |
| 12 | `REG-05: Blocks registration when health survey questions are invalid` | Unit Test (Fake Repo) | Chặn đăng ký khi trả lời khảo sát sức khỏe không đạt (`health_q1 = "no"`, `health_q2 = "yes"`) | Hồ sơ hợp lệ nhưng truyền `health_q1 = "no"`, `health_q2 = "yes"` | Gọi `Create [POST]`, kiểm tra trả về `ViewResult` có `TempData["Error"]`, không lưu phiếu mới | **PASS** |
| 13 | `REG-06: Blocks duplicate registration in the same campaign` | Unit Test (Fake Repo) | Ngăn đăng ký trùng khi đã có phiếu `"Chờ duyệt"` trong cùng đợt | Đã có phiếu `MaDot = 1, MaNguoiHien = 1, TrangThai = "Chờ duyệt"` | Gọi `Create [POST]` cho `maDot = 1`, kiểm tra `RedirectToAction("TheoDoi")`, `TempData["Warning"]` báo đã đăng ký, `PhieuDangKys.Count` giữ nguyên `1` | **PASS** |
| 14 | `REG-07: Blocks registration when last confirmed donation was less than 84 days ago` | Unit Test (Fake Repo) | Ngăn đăng ký mới khi lần hiến máu thành công gần nhất chưa đủ 84 ngày | Có 1 phiếu `TrangThai = "Đã hiến máu"` với `KetQuaHienMau.NgayHien = DateTime.Today.AddDays(-30)` (cách 30 ngày) | Gọi `Create [POST]` cho đợt mới `maDot = 2`, kiểm tra `RedirectToAction("Create")`, `TempData["Warning"]` chứa `"84 ngày"`, không tạo phiếu mới | **PASS** |
| 15 | `REG-08: Successful registration runs inside Serializable Transaction, updates profile + account, and creates ticket` | Unit Test (Fake Repo) | Kiểm tra đăng ký hợp lệ mở Transaction, cập nhật `NguoiHienMau` + `TaiKhoan`, thêm `PhieuDangKyHienMau` với `TrangThai = "Chờ duyệt"` và Commit | Hồ sơ hợp lệ (`Tuổi = 25`, `CanNang = 55`), đợt mở (`CanNangToiThieu = 45`), `health_q1 = "yes"`, `health_q2 = "no"`, chưa có phiếu cũ | Gọi `Create [POST]`, kiểm tra `BeginTxCalled == true`, `Committed == true`, `RolledBack == false`, `SaveChangesCount == 1`, `PhieuDangKys.Count == 1` (`TrangThai == "Chờ duyệt"`), đồng bộ `donor.HoTen == tk.HoTen` | **PASS** |
| 16 | `REG-09: In-transaction race condition rolls back transaction without saving` | Unit Test (Fake Repo) | Kiểm tra cơ chế Double-Check bên trong Transaction thực hiện Rollback khi phát hiện phiếu trùng vừa phát sinh | Cấu hình `OnBeginTransaction` chèn 1 phiếu `"Chờ duyệt"` vào `PhieuDangKys` ngay thời điểm mở Transaction (mô phỏng Race Condition) | Gọi `Create [POST]`, kiểm tra `BeginTxCalled == true`, `RolledBack == true`, `Committed == false`, `SaveChangesCount == 0` | **PASS** |
| 17 | `REG-10: Blocks viewing or cancelling another donor's registration ticket (IDOR protection)` | Unit Test (Fake Repo) | Chặn xem chi tiết hoặc hủy phiếu của người hiến máu khác (chống lỗ hổng IDOR) | Người dùng đăng nhập `MaNguoiHien = 1`, phiếu `MaPhieu = 99` thuộc `MaNguoiHien = 2` | Gọi `ChiTietPhieu(99)` và `HuyDangKy(99)`, kiểm tra cả 2 đều redirect về `"TheoDoi"` kèm `TempData["Error"]` từ chối quyền truy cập, trạng thái phiếu giữ nguyên | **PASS** |
| 18 | `REG-11: Allows cancelling pending ticket ('Chờ duyệt') and blocks cancelling completed ticket ('Đã hiến máu')` | Unit Test (Fake Repo) | Cho phép hủy phiếu `"Chờ duyệt"` nhưng chặn hủy phiếu `"Đã hiến máu"` | Phiếu #10 (`TrangThai = "Chờ duyệt"`) và Phiếu #11 (`TrangThai = "Đã hiến máu"`) của cùng `MaNguoiHien = 1` | Gọi `HuyDangKy(10)` $\rightarrow$ chuyển sang `"Đã hủy"`; gọi `HuyDangKy(11)` $\rightarrow$ giữ nguyên `"Đã hiến máu"` và báo `TempData["Error"]` | **PASS** |
| 19 | `REG-12A: Static IsConfirmedSuccessfulDonation requires TrangThai='Đã hiến máu' and safe results` | Unit Test | Kiểm tra phương thức static `IsConfirmedSuccessfulDonation` mà Razor View gọi | Phiếu 1: `TrangThai = "Đã hiến máu"`, kết quả `"Đạt"`, `"Âm tính"`; Phiếu 2: `TrangThai = "Chờ duyệt"` | Gọi trực tiếp `DangKyController.IsConfirmedSuccessfulDonation(...)`, kiểm tra trả về `true` cho Phiếu 1 và `false` cho Phiếu 2 | **PASS** |
| 20 | `REG-12B: Static GetActualDonationDate prioritizes KetQuaHienMau.NgayHien` | Unit Test | Kiểm tra phương thức static `GetActualDonationDate` ưu tiên `KetQuaHienMau.NgayHien` | Phiếu có `NgayDangKy = Today - 50 ngày` và `KetQuaHienMau.NgayHien = Today - 40 ngày` | Gọi `DangKyController.GetActualDonationDate(phieu)`, kiểm tra trả về `Today - 40 ngày` | **PASS** |
| 21 | `REG-12C: Static EvaluateRecentDonationEligibility calculates 84-day cooldown accurately` | Unit Test | Kiểm tra phương thức static `EvaluateRecentDonationEligibility` tính toán đúng số ngày còn lại trong chu kỳ 84 ngày | Lịch sử có 1 phiếu hiến thành công cách đây 20 ngày (`Today.AddDays(-20)`) | Gọi `DangKyController.EvaluateRecentDonationEligibility(list, Today)`, kiểm tra `IsBlocked == true`, `DaysSinceLastDonation == 20`, `DaysRemaining == 64` | **PASS** |

---

## 4. So sánh Nghiệp vụ Trước và Sau khi chuyển đổi

Đã chạy lệnh đối chiếu dòng-đối-dòng (`Compare-Object`) giữa bản sao lưu gốc trước khi chuyển đổi (`scratch/backup_before_repo/NguoiHienMauController.cs.bak`, `DangKyController.cs.bak`) và code hiện tại.

### 4.1. Kết quả đối chiếu toàn bộ các quy tắc nghiệp vụ cốt lõi

| Nhóm nghiệp vụ | Vị trí trong code hiện tại | Đối chiếu với bản `.bak` trước khi chuyển đổi | Kết luận |
| :--- | :--- | :--- | :---: |
| **1. Quy tắc 84 ngày giữa 2 lần hiến máu** | `MinDaysBetweenDonations = 84` ([DangKyController.cs dòng 23](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L23)); kiểm tra tại `Create [GET]` ([dòng 268–278](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L268-L278)), `Create [POST]` pre-check ([dòng 480–490](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L480-L490)), và `Create [POST]` in-transaction ([dòng 517–527](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L517-L527)) | Giữ nguyên 100% logic tính ngày, câu thông báo `TempData["Warning"]` và nhánh redirect | **Giữ nguyên 100%** |
| **2. Độ tuổi 18–60** | `CapNhatHoSo` ([NguoiHienMauController.cs dòng 109–120](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs#L109-L120)) và `Create [POST]` ([DangKyController.cs dòng 389–401](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L389-L401)) | Giữ nguyên 100% cách tính tuổi theo ngày sinh và điều kiện `tuoi < 18 \|\| tuoi > 60` | **Giữ nguyên 100%** |
| **3. Cân nặng tối thiểu** | Kiểm tra `30–200kg` tại `CapNhatHoSo` ([dòng 122–128](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs#L122-L128)) & `Create [POST]` ([dòng 418–425](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L418-L425)); kiểm tra theo đợt `Math.Max(42m, dot.CanNangToiThieu ?? 45m)` tại [dòng 451–459](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L451-L459) | Không thay đổi bất kỳ ký tự nào | **Giữ nguyên 100%** |
| **4. Kiểm tra sức khỏe (`health_q1`, `health_q2`)** | `Create [POST]` ([DangKyController.cs dòng 461–478](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L461-L478)): bắt buộc trả lời đủ 2 câu, `q1 == "yes"`, `q2 == "no"` | Không thay đổi bất kỳ ký tự nào | **Giữ nguyên 100%** |
| **5. Ngăn đăng ký trùng** | `Create [GET]` ([dòng 280–289](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L280-L289)), `Create [POST]` pre-check ([dòng 492–502](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L492-L502)), và `Create [POST]` in-transaction ([dòng 529–540](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L529-L540)) | Giữ nguyên điều kiện `p.MaDot == maDot && p.TrangThai != "Đã hủy" && p.TrangThai != "Từ chối"` | **Giữ nguyên 100%** |
| **6. Quyền sở hữu phiếu (Chống IDOR)** | `ChiTietPhieu` ([dòng 650–661](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L650-L661)) và `HuyDangKy` ([dòng 683–691](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L683-L691)) | Giữ nguyên kiểm tra `phieu.MaNguoiHien != nguoiHienMau.MaNguoiHien` | **Giữ nguyên 100%** |
| **7. Hủy phiếu đăng ký** | `HuyDangKy` ([dòng 669–713](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L669-L713)) | Chỉ cho phép hủy khi `TrangThai` là `"Chờ duyệt"` hoặc `"Chờ khám sàng lọc"`, từ chối khi `"Đã hiến máu"` / `"Đã hủy"` / `"Từ chối"` | **Giữ nguyên 100%** |
| **8. Đồng bộ `TaiKhoan` (`HoTen`, `Email`)** | `CapNhatHoSo` ([NguoiHienMauController.cs dòng 169–179](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs#L169-L179)) và `Create [POST]` ([DangKyController.cs dòng 556–565](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L556-L565)) | Giữ nguyên đồng bộ 2 chiều giữa `NguoiHienMau` và `TaiKhoan` | **Giữ nguyên 100%** |
| **9. Các phương thức `public static` cho Razor Views** | `EvaluateRecentDonationEligibility`, `IsConfirmedSuccessfulDonation`, `GetActualDonationDate`, và class `RecentDonationEligibilityResult` ([DangKyController.cs dòng 47–223](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs#L47-L223)) | Khớp từng chữ ký và từng dòng lệnh 100% | **Giữ nguyên 100%** |

---

## 5. Kiểm tra Phạm vi Ảnh hưởng (Git Scope)

Kết quả thực thi `git status --short` và `git diff --stat` trên nhánh `DamThuTrang_23103100243`:

### 5.1. Danh sách các file Code bị thay đổi (`Modified`)
Chỉ có **đúng 3 file** trong phạm vi được phê duyệt:
1. `M QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs`
2. `M QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs`
3. `M QuanLyHienMau_UNETI2_TI17A5HN/Program.cs` (+5 dòng đăng ký DI cho Repository Module 3)

### 5.2. Danh sách các file Code tạo mới (`Untracked`)
Nằm gọn trong thư mục `QuanLyHienMau_UNETI2_TI17A5HN/Repositories/`:
1. `?? QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/IRepository.cs`
2. `?? QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Repository.cs`
3. `?? QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/INguoiHienMauRepository.cs`
4. `?? QuanLyHienMau_UNETI2_TI17A5HN/Repositories/NguoiHienMauRepository.cs`
5. `?? QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/IPhieuDangKyRepository.cs`
6. `?? QuanLyHienMau_UNETI2_TI17A5HN/Repositories/PhieuDangKyRepository.cs`

### 5.3. Xác nhận các thành phần KHÔNG bị chạm tới
- `Controllers/AdminController.cs`: **Không thay đổi** (0 dòng diff).
- `Data/ApplicationDbContext.cs`: **Không thay đổi** (0 dòng diff).
- `Models/*` và cấu trúc bảng Database / Migrations: **Không thay đổi** (0 dòng diff).
- `Views/*`: **Không thay đổi** (0 dòng diff).
- Các Controller thuộc Module 1, 2, 4, 5 (`AccountController`, `DotHienMauController`, `SuKienController`, `KetQuaHienMauController`, `HomeController`...): **Không thay đổi** (0 dòng diff).
- Dự án test `Module3RepoTests` và thư mục `backup_before_repo` được đặt hoàn toàn bên ngoài workspace Git (`C:\Users\ADMIN\.gemini\antigravity\brain\6d75e986-2b72-47bc-aa79-7c1c7a1366e0\scratch\`), không làm bẩn cây thư mục dự án.

---

## 6. Danh sách Lỗi hoặc Rủi ro còn tồn tại

Qua kiểm tra độc lập, **không có lỗi biên dịch (0 Warning, 0 Error) và không có lỗi logic nghiệp vụ**. Tuy nhiên, có **4 điểm lưu ý kỹ thuật & vận hành** cần nắm rõ:

1. **Chưa có Integration Test chạy trực tiếp trên SQL Server LocalDB thật**:
   - Như đã phân tích ở Mục 3, 21/21 test tự động hiện tại gồm 4 test Reflection, 1 test DI Container và 16 Unit Test dùng Fake Repository trên bộ nhớ.
   - **Khuyến nghị**: Trước hoặc sau khi commit, bạn nên chạy ứng dụng (`dotnet run` hoặc nhấn `F5` trong Visual Studio) và thao tác thử 1 lượt trên trình duyệt với LocalDB thật (Vào *Hồ sơ cá nhân* $\rightarrow$ *Cập nhật hồ sơ* $\rightarrow$ *Đăng ký hiến máu* $\rightarrow$ *Theo dõi & Hủy phiếu*) để xác nhận lần cuối trên cơ sở dữ liệu thật.
2. **Các file tài liệu Markdown (`.md`) đang nằm ở thư mục gốc của Git repo**:
   - Trong thư mục gốc `e:\InformationTechnology\visual tím\QuanLyHienMau_UNETI2_TI17A5HN\` hiện có các file báo cáo `Untracked` (`MODULE3_REPOSITORY_AUDIT.md`, `MODULE3_REPOSITORY_IMPLEMENTATION_REPORT.md`, `MODULE3_REPOSITORY_REGRESSION_TEST.md`, `MODULE3_REPOSITORY_FINAL_AUDIT.md`, `MODULE3_AUDIT_REPORT.md`, `MODULE3_FIX_REPORT.md`, `MODULE3_TEST_MATRIX.md`, và thư mục `MODULE3_DEFENSE_PREPARATION/`).
   - **Khuyến nghị**: Khi `git add`, **không** nên gõ `git add .` ở thư mục gốc nếu không muốn đẩy các file báo cáo cá nhân này lên nhánh chung của nhóm. Chỉ nên `git add` đúng thư mục code dự án `QuanLyHienMau_UNETI2_TI17A5HN/`.
3. **Sự phụ thuộc nhẹ vào kiểu `IDbContextTransaction` của EF Core trong `IPhieuDangKyRepository`**:
   - [`IPhieuDangKyRepository.cs`](file:///e:/InformationTechnology/visual%20t%C3%ADm/QuanLyHienMau_UNETI2_TI17A5HN/QuanLyHienMau_UNETI2_TI17A5HN/Repositories/Interfaces/IPhieuDangKyRepository.cs#L7) có `using Microsoft.EntityFrameworkCore.Storage;` để trả về `Task<IDbContextTransaction>` cho phương thức `BeginSerializableTransactionAsync()`.
   - Với quyết định kiến trúc hiện tại (*Controller → Repository Interface → Repository Implementation → ApplicationDbContext*, chưa triển khai Service Layer hay Unit of Work riêng), đây là cách tiếp cận gọn nhẹ và an toàn nhất để bảo toàn `await using var transaction` cùng mức cô lập `IsolationLevel.Serializable`.
4. **Bộ nhớ của `_submissionLocks` (`ConcurrentDictionary<int, SemaphoreSlim>`)**:
   - Dictionary tĩnh lưu một đối tượng `SemaphoreSlim` cho mỗi `MaNguoiHien` từng bấm đăng ký hiến máu trong vòng đời ứng dụng. Với quy mô hệ thống quản lý hiến máu cấp trường, số lượng người hiến chỉ vài nghìn người (tương đương vài trăm KB RAM), hoàn toàn không gây rò rỉ bộ nhớ đáng kể.

---

## 7. Kết luận: Đã đủ điều kiện Commit và Push hay chưa?

> [!TIP]
> **KẾT LUẬN: ĐỦ ĐIỀU KIỆN COMMIT VÀ PUSH (READY TO COMMIT & PUSH)**

Mã nguồn chuyển đổi sang Repository Pattern của **Module 3** đáp ứng đầy đủ 100% tiêu chuẩn an toàn kiến trúc, bảo toàn nguyên vẹn nghiệp vụ, đảm bảo tính nguyên tử (Atomicity) của Transaction `Serializable`, và tuân thủ tuyệt đối phạm vi file được phép chỉnh sửa.

### Hướng dẫn lệnh Git chuẩn xác khi bạn muốn Commit (chỉ stage đúng code Module 3):
```powershell
git add QuanLyHienMau_UNETI2_TI17A5HN/Program.cs
git add QuanLyHienMau_UNETI2_TI17A5HN/Controllers/NguoiHienMauController.cs
git add QuanLyHienMau_UNETI2_TI17A5HN/Controllers/DangKyController.cs
git add QuanLyHienMau_UNETI2_TI17A5HN/Repositories/
git status
```
*(Sau khi kiểm tra `git status` chỉ gồm 9 file `.cs` của Module 3 trong vùng Staged, bạn có thể thực hiện `git commit` và `git push origin DamThuTrang_23103100243`).*
