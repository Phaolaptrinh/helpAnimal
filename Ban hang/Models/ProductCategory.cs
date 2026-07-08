namespace Ban_hang.Models;

public class ProductCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";       // "Đồ chơi", "Thuốc", "Thức ăn", "Phụ kiện"
    public string Slug { get; set; } = "";        // "do-choi", "thuoc", "thuc-an" — dùng cho URL/filter
    public string Icon { get; set; } = "";        // class icon (nếu dùng Bootstrap Icons/FontAwesome)
}