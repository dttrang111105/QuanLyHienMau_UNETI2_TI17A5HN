using System.ComponentModel.DataAnnotations;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Models;

public class PhieuDangKyHienMau
{
    [Key]
    public int MaPhieu { get; set; }
    public int MaNguoiHien { get; set; }
    public int MaDot { get; set; }
    public DateTime NgayNop { get; set; }
    [Required, StringLength(50)]
    public string TrangThai { get; set; } = "";
    public DateTime? NgayXuLy { get; set; }
    public NguoiHienMau NguoiHienMau { get; set; } = null!;
    public DotHienMau DotHienMau { get; set; } = null!;
    public ICollection<LichKhamSangLoc> LichKhamSangLocs { get; set; } = new List<LichKhamSangLoc>();
    public KetQuaHienMau? KetQuaHienMau { get; set; }
}
