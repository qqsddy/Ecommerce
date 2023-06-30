using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWeb.Models
{
    public class Order
    {
        public int ID { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
