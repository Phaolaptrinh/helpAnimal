namespace Ban_hang.Models;

public class PetWeightLog
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
    public decimal WeightKg { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.Now;
}