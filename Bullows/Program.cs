
using Bullows.Business;
using Bullows.Database;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var config = builder.Configuration;

// Register the DbContext with the DI container
//builder.Services.AddDbContext<BullowsDbContext>(options =>
//    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<BullowsDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    }));

// Register other services
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IUnitOfWork, UnitOfWorks>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<PaintBoothDesign>(); // Register PaintBoothDesign

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "UserLoginCookie";
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", config =>
{
    config.Cookie.Name = "SimpleAuthAuthe2.Cookie";
    config.LoginPath = "/LogOn/LogIn";
    config.AccessDeniedPath = "/LogOn/LogIn";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogOn}/{action=LogIn}/{id?}");

app.Run();
