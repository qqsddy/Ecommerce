using EcommerceWeb.Data;
using EcommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string productName)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var identity = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(identity != null)
            {
                HttpContext.Session.SetInt32("shoppingCart", await _db.Carts.Where(c => c.UserID == identity.Value).CountAsync());
            }

            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(productName))
            {
                products = await _db.Products
                    .Include(p => p.Category)
                    .Where(p => p.Name.Contains(productName))
                    .ToListAsync();
            }
            else
            {
                products = await _db.Products.
                    Include(p => p.Category)
                    .ToListAsync();
            }

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}