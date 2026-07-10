using System.ComponentModel.DataAnnotations;

namespace Ban_hang.Models;

public class PetFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên bé")]
    [Display(Name = "Tên thú cưng")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn loài")]
    [Display(Name = "Loài")]
    public string Species { get; set; } = "";

    [Display(Name = "Giống")]
    public string Breed { get; set; } = "";

    [Display(Name = "Giới tính")]
    public string Gender { get; set; } = "";

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [Display(Name = "Cân nặng hiện tại (kg)")]
    [Range(0.1, 200, ErrorMessage = "Cân nặng không hợp lệ")]
    public decimal? WeightKg { get; set; }

    [Display(Name = "Ảnh đại diện")]
    public string AvatarUrl { get; set; } = "";

    [Display(Name = "Ghi chú (dị ứng, bệnh nền, tính cách...)")]
    public string Note { get; set; } = "";
}