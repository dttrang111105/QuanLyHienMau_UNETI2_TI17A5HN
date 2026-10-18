using System.Text;

namespace QuanLyHienMau_UNETI2_TI17A5HN.Infrastructure;
public static class Module5Csv
{
    public static byte[] Encode(IEnumerable<IEnumerable<string>> rows)
    {
        static string Cell(string? value)
        {
            value ??= "";
            if (value.Length > 0 && "=+-@".Contains(value[0])) value = "'" + value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(string.Join("\r\n", rows.Select(row => string.Join(",", row.Select(Cell)))))).ToArray();
    }
}
