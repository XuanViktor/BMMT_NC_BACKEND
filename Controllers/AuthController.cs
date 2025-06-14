using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.RegularExpressions;
using BMMT_NC.Models;
using Microsoft.AspNetCore.Authorization;

namespace BMMT_NC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CsdlContext _db;

        public AuthController(CsdlContext db)
        {
            _db = db;
        }

        // DTO đăng ký
        public class RegisterDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            dto.Email = dto.Email?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 6 ||
                !Regex.IsMatch(dto.Username, @"^[a-zA-Z0-9_.-]+$"))
            {
                return BadRequest("Tên đăng nhập phải có ít nhất 6 ký tự và chỉ chứa chữ, số, dấu _ . -");
            }

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8 ||
                !Regex.IsMatch(dto.Password, @"[A-Z]") ||
                !Regex.IsMatch(dto.Password, @"[a-z]") ||
                !Regex.IsMatch(dto.Password, @"[0-9]") ||
                !Regex.IsMatch(dto.Password, @"[\W_]"))
            {
                return BadRequest("Mật khẩu phải ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.");
            }

            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest("Tên đăng nhập đã tồn tại.");
            }

            if (!string.IsNullOrEmpty(dto.Email) &&
                await _db.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("Email đã được sử dụng.");
            }

            string hashedPassword = PasswordHasher.HashPassword(dto.Password);

            var newUser = new User
            {
                Username = dto.Username,
                Password = hashedPassword,
                Name = dto.Name,
                Email = dto.Email ?? "",
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công!" });
        }

        // DTO đăng nhập
        public class LoginDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            string hashedPassword = PasswordHasher.HashPassword(dto.Password);

            var user = await _db.Users.FirstOrDefaultAsync(x =>
                x.Username == dto.Username && x.Password == hashedPassword);

            if (user == null)
            {
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu!");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new { message = "Đăng nhập thành công!" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Đăng xuất thành công!" });
        }

        public class ChangePasswordDto
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var username = User.Identity?.Name;

            if (username == null)
            {
                return Unauthorized("Không xác định người dùng.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                return Unauthorized("Tài khoản không tồn tại.");
            }

            // So sánh mật khẩu cũ
            var hashedOld = PasswordHasher.HashPassword(dto.OldPassword);
            if (user.Password != hashedOld)
            {
                return BadRequest("Mật khẩu cũ không đúng!");
            }

            // Validate mật khẩu mới
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 8 ||
                !Regex.IsMatch(dto.NewPassword, @"[A-Z]") ||
                !Regex.IsMatch(dto.NewPassword, @"[a-z]") ||
                !Regex.IsMatch(dto.NewPassword, @"[0-9]") ||
                !Regex.IsMatch(dto.NewPassword, @"[\W_]"))
            {
                return BadRequest("Mật khẩu mới phải ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.");
            }

            // Cập nhật mật khẩu
            user.Password = PasswordHasher.HashPassword(dto.NewPassword);
            await _db.SaveChangesAsync();

            // Đăng xuất bắt buộc
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new { message = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại." });
        }

    }

    // Class hash mật khẩu
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
