using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollectionManagement.Data;
using CollectionManagement.Models.Domain;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionManagement.Controllers
{
    [Authorize]  // Ensure only valid users can access this controller
    public class ValidUserCollectionsController : Controller
    {
        private readonly MVCmainDBConetxt _context;

        public ValidUserCollectionsController(MVCmainDBConetxt context)
        {
            _context = context;
        }

        // GET: ValidUserCollections
        public async Task<IActionResult> Index()
        {
            var collections = await _context.Collections
                .Include(c => c.Items)
                .ToListAsync();
            return View(collections);
        }

        // GET: ValidUserCollections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .Include(c => c.Items)
                    .ThenInclude(i => i.Comments)
                         .ThenInclude(c => c.User)
                .Include(c => c.Items)
                     .ThenInclude(i => i.Likes)
                            .FirstOrDefaultAsync(m => m.CollectionId == id);

            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: ValidUserCollections/Like/5
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int itemId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null) return Unauthorized();

            var like = await _context.Likes.FirstOrDefaultAsync(l => l.ItemId == itemId && l.UserId == user.Id);

            if (like == null)
            {
                // User hasn't liked this item yet, so add a new like
                like = new Like
                {
                    ItemId = itemId,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now
                };
                _context.Likes.Add(like);
            }
            else
            {
                // User has already liked this item, so remove the like
                _context.Likes.Remove(like);
            }

            await _context.SaveChangesAsync();

            // Return the current like count
            var likeCount = await _context.Likes.CountAsync(l => l.ItemId == itemId);
            return Json(new { likeCount });
        }

        // POST: ValidUserCollections/Comment/5
        [HttpPost]
        public async Task<IActionResult> Comment(int itemId, string content)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null) return Unauthorized();

            var comment = new Comment
            {
                ItemId = itemId,
                UserId = user.Id,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok();  // No redirect, just return OK status for AJAX
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int itemId)
        {
            var comments = await _context.Comments
                .Where(c => c.ItemId == itemId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return PartialView("_CommentsPartial", comments);
        }

    }
}
