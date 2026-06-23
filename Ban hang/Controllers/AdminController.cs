using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ban_hang.Data;

namespace Ban_hang.Controllers;

[Authorize(Roles = "Admin")] // Chỉ Admin mới vào được
public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    // GET /Admin
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Dashboard";
        ViewData["TotalServices"] = await _db.Services.CountAsync();
        ViewData["TotalTeam"] = await _db.TeamMembers.CountAsync();
        ViewData["TotalBlogs"] = await _db.BlogPosts.CountAsync();
        ViewData["TotalFaqs"] = await _db.FaqItems.CountAsync();
        return View();
    }
}