using Ban_hang.Data;
using Ban_hang.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace Ban_hang.Controllers;

[Authorize(Roles = "Admin")] // Chỉ Admin mới vào được
public class AdminController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    public AdminController(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // GET /Admin?range=7days
    public async Task<IActionResult> Index(string range = "7days")
    {
        ViewData["Title"] = "Dashboard";
        ViewData["CurrentRange"] = range;

        DateTime rangeStart;
        var today = DateTime.Now.Date;

        switch (range)
        {
            case "30days":
                rangeStart = today.AddDays(-29);
                break;
            case "thismonth":
                rangeStart = new DateTime(today.Year, today.Month, 1);
                break;
            case "thisyear":
                rangeStart = new DateTime(today.Year, 1, 1);
                break;
            default: // "7days"
                rangeStart = today.AddDays(-6);
                range = "7days";
                break;
        }

        // Toàn bộ query bên dưới lọc theo rangeStart thay vì cố định 7 ngày
        var totalBookings = await _db.Bookings.CountAsync(b => b.CreatedAt.Date >= rangeStart);
        var pendingBookings = await _db.Bookings.CountAsync(b => b.CreatedAt.Date >= rangeStart && b.Status == "Chờ xác nhận");
        var completedBookings = await _db.Bookings.CountAsync(b => b.CreatedAt.Date >= rangeStart && b.Status == "Hoàn thành");
        var cancelledBookings = await _db.Bookings.CountAsync(b => b.CreatedAt.Date >= rangeStart && b.Status == "Đã hủy");
        var confirmedBookings = await _db.Bookings.CountAsync(b => b.CreatedAt.Date >= rangeStart && b.Status == "Đã xác nhận");

        ViewData["TotalBookings"] = totalBookings;
        ViewData["PendingBookings"] = pendingBookings;
        ViewData["CompletedBookings"] = completedBookings;
        ViewData["CancelledBookings"] = cancelledBookings;

        // Biểu đồ cột: số ngày hiển thị tùy theo range (tối đa hiển thị gọn 14 điểm cho 30 ngày)
        var totalDays = (today - rangeStart).Days + 1;
        var daysList = Enumerable.Range(0, totalDays)
            .Select(i => rangeStart.AddDays(i))
            .ToList();

        var bookingsByDay = daysList.Select(day => new
        {
            Date = day.ToString("dd/MM"),
            Count = _db.Bookings.Count(b => b.CreatedAt.Date == day)
        }).ToList();

        ViewData["DaysLabels"] = System.Text.Json.JsonSerializer.Serialize(bookingsByDay.Select(x => x.Date));
        ViewData["DaysCounts"] = System.Text.Json.JsonSerializer.Serialize(bookingsByDay.Select(x => x.Count));

        ViewData["StatusPending"] = pendingBookings;
        ViewData["StatusConfirmed"] = confirmedBookings;
        ViewData["StatusCompleted"] = completedBookings;
        ViewData["StatusCancelled"] = cancelledBookings;

        var topServices = await _db.Bookings
            .Where(b => b.CreatedAt.Date >= rangeStart)
            .GroupBy(b => b.ServiceType)
            .Select(g => new { Service = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        ViewData["ServiceLabels"] = System.Text.Json.JsonSerializer.Serialize(topServices.Select(x => x.Service));
        ViewData["ServiceCounts"] = System.Text.Json.JsonSerializer.Serialize(topServices.Select(x => x.Count));

        var completionRate = totalBookings > 0 ? Math.Round((double)completedBookings / totalBookings * 100, 1) : 0;
        ViewData["CompletionRate"] = completionRate;

        var topPetType = await _db.Bookings
            .Where(b => b.CreatedAt.Date >= rangeStart)
            .GroupBy(b => b.PetType)
            .Select(g => new { PetType = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync();

        ViewData["TopPetType"] = topPetType?.PetType ?? "N/A";
        ViewData["TopPetCount"] = topPetType?.Count ?? 0;

        var recentBookings = await _db.Bookings
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .ToListAsync();
        ViewData["RecentBookings"] = recentBookings;

        var allBookingsInRange = await _db.Bookings.Where(b => b.CreatedAt.Date >= rangeStart).ToListAsync();

        var repeatCustomers = allBookingsInRange
            .GroupBy(b => b.OwnerPhone)
            .Where(g => g.Count() > 1)
            .Select(g => new { Phone = g.Key, Name = g.First().OwnerName, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();
        ViewData["RepeatCustomers"] = repeatCustomers;

        ViewData["TotalServices"] = await _db.Services.CountAsync();
        ViewData["TotalTeam"] = await _db.TeamMembers.CountAsync();
        ViewData["TotalBlogs"] = await _db.BlogPosts.CountAsync();
        ViewData["TotalFaqs"] = await _db.FaqItems.CountAsync();
        ViewData["TotalTestimonials"] = await _db.Testimonials.CountAsync();
        ViewData["TotalPriceTiers"] = await _db.PriceTiers.CountAsync();

        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

        ViewData["BookingsToday"] = await _db.Bookings.CountAsync(b => b.CreatedAt.Date == today);
        ViewData["BookingsThisWeek"] = await _db.Bookings.CountAsync(b => b.CreatedAt.Date >= startOfWeek);
        ViewData["BookingsThisMonth"] = await _db.Bookings.CountAsync(b => b.CreatedAt.Date >= startOfMonth);

        var firstBooking = await _db.Bookings.OrderBy(b => b.CreatedAt).FirstOrDefaultAsync();
        double avgPerDay = 0;
        if (firstBooking != null)
        {
            var daysSinceFirst = Math.Max(1, (today - firstBooking.CreatedAt.Date).Days + 1);
            var allTimeTotal = await _db.Bookings.CountAsync();
            avgPerDay = Math.Round((double)allTimeTotal / daysSinceFirst, 1);
        }
        ViewData["AvgBookingsPerDay"] = avgPerDay;

        var peakHour = await _db.Bookings
            .GroupBy(b => b.BookTime.Hours)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync();
        ViewData["PeakHour"] = peakHour != null ? $"{peakHour.Hour}:00 - {peakHour.Hour + 1}:00" : "N/A";

        var allBookings = await _db.Bookings.ToListAsync();
        var peakDayOfWeek = allBookings
            .GroupBy(b => b.CreatedAt.DayOfWeek)
            .Select(g => new { Day = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        string[] vietnameseDays = { "Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7" };
        ViewData["PeakDayOfWeek"] = peakDayOfWeek != null ? vietnameseDays[(int)peakDayOfWeek.Day] : "N/A";

        var uniqueCustomers = await _db.Bookings.Select(b => b.OwnerPhone).Distinct().CountAsync();
        ViewData["TotalCustomers"] = uniqueCustomers;

        var newCustomersThisMonth = allBookings
            .GroupBy(b => b.OwnerPhone)
            .Count(g => g.Min(b => b.CreatedAt) >= startOfMonth);
        ViewData["NewCustomersThisMonth"] = newCustomersThisMonth;

        var repeatCount = allBookings.GroupBy(b => b.OwnerPhone).Count(g => g.Count() > 1);
        double retentionRate = uniqueCustomers > 0 ? Math.Round((double)repeatCount / uniqueCustomers * 100, 1) : 0;
        ViewData["RetentionRate"] = retentionRate;

        var leastService = await _db.Bookings
            .GroupBy(b => b.ServiceType)
            .Select(g => new { Service = g.Key, Count = g.Count() })
            .OrderBy(x => x.Count)
            .FirstOrDefaultAsync();
        ViewData["LeastService"] = leastService?.Service ?? "N/A";

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
    // ============ XEM DANH SÁCH ĐƠN HÀNG ============
    // GET /Admin/Orders
    public async Task<IActionResult> Orders()
    {
        ViewData["Title"] = "Quản lý Đơn hàng";
        var orders = await _db.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return View(orders);
    }

    // GET /Admin/OrderDetail/5
    public async Task<IActionResult> OrderDetail(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        ViewData["Title"] = "Chi tiết đơn hàng #" + order.Id;
        return View(order);
    }

    // ============ ĐỔI TRẠNG THÁI ============
    // POST /Admin/UpdateOrderStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, string status)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order != null)
        {
            order.Status = status;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật trạng thái đơn hàng!";
        }
        return RedirectToAction(nameof(Orders));
    }

    // ============ XÓA ============
    // POST /Admin/DeleteOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order != null)
        {
            _db.OrderItems.RemoveRange(order.Items);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa đơn hàng!";
        }
        return RedirectToAction(nameof(Orders));
    }
    // ============ XEM DANH SÁCH NGƯỜI DÙNG ============
    // GET /Admin/Users
    public async Task<IActionResult> Users()
    {
        ViewData["Title"] = "Quản lý Tài khoản";

        var allUsers = await _userManager.Users.ToListAsync();
        var userList = new List<AdminUserViewModel>();

        foreach (var u in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(u);
            var bookingCount = await _db.Bookings.CountAsync(b => b.OwnerPhone == u.PhoneNumber);
            var orderCount = await _db.Orders.CountAsync(o => o.UserId == u.Id);

            userList.Add(new AdminUserViewModel
            {
                Id = u.Id,
                UserName = u.UserName ?? "",
                Email = u.Email ?? "",
                PhoneNumber = u.PhoneNumber ?? "Chưa cập nhật",
                IsAdmin = roles.Contains("Admin"),
                TotalBookings = bookingCount,
                TotalOrders = orderCount
            });
        }

        return View(userList);
    }

    // ============ KHÓA / MỞ KHÓA TÀI KHOẢN ============
    // POST /Admin/ToggleLockUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = "Đã mở khóa tài khoản!";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["Success"] = "Đã khóa tài khoản!";
            }
        }
        return RedirectToAction(nameof(Users));
    }

    // ============ XÓA TÀI KHOẢN ============
    // POST /Admin/DeleteUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                TempData["Success"] = "Không thể xóa tài khoản Admin!";
                return RedirectToAction(nameof(Users));
            }

            await _userManager.DeleteAsync(user);
            TempData["Success"] = "Đã xóa tài khoản!";
        }
        return RedirectToAction(nameof(Users));
    }
    // ============ XEM DANH SÁCH SẢN PHẨM ============
    // GET /Admin/Products
    public async Task<IActionResult> Products()
    {
        ViewData["Title"] = "Quản lý Sản phẩm";
        var products = await _db.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.Id)
            .ToListAsync();
        return View(products);
    }

    // ============ THÊM MỚI ============
    // GET /Admin/CreateProduct
    public async Task<IActionResult> CreateProduct()
    {
        ViewData["Title"] = "Thêm sản phẩm";
        ViewData["Categories"] = await _db.ProductCategories.ToListAsync();
        return View();
    }

    // POST /Admin/CreateProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Thêm sản phẩm";
            ViewData["Categories"] = await _db.ProductCategories.ToListAsync();
            return View(product);
        }

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm sản phẩm thành công!";
        return RedirectToAction(nameof(Products));
    }

    // ============ SỬA ============
    // GET /Admin/EditProduct/5
    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();

        ViewData["Title"] = "Sửa sản phẩm";
        ViewData["Categories"] = await _db.ProductCategories.ToListAsync();
        return View(product);
    }

    // POST /Admin/EditProduct/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(int id, Product product)
    {
        if (id != product.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Sửa sản phẩm";
            ViewData["Categories"] = await _db.ProductCategories.ToListAsync();
            return View(product);
        }

        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật sản phẩm!";
        return RedirectToAction(nameof(Products));
    }

    // ============ XÓA ============
    // POST /Admin/DeleteProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product != null)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa sản phẩm!";
        }
        return RedirectToAction(nameof(Products));
    }

    // ============ QUẢN LÝ DANH MỤC SẢN PHẨM ============
    // GET /Admin/Categories
    public async Task<IActionResult> Categories()
    {
        ViewData["Title"] = "Quản lý Danh mục";
        var categories = await _db.ProductCategories.ToListAsync();
        return View(categories);
    }

    // POST /Admin/CreateCategory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(ProductCategory category)
    {
        if (!string.IsNullOrWhiteSpace(category.Name))
        {
            _db.ProductCategories.Add(category);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã thêm danh mục!";
        }
        return RedirectToAction(nameof(Categories));
    }

    // POST /Admin/DeleteCategory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _db.ProductCategories.FindAsync(id);
        if (category != null)
        {
            var hasProducts = await _db.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                TempData["Success"] = "Không thể xóa danh mục đang có sản phẩm!";
            }
            else
            {
                _db.ProductCategories.Remove(category);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã xóa danh mục!";
            }
        }
        return RedirectToAction(nameof(Categories));
    }
    // ============ AJAX: LẤY DANH SÁCH DỊCH VỤ (JSON) ============
    [HttpGet]
    public async Task<IActionResult> GetServicesJson()
    {
        var services = await _db.Services.ToListAsync();
        return Json(services);
    }

    // ============ AJAX: THÊM DỊCH VỤ ============
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> AjaxCreateService([FromBody] ServiceItem service)
    {
        if (string.IsNullOrWhiteSpace(service.Title))
        {
            return BadRequest(new { message = "Tên dịch vụ không được để trống." });
        }

        _db.Services.Add(service);
        await _db.SaveChangesAsync();
        return Json(new { success = true, message = "Đã thêm dịch vụ!", data = service });
    }

    // ============ AJAX: SỬA DỊCH VỤ ============
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> AjaxEditService([FromBody] ServiceItem service)
    {
        var existing = await _db.Services.FindAsync(service.Id);
        if (existing == null) return NotFound();

        existing.IconKey = service.IconKey;
        existing.Title = service.Title;
        existing.Description = service.Description;
        existing.Price = service.Price;

        await _db.SaveChangesAsync();
        return Json(new { success = true, message = "Đã cập nhật dịch vụ!" });
    }

    // ============ AJAX: XÓA DỊCH VỤ ============
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> AjaxDeleteService(int id)
    {
        var service = await _db.Services.FindAsync(id);
        if (service != null)
        {
            _db.Services.Remove(service);
            await _db.SaveChangesAsync();
        }
        return Json(new { success = true, message = "Đã xóa dịch vụ!" });
    }
}