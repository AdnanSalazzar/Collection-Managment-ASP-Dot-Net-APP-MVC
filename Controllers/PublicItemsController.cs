using CollectionManagement.Data;
using CollectionManagement.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionManagement.Controllers
{
    public class PublicItemsController : Controller
    {
        private readonly MVCmainDBConetxt _context;

        public PublicItemsController(MVCmainDBConetxt context)
        {
            _context = context;
        }

        //// GET: PublicItems
        //public async Task<IActionResult> Index()
        //{
        //    var items = await _context.Items
        //        .Include(i => i.Tags)
        //        .ThenInclude(it => it.Tag) // Include the related Tag
        //        .Include(i => i.Collection)
        //        .ToListAsync();

        //    return View(items);
        //}


        // GET: PublicItems
        public async Task<IActionResult> Index(string searchTags, string searchMode, string sortOrder)
        {
            var itemsQuery = _context.Items
                .Include(i => i.Tags)
                    .ThenInclude(it => it.Tag)
                .Include(i => i.Collection)
                .AsQueryable();

            // Handle search tags
            if (!string.IsNullOrEmpty(searchTags))
            {
                var tags = searchTags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(t => t.Trim())
                                     .ToList();

                if (searchMode == "AND")
                {
                    foreach (var tag in tags)
                    {
                        itemsQuery = itemsQuery.Where(i => i.Tags.Any(it => it.Tag.Name == tag));
                    }
                }
                else // OR logic
                {
                    itemsQuery = itemsQuery.Where(i => i.Tags.Any(it => tags.Contains(it.Tag.Name)));
                }
            }

            // Handle sort order
            if (sortOrder == "latest")
            {
                itemsQuery = itemsQuery.OrderByDescending(i => i.CreatedAt);
            }
            else if (sortOrder == "oldest")
            {
                itemsQuery = itemsQuery.OrderBy(i => i.CreatedAt);
            }

            var items = await itemsQuery.ToListAsync();

            // Passing the search parameters back to the view
            ViewData["searchTags"] = searchTags;
            ViewData["searchMode"] = searchMode;
            ViewData["sortOrder"] = sortOrder;

            return View(items);
        }

        // GET: PublicItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var item = await _context.Items
            //    .Include(i => i.Tags)
            //    .ThenInclude(it => it.Tag)
            //    .Include(i => i.Comments)
            //    .ThenInclude(c => c.User)
            //    .Include(i => i.Likes)
            //    .Include(i => i.Collection)
            //    .FirstOrDefaultAsync(m => m.ItemId == id);

            var item = await _context.Items
                .Include(i => i.Tags)
                    .ThenInclude(it => it.Tag)
                .Include(i => i.Comments)
                    .ThenInclude(c => c.User)
                .Include(i => i.Likes)
                .Include(i => i.Collection)
                .Include(i => i.CustomFieldValues) // Include CustomFieldValues
                    .ThenInclude(cf => cf.CustomField) // Include related CustomField for field names
                .FirstOrDefaultAsync(m => m.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: PublicItems/Comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(int itemId, string content)
        {


            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (string.IsNullOrEmpty(content))
            {
                return BadRequest("Comment content cannot be empty.");
            }


            if (user == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                ItemId = itemId,
                UserId = user.Id,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = itemId });
        }

        // GET: PublicItems/CollectionDetails/5
        public async Task<IActionResult> CollectionDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .Include(c => c.Items)
                .ThenInclude(i => i.Tags)
                .ThenInclude(it => it.Tag)
                .FirstOrDefaultAsync(c => c.CollectionId == id);

            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }


        // POST: PublicItems/ToggleLike
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
    }
}
