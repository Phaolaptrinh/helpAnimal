namespace Ban_hang.Models;

public class AccountDashboardViewModel
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";

    public int TotalOrders { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalSpentOrders { get; set; }

    public List<Order> Orders { get; set; } = new();
    public List<Booking> Bookings { get; set; } = new();
}