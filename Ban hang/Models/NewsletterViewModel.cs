using System.ComponentModel.DataAnnotations;

namespace Ban_hang.Models;

public class NewsletterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Vui lòng nhập email hợp lệ.")]
    public string Email { get; set; } = "";
}
