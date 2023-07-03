
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

        public IActionResult Index()
        {
            IEnumerable<Product> Products = _db.Products
                .Include(p => p.Category);
            return View(Products);
        }

        // Get
        public IActionResult Create() 
        {
            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name");
            return View(); 
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]  // For csrf token
        public IActionResult Create(Product product, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                string wwwrootPath = _webHostEnvironment.WebRootPath;
                if (image != null)
                {
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string productPath = Path.Combine(wwwrootPath, @"image\product");

                    using (var fileStream = new FileStream(Path.Combine(productPath, imageName), FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }

                    product.ImageUrl = @"\image\product\" + imageName;
                }

                _db.Products.Add(product);
                _db.SaveChanges();
                TempData["success"] = "Proudct created successfully";

                return RedirectToAction("Index");
            }
            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name", product.CategoryID);
            return View(product);
        }

        // Get
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }
            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name", product.Category);
            return View(product);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]  // For csrf token
        public IActionResult Edit(Product product, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                string wwwrootPath = _webHostEnvironment.WebRootPath;
                if (image != null)
                {
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string productPath = Path.Combine(wwwrootPath, @"image\product");

                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwrootPath, product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, imageName), FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }

                    product.ImageUrl = @"\image\product\" + imageName;
                }

                _db.Products.Update(product);
                _db.SaveChanges();
                TempData["success"] = "Proudct updated successfully";

                return RedirectToAction("Index");
            }
            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name", product.CategoryID);
            return View(product);
        }

        // Get
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }
            ViewData["Categories"] = new SelectList(_db.Set<Category>(), "ID", "Name", product.Category);
            return View(product);
        }

        //Post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]  // For csrf token
        public IActionResult DeletePost(int? id)
        {
            var product = _db.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            _db.Products.Remove(product);
            _db.SaveChanges();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
