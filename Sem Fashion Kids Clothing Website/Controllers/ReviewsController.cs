using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem_Fashion_Kids_Clothing_Website.Models;
using System.Security.Claims;

namespace Sem_Fashion_Kids_Clothing_Website.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // CUSTOMER CREATE REVIEW GET
        // =========================
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var reviews = _context.Reviews
                .Include(r => r.User)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();

            return View(reviews);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }


        // =========================
        // CUSTOMER CREATE REVIEW POST
        // =========================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            // Set values that are not submitted by the form
            review.UserId = userId;
            review.IsApproved = false;
            review.ReviewDate = DateTime.Now;

            // Remove validation for properties that are set in code
            ModelState.Remove(nameof(Review.UserId));
            ModelState.Remove(nameof(Review.User));

            // If validation fails, show all errors
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}")
                    .ToList();

                return Content(string.Join("\n", errors));
            }

            try
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Thank you! Your review was submitted and is waiting for approval.";

                return RedirectToAction(nameof(MyReviews));
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        // =========================
        // CUSTOMER OWN REVIEWS
        // =========================
        [Authorize]
        public IActionResult MyReviews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reviews = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();

            return View(reviews);
        }


        // =========================
        // EDIT REVIEW
        // =========================
        [Authorize]
        public IActionResult Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var review = _context.Reviews
                .FirstOrDefault(r => r.Id == id && r.UserId == userId);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Review review)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingReview = _context.Reviews
                .FirstOrDefault(r =>
                    r.Id == review.Id &&
                    r.UserId == userId);

            if (existingReview == null)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                existingReview.Comment = review.Comment;
                existingReview.Rating = review.Rating;

                // Send again for approval
                existingReview.IsApproved = false;

                _context.SaveChanges();

                TempData["SuccessMessage"] =
                    "Review updated and waiting for approval.";

                return RedirectToAction(nameof(MyReviews));
            }

            return View(review);
        }


        // =========================
        // DELETE REVIEW
        // =========================
        // =========================
        // ADMIN DELETE REVIEW
        // =========================
        [Authorize(Roles = "Admin")]
        // =========================
        // ADMIN DELETE REVIEW
        // =========================
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAdmin(int id)
        {
            var review = _context.Reviews
                .FirstOrDefault(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            _context.SaveChanges();

            TempData["SuccessMessage"] =
                "Review deleted successfully.";

            return RedirectToAction(nameof(Index));
        }



        // =========================
        // CUSTOMER DELETE REVIEW GET
        // =========================
        [Authorize]
        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var review = _context.Reviews
                .FirstOrDefault(r =>
                    r.Id == id &&
                    r.UserId == userId);

            if (review == null)
            {
                return NotFound();
            }

            // Professional rule:
            // Customer cannot delete approved reviews
            if (review.IsApproved)
            {
                TempData["ErrorMessage"] =
                    "Approved reviews cannot be deleted. Please contact support.";

                return RedirectToAction(nameof(MyReviews));
            }

            return View(review);
        }



        // =========================
        // CUSTOMER DELETE REVIEW POST
        // =========================
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var review = _context.Reviews
                .FirstOrDefault(r =>
                    r.Id == id &&
                    r.UserId == userId);


            if (review == null)
            {
                return NotFound();
            }


            // Do not allow deleting approved reviews
            if (review.IsApproved)
            {
                TempData["ErrorMessage"] =
                    "Approved reviews cannot be deleted.";

                return RedirectToAction(nameof(MyReviews));
            }


            _context.Reviews.Remove(review);
            _context.SaveChanges();


            TempData["SuccessMessage"] =
                "Your review was deleted successfully.";


            return RedirectToAction(nameof(MyReviews));
        }


        // =========================
        // ADMIN VIEW ALL REVIEWS
        // =========================



        // =========================
        // ADMIN APPROVE
        // =========================
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            var review = _context.Reviews
                .FirstOrDefault(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            review.IsApproved = true;

            _context.SaveChanges();

            TempData["SuccessMessage"] =
                "Review approved.";

            return RedirectToAction(nameof(Index));
        }



        // =========================
        // ADMIN REJECT
        // =========================
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id)
        {
            var review = _context.Reviews
                .FirstOrDefault(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            review.IsApproved = false;

            _context.SaveChanges();

            TempData["SuccessMessage"] =
                "Review rejected.";

            return RedirectToAction(nameof(Index));
        }
    }
}