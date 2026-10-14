using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem_Fashion_Kids_Clothing_Website.Models;

namespace Sem_Fashion_Kids_Clothing_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // HOME PAGE
        // =========================
        public IActionResult Index(int? categoryId)
        {
            try
            {
                var productsQuery = _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsFeatured);

                if (categoryId.HasValue)
                {
                    productsQuery = productsQuery
                        .Where(p => p.CategoryId == categoryId.Value);
                }

                var products = productsQuery
                    .OrderByDescending(p => p.Id)
                    .Take(12)
                    .ToList();

                ViewBag.ProductCount = _context.Products.Count(p => p.IsFeatured);
                ViewBag.SelectedCategory = categoryId;
                ViewBag.CustomerCount = _context.Orders
              .Select(o => o.UserId)
              .Distinct()
              .Count();
              ViewBag.Reviews = _context.Reviews
             .Include(r => r.User)
             .Where(r => r.IsApproved)
             .OrderByDescending(r => r.ReviewDate)
             .Take(6)
             .ToList();
                if (!products.Any())
                {
                    TempData["ErrorMessage"] = "No featured products available.";
                }

                return View(products);
            }
            catch
            {
                TempData["ErrorMessage"] = "Error loading products.";
                return View(new List<Product>());
            }
        }

        // =========================
        // ABOUT
        // =========================
        public IActionResult About()
        {
            return View();
        }

        // =========================
        // CONTACT (GET)
        // =========================
        public IActionResult Contact()
        {
            return View();
        }

        // =========================
        // CONTACT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string Name, string Email, string Message)
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Message))
            {
                TempData["ErrorMessage"] = "? All fields are required.";
                return RedirectToAction("Contact");
            }

            TempData["SuccessMessage"] = "? Message sent successfully!";
            return RedirectToAction("Index");
        }
    }
}