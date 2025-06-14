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
    public class MessageController : ControllerBase
    {
        private readonly CsdlContext _context;

        public MessageController(CsdlContext context)
        {
            _context = context;
        }

        // GET: api/Message/conversation/5/6
        [HttpGet("conversation/{user1Id:int}/{user2Id:int}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Message>>> GetConversation(int user1Id, int user2Id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId != user1Id && userId != user2Id)
                return Forbid();

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.MessagePhotos)
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                            (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }

        // GET: api/Message/5
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.MessagePhotos)
                .FirstOrDefaultAsync(m => m.MessageId == id);

            if (message == null)
                return NotFound();

            if (message.SenderId != userId && message.ReceiverId != userId)
                return Forbid();

            return Ok(message);
        }

        // POST: api/Message
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Message>> CreateMessage(Message message)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (message.SenderId != userId)
                return Forbid();

            message.SentAt = DateTime.UtcNow;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMessage), new { id = message.MessageId }, message);
        }

        // PUT: api/Message/5
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateMessage(int id, Message message)
        {
            if (id != message.MessageId)
                return BadRequest();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var existingMessage = await _context.Messages.AsNoTracking().FirstOrDefaultAsync(m => m.MessageId == id);

            if (existingMessage == null)
                return NotFound();

            if (existingMessage.SenderId != userId)
                return Forbid();

            // preserve original SentAt if not being updated
            if (message.SentAt == null)
                message.SentAt = existingMessage.SentAt;

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Message/5
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var message = await _context.Messages.FindAsync(id);

            if (message == null)
                return NotFound();

            if (message.SenderId != userId && message.ReceiverId != userId)
                return Forbid();

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
