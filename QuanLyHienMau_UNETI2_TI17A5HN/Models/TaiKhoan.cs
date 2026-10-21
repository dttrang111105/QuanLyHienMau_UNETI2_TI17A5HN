using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public class TaiKhoan
{
    [Key]
    public int MaTaiKhoan { get; set; }
    [Required, StringLength(150)]
    public string HoTen { get; set; } = "";
    [Required, StringLength(50)]
    public string VaiTro { get; set; } = "";
    public bool TrangThai { get; set; } = true;
}
