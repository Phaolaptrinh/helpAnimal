namespace Ban_hang.Models;

public class BookingPrice
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string ServiceType { get; set; } = "";
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}