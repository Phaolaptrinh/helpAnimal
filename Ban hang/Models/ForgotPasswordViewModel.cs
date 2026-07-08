using System.ComponentModel.DataAnnotations;

namespace Ban_hang.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; } = "";
}

public class ResetPasswordViewModel
{
    [Required]
    public string Email { get; set; } = "";

    [Required]
    public string Token { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string NewPassword { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = "";
}