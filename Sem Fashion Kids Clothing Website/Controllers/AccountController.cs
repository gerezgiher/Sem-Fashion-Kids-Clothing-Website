using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sem_Fashion_Kids_Clothing_Website.Models;
using System.Linq;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // =========================
    // LOGIN (GET)
    // =========================
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // =========================
    // LOGIN (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(
            email,
            password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Index", "Products");
            }

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Invalid login attempt";
        return View();
    }

    // =========================
    // REGISTER (GET)
    // =========================
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // =========================
    // REGISTER (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
     string fullName,
     string email,
     string password)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            ViewBag.Error = "Full Name is required";
            return View();
        }

        var user = new ApplicationUser
        {
            FullName = fullName,
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");

            TempData["Success"] =
                "Account created successfully. Please login.";

            return RedirectToAction("Login");
        }

        ViewBag.Error = string.Join(", ",
            result.Errors.Select(e => e.Description));

        return View();
    }

    // =========================
    // LOGOUT
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}