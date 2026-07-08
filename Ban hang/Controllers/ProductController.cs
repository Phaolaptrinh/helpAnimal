using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ban_hang.Data;
using Ban_hang.Models;

namespace Ban_hang.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _db;

    public ProductController(AppDbContext db)
    {
        _db = db;
    }

    // GET /Product?categorySlug=do-choi&search=bong
    [HttpGet]
    public async Task<IActionResult> Index(string? categorySlug, string? search)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(categorySlug))
        {
            query = query.Where(p => p.Category!.Slug == categorySlug);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        var vm = new ProductListViewModel
        {
            Products = await query.OrderByDescending(p => p.Id).ToListAsync(),
            Categories = await _db.ProductCategories.ToListAsync(),
            SelectedCategorySlug = categorySlug,
            SearchTerm = search
        };

        return View(vm);
    }
}