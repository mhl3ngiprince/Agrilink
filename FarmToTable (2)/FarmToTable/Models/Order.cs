using FarmToTable.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmToTable.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        // Navigation properties
        public virtual Buyer Buyer { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string BuyerName { get; internal set; }
        public string BuyerEmail { get; internal set; }
    }
}