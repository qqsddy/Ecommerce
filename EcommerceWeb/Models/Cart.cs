using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWeb.Models
{
    public class Cart
    {
        public int ID { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}
