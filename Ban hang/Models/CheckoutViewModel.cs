using System.ComponentModel.DataAnnotations;

namespace Ban_hang.Models;

public class CheckoutViewModel
{
    public List<CartItem> CartItems { get; set; } = new();
    public decimal Total { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [Display(Name = "Họ và tên")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string CustomerPhone { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
    [Display(Name = "Địa chỉ giao hàng")]
    public string ShippingAddress { get; set; } = "";

    [Display(Name = "Ghi chú")]
    public string? Note { get; set; }
}