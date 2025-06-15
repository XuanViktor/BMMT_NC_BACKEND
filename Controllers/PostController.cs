using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BMMT_NC.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BMMT_NC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly CsdlContext _context;

        public PostController(CsdlContext context)
        {
            _context = context;
        }

        // GET: api/Post
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Photo)
                .Include(p => p.Video)
                .Include(p => p.Hashtags)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.PostId,
                    p.Caption,
                    p.Location,
                    p.CreatedAt,
                    User = new
                    {
                        p.User.UserId,
                        p.User.Username,
                        p.User.ProfilePhotoUrl
                    },
                    Photo = p.Photo != null ? new
                    {
                        p.Photo.PhotoId,
                        Url = p.Photo.PhotoUrl
                    } : null,
                    Video = p.Video != null ? new
                    {
                        p.Video.VideoId,
                        p.Video.VideoUrl
                    } : null,
                    Hashtags = p.Hashtags.Select(h => new
                    {
                        h.HashtagId,
                        Text = h.HashtagName
                    })
                })
                .ToListAsync();

            return Ok(posts);
        }

        // GET: api/Post/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetPost(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Photo)
                .Include(p => p.Video)
                .Include(p => p.Hashtags)
                .Where(p => p.PostId == id)
                .Select(p => new
                {
                    p.PostId,
                    p.Caption,
                    p.Location,
                    p.CreatedAt,
                    User = new
                    {
                        p.User.UserId,
                        p.User.Username,
                        p.User.ProfilePhotoUrl
                    },
                    Photo = p.Photo != null ? new
                    {
                        p.Photo.PhotoId,
                        Url = p.Photo.PhotoUrl
                    } : null,
                    Video = p.Video != null ? new
                    {
                        p.Video.VideoId,
                        p.Video.VideoUrl
                    } : null,
                    Hashtags = p.Hashtags.Select(h => new
                    {
                        h.HashtagId,
                        Text = h.HashtagName
                    })
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        // GET: api/Post/user/5
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetUserPosts(int userId)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Photo)
                .Include(p => p.Video)
                .Include(p => p.Hashtags)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.PostId,
                    p.Caption,
                    p.Location,
                    p.CreatedAt,
                    User = new
                    {
                        p.User.UserId,
                        p.User.Username,
                        p.User.ProfilePhotoUrl
                    },
                    Photo = p.Photo != null ? new
                    {
                        p.Photo.PhotoId,
                        Url = p.Photo.PhotoUrl
                    } : null,
                    Video = p.Video != null ? new
                    {
                        p.Video.VideoId,
                        p.Video.VideoUrl
                    } : null,
                    Hashtags = p.Hashtags.Select(h => new
                    {
                        h.HashtagId,
                        Text = h.HashtagName
                    })
                })
                .ToListAsync();

            return Ok(posts);
        }

        // POST: api/Post
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Post>> CreatePost([FromBody] PostCreateDto postDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var post = new Post
            {
                Caption = postDto.Caption,
                Location = postDto.Location,
                PhotoId = postDto.PhotoId,
                VideoId = postDto.VideoId,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.PostId }, post);
        }

        // PUT: api/Post/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, Post post)
        {
            if (id != post.PostId)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var existingPost = await _context.Posts.FindAsync(id);

            if (existingPost == null)
            {
                return NotFound();
            }

            if (existingPost.UserId != userId)
            {
                return Forbid();
            }

            existingPost.Caption = post.Caption;
            existingPost.Location = post.Location;
            existingPost.PhotoId = post.PhotoId;
            existingPost.VideoId = post.VideoId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Post/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            if (post.UserId != userId)
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}

public class PostCreateDto
{
    public string? Caption { get; set; }
    public string? Location { get; set; }
    public int? PhotoId { get; set; }
    public int? VideoId { get; set; }
}