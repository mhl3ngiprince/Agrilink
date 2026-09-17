using FarmToTable.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmToTable.API.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public int FarmerId { get; set; }
        public int StockQuantity { get; set; }
        public bool IsOrganic { get; set; }
        public bool IsFeatured { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; set; }

        public int ReviewCount { get; set; }

        [MaxLength(50)]
        public string DeliveryTime { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Category Category { get; set; } = null!;
        public virtual Farmer Farmer { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public int AvailableQuantity { get; set; }
        public int ProductCount { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    public class ProductReview
    {
    }
}