using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollectionManagement.Data;
using CollectionManagement.Models.Domain;
using System.Linq;
using System.Threading.Tasks;
using CollectionManagement.Models.ViewModels;
using CollectionManagement.Models;

namespace CollectionManagement.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly MVCmainDBConetxt _context;

        public ItemsController(MVCmainDBConetxt context)
        {
            _context = context;
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var item = await _context.Items
            //    .Include(i => i.Collection)
            //    .Include(i => i.Tags)
            //    .ThenInclude(it => it.Tag) // Include the related Tag
            //    .FirstOrDefaultAsync(m => m.ItemId == id);

            var item = await _context.Items
                .Include(i => i.Collection)
                .Include(i => i.Tags)
                    .ThenInclude(it => it.Tag) // Include the related Tag
                .Include(i => i.CustomFieldValues)
                    .ThenInclude(cf => cf.CustomField) // Include related CustomField for field names
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public async Task<IActionResult> Create(int collectionId)
        {
            // Fetch the collection
            var collection = await _context.Collections
                .Include(c => c.CustomFields)
                .FirstOrDefaultAsync(c => c.CollectionId == collectionId);

            if (collection == null)
            {
                return NotFound();
            }

            // Pass the custom fields to the view via ViewBag
            ViewBag.CustomFields = collection.CustomFields;

            // Initialize a new item with the collection ID
            var viewModel = new ItemCreateViewModel
            {
                CollectionId = collectionId,
            };

            return View(viewModel);
        }

        //// POST: Items/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(ItemCreateViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var item = new Item
        //        {
        //            Name = viewModel.Name,
        //            CollectionId = viewModel.CollectionId,
        //            CreatedAt = DateTime.Now,
        //            UpdatedAt = DateTime.Now
        //        };

        //        // Process tags
        //        if (viewModel.Tags != null)
        //        {
        //            item.Tags = new List<ItemTag>();
        //            foreach (var tagName in viewModel.Tags)
        //            {
        //                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
        //                if (tag == null)
        //                {
        //                    tag = new Tag { Name = tagName };
        //                    _context.Tags.Add(tag);
        //                    await _context.SaveChangesAsync();
        //                }
        //                item.Tags.Add(new ItemTag { Tag = tag });
        //            }
        //        }

        //        _context.Add(item);
        //        await _context.SaveChangesAsync();

        //        foreach (var fieldValue in viewModel.CustomFieldValues)
        //        {
        //            var itemCustomFieldValue = new ItemCustomFieldValue
        //            {
        //                ItemId = item.ItemId,
        //                CustomFieldId = fieldValue.Key,
        //                FieldValue = fieldValue.Value
        //            };

        //            _context.ItemCustomFieldValues.Add(itemCustomFieldValue);
        //        }

        //        await _context.SaveChangesAsync();

        //        return RedirectToAction(nameof(Index), "Collections");
        //    }

        //    var collection = await _context.Collections
        //        .Include(c => c.CustomFields)
        //        .FirstOrDefaultAsync(c => c.CollectionId == viewModel.CollectionId);

        //    ViewBag.CustomFields = collection.CustomFields;

        //    return View(viewModel);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetTags()
        //{
        //    var tags = await _context.Tags.Select(t => t.Name).ToListAsync();
        //    return Json(tags);
        //}

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var item = new Item
                {
                    Name = viewModel.Name,
                    CollectionId = viewModel.CollectionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Process tags
                if (viewModel.Tags != null)
                {
                    item.Tags = new List<ItemTag>();
                    foreach (var tagName in viewModel.Tags)
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                        if (tag == null)
                        {
                            tag = new Tag { Name = tagName };
                            _context.Tags.Add(tag);
                            await _context.SaveChangesAsync();
                        }
                        item.Tags.Add(new ItemTag { Tag = tag });
                    }
                }

                _context.Add(item);
                await _context.SaveChangesAsync();

                // Process custom field values
                foreach (var fieldValue in viewModel.CustomFieldValues)
                {
                    var itemCustomFieldValue = new ItemCustomFieldValue
                    {
                        ItemId = item.ItemId,
                        CustomFieldId = fieldValue.Key,
                        FieldValue = fieldValue.Value
                    };

                    _context.ItemCustomFieldValues.Add(itemCustomFieldValue);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), "Collections");
            }

            // If we get here, something went wrong with the model validation
            // Fetch the collection and repopulate the ViewBag
            var collection = await _context.Collections
                .Include(c => c.CustomFields)
                .FirstOrDefaultAsync(c => c.CollectionId == viewModel.CollectionId);

            if (collection == null)
            {
                return NotFound();
            }

            ViewBag.CustomFields = collection.CustomFields;

            // Return the same view with the current view model
            return View(viewModel);
        }




        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
            .Include(i => i.CustomFieldValues)
            .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }

            // Fetch the collection with CustomFields
            var collection = await _context.Collections
                .Include(c => c.CustomFields)
                .FirstOrDefaultAsync(c => c.CollectionId == item.CollectionId);

            if (collection == null)
            {
                return NotFound();
            }

            // Populate the view model
            var viewModel = new ItemEditViewModel
            {
                ItemId = item.ItemId,
                Name = item.Name,
                //Tags = item.Tags,
                CollectionId = item.CollectionId,
                UpdatedAt = DateTime.Now,

                // Convert the ItemCustomFieldValues into a dictionary
                CustomFieldValues = item.CustomFieldValues.ToDictionary(cf => cf.CustomFieldId, cf => cf.FieldValue),

                // Fetch the list of ItemCustomFields from the Collection
                //CustomField = item.Collection.CustomFields.ToList()
            };

            // Pass the CustomFields to the ViewBag
            ViewBag.CustomFields = collection.CustomFields?.ToList() ?? new List<ItemCustomField>();

            //ViewBag.CustomFields = item.Collection.CustomFields.ToList();
            //ViewBag.CustomFields = item.Collection.CustomFields;

            //ViewBag.CustomFields = viewModel.CustomFields;
            return View(viewModel);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemEditViewModel viewModel)
        {
            if (id != viewModel.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var item = await _context.Items
                    .Include(i => i.CustomFieldValues)
                    .FirstOrDefaultAsync(i => i.ItemId == id);

                if (item == null)
                {
                    return NotFound();
                }

                // Update the item properties
                item.Name = viewModel.Name;
                //item.Tags = viewModel.Tags;
                item.UpdatedAt = DateTime.Now;

                // Update custom fields
                foreach (var customFieldValue in viewModel.CustomFieldValues)
                {
                    var existingFieldValue = item.CustomFieldValues
                        .FirstOrDefault(cf => cf.CustomFieldId == customFieldValue.Key);

                    if (existingFieldValue != null)
                    {
                        existingFieldValue.FieldValue = customFieldValue.Value;
                    }
                    else
                    {
                        item.CustomFieldValues.Add(new ItemCustomFieldValue
                        {
                            ItemId = item.ItemId,
                            CustomFieldId = customFieldValue.Key,
                            FieldValue = customFieldValue.Value
                        });
                    }
                }

                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(viewModel.ItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.ItemCustomFields = await _context.ItemCustomFields
                .Where(cf => cf.CollectionId == viewModel.CollectionId)
                .ToListAsync();

            return View(viewModel);
        }

       


        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
               .Include(i => i.Collection)
               .Include(i => i.Tags)
               .ThenInclude(it => it.Tag) // Include the related Tag
               .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items
                 .Include(i => i.Tags)  // Include ItemTags to ensure they are also removed
                 .Include(i => i.CustomFieldValues)  // Include related custom field values
                 .FirstOrDefaultAsync(i => i.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            // Remove associated ItemTags
            _context.ItemTags.RemoveRange(item.Tags);

            // Remove associated ItemCustomFieldValues
            _context.ItemCustomFieldValues.RemoveRange(item.CustomFieldValues);

            // Remove the item itself
            _context.Items.Remove(item);

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Collections", new { id = item.CollectionId });
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }
    }
}
