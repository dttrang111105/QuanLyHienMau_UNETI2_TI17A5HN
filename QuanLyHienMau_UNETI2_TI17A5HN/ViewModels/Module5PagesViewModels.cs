using QuanLyHienMau_UNETI2_TI17A5HN.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyHienMau_UNETI2_TI17A5HN.ViewModels;

public class LichPageVm
{
    public DateTime Ngay { get; set; }
    public DateTime Now { get; set; }
    public string View { get; set; } = "calendar";
    public string? Q { get; set; }
    public string? TrangThai { get; set; }
    public int? MaDot { get; set; }
    public string? BacSi { get; set; }
    public int Page { get; set; } = 1;
    public int Pages { get; set; } = 1;
    public int Count { get; set; }
    public int Total { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public int Upcoming { get; set; }
    public List<LichKhamSangLoc> Rows { get; set; } = [];
    public List<LichKhamSangLoc> TrongNgay { get; set; } = [];
    public List<LichKhamSangLoc> TrungLich { get; set; } = [];
    public List<LichKhamSangLoc> SapToi { get; set; } = [];
    public List<string> BacSis { get; set; } = [];
    public List<SelectListItem> Dots { get; set; } = [];
    public LichFormVm Form { get; set; } = new();
    public bool ShowForm { get; set; }
}

public class KetQuaPageVm
{
    public string? Q { get; set; }
    public string? TrangThai { get; set; }
    public string? NhomMau { get; set; }
    public int? MaDot { get; set; }
    public int Page { get; set; } = 1;
    public int Pages { get; set; } = 1;
    public int Count { get; set; }
    public int Total { get; set; }
    public int Success { get; set; }
    public int Waiting { get; set; }
    public int Ineligible { get; set; }
    public List<PhieuDangKyHienMau> Rows { get; set; } = [];
    public PhieuDangKyHienMau? Selected { get; set; }
    public List<SelectListItem> Dots { get; set; } = [];
    public KetQuaFormVm Form { get; set; } = new();
}
