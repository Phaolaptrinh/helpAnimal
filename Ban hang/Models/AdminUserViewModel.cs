namespace Ban_hang.Models;

public class AdminUserViewModel
{
    public string Id { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public bool IsAdmin { get; set; }
    public int TotalBookings { get; set; }
    public int TotalOrders { get; set; }
}