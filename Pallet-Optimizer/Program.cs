using Pallet_Optimizer.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Tilføj HttpContextAccessor (nødvendig for session i views)
builder.Services.AddHttpContextAccessor();

// Tilføj session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IPalletRepository, InMemoryPalletRepository>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Tilføj session middleware
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();