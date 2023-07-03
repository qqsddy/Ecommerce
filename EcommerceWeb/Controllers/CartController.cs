using EcommerceWeb.Data;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Authorization;
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
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<Cart> cartFromDb = _db.Carts
                .Include(c => c.Product)
                .Where(c => c.UserID == userId)
                .ToList();

            decimal total = cartFromDb.Sum(c => c.Subtotal);

            ViewBag.Total = total;

            return View(cartFromDb);
        }

        //Get
        public IActionResult AddtoCart(int? productID)
        {
            
            if (productID == null || productID == 0)
            {
                return NotFound();
            }
            var product = _db.Products.Find(productID);
            
            if (product == null)
            {
                return NotFound();
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            Cart cartFromDb = _db.Carts.FirstOrDefault(c => c.UserID == userId && c.ProductID == productID);

            if (cartFromDb != null)
            {
                cartFromDb.Quantity++;
                _db.Carts.Update(cartFromDb);
                _db.SaveChanges();
            }
            else
            {
                Cart cart = new Cart()
                {
                    Product = product,
                    UserID = userId,
                    Quantity = 1

                };
                _db.Carts.Add(cart);
                _db.SaveChanges();
            }
            
            return NoContent();
        }

        //Get
        public IActionResult Delete(int? cartID)
        {

            if (cartID == null || cartID == 0)
            {
                return NotFound();
            }
            var cart = _db.Carts.Find(cartID);

            if (cart == null)
            {
                return NotFound();
            }
            _db.Carts.Remove(cart);
            _db.SaveChanges();
       
            return RedirectToAction("Index");
        }

    }
}
