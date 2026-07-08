using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ban_hang.Data;
using Ban_hang.Models;

namespace Ban_hang.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public CartController(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId!);

        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var bookings = new List<Booking>();
        if (!string.IsNullOrEmpty(user?.PhoneNumber))
        {
            bookings = await _db.Bookings
                .Where(b => b.OwnerPhone == user.PhoneNumber)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToListAsync();
        }

        var vm = new CartPageViewModel
        {
            CartItems = items,
            RecentBookings = bookings
        };

        return View(vm);
    }

    // POST /Cart/Add
    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var userId = _userManager.GetUserId(User);

        var product = await _db.Products.FindAsync(productId);
        if (product == null || !product.IsActive)
        {
            return Json(new { success = false, message = "Sản phẩm không tồn tại." });
        }

        var existing = await _db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _db.CartItems.Add(new CartItem
            {
                UserId = userId!,
                ProductId = productId,
                Quantity = quantity
            });
        }

        await _db.SaveChangesAsync();

        var cartCount = await _db.CartItems.Where(c => c.UserId == userId).SumAsync(c => c.Quantity);

        return Json(new { success = true, message = "Đã thêm vào giỏ hàng!", cartCount });
    }

    // POST /Cart/UpdateQuantity
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        var userId = _userManager.GetUserId(User);
        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

        if (item == null) return NotFound();

        if (quantity <= 0)
        {
            _db.CartItems.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST /Cart/Remove
    [HttpPost]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var userId = _userManager.GetUserId(User);
        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

        if (item != null)
        {
            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ApplyVoucher(string code)
    {
        var userId = _userManager.GetUserId(User);
        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var total = items.Sum(c => c.Product!.Price * c.Quantity);

        var voucher = await _db.Vouchers
            .FirstOrDefaultAsync(v => v.Code.ToUpper() == code.ToUpper() && v.IsActive);

        if (voucher == null)
        {
            return Json(new { success = false, message = "Mã giảm giá không tồn tại hoặc đã hết hạn." });
        }

        if (voucher.ExpiryDate < DateTime.Now)
        {
            return Json(new { success = false, message = "Mã giảm giá đã hết hạn." });
        }

        if (voucher.MaxUses.HasValue && voucher.UsedCount >= voucher.MaxUses.Value)
        {
            return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng." });
        }

        if (total < voucher.MinOrderAmount)
        {
            return Json(new { success = false, message = $"Đơn hàng cần tối thiểu {voucher.MinOrderAmount:N0} đ để áp dụng mã này." });
        }

        decimal discount;
        if (voucher.DiscountType == "percent")
        {
            discount = total * voucher.DiscountValue / 100;
            if (voucher.MaxDiscountAmount.HasValue && discount > voucher.MaxDiscountAmount.Value)
            {
                discount = voucher.MaxDiscountAmount.Value;
            }
        }
        else
        {
            discount = voucher.DiscountValue;
        }

        // Lưu tạm vào session để Checkout dùng lại
        HttpContext.Session.SetString("VoucherCode", voucher.Code);
        HttpContext.Session.SetString("VoucherDiscount", discount.ToString());

        var newTotal = total - discount;

        return Json(new
        {
            success = true,
            message = $"Áp dụng mã \"{voucher.Code}\" thành công!",
            discount = discount.ToString("N0"),
            newTotal = newTotal.ToString("N0"),
            voucherCode = voucher.Code
        });
    }

    [HttpPost]
    public IActionResult RemoveVoucher()
    {
        HttpContext.Session.Remove("VoucherCode");
        HttpContext.Session.Remove("VoucherDiscount");
        return Json(new { success = true });
    }
}