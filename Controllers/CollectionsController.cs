using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollectionManagement.Data;
using CollectionManagement.Models.Domain;
using System.Linq;
using System.Threading.Tasks;
using CollectionManagement.Models;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace CollectionManagement.Controllers
{
    [Authorize]
    public class CollectionsController : Controller
    {
        private readonly MVCmainDBConetxt _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CollectionsController(MVCmainDBConetxt context, UserManager<User> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Collections
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var collections = await _context.Collections
                .Where(c => c.UserId == user.Id)
                .ToListAsync();
            return View(collections);
        }

        // GET: Collections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                 .Include(c => c.Items)
                     .ThenInclude(i => i.Tags)
                         .ThenInclude(it => it.Tag)
                 .FirstOrDefaultAsync(m => m.CollectionId == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // GET: Collections/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CollectionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                string imagePath = null;

                if (model.ImageUrl != null && model.ImageUrl.Length > 0)
                {
                    var uploads = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    var fileName = Path.GetRandomFileName() + Path.GetExtension(model.ImageUrl.FileName);
                    imagePath = Path.Combine("uploads", fileName);

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                        await model.ImageUrl.CopyToAsync(fileStream);
                    }
                }

                var collection = new Collection
                {
                    UserId = user.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Category,
                    ImageUrl = imagePath,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _context.Add(collection);
                await _context.SaveChangesAsync();

                return RedirectToAction("SelectCustomFieldType", new { collectionId = collection.CollectionId });
            }
            return View(model);
        }


        // GET: Collections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            // Map the Collection entity to the EditCollectionViewModel
            var viewModel = new EditCollectionViewModel
            {
                CollectionId = collection.CollectionId,
                Name = collection.Name,
                Description = collection.Description,
                Category = collection.Category,
                ImageUrl = collection.ImageUrl
            };

            return View(viewModel);
        }

        // POST: Collections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCollectionViewModel viewModel)
        {
            if (id != viewModel.CollectionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing collection from the database
                    var existingCollection = await _context.Collections.FindAsync(id);
                    if (existingCollection == null)
                    {
                        return NotFound();
                    }

                    // Update only the fields that are allowed to change
                    existingCollection.Name = viewModel.Name;
                    existingCollection.Description = viewModel.Description;
                    existingCollection.Category = viewModel.Category;
                    existingCollection.ImageUrl = viewModel.ImageUrl;
                    existingCollection.UpdatedAt = DateTime.Now;

                    _context.Update(existingCollection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(viewModel.CollectionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Collections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .FirstOrDefaultAsync(m => m.CollectionId == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectionExists(int id)
        {
            return _context.Collections.Any(e => e.CollectionId == id);
        }

        // GET: Collections/SelectCustomFieldType
        public IActionResult SelectCustomFieldType(int collectionId)
        {
            ViewBag.CollectionId = collectionId;
            return View();
        }


        // Common method to handle custom field creation
        private async Task<IActionResult> CreateCustomField(int collectionId, string fieldName, string fieldType)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                ModelState.AddModelError("", "Field name is required.");
                ViewBag.CollectionId = collectionId;
                return View();
            }

            var collection = await _context.Collections.FindAsync(collectionId);
            if (collection == null)
            {
                return NotFound();
            }

            var customField = new ItemCustomField
            {
                CollectionId = collectionId,
                FieldName = fieldName,
                FieldType = fieldType
            };

            _context.ItemCustomFields.Add(customField);
            await _context.SaveChangesAsync();

            // Redirect back to the custom field selection page to add more fields
            return RedirectToAction("SelectCustomFieldType", new { collectionId = collectionId });
        }

        // POST: Collections/CreateIntegerField
        public IActionResult CreateIntegerField(int collectionId)
        {
            ViewBag.CollectionId = collectionId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIntegerField(int collectionId, string fieldName)
        {
            return await CreateCustomField(collectionId, fieldName, "Integer");
        }

        // POST: Collections/CreateStringField
        public IActionResult CreateStringField(int collectionId)
        {
            ViewBag.CollectionId = collectionId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStringField(int collectionId, string fieldName)
        {
            return await CreateCustomField(collectionId, fieldName, "String");
        }

        // POST: Collections/CreateMultilineTextField
        public IActionResult CreateMultilineTextField(int collectionId)
        {
            ViewBag.CollectionId = collectionId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultilineTextField(int collectionId, string fieldName)
        {
            return await CreateCustomField(collectionId, fieldName, "MultilineText");
        }

        // POST: Collections/CreateBooleanField
        public IActionResult CreateBooleanField(int collectionId)
        {
            ViewBag.CollectionId = collectionId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooleanField(int collectionId, string fieldName)
        {
            return await CreateCustomField(collectionId, fieldName, "Boolean");
        }

        // POST: Collections/CreateDateField
        public IActionResult CreateDateField(int collectionId)
        {
            ViewBag.CollectionId = collectionId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDateField(int collectionId, string fieldName)
        {
            return await CreateCustomField(collectionId, fieldName, "Date");
        }
    }
}
