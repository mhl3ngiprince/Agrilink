namespace FarmToTable.Models
{
    public class DatabaseStatus
    {
        public int TotalCategories { get; set; }
        public int TotalFarmers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalBuyers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalOrderItems { get; set; }
        public DateTime LastUpdated { get; set; }
        public string DatabaseName { get; set; }
        public bool IsSeeded { get; set; }
    }
}
