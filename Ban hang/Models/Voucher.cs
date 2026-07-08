namespace Ban_hang.Models;

public class Voucher
{
    public int Id { get; set; }
    public string Code { get; set; } = "";           // "PETLOR10"
    public string Description { get; set; } = "";     // "Giảm 10% cho đơn từ 200k"
    public string DiscountType { get; set; } = "percent"; // "percent" hoặc "fixed"
    public decimal DiscountValue { get; set; }         // 10 (=10%) hoặc 50000 (=50k)
    public decimal MinOrderAmount { get; set; }        // đơn tối thiểu để áp dụng
    public decimal? MaxDiscountAmount { get; set; }     // giảm tối đa (chặn trần khi %)
    public DateTime ExpiryDate { get; set; }
    public int? MaxUses { get; set; }                   // null = không giới hạn
    public int UsedCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}