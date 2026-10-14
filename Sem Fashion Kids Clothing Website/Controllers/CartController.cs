using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem_Fashion_Kids_Clothing_Website.Models;
using System.Security.Claims;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================
    // ADD TO CART
    // =========================
    public async Task<IActionResult> AddToCart(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            TempData["ErrorMessage"] = "❌ Product not found.";
            return RedirectToAction("Index", "Products");
        }

        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.ProductId == id && c.UserId == userId);

        if (item == null)
        {
            await _context.CartItems.AddAsync(new CartItem
            {
                ProductId = id,
                Quantity = 1,
                UserId = userId
            });

            TempData["SuccessMessage"] = $"✅ {product.Name} added to cart!";
        }
        else
        {
            item.Quantity += 1;
            TempData["SuccessMessage"] = $"🔄 {product.Name} quantity updated!";
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // CART LIST
    // =========================
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cart = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cart.Any())
        {
            TempData["ErrorMessage"] = "🛒 Your cart is empty.";
        }

        return View(cart);
    }

    // =========================
    // REMOVE ITEM (SECURE)
    // =========================
    public async Task<IActionResult> Remove(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var item = await _context.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (item == null)
        {
            TempData["ErrorMessage"] = "❌ Item not found.";
            return RedirectToAction(nameof(Index));
        }

        var productName = item.Product?.Name ?? "Item";

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"🗑️ {productName} removed from cart!";
        return RedirectToAction(nameof(Index));
    }

    // =========================
    // CLEAR CART
    // =========================
    public async Task<IActionResult> Clear()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cart = await _context.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cart.Any())
        {
            TempData["ErrorMessage"] = "⚠️ Cart is already empty.";
            return RedirectToAction(nameof(Index));
        }

        _context.CartItems.RemoveRange(cart);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "🧹 Cart cleared successfully!";
        return RedirectToAction(nameof(Index));
    }
}