namespace Ban_hang.Models;

public class CartPageViewModel
{
    public List<CartItem> CartItems { get; set; } = new();
    public List<Booking> RecentBookings { get; set; } = new();
}