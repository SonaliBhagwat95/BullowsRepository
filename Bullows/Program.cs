using Bullows.Database;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.
builder.Services.AddControllersWithViews();

//Add database connection settings
var mySqlConStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<BullowsDbContext>(options => options.UseMySql(mySqlConStr, MySqlServerVersion.LatestSupportedServerVersion));


builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

builder.Services.AddScoped(serviceType: typeof(IUnitOfWork), implementationType: typeof(UnitOfWorks));
builder.Services.AddScoped(typeof(IRepository<>), implementationType: typeof(GenericRepository<>));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

#region Configure Session         
//Session Configure
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
// services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(30));
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "UserLoginCookie";
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.IsEssential = true;

});
//end Session
#endregion

#region Configure Claim Authentication
builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", config =>
{
    config.Cookie.Name = "SimpleAuthAuthe2.Cookie";
    config.LoginPath = "/LogOn/LogIn";
    config.AccessDeniedPath = "/LogOn/LogIn";
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogOn}/{action=LogIn}/{id?}");

app.Run();
