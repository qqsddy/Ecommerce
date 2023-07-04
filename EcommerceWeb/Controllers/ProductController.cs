
using EcommerceWeb.Data;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace EcommerceWeb.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        /// GET: Product
        /// <summary>
        /// Displays a list of products for the admin to manage.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _db.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }

        /// GET: Product/Create 
        /// <summary>
        /// Displays the product creation form to the admin.
        /// </summary>
        public IActionResult Create() 
        {
            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name");
            
            return View(); 
        }

        /// POST: Product/Create
        /// <summary>
        /// Handles the HTTP POST request to create a new product.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="image"></param>
        /// <returns>
        /// If the model state is valid and the product is successfully created, redirects to the product list page.
        /// Otherwise, redisplays the product creation form with validation errors.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                await HandleImageUpload(product, image);

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                //TempData["success"] = "Proudct created successfully";

                return RedirectToAction("Index");
            }

            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name", product.CategoryID);

            return View(product);
        }

        /// GET: Product/Edit
        /// <summary>
        /// Handles the HTTP GET request to display the edit form for a product.
        /// </summary>
        /// <param name="id">ProductID</param>
        /// <returns>
        /// If the product ID is null or the product is not found, returns a "Not Found" response.
        /// Otherwise, displays the edit form for the specified product.
        /// </returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name", product.Category);
            return View(product);
        }

        /// POST: Product/Edit
        /// <summary>
        /// Handles the HTTP POST request to update a product.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="image"></param>
        /// <returns>
        /// If the model state is valid, updates the product in the database, including handling the image file.
        /// Redirects to the "Index" action if the update is successful.
        /// If the model state is invalid, returns the edit view with the current product and validation errors.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Edit(Product product, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _db.Products.FindAsync(product.ID);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                await HandleImageUpload(product, image);

                // Copy the edited properties to the existing product entity
                _db.Entry(existingProduct).CurrentValues.SetValues(product);

                _db.Products.Update(existingProduct);
                await _db.SaveChangesAsync();
                //TempData["success"] = "Proudct updated successfully";

                return RedirectToAction("Index");
            }
            // If the model is not valid, retrieve categories for dropdown list again
            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name", product.CategoryID);

            return View(product);
        }

        /// GET: Product/Delete
        /// <summary>
        /// Displays the delete confirmation page for a product.
        /// </summary>
        /// <param name="id">productID</param>
        /// <returns>
        /// If the productID is null or the product is not found, returns a "Not Found" page.
        /// Otherwise, retrieves the product from the database and displays the delete confirmation view.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name", product.Category);
            return View(product);
        }

        /// POST: Product/Delete
        /// <summary>
        /// Handles the HTTP POST request to delete a product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// If the product is found, removes it from the database and redirects to the "Index" action.
        /// If the product is not found, returns a "Not Found" page.
        /// </returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]  // For csrf token
        public async Task<IActionResult> DeletePost(int? id)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            //TempData["success"] = "Product deleted successfully";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles the upload of an image file for a product.
        /// If an image file is provided, it saves the file to the server and updates the product's image URL.
        /// If the product already has an existing image, the old image file is deleted before saving the new one.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="image"></param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task HandleImageUpload(Product product, IFormFile? image)
        {
            if (image != null)
            {
                string wwwrootPath = _webHostEnvironment.WebRootPath;
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string productPath = Path.Combine(wwwrootPath, @"image\product");

                // Delete the old image file if it exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    string oldImagePath = Path.Combine(wwwrootPath, product.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, imageName), FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                product.ImageUrl = @"\image\product\" + imageName;
            }
        }
    }
}
