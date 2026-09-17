using System.Globalization;
using FarmToTable.API.Models;
using FarmToTable.Models;
using Microsoft.EntityFrameworkCore;

namespace FarmToTable.API.Data
{
    public static class SampleDataSeeder
    {
        private record RawProduct(
            string Name,
            string Category,
            string Description,
            decimal Price,
            string Unit,
            int AvailableQuantity,
            string Image,
            string FarmerName,
            string FarmerLocation,
            decimal Rating,
            int Reviews,
            bool IsOrganic,
            bool IsFeatured,
            string DeliveryTime
        );

        public static async Task SeedAsync(FarmToTableContext db)
        {
            // Always attempt to seed or update, to ensure quantities and metadata are consistent

            var categoryIcons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Vegetables"] = "🥬",
                ["Fruits"] = "🍎",
                ["Eggs"] = "🥚",
                ["Dairy"] = "🧀",
                ["Grains"] = "🌾",
                ["Herbs"] = "🌿",
                ["Meat"] = "🥩"
            };

            var data = new List<RawProduct>
            {
                new("Organic Cherry Tomatoes","Vegetables","Sweet, vine-ripened organic tomatoes packed with flavor",45.99m,"kg",150,"https://plus.unsplash.com/premium_photo-1675366298841-4451b04055fa?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1170","Green Valley Organics","Pretoria, GP",4.8m,124,true,true,"1-2 days"),
                new("Fresh Strawberries","Fruits","Sweet and juicy strawberries, handpicked daily from our fields",89.99m,"kg",80,"https://images.unsplash.com/photo-1464965911861-746a04b4bca6?w=400&h=300&fit=crop","Berry Bliss Farms","Johannesburg, GP",4.9m,89,true,true,"Same day"),
                new("Farm Fresh Eggs","Eggs","Free-range chicken eggs from happy, healthy hens",55.00m,"dozen",200,"https://images.unsplash.com/photo-1582722872445-44dc5f7e3c8f?w=400&h=300&fit=crop","Happy Hen Coop","Midrand, GP",5.0m,156,false,false,"1-2 days"),
                new("Organic Spinach","Vegetables","Nutrient-rich fresh spinach leaves, perfect for salads",35.00m,"bunch",0,"https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=400&h=300&fit=crop","Leafy Greens Farm","Centurion, GP",4.7m,67,true,false,"1-2 days"),
                new("Golden Sweet Corn","Vegetables","Sweet, crunchy corn on the cob, freshly harvested",25.00m,"each",120,"https://images.unsplash.com/photo-1551754655-cd27e38d2076?w=400&h=300&fit=crop","Sunshine Farms","Sandton, GP",4.6m,92,false,true,"Same day"),
                new("Fresh Carrots","Vegetables","Crisp and sweet carrots, rich in vitamins and minerals",30.00m,"kg",95,"https://images.unsplash.com/photo-1598170845058-32b9d6a5da37?w=400&h=300&fit=crop","Roots & Shoots","Roodepoort, GP",4.8m,78,true,false,"1-2 days"),
                new("Fresh Broccoli","Vegetables","Green, fresh broccoli crowns full of nutrients",38.00m,"kg",65,"https://plus.unsplash.com/premium_photo-1724250161295-ccb9c5f4f63d?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=400&h=300","Green Valley Organics","Pretoria, GP",4.9m,102,true,true,"Same day"),
                new("Butternut Squash","Vegetables","Sweet and creamy butternut squash, perfect for soups",32.00m,"kg",90,"https://images.unsplash.com/photo-1528496237352-2cb43f05806d?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=400&h=300","Harvest Moon Farm","Benoni, GP",4.5m,45,false,false,"1-2 days"),
                new("Fresh Cucumber","Vegetables","Crisp and refreshing cucumbers, perfect for salads",26.00m,"kg",105,"https://images.unsplash.com/photo-1449300079323-02e209d9d3a6?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8Y3VjdW1iZXJ8ZW58MHx8MHx8fDA%3D&auto=format&fit=crop&w=400&h=300","Cool Greens","Midrand, GP",4.7m,61,true,false,"Same day"),
                new("Green Beans","Vegetables","Tender green beans, freshly picked",34.00m,"kg",75,"https://plus.unsplash.com/premium_photo-1725384940646-ef6aa8c2a091?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=400&h=300","Sunshine Farms","Sandton, GP",4.6m,52,false,false,"1-2 days"),
                new("Red Onions","Vegetables","Fresh red onions with a mild, sweet flavor",22.00m,"kg",130,"https://images.unsplash.com/photo-1668295037469-8b0e8d11cd2a?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8cmVkJTIwb25pb25zfGVufDB8fDB8fHww&auto=format&fit=crop&w=400&h=300","Roots & Shoots","Roodepoort, GP",4.5m,72,false,false,"1-2 days"),
                new("Fresh Lettuce","Vegetables","Crispy lettuce heads, perfect for fresh salads",24.00m,"head",95,"https://images.unsplash.com/photo-1622206151226-18ca2c9ab4a1?w=400&h=300&fit=crop","Leafy Greens Farm","Centurion, GP",4.8m,84,true,false,"Same day"),
                new("Red Apples","Fruits","Crisp and sweet red apples, perfect for snacking",52.00m,"kg",140,"https://images.unsplash.com/photo-1560806887-1e4cd0b6cbd6?w=400&h=300&fit=crop","Orchard Heights","Pretoria, GP",4.8m,112,false,true,"1-2 days"),
                new("Fresh Bananas","Fruits","Ripe yellow bananas, naturally sweet and creamy",38.00m,"kg",200,"https://images.unsplash.com/photo-1571771894821-ce9b6c11b08e?w=400&h=300&fit=crop","Tropical Harvest","Johannesburg, GP",4.7m,156,false,false,"Same day"),
                new("Juicy Oranges","Fruits","Fresh citrus oranges bursting with vitamin C",46.00m,"kg",125,"https://images.unsplash.com/photo-1582979512210-99b6a53386f9?w=400&h=300&fit=crop","Citrus Grove","Centurion, GP",4.9m,98,true,false,"1-2 days"),
                new("Fresh Blueberries","Fruits","Antioxidant-rich blueberries, sweet and tangy",125.00m,"kg",45,"https://images.unsplash.com/photo-1498557850523-fd3d118b962e?w=400&h=300&fit=crop","Berry Bliss Farms","Johannesburg, GP",5.0m,78,true,true,"Same day"),
                new("Ripe Avocados","Fruits","Creamy avocados, perfect for guacamole or toast",68.00m,"kg",90,"https://images.unsplash.com/photo-1523049673857-eb18f1d7b578?w=400&h=300&fit=crop","Green Gold Farm","Midrand, GP",4.8m,134,true,false,"1-2 days"),
                new("Sweet Peaches","Fruits","Juicy peaches with velvety skin and sweet flesh",72.00m,"kg",65,"https://images.unsplash.com/photo-1568584711611-4dfa4cd196f1?w=400&h=300&fit=crop","Orchard Heights","Pretoria, GP",4.7m,87,false,false,"1-2 days"),
                new("Fresh Grapes","Fruits","Seedless green grapes, sweet and refreshing",58.00m,"kg",110,"https://images.unsplash.com/photo-1632404947032-d0d4bdf1a185?w=400&h=300&fit=crop","Vineyard Valley","Stellenbosh, WC",4.6m,92,false,false,"Same day"),
                new("Fresh Whole Milk","Dairy","Pure farm-fresh whole milk, unhomogenized",32.00m,"liter",85,"https://images.unsplash.com/photo-1550583724-b2692b85b150?w=400&h=300&fit=crop","Dairy Dream Farm","Pretoria, GP",4.9m,143,true,true,"Same day"),
                new("Artisan Cheese","Dairy","Handcrafted farmhouse cheese with rich flavor",145.00m,"kg",35,"https://images.unsplash.com/photo-1452195100486-9cc805987862?w=400&h=300&fit=crop","Cheese Crafters","Benoni, GP",5.0m,67,false,true,"1-2 days"),
                new("Greek Yogurt","Dairy","Creamy Greek yogurt made from fresh milk",48.00m,"500g",95,"https://images.unsplash.com/photo-1488477181946-6428a0291777?w=400&h=300&fit=crop","Dairy Dream Farm","Pretoria, GP",4.8m,89,true,false,"Same day"),
                new("Farm Butter","Dairy","Rich, creamy butter churned from fresh cream",85.00m,"500g",60,"https://images.unsplash.com/photo-1589985270826-4b7bb135bc9d?w=400&h=300&fit=crop","Golden Dairy","Centurion, GP",4.9m,102,false,false,"1-2 days"),
                new("Fresh Cream","Dairy","Heavy cream perfect for cooking and desserts",52.00m,"500ml",75,"https://images.unsplash.com/photo-1628088062854-d1870b4553da?w=400&h=300&fit=crop","Dairy Dream Farm","Pretoria, GP",4.7m,71,true,false,"Same day"),
                new("Organic Wheat","Grains","Stone-ground organic wheat flour",42.00m,"kg",150,"https://images.unsplash.com/photo-1574323347407-f5e1ad6d020b?w=400&h=300&fit=crop","Golden Fields","Krugersdorp, GP",4.8m,94,true,false,"1-2 days"),
                new("Brown Rice","Grains","Whole grain brown rice, nutrient-rich",38.00m,"kg",200,"https://images.unsplash.com/photo-1586201375761-83865001e31c?w=400&h=300&fit=crop","Rice Valley Farm","Johannesburg, GP",4.7m,86,false,false,"1-2 days"),
                new("Organic Oats","Grains","Rolled oats perfect for breakfast",45.00m,"kg",135,"https://images.unsplash.com/photo-1614373532018-92a75430a0da?w=400&h=300&fit=crop","Morning Harvest","Pretoria, GP",4.9m,112,true,true,"1-2 days"),
                new("Quinoa","Grains","Protein-rich quinoa seeds",125.00m,"kg",55,"https://images.unsplash.com/photo-1586201375761-83865001e31c?w=400&h=300&fit=crop","Ancient Grains Co","Sandton, GP",4.8m,67,true,false,"1-2 days"),
                new("Barley","Grains","Whole barley grains for soups and stews",36.00m,"kg",110,"https://images.unsplash.com/photo-1582363476910-3223e5fd0b32?w=400&h=300&fit=crop","Golden Fields","Krugersdorp, GP",4.6m,52,false,false,"1-2 days"),
                new("Millet","Grains","Gluten-free millet grains",48.00m,"kg",85,"https://plus.unsplash.com/premium_photo-1671130295829-23e2b49874a2?w=400&h=300&fit=crop","Ancient Grains Co","Sandton, GP",4.7m,48,true,false,"1-2 days"),
                new("Fresh Basil","Herbs","Aromatic fresh basil leaves",28.00m,"bunch",70,"https://images.unsplash.com/photo-1618375569909-3c8616cf7733?w=400&h=300&fit=crop","Herb Garden","Midrand, GP",4.9m,82,true,true,"Same day"),
                new("Fresh Rosemary","Herbs","Fragrant rosemary sprigs",24.00m,"bunch",85,"https://images.unsplash.com/photo-1603129624917-3c579e864025?w=400&h=300&fit=crop","Herb Garden","Midrand, GP",4.8m,68,true,false,"Same day"),
                new("Fresh Mint","Herbs","Cool and refreshing mint leaves",22.00m,"bunch",95,"https://images.unsplash.com/photo-1628556270448-4d4e4148e1b1?w=400&h=300&fit=crop","Green Herbs Farm","Centurion, GP",4.7m,72,true,false,"Same day"),
                new("Fresh Thyme","Herbs","Earthy thyme sprigs perfect for seasoning",26.00m,"bunch",78,"https://plus.unsplash.com/premium_photo-1726138617688-e6bfd9f0de5c?w=400&h=300&fit=crop","Herb Garden","Midrand, GP",4.8m,56,true,false,"Same day"),
                new("Grass-Fed Beef","Meat","Premium grass-fed beef, ethically raised",185.00m,"kg",45,"https://images.unsplash.com/photo-1690983330536-3b0089d07cf9?w=400&h=300&fit=crop","Ranch Prime","Pretoria, GP",5.0m,124,true,true,"1-2 days"),
                new("Free-Range Chicken","Meat","Tender free-range chicken, hormone-free",115.00m,"kg",68,"https://images.unsplash.com/photo-1587593810167-a84920ea0781?w=400&h=300&fit=crop","Happy Farm Poultry","Centurion, GP",4.9m,156,false,true,"1-2 days"),
                new("Lamb Chops","Meat","Succulent lamb chops from pasture-raised sheep",225.00m,"kg",32,"https://plus.unsplash.com/premium_photo-1670357425093-6297b188c4a7?w=400&h=300&fit=crop","Meadow Meats","Johannesburg, GP",4.8m,89,true,false,"1-2 days"),
                new("Organic Duck Eggs","Eggs","Rich and creamy duck eggs, perfect for baking",78.00m,"dozen",65,"https://plus.unsplash.com/premium_photo-1723795259709-255a16373241?w=400&h=300&fit=crop","Waterside Farm","Benoni, GP",4.9m,74,true,true,"1-2 days")
            };

