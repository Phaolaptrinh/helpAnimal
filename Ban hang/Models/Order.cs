namespace Ban_hang.Models;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string ShippingAddress { get; set; } = "";
    public string Note { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Chờ xác nhận";  // Chờ xác nhận | Đang giao | Hoàn thành | Đã huỷ
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<OrderItem> Items { get; set; } = new();
    public string? VoucherCode { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
}