using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ban_hang.Data;
using Ban_hang.Models;

namespace Ban_hang.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public CheckoutController(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // GET /Checkout
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId!);

        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!items.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var vm = new CheckoutViewModel
        {
            CartItems = items,
            Total = items.Sum(c => c.Product!.Price * c.Quantity),
            CustomerPhone = user?.PhoneNumber ?? ""
        };

        return View(vm);
    }

    // POST /Checkout/PlaceOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel form)
    {
        var userId = _userManager.GetUserId(User);

        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!items.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        if (!ModelState.IsValid)
        {
            form.CartItems = items;
            form.Total = items.Sum(c => c.Product!.Price * c.Quantity);
            return View("Index", form);
        }

        var order = new Order
        {
            UserId = userId!,
            CustomerName = form.CustomerName,
            CustomerPhone = form.CustomerPhone,
            ShippingAddress = form.ShippingAddress,
            Note = form.Note ?? "",
            TotalAmount = items.Sum(c => c.Product!.Price * c.Quantity),
            Status = "Chờ xác nhận",
            CreatedAt = DateTime.Now
        };

        foreach (var item in items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.Product!.Name,
                Price = item.Product.Price,
                Quantity = item.Quantity
            });

            // Trừ tồn kho
            item.Product.Stock = Math.Max(0, item.Product.Stock - item.Quantity);
        }

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(items); // xoá giỏ hàng sau khi đặt
        await _db.SaveChangesAsync();

        TempData["OrderSuccessId"] = order.Id;
        return RedirectToAction(nameof(Success), new { id = order.Id });
    }

    // GET /Checkout/Success/5
    public async Task<IActionResult> Success(int id)
    {
        var userId = _userManager.GetUserId(User);
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order == null) return NotFound();

        return View(order);
    }
}