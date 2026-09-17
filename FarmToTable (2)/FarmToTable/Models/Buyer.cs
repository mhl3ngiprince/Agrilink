using FarmToTable.API.Models;
using System.ComponentModel.DataAnnotations;

namespace FarmToTable.Models
{
    public class Buyer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; }= string.Empty;

        [Phone]
        public string Phone { get; set; }=string.Empty;

        [MaxLength(500)]
        public string Address { get; set; } =string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
