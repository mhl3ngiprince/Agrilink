using System.ComponentModel.DataAnnotations;

namespace FarmToTable.API.Models
{
    public class Farmer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Farm location is required")]
        [MaxLength(200)]
        public string FarmLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public DateTime RegistrationDate { get; internal set; }
        public bool IsVerified { get; internal set; }
    }
}