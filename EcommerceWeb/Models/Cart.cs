using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWeb.Models
{
    public class Cart
    {
        public int ID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [NotMapped]
        public decimal Subtotal => Product.Price * Quantity;

        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}
