using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem_Fashion_Kids_Clothing_Website.Models;
using System.Security.Claims;

[Authorize]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(
        ApplicationDbContext context,
        IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _env = env;
        _userManager = userManager;
    }

    // =========================
    // ADMIN: ALL ORDERS
    // =========================
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var orders = _context.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        return View(orders);
    }

    // =========================
    // CUSTOMER: MY ORDERS
    // =========================
    public async Task<IActionResult> MyOrders()
    {
        var user = await _userManager.GetUserAsync(User);

        var orders = _context.Orders
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        return View(orders);
    }

    // =========================
    // CHECKOUT GET
    // =========================
    public async Task<IActionResult> Checkout()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
            return RedirectToAction("Login", "Account");

        var user = await _userManager.GetUserAsync(User);

        var cart = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cart.Any())
        {
            TempData["ErrorMessage"] = "🛒 Your cart is empty!";
            return RedirectToAction("Index", "Cart");
        }

        ViewBag.Cart = cart;
        ViewBag.Total = cart.Sum(c => c.Product.Price * c.Quantity);

        // ✅ SAFE FULL NAME FALLBACK
        ViewBag.FullName =
            user?.FullName ??
            user?.UserName ??
            user?.Email;

        return View(new Order());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(Order order, IFormFile proofImage)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
            return RedirectToAction("Login", "Account");

        var user = await _userManager.GetUserAsync(User);

        var cart = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cart.Any())
        {
            TempData["ErrorMessage"] = "🛒 Your cart is empty!";
            return RedirectToAction("Index", "Cart");
        }

        // =========================
        // VALIDATION
        // =========================
        if (string.IsNullOrWhiteSpace(order.PaymentMethod))
        {
            ModelState.AddModelError("", "Please select payment method");
        }

        if ((order.PaymentMethod == "Bank" || order.PaymentMethod == "Telebirr") && proofImage == null)
        {
            ModelState.AddModelError("", "Payment receipt is required");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(c => c.Product.Price * c.Quantity);
            ViewBag.FullName = user?.FullName ?? user?.Email;

            return View(order);
        }

        // =========================
        // SAVE IMAGE FIRST (IMPORTANT FIX)
        // =========================
        string fileName = null;

        if (proofImage != null)
        {
            string folder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            fileName = Guid.NewGuid() + Path.GetExtension(proofImage.FileName);
            string path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await proofImage.CopyToAsync(stream);
            }
        }

        // =========================
        // CREATE ORDER (ALL DATA SET BEFORE SAVE)
        // =========================
        order.UserId = userId;
        order.CustomerName = user?.FullName ?? user?.Email;
        order.OrderDate = DateTime.Now;
        order.TotalAmount = cart.Sum(c => c.Product.Price * c.Quantity);
        order.Status = "PendingApproval";
        order.IsPaid = false;
        order.PaymentProofImage = fileName;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // =========================
        // CLEAR CART
        // =========================
        _context.CartItems.RemoveRange(cart);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Order placed successfully!";

        return RedirectToAction("Success", new { id = order.Id });
    }

    // =========================
    // APPROVE
    // =========================
    // =========================
    // APPROVE
    // =========================
    [Authorize(Roles = "Admin")]
    public IActionResult Approve(int id)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
            return NotFound();

        order.Status = "Approved";
        order.IsPaid = true;

        // Optional
        // order.ApprovedDate = DateTime.Now;

        _context.SaveChanges();

        TempData["SuccessMessage"] = $"Order #{order.Id} approved successfully.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // REJECT
    // =========================
    // =========================
    // REJECT
    // =========================
    [Authorize(Roles = "Admin")]
    public IActionResult Reject(int id)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
            return NotFound();

        order.Status = "Rejected";
        order.IsPaid = false;

        _context.SaveChanges();

        TempData["SuccessMessage"] = $"Order #{order.Id} rejected.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // DELETE
    // =========================
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var order = _context.Orders.Find(id);
        if (order == null) return NotFound();

        _context.Orders.Remove(order);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // =========================
    // DETAILS
    // =========================
    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);


        if (order == null)
            return NotFound();


        var userId = _userManager.GetUserId(User);


        if (!User.IsInRole("Admin") && order.UserId != userId)
        {
            return Forbid();
        }


        return View(order);
    }

    // =========================
    // SUCCESS
    // =========================
    public IActionResult Success(int id)
    {
        var order = _context.Orders
            .FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}