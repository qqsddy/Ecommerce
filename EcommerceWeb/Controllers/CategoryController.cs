using EcommerceWeb.Data;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWeb.Controllers
{
    [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// GET: Category
        /// <summary>
        /// Displays a list of categories for the admin to manage.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _db.Categories.ToListAsync();

            return View(categories);
        }

        /// GET: Category/Create
        /// <summary>
        /// Displays the category creation form.
        /// </summary>
        public IActionResult Create() 
        { 
            return View(); 
        }

        /// POST: Category/Create
        /// <summary>
        /// Handles the HTTP POST request to create a new category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns>
        /// If the model state is valid and the category is successfully created, redirects to the category list page.
        /// Otherwise, redisplays the category creation form with validation errors.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
                //TempData["success"] = "Category created successfully";

                return RedirectToAction("Index");
            }

            return View(category);
        }

        /// GET: Category/Edit
        /// <summary>
        /// Displays the category edit form.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns>
        /// If the category ID is null or the category is not found, returns a "Not Found" response.
        /// Otherwise, displays the edit form for the specified category.
        /// </returns>
        public async Task<IActionResult> Edit(int? categoryID) 
        {
            if (categoryID == null)
            {
                return NotFound();
            }
            var category = await _db.Categories.FindAsync(categoryID);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        /// POST: Category/Edit
        /// <summary>
        /// Handles the HTTP POST request to update a category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns>
        /// If the model state is valid, updates the category in the database.
        /// Redirects to the "Index" action if the update is successful.
        /// If the model state is invalid, returns the edit view with the current category and validation errors.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                await _db.SaveChangesAsync();
                //TempData["success"] = "Category updated successfully";

                return RedirectToAction("Index");
            }

            return View(category);
        }

        /// GET: Category/Delete
        /// <summary>
        /// Displays the delete confirmation page for a category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns>
        /// If the category ID is null or the category is not found, returns a "Not Found" page.
        /// Otherwise, retrieves the category from the database and displays the delete confirmation view.
        /// </returns>
        public async Task<IActionResult> Delete(int? categoryID)
        {
            if (categoryID == null)
            {
                return NotFound();
            }
            var category = await _db.Categories.FindAsync(categoryID);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        /// POST: Category/Delete
        /// <summary>
        /// Handles the HTTP POST request to delete a category.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns>
        /// If the category is found, removes it from the database and redirects to the "Index" action.
        /// If the category is not found, returns a "Not Found" page.
        /// </returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? categoryID)
        {
            var category = await _db.Categories.FindAsync(categoryID);

            if (category == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            //TempData["success"] = "Category deleted successfully";
            
            return RedirectToAction("Index");
        }
    }
}
