
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
        public ProductController(ApplicationDbContext db)
        {
            _db = db;
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
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
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
            if (id == null || id == 0)
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
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
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
            if (id == null || id == 0)
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
