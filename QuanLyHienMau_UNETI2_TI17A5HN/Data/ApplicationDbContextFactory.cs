// Họ và tên: ĐIỀN HỌ TÊN CỦA BẠN
// Mã sinh viên: ĐIỀN MSSV CỦA BẠN
// Nội dung: tạo DbContext lúc chạy Migration SQL Server cho Module 5.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Data;
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true).AddEnvironmentVariables().Build();
        var connection = config.GetConnectionString("Module5SqlServer") ?? "Server=(localdb)\\MSSQLLocalDB;Database=QuanLyHienMau_Module5_Original;Trusted_Connection=True;TrustServerCertificate=True";
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connection).Options);
    }
}
