namespace Ban_hang.Models;

public class Pet
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";       // chủ nuôi (IdentityUser)

    public string Name { get; set; } = "";           // "Milu"
    public string Species { get; set; } = "";         // "Chó" | "Mèo" | "Khác"
    public string Breed { get; set; } = "";           // "Poodle", "Anh lông ngắn"...
    public string Gender { get; set; } = "";          // "Đực" | "Cái"
    public DateTime? BirthDate { get; set; }           // để tự tính tuổi
    public decimal? WeightKg { get; set; }              // cân nặng hiện tại
    public string AvatarUrl { get; set; } = "";
    public string Note { get; set; } = "";             // dị ứng, bệnh nền, tính cách...

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<PetWeightLog> WeightLogs { get; set; } = new();
}