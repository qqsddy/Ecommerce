using EcommerceWeb.Models;

namespace EcommerceWeb.ViewModels
{
    public class CartOrderViewModel
    {
        public Order Order { get; set; }
        public List<Cart> Carts { get; set; }
    }
}
