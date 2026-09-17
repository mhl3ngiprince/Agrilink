namespace FarmToTable.Models
{
    public class SeedResult
    {
        public int CategoriesAdded { get; set; }
        public int FarmersAdded { get; set; }
        public int ProductsAdded { get; set; }
        public int BuyersAdded { get; set; }
        public int OrdersAdded { get; set; }
        public int OrderItemsAdded { get; set; }
        public int TotalRecordsAdded { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }
}
