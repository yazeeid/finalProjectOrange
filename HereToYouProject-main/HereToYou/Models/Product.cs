using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HereToYou.Models
{
	public class Product
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ProductId { get; set; }
        [MaxLength(255)]
        public string ProductName { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int Stock { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]

        public Category Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }

}
