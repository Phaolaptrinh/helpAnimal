namespace Ban_hang.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }         // giá gốc (nếu đang giảm giá), null nếu không giảm
    public int Stock { get; set; }                  // số lượng tồn kho
    public bool IsActive { get; set; } = true;       // ẩn/hiện sản phẩm
    public int CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
}