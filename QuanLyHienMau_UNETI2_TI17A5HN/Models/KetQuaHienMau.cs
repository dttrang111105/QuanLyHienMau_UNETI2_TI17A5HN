using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public class KetQuaHienMau
{
    [Key]
    public int MaKetQuaHienMau { get; set; }
    public int MaPhieu { get; set; }
    [Range(0, 100)]
    public decimal? DiemDanhGia { get; set; }
    [StringLength(2000)]
    public string? NhanXet { get; set; }
    [Required, StringLength(50)]
    public string KetQua { get; set; } = "";
    public DateTime NgayCapNhat { get; set; }
    public PhieuDangKyHienMau PhieuDangKyHienMau { get; set; } = null!;
}
