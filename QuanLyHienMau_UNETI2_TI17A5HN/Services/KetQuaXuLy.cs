namespace QuanLyHienMau_UNETI2_TI17A5HN.Services;

public sealed record KetQuaXuLy(bool Success, string Message, int? Id = null)
{
    public static KetQuaXuLy Fail(string message) => new(false, message);
    public static KetQuaXuLy Ok(string message, int? id = null) => new(true, message, id);
}
