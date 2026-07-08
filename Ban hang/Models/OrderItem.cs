namespace Ban_hang.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";   // lưu lại tên tại thời điểm mua (phòng khi sản phẩm bị xoá/đổi tên sau này)
    public decimal Price { get; set; }               // giá tại thời điểm mua
    public int Quantity { get; set; }
}