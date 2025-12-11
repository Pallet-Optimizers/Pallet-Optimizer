using Pallet_Optimizer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HttpContextAccessor for filters/layouts that need HttpContext
builder.Services.AddHttpContextAccessor();

// Register session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".PalletOptimizer.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});

// register both concrete repositories and expose the hybrid as IPalletRepository
builder.Services.AddScoped<InMemoryPalletRepository>();
builder.Services.AddScoped<EfPalletRepository>();
builder.Services.AddScoped<IPalletRepository, HybridPalletRepository>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Enable session after routing
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}");

app.Run();