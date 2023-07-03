using EcommerceWeb.Data;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            IEnumerable<Cart> CartList = _db.Carts;
            return View(CartList);
        }

        // Get
        public IActionResult Create() { 
            return View(); 
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]  // For csrf token
        public IActionResult Create(int? productID)
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

            Cart cart = new Cart()
            {
                Product = product,
                UserID = userId

            };

            _db.Carts.Add(cart);
            _db.SaveChanges();

            return RedirectToAction("Index", "HomeController");
        }

    }
}
