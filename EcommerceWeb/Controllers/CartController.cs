using EcommerceWeb.Data;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceWeb.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves the cart items for the current user and calculates the total.
        /// </summary>
        /// <returns>The cart view with the cart items and total.</returns>
        public async Task<IActionResult> Index()
        {
            string userID = GetUserId();

            List<Cart> cartFromDb = await _db.Carts
                .Include(c => c.Product)
                .Where(c => c.UserID == userID)
                .ToListAsync();

            decimal total = cartFromDb.Sum(c => c.Subtotal);
            ViewBag.Total = total;

            return View(cartFromDb);
        }

        /// <summary>
        /// Adds a product to the cart for the current user.
        /// </summary>
        /// <param name="id">ProductID</param>
        /// <returns>Redirects to the home page.</returns>
        public async Task<IActionResult> AddtoCart(int? id)
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

            string userID = GetUserId();

            // Check if the product already exists in the cart
            Cart cartFromDb = await _db.Carts.SingleOrDefaultAsync(c => c.UserID == userID && c.ProductID == id);

            if (cartFromDb != null)
            {
                cartFromDb.Quantity++;
                _db.Carts.Update(cartFromDb);
                await _db.SaveChangesAsync();
            }
            else
            {
                // Create a new cart item for the product
                Cart cart = new Cart()
                {
                    Product = product,
                    UserID = userID,
                    Quantity = 1

                };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();

                HttpContext.Session.SetInt32("shoppingCart", await _db.Carts.Where(c => c.UserID == userID).CountAsync());
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Removes a cart item for the current user.
        /// </summary>
        /// <param name="Id">cartID</param>
        /// <returns>Redirects to the cart page.</returns>
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var cart = await _db.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetInt32("shoppingCart", await _db.Carts.Where(c => c.UserID == cart.UserID).CountAsync() - 1);

            _db.Carts.Remove(cart);
            await _db.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Retrieves the ID of the current user.
        /// </summary>
        private string GetUserId()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
