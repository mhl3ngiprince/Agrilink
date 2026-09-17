namespace FarmToTable.Models
{
    public class SeedRequest
    {
        public bool SeedCategories { get; set; } = true;
        public bool SeedFarmers { get; set; } = true;
        public bool SeedProducts { get; set; } = true;
        public bool SeedBuyers { get; set; } = true;
        public bool SeedOrders { get; set; } = true;
        public bool ClearExistingData { get; set; } = false;
    }
}
