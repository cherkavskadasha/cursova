using CafeBase.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        //options.AccessDeniedPath = "/Home/AccessDenied";
    });

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CafeBaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();
builder.Services.AddAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

    endpoints.MapControllerRoute(
    name: "Home",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
    name: "MenuItemRoute",
    pattern: "MenuItem/{action=Index}/{id?}",
    defaults: new { controller = "MenuItems" });

    endpoints.MapControllerRoute(
    name: "CustomerRoute",
    pattern: "Customer/{action=Index}/{id?}",
    defaults: new { controller = "Customers" });

    endpoints.MapControllerRoute(
    name: "OrderRoute",
    pattern: "Order/{action=Index}/{id?}",
    defaults: new { controller = "Orders" });

    endpoints.MapControllerRoute(
    name: "PaymentRoute",
    pattern: "Payment/{action=Index}/{id?}",
    defaults: new { controller = "Payments" });

    endpoints.MapControllerRoute(
    name: "OrderDayRoute",
    pattern: "{controller=Home}/{action=OrderDay}/{id?}");

    endpoints.MapControllerRoute(
    name: "CustomerDayRoute",
    pattern: "{controller=Home}/{action=CustomerDay}/{id?}");

    endpoints.MapControllerRoute(
    name: "PaymentDayRoute",
    pattern: "{controller=Home}/{action=PaymentDay}/{id?}");

});

app.Run();
