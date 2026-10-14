using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Sem_Fashion_Kids_Clothing_Website.Models
{
    // =========================
    // APPLICATION USER
    // =========================
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }

        // Navigation property
        public List<Order>? Orders { get; set; }
    }

    // =========================
    // CATEGORY
    // =========================
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        public List<Product>? Products { get; set; }
    }

    // =========================
    // PRODUCT
    // =========================
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Price (ETB)")]
        [Range(1, 100000)]
        public decimal Price { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public bool IsFeatured { get; set; } = true;

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
    }

    // =========================
    // CART ITEM
    // =========================
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; } = 1;

        // =========================
        // USER RELATION
        // =========================
        public string UserId { get; set; }

        public ApplicationUser? User { get; set; }
    }

    // =========================
    // ORDER
    // =========================
    public class Order
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        // Filled automatically in controller
        public string? CustomerName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime OrderDate { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public string? PaymentReference { get; set; }

        public bool IsPaid { get; set; } = false;

        public string? TelebirrTransactionId { get; set; }

        public string? PaymentProofImage { get; set; }
    }
    public class Review
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Please enter your review.")]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Please select a rating.")]
        public int Rating { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }
    // =========================
    // REGISTER VIEW MODEL
    // =========================
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}