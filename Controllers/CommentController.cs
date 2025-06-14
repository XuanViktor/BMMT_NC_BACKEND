using BMMT_NC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BMMT_NC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly CsdlContext _context;

        public CommentController(CsdlContext context)
        {
            _context = context;
        }

        // GET: api/Comment/post/5
        [HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetPostComments(int postId)
        {
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)  // Bao gồm thông tin người dùng (có thể bỏ nếu không cần thiết)
                .ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound(new { message = "No comments found for this post." });
            }

            // Trả về danh sách bình luận với chỉ thông tin cần thiết
            return Ok(comments.Select(c => new
            {
                c.CommentId,
                c.PostId,
                c.UserId,
                c.CommentText,
                c.CreatedAt
            }));
        }

        // GET: api/Comment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.User)  // Bao gồm thông tin người dùng (có thể bỏ nếu không cần thiết)
                .FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
            {
                return NotFound(new { message = "Comment not found." });
            }

            return Ok(new
            {
                comment.CommentId,
                comment.PostId,
                comment.UserId,
                comment.CommentText,
                comment.CreatedAt
            });
        }

        // POST: api/Comment
        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment([FromBody] CreateCommentRequest comment)
        {
            // Kiểm tra các trường bắt buộc: PostId và UserId
            if (comment.PostId == 0 || comment.UserId == 0)
            {
                return BadRequest(new { message = "PostId and UserId are required." });
            }

            // Đặt thời gian tạo cho bình luận
            comment.CreatedAt = DateTime.Now;

            var newComment = new Comment
            {
                PostId = comment.PostId,
                UserId = comment.UserId,
                CommentText = comment.CommentText,
                CreatedAt = comment.CreatedAt
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            // Trả về bình luận mới được tạo với thông tin cần thiết
            return CreatedAtAction(nameof(GetComment), new { id = newComment.CommentId }, new
            {
                newComment.CommentId,
                newComment.PostId,
                newComment.UserId,
                newComment.CommentText,
                newComment.CreatedAt
            });
        }

        // PUT: api/Comment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentRequest updateComment)
        {
            // Kiểm tra nếu PostId và UserId hợp lệ
            if (updateComment.PostId == 0 || updateComment.UserId == 0)
            {
                return BadRequest(new { message = "PostId and UserId are required." });
            }

            // Kiểm tra xem comment có tồn tại trong cơ sở dữ liệu không
            var newComment = await _context.Comments.FindAsync(id);
            if (newComment == null)
            {
                return NotFound(new { message = "Comment not found." });
            }

            // Cập nhật thông tin bình luận với dữ liệu từ request
            newComment.PostId = updateComment.PostId;
            newComment.UserId = updateComment.UserId;
            newComment.CommentText = updateComment.CommentText;
            newComment.CreatedAt = updateComment.CreatedAt ?? DateTime.Now;

            // Đánh dấu entity là đã thay đổi
            _context.Entry(newComment).State = EntityState.Modified;

            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound(new { message = "Comment not found." });
                }
                else
                {
                    throw;
                }
            }

            // Trả về thông tin bình luận đã được cập nhật
            return Ok(new
            {
                newComment.CommentId,
                newComment.PostId,
                newComment.UserId,
                newComment.CommentText,
                newComment.CreatedAt
            });
        }

        // DELETE: api/Comment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(new { message = "Comment not found." });
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            // Trả về message thành công khi xóa bình luận
            return Ok(new { message = "Comment deleted successfully." });
        }


        // Kiểm tra sự tồn tại của comment
        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }

    public class CreateCommentRequest
    {
        [Required]
        public int PostId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(500, ErrorMessage = "Comment text cannot be longer than 500 characters.")]
        public string? CommentText { get; set; }
        public DateTime? CreatedAt
        {
            get; set;
        }
    }

    public class UpdateCommentRequest
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Comment text cannot be longer than 500 characters.")]
        public string CommentText { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
    }

}
