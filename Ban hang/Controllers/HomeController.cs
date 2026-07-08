using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ban_hang.Models;
using Ban_hang.Data;

namespace Ban_hang.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    // Inject AppDbContext vào controller qua constructor
    // ASP.NET Core tự động truyền vào nhờ DI (Dependency Injection)
    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    // GET /
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Đọc dữ liệu từ database thay vì hard-code
        var vm = new HomeViewModel
        {
            Services = await _db.Services.ToListAsync(),
            Team = await _db.TeamMembers.ToListAsync(),
            Testimonials = await _db.Testimonials.ToListAsync(),
            PriceTiers = await _db.PriceTiers.ToListAsync(),
            BlogPosts = await _db.BlogPosts.ToListAsync(),
            Faqs = await _db.FaqItems.ToListAsync(),
            Gallery = await _db.GalleryItems.ToListAsync(),
            Booking = new BookingViewModel()
        };

        if (TempData["BookingSuccessName"] is string name)
        {
            ViewBag.BookingSuccessName = name;
        }

        return View(vm);
    }

    // POST /Home/BookingSubmit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BookingSubmit(BookingViewModel booking)
    {
        if (!ModelState.IsValid)
        {
            var vm = new HomeViewModel
            {
                Services = await _db.Services.ToListAsync(),
                Team = await _db.TeamMembers.ToListAsync(),
                Testimonials = await _db.Testimonials.ToListAsync(),
                PriceTiers = await _db.PriceTiers.ToListAsync(),
                BlogPosts = await _db.BlogPosts.ToListAsync(),
                Faqs = await _db.FaqItems.ToListAsync(),
                Gallery = await _db.GalleryItems.ToListAsync(),
                Booking = booking
            };
            return View("Index", vm);
        }

        // ← Lưu vào database thật sự
        var newBooking = new Booking
        {
            OwnerName = booking.OwnerName,
            OwnerPhone = booking.OwnerPhone,
            PetType = booking.PetType,
            ServiceType = booking.ServiceType,
            BookDate = booking.BookDate!.Value,
            BookTime = booking.BookTime!.Value,
            Note = booking.Note,
            Status = "Chờ xác nhận",
            CreatedAt = DateTime.Now
        };

        _db.Bookings.Add(newBooking);
        await _db.SaveChangesAsync();

        TempData["BookingSuccessName"] = booking.OwnerName;
        return RedirectToAction(nameof(Index));
    }

    // POST /Home/NewsletterSubmit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult NewsletterSubmit(NewsletterViewModel newsletter)
    {
        if (ModelState.IsValid)
        {
            TempData["NewsletterSuccess"] = true;
        }
        return RedirectToAction(nameof(Index));
    }
}