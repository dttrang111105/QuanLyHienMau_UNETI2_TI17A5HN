// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: hoàn thiện Module 5 trên project gốc.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Controllers;
[Authorize(Roles = "Admin,NhanVienYTe")]
public class ThongKeController : Controller
{
    public IActionResult Index() => RedirectToAction("Dashboard", "Admin");
    public IActionResult Dashboard() => RedirectToAction("Dashboard", "Admin");
    public IActionResult BaoCao(int? nam) => RedirectToAction("BaoCao", "Admin", new { nam });
}
