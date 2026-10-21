using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public class LichKhamSangLoc
{
    [Key]
    public int MaLichKhamSangLoc { get; set; }
    public int MaPhieu { get; set; }
    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }
    [Required, StringLength(50)]
    public string HinhThucTiepNhan { get; set; } = "Trực tiếp";
    [Required, StringLength(500)]
    public string DiaDiemHoacLienKet { get; set; } = "";
    [Required, StringLength(150)]
    public string BacSiSangLoc { get; set; } = "";
    [StringLength(2000)]
    public string? GhiChu { get; set; }
    [Required, StringLength(50)]
    public string TrangThai { get; set; } = Module5TrangThai.LichKham.DaLenLich;
    public PhieuDangKyHienMau PhieuDangKyHienMau { get; set; } = null!;
}
