using Microsoft.AspNetCore.Mvc;
using CollectionManagement.Models.Domain;
using CollectionManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CollectionManagement.Controllers
{
    public class PublicCollectionsController : Controller
    {
        private readonly MVCmainDBConetxt _context;

        public PublicCollectionsController(MVCmainDBConetxt context)
        {
            _context = context;
        }

        // Action to display all collections for non-authenticated users
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var collections = await _context.Collections
                                             .Include(c => c.User)
                                             .ToListAsync();
            return View(collections);
        }

        // Action to display items within a specific collection
        [AllowAnonymous]
        public async Task<IActionResult> ViewItems(int? id)
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
    }
}
