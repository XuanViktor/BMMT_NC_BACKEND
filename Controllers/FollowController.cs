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
    public class FollowController : ControllerBase
    {
        private readonly CsdlContext _context;

        public FollowController(CsdlContext context)
        {
            _context = context;
        }

        // GET: api/Follow/followers/5
        [HttpGet("followers/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Follow>>> GetFollowers(int userId)
        {
            return await _context.Follows
                .Include(f => f.Follower)
                .Where(f => f.FolloweeId == userId)
                .ToListAsync();
        }

        // GET: api/Follow/following/5
        [HttpGet("following/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Follow>>> GetFollowing(int userId)
        {
            return await _context.Follows
                .Include(f => f.Followee)
                .Where(f => f.FollowerId == userId)
                .ToListAsync();
        }

        // POST: api/Follow
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Follow>> CreateFollow(Follow follow)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (follow.FollowerId != userId)
            {
                return Forbid();
            }

            if (await _context.Follows.AnyAsync(f => f.FollowerId == follow.FollowerId && f.FolloweeId == follow.FolloweeId))
            {
                return BadRequest("Already following this user");
            }

            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFollowers), new { userId = follow.FolloweeId }, follow);
        }

        // DELETE: api/Follow/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFollow(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var follow = await _context.Follows.FindAsync(id);
            
            if (follow == null)
            {
                return NotFound();
            }

            if (follow.FollowerId != userId)
            {
                return Forbid();
            }

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Follow/check
        [HttpGet("check")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> CheckFollow(int followerId, int followeeId)
        {
            return await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
        }
    }
} 