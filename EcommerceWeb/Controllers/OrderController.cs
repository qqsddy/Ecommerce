using EcommerceWeb.Data;
using EcommerceWeb.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceWeb.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Checkout()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<Cart> cartFromDb = _db.Carts
                .Include(c => c.Product)
                .Where(c => c.UserID == userId)
                .ToList();

            var viewModel = new CartOrderViewModel
            {
                Carts = cartFromDb,
                Order = new Models.Order()
            };

            decimal total = cartFromDb.Sum(c => c.Subtotal);
            ViewBag.Total = total;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderConfirmation(Order order)
        {
            if (!ModelState.IsValid)
            {
                // Retrieve all error messages from ModelState
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                // Log or display the error messages
                foreach (var errorMessage in errorMessages)
                {
                    // Log the error or display it to the user
                    Debug.WriteLine(errorMessage);
                }

                // Return the view with the updated model
                return View(order);
            }

            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                List<Cart> cartFromDb = _db.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserID == userId)
                    .ToList();

                decimal total = cartFromDb.Sum(c => c.Subtotal);

                order.UserID = userId;
                order.PaymentStatus = "Pending";
                order.Status = "Pending";
                order.Total = total;
                _db.Orders.Add(order);
                _db.SaveChanges();

                foreach (var cart in cartFromDb)
                {
                    OrderDetail orderdetail = new()
                    {
                        OrderID = order.ID,
                        ProductID = cart.ProductID,
                        Quantity = cart.Quantity,
                        Price = cart.Product.Price
                    };
                    _db.OrderDetails.Add(orderdetail);
                    _db.SaveChanges();
                }

                var viewModel = new CartOrderViewModel
                {
                    Carts = cartFromDb,
                    Order = new Models.Order()
                };
                List<OrderDetail> orderDetails = _db.OrderDetails.Where(o => o.OrderID == order.ID).ToList();
                ViewBag.orderDetails = orderDetails;

                return View(order);
            }

            return View("Checkout");
        }

        //Get
        public IActionResult Payment(int? orderID, bool isSuccess)
        {
            if(orderID == null)
            {
                return NotFound();
            }

            var order = _db.Orders.Find(orderID);

            if (order == null)
            {
                return NotFound();
            }
            
            if (isSuccess)
            {
                order.PaymentStatus = "Approved";
                _db.Orders.Update(order);
                _db.SaveChanges();

            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            HttpContext.Session.Clear();
            var cartsToRemove = _db.Carts.Where(c => c.UserID == userID);
            _db.Carts.RemoveRange(cartsToRemove);
            _db.SaveChanges();

            ViewBag.isSuccess = isSuccess;

            return View(order);
        }
    }
}
