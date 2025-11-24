using Pallet_Optimizer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// register both concrete repositories and expose the hybrid as IPalletRepository
builder.Services.AddScoped<InMemoryPalletRepository>();
builder.Services.AddScoped<EfPalletRepository>();
builder.Services.AddScoped<IPalletRepository, HybridPalletRepository>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pallet}/{action=Index}");

app.Run();

