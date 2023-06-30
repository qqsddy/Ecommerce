using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWeb.Models
{
    public class Customer
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Username cannot be longer then 50 characters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
