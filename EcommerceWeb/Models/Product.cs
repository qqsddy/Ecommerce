using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWeb.Models
{
    public class Product
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }

        [DisplayName("Image")]
        public string? ImageUrl { get; set; }    
    }
}
