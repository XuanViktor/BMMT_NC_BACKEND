using BMMT_NC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Đăng ký DbContext
builder.Services.AddDbContext<CsdlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ✅ 2. Đăng ký Controller + View
builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // Cho API

// ✅ 3. Đăng ký Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.Cookie.Name = "AuthCookie";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// ✅ 4. Build App sau khi đăng ký service xong
var app = builder.Build();

// ✅ 5. Cấu hình Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // phải đặt trước UseAuthorization
app.UseAuthorization();

// ✅ 6. Map route cho API controller
app.MapControllers();

// ✅ 7. Map route cho MVC Razor (nếu có)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ✅ 8. Run App
app.Run();
