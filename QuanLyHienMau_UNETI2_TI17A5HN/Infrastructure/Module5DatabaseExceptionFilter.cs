// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: xử lý xung đột dữ liệu của Module 5.
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure;
public class Module5DatabaseExceptionFilter(ILogger<Module5DatabaseExceptionFilter> logger, ITempDataDictionaryFactory tempData) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DbException && context.Exception is not DbUpdateException)
            return;
        logger.LogWarning(context.Exception, "Module 5 database conflict");
        tempData.GetTempData(context.HttpContext)["Error"] = "Dữ liệu vừa thay đổi. Vui lòng tải lại và thử lại.";
        var results = context.RouteData.Values["controller"]?.ToString() == "KetQuaHienMau";
        context.Result = new RedirectToActionResult(results ? "KetQuaXetNghiem" : "LichKhamSangLoc", "Admin", null);
        context.ExceptionHandled = true;
    }
}
