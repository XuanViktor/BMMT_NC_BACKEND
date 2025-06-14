using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BMMT_NC.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BMMT_NC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly CsdlContext _context;

        public NotificationController(CsdlContext context)
        {
            _context = context;
        }

        // GET: api/Notification/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (currentUserId != userId)
            {
                return Forbid();
            }

            return await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Notification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.NotificationId == id);

            if (notification == null)
            {
                return NotFound();
            }

            if (notification.UserId != userId)
            {
                return Forbid();
            }

            return notification;
        }

        // POST: api/Notification
        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification(NotificationDto notification)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (notification.UserId != userId)
            {
                return Forbid();
            }

            notification.CreatedAt = DateTime.Now;
            var newNotification = new Notification
            {
                UserId = notification.UserId,
                Content = notification.Content,
                IsRead = notification.IsRead ?? false,
                CreatedAt = notification.CreatedAt
            };
            _context.Notifications.Add(newNotification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotification), new { id = newNotification.NotificationId }, notification);
        }

        // Update the `Forbid` calls to use `JsonResult` explicitly for returning JSON responses.

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound(new JsonResult(new { message = "Notification not found." }));
            }

            if (notification.UserId != userId)
            {
                return new JsonResult(new { message = "You are not authorized to delete this notification." })
                {
                    StatusCode = StatusCodes.Status403Forbidden

                };
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification deleted successfully." });
        }


        [HttpDelete("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAllUserNotifications(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (currentUserId != userId)
            {
                return new JsonResult(new { message = "You are not authorized to delete notifications for this user." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();

            if (notifications == null || !notifications.Any())
            {
                return NotFound(new JsonResult(new { message = "No notifications found for this user." }));
            }

            _context.Notifications.RemoveRange(notifications);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new JsonResult(new { message = "An error occurred while deleting notifications." }));
            }

            return Ok(new JsonResult(new { message = "All notifications for this user have been deleted." }));
        }


    }

    public class NotificationDto
    {
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public bool? IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

}