using System.ComponentModel.DataAnnotations;

namespace Ban_hang.Models;

// This replaces the hand-rolled JS validation in the original <script> block.
// Data annotations here drive BOTH:
//   1) server-side validation (ModelState.IsValid in the controller)
//   2) client-side validation (free, via jQuery Unobtrusive Validation,
//      as long as the _ValidationScriptsPartial is included on the page)
public class BookingViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên của bạn.")]
    [MinLength(2, ErrorMessage = "Vui lòng nhập họ tên của bạn.")]
    [Display(Name = "Họ và tên")]
    public string OwnerName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại hợp lệ (10 số).")]
    [RegularExpression(@"^(0|\+84)\d{9,10}$",
        ErrorMessage = "Vui lòng nhập số điện thoại hợp lệ (10 số).")]
    [Display(Name = "Số điện thoại")]
    public string OwnerPhone { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn loại thú cưng.")]
    [Display(Name = "Loại thú cưng")]
    public string PetType { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn dịch vụ.")]
    [Display(Name = "Dịch vụ cần đặt")]
    public string ServiceType { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn ngày hẹn.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày mong muốn")]
    public DateTime? BookDate { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giờ hẹn.")]
    [DataType(DataType.Time)]
    [Display(Name = "Giờ mong muốn")]
    public TimeSpan? BookTime { get; set; }

    [Display(Name = "Ghi chú thêm")]
    public string? Note { get; set; }
}
