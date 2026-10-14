using Microsoft.AspNetCore.Mvc;
using Sem_Fashion_Kids_Clothing_Website.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Sem_Fashion_Kids_Clothing_Website.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LIST ALL CATEGORIES
        // =========================
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            try
            {
                var categories = _context.Categories.ToList();

                if (!categories.Any())
                {
                    TempData["ErrorMessage"] = "⚠️ No categories found.";
                }

                return View(categories);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "❌ Error loading categories.";
                return View(new List<Category>());
            }
        }

        // =========================
        // CREATE (GET)
        // =========================
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "❌ Invalid data. Please check your input.";
                    return View(category);
                }

                _context.Categories.Add(category);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "✅ Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "❌ Failed to create category.";
                return View(category);
            }
        }

        // =========================
        // EDIT (GET)
        // =========================
        public IActionResult Edit(int id)
        {
            try
            {
                var category = _context.Categories.Find(id);

                if (category == null)
                {
                    TempData["ErrorMessage"] = "❌ Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(category);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "❌ Error loading category.";
                return RedirectToAction(nameof(Index));
            }
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "❌ Invalid data. Update failed.";
                    return View(category);
                }

                _context.Categories.Update(category);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "✅ Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "❌ Failed to update category.";
                return View(category);
            }
        }

        // =========================
        // DELETE
        // =========================
        public IActionResult Delete(int id)
        {
            try
            {
                var category = _context.Categories.Find(id);

                if (category == null)
                {
                    TempData["ErrorMessage"] = "❌ Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "🗑️ Category deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "❌ Error deleting category.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}