namespace Ban_hang.Models;

public class ProductListViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<ProductCategory> Categories { get; set; } = new();
    public string? SelectedCategorySlug { get; set; }
    public string? SearchTerm { get; set; }

    // ===== PHÂN TRANG =====
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 8;
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}