using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ban_hang.Data;
using Ban_hang.Models;

namespace Ban_hang.Controllers;

[Authorize]
public class PetController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public PetController(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // GET /Pet — danh sách thú cưng của tôi
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var pets = await _db.Pets
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(pets);
    }

    // GET /Pet/Create
    public IActionResult Create()
    {
        return View(new PetFormViewModel());
    }

    // POST /Pet/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PetFormViewModel form)
    {
        if (!ModelState.IsValid) return View(form);

        var userId = _userManager.GetUserId(User);

        var pet = new Pet
        {
            UserId = userId!,
            Name = form.Name,
            Species = form.Species,
            Breed = form.Breed,
            Gender = form.Gender,
            BirthDate = form.BirthDate,
            WeightKg = form.WeightKg,
            AvatarUrl = string.IsNullOrWhiteSpace(form.AvatarUrl)
                ? $"https://loremflickr.com/300/300/{(form.Species == "Mèo" ? "cat" : "dog")}"
                : form.AvatarUrl,
            Note = form.Note
        };

        _db.Pets.Add(pet);
        await _db.SaveChangesAsync();

        // Lưu cân nặng đầu tiên vào lịch sử (nếu có nhập)
        if (form.WeightKg.HasValue)
        {
            _db.PetWeightLogs.Add(new PetWeightLog { PetId = pet.Id, WeightKg = form.WeightKg.Value });
            await _db.SaveChangesAsync();
        }

        TempData["PetSuccess"] = $"Đã thêm hồ sơ cho bé {pet.Name} 🐾";
        return RedirectToAction(nameof(Index));
    }

    // GET /Pet/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (pet == null) return NotFound();

        var form = new PetFormViewModel
        {
            Id = pet.Id,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            Gender = pet.Gender,
            BirthDate = pet.BirthDate,
            WeightKg = pet.WeightKg,
            AvatarUrl = pet.AvatarUrl,
            Note = pet.Note
        };

        return View(form);
    }

    // POST /Pet/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PetFormViewModel form)
    {
        if (!ModelState.IsValid) return View(form);

        var userId = _userManager.GetUserId(User);
        var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (pet == null) return NotFound();

        bool weightChanged = pet.WeightKg != form.WeightKg;

        pet.Name = form.Name;
        pet.Species = form.Species;
        pet.Breed = form.Breed;
        pet.Gender = form.Gender;
        pet.BirthDate = form.BirthDate;
        pet.WeightKg = form.WeightKg;
        if (!string.IsNullOrWhiteSpace(form.AvatarUrl)) pet.AvatarUrl = form.AvatarUrl;
        pet.Note = form.Note;

        // Nếu cân nặng thay đổi, ghi thêm 1 mốc lịch sử mới
        if (weightChanged && form.WeightKg.HasValue)
        {
            _db.PetWeightLogs.Add(new PetWeightLog { PetId = pet.Id, WeightKg = form.WeightKg.Value });
        }

        await _db.SaveChangesAsync();

        TempData["PetSuccess"] = $"Đã cập nhật hồ sơ bé {pet.Name}";
        return RedirectToAction(nameof(Index));
    }

    // POST /Pet/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (pet != null)
        {
            _db.Pets.Remove(pet);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // GET /Pet/Details/5 — trang hồ sơ chi tiết (lịch sử khám/spa sẽ nối vào sau)
    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        var pet = await _db.Pets
            .Include(p => p.WeightLogs)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (pet == null) return NotFound();

        return View(pet);
    }
}
