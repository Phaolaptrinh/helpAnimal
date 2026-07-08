using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ban_hang.Data;
using Ban_hang.Models;

namespace Ban_hang.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(AppDbContext db, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // ============ ĐĂNG NHẬP (dùng chung Admin + Khách hàng) ============

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Tìm user theo Email trước, vì UserName có thể là tên hiển thị (không phải email)
        var userByEmail = await _userManager.FindByEmailAsync(model.Email);

        if (userByEmail == null)
        {
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            userByEmail.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var isAdmin = await _userManager.IsInRoleAsync(userByEmail, "Admin");

            if (isAdmin)
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
        return View(model);
    }

    // ============ ĐĂNG KÝ (chỉ dành cho khách hàng) ============

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Email này đã được đăng ký.");
            return View(model);
        }

        var newUser = new IdentityUser
        {
            UserName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.Phone,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(newUser, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    // ============ ĐĂNG XUẤT ============

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // ============ TRANG TÀI KHOẢN KHÁCH HÀNG ============

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId!);

        var orders = await _db.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var bookings = new List<Booking>();
        if (!string.IsNullOrEmpty(user?.PhoneNumber))
        {
            bookings = await _db.Bookings
                .Where(b => b.OwnerPhone == user.PhoneNumber)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        var validOrders = orders.Where(o => o.Status != "Đã huỷ").ToList();

        var vm = new AccountDashboardViewModel
        {
            FullName = user?.UserName ?? "",
            Email = user?.Email ?? "",
            Phone = user?.PhoneNumber ?? "Chưa cập nhật",
            TotalOrders = orders.Count,
            TotalBookings = bookings.Count,
            TotalSpentOrders = validOrders.Sum(o => o.TotalAmount),
            Orders = orders,
            Bookings = bookings
        };

        return View(vm);
    }

    // ============ HUỶ ĐƠN HÀNG ============

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var userId = _userManager.GetUserId(User);
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order != null && order.Status == "Chờ xác nhận")
        {
            order.Status = "Đã huỷ";
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
    // ============ QUÊN MẬT KHẨU ============

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("Email", "Không tìm thấy tài khoản với email này.");
            return View(model);
        }

        // Vì chưa có dịch vụ gửi email, tạo token rồi chuyển thẳng sang trang đặt lại mật khẩu
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return RedirectToAction(nameof(ResetPassword), new { email = model.Email, token = token });
    }

    // ============ ĐẶT LẠI MẬT KHẨU ============

    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction(nameof(ForgotPassword));
        }

        var model = new ResetPasswordViewModel { Email = email, Token = token };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Không tìm thấy tài khoản.");
            return View(model);
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

        if (result.Succeeded)
        {
            TempData["ResetSuccess"] = true;
            return RedirectToAction(nameof(Login));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }
}