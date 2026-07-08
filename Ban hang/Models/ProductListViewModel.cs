namespace Ban_hang.Models;

public class ProductListViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<ProductCategory> Categories { get; set; } = new();
    public string? SelectedCategorySlug { get; set; }
    public string? SearchTerm { get; set; }
}