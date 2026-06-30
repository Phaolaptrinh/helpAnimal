using Ban_hang.Data;
using Ban_hang.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    // ============ XEM DANH SÁCH ============
    // GET /Admin/Services
    public async Task<IActionResult> Services()
    {
        ViewData["Title"] = "Quản lý Dịch vụ";
        var services = await _db.Services.ToListAsync();
        return View(services);
    }

    // ============ THÊM MỚI ============
    // GET /Admin/CreateService
    public IActionResult CreateService()
    {
        ViewData["Title"] = "Thêm dịch vụ";
        return View();
    }

    // POST /Admin/CreateService
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateService(ServiceItem service)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Thêm dịch vụ";
            return View(service);
        }

        _db.Services.Add(service);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm dịch vụ thành công!";
        return RedirectToAction(nameof(Services));
    }

    // ============ SỬA ============
    // GET /Admin/EditService/5
    public async Task<IActionResult> EditService(int id)
    {
        var service = await _db.Services.FindAsync(id);
        if (service == null) return NotFound();

        ViewData["Title"] = "Sửa dịch vụ";
        return View(service);
    }

    // POST /Admin/EditService/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditService(int id, ServiceItem service)
    {
        if (id != service.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Sửa dịch vụ";
            return View(service);
        }

        _db.Services.Update(service);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật dịch vụ!";
        return RedirectToAction(nameof(Services));
    }

    // ============ XÓA ============
    // POST /Admin/DeleteService/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteService(int id)
    {
        var service = await _db.Services.FindAsync(id);
        if (service != null)
        {
            _db.Services.Remove(service);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa dịch vụ!";
        }
        return RedirectToAction(nameof(Services));
    }
    // ============ XEM DANH SÁCH ĐẶT LỊCH ============
    // GET /Admin/Bookings
    public async Task<IActionResult> Bookings()
    {
        ViewData["Title"] = "Quản lý Đặt lịch";
        var bookings = await _db.Bookings
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
        return View(bookings);
    }

    // ============ ĐỔI TRẠNG THÁI ============
    // POST /Admin/UpdateBookingStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBookingStatus(int id, string status)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking != null)
        {
            booking.Status = status;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật trạng thái!";
        }
        return RedirectToAction(nameof(Bookings));
    }

    // ============ XÓA ============
    // POST /Admin/DeleteBooking
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking != null)
        {
            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa lịch đặt!";
        }
        return RedirectToAction(nameof(Bookings));
    }
    // ============ XEM DANH SÁCH ============
    // GET /Admin/Team
    public async Task<IActionResult> Team()
    {
        ViewData["Title"] = "Quản lý Đội ngũ";
        var team = await _db.TeamMembers.ToListAsync();
        return View(team);
    }

    // ============ THÊM MỚI ============
    // GET /Admin/CreateTeamMember
    // GET /Admin/CreateTeamMember
    public IActionResult CreateTeamMember()
    {
        ViewData["Title"] = "Thêm nhân viên";

        // Gán sẵn giá trị mặc định đẹp, người dùng không cần nhập nếu không muốn đổi
        var member = new TeamMember
        {
            BgColor = "2F6F4E",
            TextColor = "ffffff"
        };

        return View(member);
    }

    // POST /Admin/CreateTeamMember
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTeamMember(TeamMember member)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Thêm nhân viên";
            return View(member);
        }

        _db.TeamMembers.Add(member);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm nhân viên thành công!";
        return RedirectToAction(nameof(Team));
    }

    // ============ SỬA ============
    // GET /Admin/EditTeamMember/5
    public async Task<IActionResult> EditTeamMember(int id)
    {
        var member = await _db.TeamMembers.FindAsync(id);
        if (member == null) return NotFound();

        ViewData["Title"] = "Sửa nhân viên";
        return View(member);
    }

    // POST /Admin/EditTeamMember/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTeamMember(int id, TeamMember member)
    {
        if (id != member.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Sửa nhân viên";
            return View(member);
        }

        _db.TeamMembers.Update(member);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật nhân viên!";
        return RedirectToAction(nameof(Team));
    }

    // ============ XÓA ============
    // POST /Admin/DeleteTeamMember
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTeamMember(int id)
    {
        var member = await _db.TeamMembers.FindAsync(id);
        if (member != null)
        {
            _db.TeamMembers.Remove(member);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa nhân viên!";
        }
        return RedirectToAction(nameof(Team));
    }
    // ============ XEM DANH SÁCH ============
    // GET /Admin/BlogPosts
    public async Task<IActionResult> BlogPosts()
    {
        ViewData["Title"] = "Quản lý Blog";
        var posts = await _db.BlogPosts.ToListAsync();
        return View(posts);
    }

    // ============ THÊM MỚI ============
    // GET /Admin/CreateBlogPost
    public IActionResult CreateBlogPost()
    {
        ViewData["Title"] = "Thêm bài viết";
        return View();
    }

    // POST /Admin/CreateBlogPost
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBlogPost(BlogPost post)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Thêm bài viết";
            return View(post);
        }

        _db.BlogPosts.Add(post);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm bài viết thành công!";
        return RedirectToAction(nameof(BlogPosts));
    }

    // ============ SỬA ============
    // GET /Admin/EditBlogPost/5
    public async Task<IActionResult> EditBlogPost(int id)
    {
        var post = await _db.BlogPosts.FindAsync(id);
        if (post == null) return NotFound();

        ViewData["Title"] = "Sửa bài viết";
        return View(post);
    }

    // POST /Admin/EditBlogPost/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBlogPost(int id, BlogPost post)
    {
        if (id != post.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Sửa bài viết";
            return View(post);
        }

        _db.BlogPosts.Update(post);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật bài viết!";
        return RedirectToAction(nameof(BlogPosts));
    }

    // ============ XÓA ============
    // POST /Admin/DeleteBlogPost
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBlogPost(int id)
    {
        var post = await _db.BlogPosts.FindAsync(id);
        if (post != null)
        {
            _db.BlogPosts.Remove(post);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa bài viết!";
        }
        return RedirectToAction(nameof(BlogPosts));
    }
    // ============ XEM DANH SÁCH ============
    // GET /Admin/Faqs
    public async Task<IActionResult> Faqs()
    {
        ViewData["Title"] = "Quản lý FAQ";
        var faqs = await _db.FaqItems.ToListAsync();
        return View(faqs);
    }

    // ============ THÊM MỚI ============
    // GET /Admin/CreateFaq
    public IActionResult CreateFaq()
    {
        ViewData["Title"] = "Thêm câu hỏi";
        return View();
    }

    // POST /Admin/CreateFaq
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFaq(FaqItem faq)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Thêm câu hỏi";
            return View(faq);
        }

        _db.FaqItems.Add(faq);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm câu hỏi thành công!";
        return RedirectToAction(nameof(Faqs));
    }

    // ============ SỬA ============
    // GET /Admin/EditFaq/5
    public async Task<IActionResult> EditFaq(int id)
    {
        var faq = await _db.FaqItems.FindAsync(id);
        if (faq == null) return NotFound();

        ViewData["Title"] = "Sửa câu hỏi";
        return View(faq);
    }

    // POST /Admin/EditFaq/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFaq(int id, FaqItem faq)
    {
        if (id != faq.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Sửa câu hỏi";
            return View(faq);
        }

        _db.FaqItems.Update(faq);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật câu hỏi!";
        return RedirectToAction(nameof(Faqs));
    }

    // ============ XÓA ============
    // POST /Admin/DeleteFaq
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFaq(int id)
    {
        var faq = await _db.FaqItems.FindAsync(id);
        if (faq != null)
        {
            _db.FaqItems.Remove(faq);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa câu hỏi!";
        }
        return RedirectToAction(nameof(Faqs));
    }
}