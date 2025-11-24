using Pallet_Optimizer.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Force PascalCase for JSON (matches C# property names)
        //options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddSingleton<IPalletRepository, InMemoryPalletRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pallet}/{action=Index}");

app.Run();
