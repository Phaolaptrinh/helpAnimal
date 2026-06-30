namespace Ban_hang.Models;

// Đây là bảng LƯU TRỮ trong database (khác với BookingViewModel chỉ là form nhập liệu)
public class Booking
{
    public int Id { get; set; }
    public string OwnerName { get; set; } = "";
    public string OwnerPhone { get; set; } = "";
    public string PetType { get; set; } = "";
    public string ServiceType { get; set; } = "";
    public DateTime BookDate { get; set; }
    public TimeSpan BookTime { get; set; }
    public string? Note { get; set; }

    // Trạng thái: Chờ xác nhận / Đã xác nhận / Hoàn thành / Đã hủy
    public string Status { get; set; } = "Chờ xác nhận";

    // Thời gian khách đặt lịch (tự động ghi nhận)
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}