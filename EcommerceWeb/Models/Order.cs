using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWeb.Models
{
    public class Order
    {
        public int ID { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public string Name { get; set; }

        public int Phone { get; set; }

        public string Address { get; set; }

        public string? PaymentStatus { get; set; }

        public string? Status { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string? UserID { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