            // Ensure Categories
            var categories = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);
            foreach (var rp in data.Select(d => d.Category).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var cat = await db.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == rp.ToLower());
                if (cat == null)
                {
                    cat = new Category
                    {
                        Name = rp,
                        Icon = categoryIcons.TryGetValue(rp, out var ic) ? ic : "📦",
                        ProductCount = 0
                    };
                    db.Categories.Add(cat);
                    await db.SaveChangesAsync();
                }
                categories[rp] = cat;
            }

            // Ensure Farmers
            var farmers = new Dictionary<string, Farmer>(StringComparer.OrdinalIgnoreCase);
            foreach (var rp in data)
            {
                var key = rp.FarmerName.Trim();
                if (farmers.ContainsKey(key)) continue;
                var farmer = await db.Farmers.FirstOrDefaultAsync(f => f.Name.ToLower() == key.ToLower());
                if (farmer == null)
                {
                    farmer = new Farmer
                    {
                        Name = rp.FarmerName,
                        FarmLocation = rp.FarmerLocation,
                        Email = $"{Slug(rp.FarmerName)}@farm.local",
                        Phone = "+0000000000",
                        RegistrationDate = DateTime.UtcNow,
                        IsVerified = true
                    };
                    db.Farmers.Add(farmer);
                    await db.SaveChangesAsync();
                }
                farmers[key] = farmer;
            }

            // Upsert Products (update if exists, otherwise add)
            foreach (var rp in data)
            {
                var farmer = farmers[rp.FarmerName.Trim()];
                var category = categories[rp.Category];

                var existing = await db.Products.FirstOrDefaultAsync(p => p.FarmerId == farmer.Id && p.Name.ToLower() == rp.Name.ToLower());
                if (existing != null)
                {
                    existing.Description = rp.Description;
                    existing.Price = rp.Price;
                    existing.Unit = rp.Unit;
                    existing.ImageUrl = rp.Image;
                    existing.IsOrganic = rp.IsOrganic;
                    existing.IsFeatured = rp.IsFeatured;
                    existing.DeliveryTime = rp.DeliveryTime;
                    existing.AverageRating = rp.Rating;
                    existing.ReviewCount = rp.Reviews;
                    existing.StockQuantity = Math.Max(existing.StockQuantity, Math.Max(0, rp.AvailableQuantity));
                    existing.AvailableQuantity = Math.Max(existing.AvailableQuantity, Math.Max(0, rp.AvailableQuantity));
                    existing.CategoryId = category.Id;
                    existing.FarmerId = farmer.Id;
                }
                else
                {
                    var product = new Product
                    {
                        Name = rp.Name,
                        Description = rp.Description,
                        Price = rp.Price,
                        Unit = rp.Unit,
                        ImageUrl = rp.Image,
                        IsOrganic = rp.IsOrganic,
                        IsFeatured = rp.IsFeatured,
                        DeliveryTime = rp.DeliveryTime,
                        AverageRating = rp.Rating,
                        ReviewCount = rp.Reviews,
                        StockQuantity = Math.Max(rp.AvailableQuantity, 0),
                        AvailableQuantity = Math.Max(rp.AvailableQuantity, 0),
                        CategoryId = category.Id,
                        FarmerId = farmer.Id,
                        CreatedDate = DateTime.UtcNow
                    };

                    db.Products.Add(product);
                }
            }

            await db.SaveChangesAsync();

            // Update ProductCount per category
            foreach (var cat in db.Categories)
            {
                cat.ProductCount = await db.Products.CountAsync(p => p.CategoryId == cat.Id);
            }

            await db.SaveChangesAsync();
        }

        private static string Slug(string s)
        {
            s = s.ToLowerInvariant();
            var arr = s.Where(c => char.IsLetterOrDigit(c) || c == ' ').ToArray();
            return new string(arr).Trim().Replace(' ', '-');
        }
    }
}
