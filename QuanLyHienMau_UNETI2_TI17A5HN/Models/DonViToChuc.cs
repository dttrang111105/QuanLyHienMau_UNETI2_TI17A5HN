using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public class DonViToChuc
{
    [Key]
    public int MaDonViToChuc { get; set; }
    [Required, StringLength(200)]
    public string TenDonViToChuc { get; set; } = "";
    public ICollection<DotHienMau> DotHienMaus { get; set; } = new List<DotHienMau>();
}
