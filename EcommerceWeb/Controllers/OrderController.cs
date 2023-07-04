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

        /// <summary>
        /// Displays the checkout page for the user.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Checkout()
        {
            string userID = GetUserId();

            List<Cart> cartFromDb = await _db.Carts
                .Include(c => c.Product)
                .Where(c => c.UserID == userID)
                .ToListAsync();

            var viewModel = new CartOrderViewModel
            {
                Carts = cartFromDb,
                Order = new Order()
            };

            decimal total = cartFromDb.Sum(c => c.Subtotal);
            ViewBag.Total = total;

            return View(viewModel);
        }

        /// <summary>
        /// Handles the submission of the order and saves it to the database.
        /// </summary>
        /// <param name="order"></param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderConfirmation(Order order)
        {
            if (ModelState.IsValid)
            {
                string userID = GetUserId();

                List<Cart> cartFromDb = await _db.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserID == userID)
                    .ToListAsync();

                decimal total = cartFromDb.Sum(c => c.Subtotal);

                // Sets the necessary properties for the order
                // and adds the order to the database
                order.UserID = userID;
                order.PaymentStatus = "Pending";
                order.Status = "Pending";
                order.Total = total;

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                // Creates the order details from the cart items
                var orderDetails = cartFromDb.Select(cart => new OrderDetail
                {
                    OrderID = order.ID,
                    ProductID = cart.ProductID,
                    Quantity = cart.Quantity,
                    Price = cart.Product.Price
                }).ToList();

                _db.OrderDetails.AddRange(orderDetails);
                await _db.SaveChangesAsync();

                var viewModel = new CartOrderViewModel
                {
                    Carts = cartFromDb,
                    Order = new Order()
                };
                List<OrderDetail> orderDetail = await _db.OrderDetails.Where(o => o.OrderID == order.ID).ToListAsync();
                ViewBag.orderDetails = orderDetail;

                return View(order);
            }

            return View("Checkout");
        }

        /// <summary>
        /// Handles the payment confirmation and updates the payment status of the order.
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public async Task<IActionResult> Payment(int? orderID, bool isSuccess)
        {
            if(orderID == null)
            {
                return NotFound();
            }

            var order = await _db.Orders.FindAsync(orderID);

            if (order == null)
            {
                return NotFound();
            }
            
            if (isSuccess)
            {
                order.PaymentStatus = "Approved";
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();

            }

            string userID = GetUserId();

            HttpContext.Session.Clear();
            var cartsToRemove = _db.Carts.Where(c => c.UserID == userID);
            _db.Carts.RemoveRange(cartsToRemove);
            await _db.SaveChangesAsync();

            ViewBag.isSuccess = isSuccess;

            return View(order);
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
