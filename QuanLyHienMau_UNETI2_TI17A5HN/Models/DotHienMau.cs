using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public class DotHienMau
{
    [Key]
    public int MaDot { get; set; }
    [Required, StringLength(200)]
    public string TenDot { get; set; } = "";
    public int MaDonViToChuc { get; set; }
    public int ChiTieuDonViMau { get; set; }
    [Required, StringLength(50)]
    public string TrangThai { get; set; } = "";
    public DonViToChuc DonViToChuc { get; set; } = null!;
    public ICollection<PhieuDangKyHienMau> PhieuDangKyHienMaus { get; set; } = new List<PhieuDangKyHienMau>();
}
