// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: Module 5 và phần nền dữ liệu cần thiết để chạy thử.
using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;
public class LichKhamSangLoc
{
    [Key]
    public int MaLichKhamSangLoc { get; set; }
    public int MaPhieu { get; set; }
    public PhieuDangKyHienMau Phieu { get; set; } = null !;
    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }

    [Required, MaxLength(80)]
    public string HinhThucTiepNhan { get; set; } = "Trực tiếp";

    [Required, MaxLength(250)]
    public string DiaDiemHoacLienKet { get; set; } = "";

    [Required, MaxLength(120)]
    public string BacSiSangLoc { get; set; } = "";

    [Required, MaxLength(120)]
    public string BacSiChuanHoa { get; set; } = "";

    [MaxLength(1000)]
    public string GhiChu { get; set; } = "";

    [Required, MaxLength(40)]
    public string TrangThai { get; set; } = TrangThaiLich.DaLenLich;
}
