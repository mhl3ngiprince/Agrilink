using FarmToTable.API.Data;
using FarmToTable.API.Models;
using FarmToTable.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FarmToTable.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedingController : ControllerBase
    {
        private readonly FarmToTableContext _context;

        public SeedingController(FarmToTableContext context)
        {
            _context = context;
        }

        [HttpGet("status")]
        public async Task<ActionResult<DatabaseStatus>> GetStatus()
        {
            var dbName = _context.Database.GetDbConnection().Database;
            var hasAny = await _context.Categories.AnyAsync() || await _context.Farmers.AnyAsync() || await _context.Products.AnyAsync();
            return new DatabaseStatus { DatabaseName = dbName, IsSeeded = hasAny };
        }

        [HttpPost]
        public async Task<ActionResult<SeedResponse>> Seed([FromBody] SeedRequest? request)
        {
            var req = request ?? new SeedRequest();
            var sw = Stopwatch.StartNew();

            if (req.ClearExistingData)
            {
                await ClearAllAsync();
            }

            int categoriesAdded = 0, farmersAdded = 0, productsAdded = 0, buyersAdded = 0, ordersAdded = 0, orderItemsAdded = 0;

            if (req.SeedCategories)
            {
                categoriesAdded = await SeedCategoriesAsync();
            }

            if (req.SeedFarmers)
            {
                farmersAdded = await SeedFarmersAsync();
            }

            if (req.SeedProducts)
            {
                productsAdded = await SeedProductsAsync();
            }

            if (req.SeedBuyers)
            {
                buyersAdded = await SeedBuyersAsync();
            }

            if (req.SeedOrders)
            {
                var (orders, items) = await SeedOrdersAsync();
                ordersAdded = orders;
                orderItemsAdded = items;
            }

            sw.Stop();

            var details = new SeedResult
            {
                CategoriesAdded = categoriesAdded,
                FarmersAdded = farmersAdded,
                ProductsAdded = productsAdded,
                BuyersAdded = buyersAdded,
                OrdersAdded = ordersAdded,
                OrderItemsAdded = orderItemsAdded,
                TotalRecordsAdded = categoriesAdded + farmersAdded + productsAdded + buyersAdded + ordersAdded + orderItemsAdded,
                ExecutionTime = sw.Elapsed
            };

            return new SeedResponse
            {
                Success = true,
                Message = "Database seeded successfully",
                Details = details
            };
        }

        private async Task ClearAllAsync()
        {
            _context.OrderItems.RemoveRange(_context.OrderItems);
            _context.Orders.RemoveRange(_context.Orders);
            _context.Products.RemoveRange(_context.Products);
            _context.Farmers.RemoveRange(_context.Farmers);
            _context.Categories.RemoveRange(_context.Categories);
            _context.Buyers.RemoveRange(_context.Buyers);
            await _context.SaveChangesAsync();
        }

        private async Task<int> SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync()) return 0;

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Vegetables", Icon = "🥬", ProductCount = 12 },
                new Category { Id = 2, Name = "Fruits", Icon = "🍓", ProductCount = 8 },
                new Category { Id = 3, Name = "Dairy", Icon = "🥛", ProductCount = 5 },
                new Category { Id = 4, Name = "Grains", Icon = "🌾", ProductCount = 6 },
                new Category { Id = 5, Name = "Herbs", Icon = "🌿", ProductCount = 4 },
                new Category { Id = 6, Name = "Meat", Icon = "🥩", ProductCount = 3 },
                new Category { Id = 7, Name = "Eggs", Icon = "🥚", ProductCount = 2 }
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            return categories.Count;
        }

        private async Task<int> SeedFarmersAsync()
        {
            if (await _context.Farmers.AnyAsync()) return 0;

            var farmers = new List<Farmer>
            {
                new Farmer { Id = 1, Name = "Green Valley Organics", Email = "info@greenvalleyorganics.co.za", FarmLocation = "Pretoria, GP", Phone = "+27123456789", IsVerified = true },
                new Farmer { Id = 2, Name = "Sunshine Farms", Email = "contact@sunshinefarms.co.za", FarmLocation = "Sandton, GP", Phone = "+27123456790", IsVerified = true },
                new Farmer { Id = 3, Name = "Berry Bliss Farms", Email = "hello@berrybliss.co.za", FarmLocation = "Johannesburg, GP", Phone = "+27123456791", IsVerified = true },
                new Farmer { Id = 4, Name = "Orchard Heights", Email = "info@orchardheights.co.za", FarmLocation = "Pretoria, GP", Phone = "+27123456792", IsVerified = true },
                new Farmer { Id = 5, Name = "Dairy Dream Farm", Email = "sales@dairydream.co.za", FarmLocation = "Pretoria, GP", Phone = "+27123456793", IsVerified = true },
                new Farmer { Id = 6, Name = "Cheese Crafters", Email = "info@cheesecrafters.co.za", FarmLocation = "Benoni, GP", Phone = "+27123456794", IsVerified = true },
                new Farmer { Id = 7, Name = "Morning Harvest", Email = "contact@morningharvest.co.za", FarmLocation = "Pretoria, GP", Phone = "+27123456795", IsVerified = true },
                new Farmer { Id = 8, Name = "Herb Garden", Email = "sales@herbgarden.co.za", FarmLocation = "Midrand, GP", Phone = "+27123456796", IsVerified = true },
                new Farmer { Id = 9, Name = "Ranch Prime", Email = "info@ranchprime.co.za", FarmLocation = "Pretoria, GP", Phone = "+27123456797", IsVerified = true },
                new Farmer { Id = 10, Name = "Happy Farm Poultry", Email = "contact@happyfarmpoultry.co.za", FarmLocation = "Centurion, GP", Phone = "+27123456798", IsVerified = true },
                new Farmer { Id = 11, Name = "Waterside Farm", Email = "info@watersidefarm.co.za", FarmLocation = "Benoni, GP", Phone = "+27123456799", IsVerified = true },
                new Farmer { Id = 12, Name = "Leafy Greens Farm", Email = "sales@leafygreens.co.za", FarmLocation = "Centurion, GP", Phone = "+27123456800", IsVerified = true },
                new Farmer { Id = 13, Name = "Roots & Shoots", Email = "info@rootshoots.co.za", FarmLocation = "Roodepoort, GP", Phone = "+27123456801", IsVerified = true },
                new Farmer { Id = 14, Name = "Rainbow Harvest", Email = "contact@rainbowharvest.co.za", FarmLocation = "Johannesburg, GP", Phone = "+27123456802", IsVerified = true },
                new Farmer { Id = 15, Name = "Earthbound Farm", Email = "info@earthboundfarm.co.za", FarmLocation = "Krugersdorp, GP", Phone = "+27123456803", IsVerified = true },
                new Farmer { Id = 16, Name = "Harvest Moon Farm", Email = "contact@harvestmoon.co.za", FarmLocation = "Benoni, GP", Phone = "+27123456804", IsVerified = true },
                new Farmer { Id = 17, Name = "Cool Greens", Email = "info@coolgreens.co.za", FarmLocation = "Midrand, GP", Phone = "+27123456805", IsVerified = true },
                new Farmer { Id = 18, Name = "Tropical Harvest", Email = "contact@tropicalharvest.co.za", FarmLocation = "Johannesburg, GP", Phone = "+27123456806", IsVerified = true },
                new Farmer { Id = 19, Name = "Citrus Grove", Email = "info@citrusgrove.co.za", FarmLocation = "Centurion, GP", Phone = "+27123456807", IsVerified = true },
                new Farmer { Id = 20, Name = "Green Gold Farm", Email = "contact@greengoldfarm.co.za", FarmLocation = "Midrand, GP", Phone = "+27123456808", IsVerified = true },
                new Farmer { Id = 21, Name = "Vineyard Valley", Email = "info@vineyardvalley.co.za", FarmLocation = "Stellenbosh, WC", Phone = "+27123456809", IsVerified = true },
                new Farmer { Id = 22, Name = "Golden Dairy", Email = "contact@goldendairy.co.za", FarmLocation = "Centurion, GP", Phone = "+27123456810", IsVerified = true },
                new Farmer { Id = 23, Name = "Golden Fields", Email = "info@goldenfields.co.za", FarmLocation = "Krugersdorp, GP", Phone = "+27123456811", IsVerified = true },
                new Farmer { Id = 24, Name = "Rice Valley Farm", Email = "contact@ricevalley.co.za", FarmLocation = "Johannesburg, GP", Phone = "+27123456812", IsVerified = true },
                new Farmer { Id = 25, Name = "Ancient Grains Co", Email = "info@ancientgrains.co.za", FarmLocation = "Sandton, GP", Phone = "+27123456813", IsVerified = true },
                new Farmer { Id = 26, Name = "Green Herbs Farm", Email = "contact@greenherbs.co.za", FarmLocation = "Centurion, GP", Phone = "+27123456814", IsVerified = true },
                new Farmer { Id = 27, Name = "Meadow Meats", Email = "info@meadowmeats.co.za", FarmLocation = "Johannesburg, GP", Phone = "+27123456815", IsVerified = true },
                new Farmer { Id = 28, Name = "Happy Hen Coop", Email = "contact@happyhencoop.co.za", FarmLocation = "Midrand, GP", Phone = "+27123456816", IsVerified = true }
            };

            await _context.Farmers.AddRangeAsync(farmers);
            await _context.SaveChangesAsync();
            return farmers.Count;
        }

        private async Task<int> SeedProductsAsync()
        {
            if (await _context.Products.AnyAsync()) return 0;

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Organic Cherry Tomatoes", Description = "Sweet, vine-ripened organic tomatoes packed with flavor", Price = 45.99m, CategoryId = 1, FarmerId = 1, StockQuantity = 150, IsOrganic = true, IsFeatured = true, AverageRating = 4.8m, ReviewCount = 124, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/tomatoes.jpg", AvailableQuantity = 150 },
                new Product { Id = 2, Name = "Golden Sweet Corn", Description = "Sweet, crunchy corn on the cob, freshly harvested", Price = 25.00m, CategoryId = 1, FarmerId = 2, StockQuantity = 120, IsOrganic = false, IsFeatured = true, AverageRating = 4.6m, ReviewCount = 92, DeliveryTime = "Same day", Unit = "each", ImageUrl = "/images/corn.jpg", AvailableQuantity = 120 },
                new Product { Id = 3, Name = "Fresh Broccoli", Description = "Green, fresh broccoli crowns full of nutrients", Price = 38.00m, CategoryId = 1, FarmerId = 1, StockQuantity = 65, IsOrganic = true, IsFeatured = true, AverageRating = 4.9m, ReviewCount = 102, DeliveryTime = "Same day", Unit = "kg", ImageUrl = "/images/broccoli.jpg", AvailableQuantity = 65 },
                new Product { Id = 4, Name = "Fresh Strawberries", Description = "Sweet and juicy strawberries, handpicked daily from our fields", Price = 89.99m, CategoryId = 2, FarmerId = 3, StockQuantity = 80, IsOrganic = true, IsFeatured = true, AverageRating = 4.9m, ReviewCount = 89, DeliveryTime = "Same day", Unit = "kg", ImageUrl = "/images/strawberries.jpg", AvailableQuantity = 80 },
                new Product { Id = 5, Name = "Red Apples", Description = "Crisp and sweet red apples, perfect for snacking", Price = 52.00m, CategoryId = 2, FarmerId = 4, StockQuantity = 140, IsOrganic = false, IsFeatured = true, AverageRating = 4.8m, ReviewCount = 112, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/apples.jpg", AvailableQuantity = 140 },
                new Product { Id = 6, Name = "Fresh Blueberries", Description = "Antioxidant-rich blueberries, sweet and tangy", Price = 125.00m, CategoryId = 2, FarmerId = 3, StockQuantity = 45, IsOrganic = true, IsFeatured = true, AverageRating = 5.0m, ReviewCount = 78, DeliveryTime = "Same day", Unit = "kg", ImageUrl = "/images/blueberries.jpg", AvailableQuantity = 45 },
                new Product { Id = 7, Name = "Fresh Whole Milk", Description = "Pure farm-fresh whole milk, unhomogenized", Price = 32.00m, CategoryId = 3, FarmerId = 5, StockQuantity = 85, IsOrganic = true, IsFeatured = true, AverageRating = 4.9m, ReviewCount = 143, DeliveryTime = "Same day", Unit = "liter", ImageUrl = "/images/milk.jpg", AvailableQuantity = 85 },
                new Product { Id = 8, Name = "Artisan Cheese", Description = "Handcrafted farmhouse cheese with rich flavor", Price = 145.00m, CategoryId = 3, FarmerId = 6, StockQuantity = 35, IsOrganic = false, IsFeatured = true, AverageRating = 5.0m, ReviewCount = 67, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/cheese.jpg", AvailableQuantity = 35 },
                new Product { Id = 9, Name = "Organic Oats", Description = "Rolled oats perfect for breakfast", Price = 45.00m, CategoryId = 4, FarmerId = 7, StockQuantity = 135, IsOrganic = true, IsFeatured = true, AverageRating = 4.9m, ReviewCount = 112, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/oats.jpg", AvailableQuantity = 135 },
                new Product { Id = 10, Name = "Fresh Basil", Description = "Aromatic fresh basil leaves", Price = 28.00m, CategoryId = 5, FarmerId = 8, StockQuantity = 70, IsOrganic = true, IsFeatured = true, AverageRating = 4.9m, ReviewCount = 82, DeliveryTime = "Same day", Unit = "bunch", ImageUrl = "/images/basil.jpg", AvailableQuantity = 70 },
                new Product { Id = 11, Name = "Grass-Fed Beef", Description = "Premium grass-fed beef, ethically raised", Price = 185.00m, CategoryId = 6, FarmerId = 9, StockQuantity = 45, IsOrganic = true, IsFeatured = true, AverageRating = 5.0m, ReviewCount = 124, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/beef.jpg", AvailableQuantity = 45 },
                new Product { Id = 12, Name = "Free-Range Chicken", Description = "Tender free-range chicken, hormone-free", Price = 115.00m, CategoryId = 6, FarmerId = 10, StockQuantity = 68, IsOrganic = false, IsFeatured = true, AverageRating = 4.9m, ReviewCount = 156, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/chicken.jpg", AvailableQuantity = 68 },
                new Product { Id = 13, Name = "Organic Duck Eggs", Description = "Rich and creamy duck eggs, perfect for baking", Price = 78.00m, CategoryId = 7, FarmerId = 11, StockQuantity = 65, IsOrganic = true, IsFeatured = true, AverageRating = 4.9m, ReviewCount = 74, DeliveryTime = "1-2 days", Unit = "dozen", ImageUrl = "/images/duck-eggs.jpg", AvailableQuantity = 65 },
                new Product { Id = 14, Name = "Organic Spinach", Description = "Nutrient-rich fresh spinach leaves, perfect for salads", Price = 35.00m, CategoryId = 1, FarmerId = 12, StockQuantity = 85, IsOrganic = true, IsFeatured = false, AverageRating = 4.7m, ReviewCount = 67, DeliveryTime = "1-2 days", Unit = "bunch", ImageUrl = "/images/spinach.jpg", AvailableQuantity = 85 },
                new Product { Id = 15, Name = "Fresh Carrots", Description = "Crisp and sweet carrots, rich in vitamins and minerals", Price = 30.00m, CategoryId = 1, FarmerId = 13, StockQuantity = 95, IsOrganic = true, IsFeatured = false, AverageRating = 4.8m, ReviewCount = 78, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/carrots.jpg", AvailableQuantity = 95 },
                new Product { Id = 16, Name = "Red Bell Peppers", Description = "Vibrant and crispy bell peppers, perfect for any dish", Price = 42.00m, CategoryId = 1, FarmerId = 14, StockQuantity = 70, IsOrganic = false, IsFeatured = false, AverageRating = 4.7m, ReviewCount = 54, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/bell-peppers.jpg", AvailableQuantity = 70 },
                new Product { Id = 17, Name = "Baby Potatoes", Description = "Tender baby potatoes, perfect for roasting", Price = 28.00m, CategoryId = 1, FarmerId = 15, StockQuantity = 110, IsOrganic = false, IsFeatured = false, AverageRating = 4.6m, ReviewCount = 88, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/potatoes.jpg", AvailableQuantity = 110 },
                new Product { Id = 18, Name = "Butternut Squash", Description = "Sweet and creamy butternut squash, perfect for soups", Price = 32.00m, CategoryId = 1, FarmerId = 16, StockQuantity = 90, IsOrganic = false, IsFeatured = false, AverageRating = 4.5m, ReviewCount = 45, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/squash.jpg", AvailableQuantity = 90 },
                new Product { Id = 19, Name = "Fresh Cucumber", Description = "Crisp and refreshing cucumbers, perfect for salads", Price = 26.00m, CategoryId = 1, FarmerId = 17, StockQuantity = 105, IsOrganic = true, IsFeatured = false, AverageRating = 4.7m, ReviewCount = 61, DeliveryTime = "Same day", Unit = "kg", ImageUrl = "/images/cucumber.jpg", AvailableQuantity = 105 },
                new Product { Id = 20, Name = "Green Beans", Description = "Tender green beans, freshly picked", Price = 34.00m, CategoryId = 1, FarmerId = 2, StockQuantity = 75, IsOrganic = false, IsFeatured = false, AverageRating = 4.6m, ReviewCount = 52, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/green-beans.jpg", AvailableQuantity = 75 },
                new Product { Id = 21, Name = "Red Onions", Description = "Fresh red onions with a mild, sweet flavor", Price = 22.00m, CategoryId = 1, FarmerId = 13, StockQuantity = 130, IsOrganic = false, IsFeatured = false, AverageRating = 4.5m, ReviewCount = 72, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/onions.jpg", AvailableQuantity = 130 },
                new Product { Id = 22, Name = "Fresh Lettuce", Description = "Crispy lettuce heads, perfect for fresh salads", Price = 24.00m, CategoryId = 1, FarmerId = 12, StockQuantity = 95, IsOrganic = true, IsFeatured = false, AverageRating = 4.8m, ReviewCount = 84, DeliveryTime = "Same day", Unit = "head", ImageUrl = "/images/lettuce.jpg", AvailableQuantity = 95 },
                new Product { Id = 23, Name = "Fresh Bananas", Description = "Ripe yellow bananas, naturally sweet and creamy", Price = 38.00m, CategoryId = 2, FarmerId = 18, StockQuantity = 200, IsOrganic = false, IsFeatured = false, AverageRating = 4.7m, ReviewCount = 156, DeliveryTime = "Same day", Unit = "kg", ImageUrl = "/images/bananas.jpg", AvailableQuantity = 200 },
                new Product { Id = 24, Name = "Juicy Oranges", Description = "Fresh citrus oranges bursting with vitamin C", Price = 46.00m, CategoryId = 2, FarmerId = 19, StockQuantity = 125, IsOrganic = true, IsFeatured = false, AverageRating = 4.9m, ReviewCount = 98, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/oranges.jpg", AvailableQuantity = 125 },
                new Product { Id = 25, Name = "Ripe Avocados", Description = "Creamy avocados, perfect for guacamole or toast", Price = 68.00m, CategoryId = 2, FarmerId = 20, StockQuantity = 90, IsOrganic = true, IsFeatured = false, AverageRating = 4.8m, ReviewCount = 134, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/avocados.jpg", AvailableQuantity = 90 },
                new Product { Id = 26, Name = "Sweet Peaches", Description = "Juicy peaches with velvety skin and sweet flesh", Price = 72.00m, CategoryId = 2, FarmerId = 4, StockQuantity = 65, IsOrganic = false, IsFeatured = false, AverageRating = 4.7m, ReviewCount = 87, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/peaches.jpg", AvailableQuantity = 65 },
                new Product { Id = 27, Name = "Fresh Grapes", Description = "Seedless green grapes, sweet and refreshing", Price = 58.00m, CategoryId = 2, FarmerId = 21, StockQuantity = 110, IsOrganic = false, IsFeatured = false, AverageRating = 4.6m, ReviewCount = 92, DeliveryTime = "Same day", Unit = "kg", ImageUrl = "/images/grapes.jpg", AvailableQuantity = 110 },
                new Product { Id = 28, Name = "Greek Yogurt", Description = "Creamy Greek yogurt made from fresh milk", Price = 48.00m, CategoryId = 3, FarmerId = 5, StockQuantity = 95, IsOrganic = true, IsFeatured = false, AverageRating = 4.8m, ReviewCount = 89, DeliveryTime = "Same day", Unit = "500g", ImageUrl = "/images/yogurt.jpg", AvailableQuantity = 95 },
                new Product { Id = 29, Name = "Farm Butter", Description = "Rich, creamy butter churned from fresh cream", Price = 85.00m, CategoryId = 3, FarmerId = 22, StockQuantity = 60, IsOrganic = false, IsFeatured = false, AverageRating = 4.9m, ReviewCount = 102, DeliveryTime = "1-2 days", Unit = "500g", ImageUrl = "/images/butter.jpg", AvailableQuantity = 60 },
                new Product { Id = 30, Name = "Fresh Cream", Description = "Heavy cream perfect for cooking and desserts", Price = 52.00m, CategoryId = 3, FarmerId = 5, StockQuantity = 75, IsOrganic = true, IsFeatured = false, AverageRating = 4.7m, ReviewCount = 71, DeliveryTime = "Same day", Unit = "500ml", ImageUrl = "/images/cream.jpg", AvailableQuantity = 75 },
                new Product { Id = 31, Name = "Organic Wheat", Description = "Stone-ground organic wheat flour", Price = 42.00m, CategoryId = 4, FarmerId = 23, StockQuantity = 150, IsOrganic = true, IsFeatured = false, AverageRating = 4.8m, ReviewCount = 94, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/wheat.jpg", AvailableQuantity = 150 },
                new Product { Id = 32, Name = "Brown Rice", Description = "Whole grain brown rice, nutrient-rich", Price = 38.00m, CategoryId = 4, FarmerId = 24, StockQuantity = 200, IsOrganic = false, IsFeatured = false, AverageRating = 4.7m, ReviewCount = 86, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/rice.jpg", AvailableQuantity = 200 },
                new Product { Id = 33, Name = "Quinoa", Description = "Protein-rich quinoa seeds", Price = 125.00m, CategoryId = 4, FarmerId = 25, StockQuantity = 55, IsOrganic = true, IsFeatured = false, AverageRating = 4.8m, ReviewCount = 67, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/quinoa.jpg", AvailableQuantity = 55 },
                new Product { Id = 34, Name = "Barley", Description = "Whole barley grains for soups and stews", Price = 36.00m, CategoryId = 4, FarmerId = 23, StockQuantity = 110, IsOrganic = false, IsFeatured = false, AverageRating = 4.6m, ReviewCount = 52, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/barley.jpg", AvailableQuantity = 110 },
                new Product { Id = 35, Name = "Millet", Description = "Gluten-free millet grains", Price = 48.00m, CategoryId = 4, FarmerId = 25, StockQuantity = 85, IsOrganic = true, IsFeatured = false, AverageRating = 4.7m, ReviewCount = 48, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/millet.jpg", AvailableQuantity = 85 },
                new Product { Id = 36, Name = "Fresh Rosemary", Description = "Fragrant rosemary sprigs", Price = 24.00m, CategoryId = 5, FarmerId = 8, StockQuantity = 85, IsOrganic = true, IsFeatured = false, AverageRating = 4.8m, ReviewCount = 68, DeliveryTime = "Same day", Unit = "bunch", ImageUrl = "/images/rosemary.jpg", AvailableQuantity = 85 },
                new Product { Id = 37, Name = "Fresh Mint", Description = "Cool and refreshing mint leaves", Price = 22.00m, CategoryId = 5, FarmerId = 26, StockQuantity = 95, IsOrganic = true, IsFeatured = false, AverageRating = 4.7m, ReviewCount = 72, DeliveryTime = "Same day", Unit = "bunch", ImageUrl = "/images/mint.jpg", AvailableQuantity = 95 },
                new Product { Id = 38, Name = "Fresh Thyme", Description = "Earthy thyme sprigs perfect for seasoning", Price = 26.00m, CategoryId = 5, FarmerId = 8, StockQuantity = 78, IsOrganic = true, IsFeatured = false, AverageRating = 4.8m, ReviewCount = 56, DeliveryTime = "Same day", Unit = "bunch", ImageUrl = "/images/thyme.jpg", AvailableQuantity = 78 },
                new Product { Id = 39, Name = "Lamb Chops", Description = "Succulent lamb chops from pasture-raised sheep", Price = 225.00m, CategoryId = 6, FarmerId = 27, StockQuantity = 32, IsOrganic = true, IsFeatured = false, AverageRating = 4.8m, ReviewCount = 89, DeliveryTime = "1-2 days", Unit = "kg", ImageUrl = "/images/lamb.jpg", AvailableQuantity = 32 },
                new Product { Id = 40, Name = "Farm Fresh Eggs", Description = "Free-range chicken eggs from happy, healthy hens", Price = 55.00m, CategoryId = 7, FarmerId = 28, StockQuantity = 200, IsOrganic = false, IsFeatured = false, AverageRating = 5.0m, ReviewCount = 156, DeliveryTime = "1-2 days", Unit = "dozen", ImageUrl = "/images/eggs.jpg", AvailableQuantity = 200 }
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            return products.Count;
        }

        private async Task<int> SeedBuyersAsync()
        {
            if (await _context.Buyers.AnyAsync()) return 0;

            var buyers = new List<Buyer>
            {
                new Buyer { Name = "John Doe", Email = "john@example.com", Phone = "+27000000001", Address = "123 Main St, Pretoria" },
                new Buyer { Name = "Jane Smith", Email = "jane@example.com", Phone = "+27000000002", Address = "456 Oak Ave, Johannesburg" },
                new Buyer { Name = "Alice Brown", Email = "alice@example.com", Phone = "+27000000003", Address = "789 Pine Rd, Centurion" }
            };

            await _context.Buyers.AddRangeAsync(buyers);
            await _context.SaveChangesAsync();
            return buyers.Count;
        }

        private async Task<(int orders, int items)> SeedOrdersAsync()
        {
            if (!await _context.Products.AnyAsync() || await _context.Orders.AnyAsync()) return (0, 0);

            var firstProduct = await _context.Products.FirstAsync();
            var order = new Order
            {
                BuyerName = "John Doe",
                BuyerEmail = "john@example.com",
                OrderDate = DateTime.UtcNow,
                Status = "Pending"
            };

            var item = new OrderItem
            {
                ProductId = firstProduct.Id,
                Quantity = 2,
                UnitPrice = firstProduct.Price
            };

            order.OrderItems.Add(item);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return (1, 1);
        }
    }
}