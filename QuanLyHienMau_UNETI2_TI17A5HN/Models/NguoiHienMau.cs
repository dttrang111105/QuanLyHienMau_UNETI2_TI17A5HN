using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public class NguoiHienMau
{
    [Key]
    public int MaNguoiHien { get; set; }
    public int MaTaiKhoan { get; set; }
    [Required, StringLength(150)]
    public string HoTen { get; set; } = "";
    public bool TrangThai { get; set; } = true;
    public ICollection<PhieuDangKyHienMau> PhieuDangKyHienMaus { get; set; } = new List<PhieuDangKyHienMau>();
}
