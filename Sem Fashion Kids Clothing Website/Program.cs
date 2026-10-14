using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sem_Fashion_Kids_Clothing_Website.Models;

var builder = WebApplication.CreateBuilder(args);

// =========================
// DATABASE
// =========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
// IDENTITY
// =========================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// =========================
// MVC
// =========================
builder.Services.AddControllersWithViews();

// =========================
// SESSION
// =========================
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =========================
// TELEBIRR SERVICE
// =========================
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// =========================
// BUILD APP
// =========================
var app = builder.Build();

// =========================
// ERROR HANDLING
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// =========================
// SESSION
// =========================
app.UseSession();

// =========================
// AUTHENTICATION
// =========================
app.UseAuthentication();
app.UseAuthorization();

// =========================
// SEED ROLES + USERS
// =========================
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

// =========================
// ROUTES
// =========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();