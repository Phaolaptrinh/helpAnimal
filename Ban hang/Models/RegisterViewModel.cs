using System.ComponentModel.DataAnnotations;

namespace Ban_hang.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^(0|\+84)\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = "";
}