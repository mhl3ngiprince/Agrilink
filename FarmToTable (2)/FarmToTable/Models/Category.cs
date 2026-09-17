using FarmToTable.API.Models;
using System.ComponentModel.DataAnnotations;

namespace FarmToTable.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Icon { get; set; } = string.Empty;

        public int ProductCount { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
