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

    // GET /Product?categorySlug=do-choi&search=bong&page=2
    [HttpGet]
    public async Task<IActionResult> Index(string? categorySlug, string? search, int page = 1)
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

        int pageSize = 8;
        int totalItems = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var vm = new ProductListViewModel
        {
            Products = products,
            Categories = await _db.ProductCategories.ToListAsync(),
            SelectedCategorySlug = categorySlug,
            SearchTerm = search,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return View(vm);
    }
}