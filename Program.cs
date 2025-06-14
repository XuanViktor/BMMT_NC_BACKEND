using BMMT_NC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Cấu hình DbContext + Ghi log SQL
builder.Services.AddDbContext<CsdlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
           .LogTo(Console.WriteLine, LogLevel.Information)  // Log SQL ra console
           .EnableSensitiveDataLogging()                    // Cho phép log dữ liệu đầu vào
);

// ✅ 2. Cấu hình Controller & MVC
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

builder.Services.AddControllersWithViews();

// ✅ 3. Cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "BMMT.Auth";
        options.Cookie.HttpOnly = true;

        if (builder.Environment.IsDevelopment())
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            options.Cookie.SameSite = SameSiteMode.Lax;
        }
        else
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
        }

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
    });

// ✅ 4. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsBuilder =>
    {
        corsBuilder.SetIsOriginAllowed(_ => true)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
    });
});

var app = builder.Build();

// ✅ 5. Test kết nối CSDL lúc khởi động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CsdlContext>();
    try
    {
        Console.WriteLine("⏳ Đang kiểm tra kết nối đến SQL Server...");
        if (db.Database.CanConnect())
        {
            Console.WriteLine("✅ Kết nối CSDL thành công!");
        }
        else
        {
            Console.WriteLine("❌ Không thể kết nối đến CSDL!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Lỗi khi kết nối CSDL: " + ex.Message);
    }
}

// ✅ 6. Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication(); // ⚠️ Bắt buộc để cookie hoạt động
app.UseAuthorization();

app.MapControllers();

// ✅ Map route mặc định cho MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();