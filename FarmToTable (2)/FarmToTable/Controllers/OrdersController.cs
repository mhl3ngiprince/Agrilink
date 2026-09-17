using FarmToTable.API.Data;
using FarmToTable.API.Models;
using FarmToTable.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.IO;

namespace FarmToTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly FarmToTableContext _context;

        public OrdersController(FarmToTableContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return order;
        }

        // POST: api/orders
        [HttpPost]
        [Consumes("application/json", "text/plain")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderCreateDto? orderDto)
        {
            // Fallback parse if model binding failed
            if (orderDto is null)
            {
                try
                {
                    Request.EnableBuffering();
                    Request.Body.Position = 0;
                    using var reader = new StreamReader(Request.Body, leaveOpen: true);
                    var raw = await reader.ReadToEndAsync();
                    Request.Body.Position = 0;
                    if (!string.IsNullOrWhiteSpace(raw))
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        using var doc = JsonDocument.Parse(raw);
                        var root = doc.RootElement;
                        // If it's a cart payload, redirect to cart order handler
                        if (root.TryGetProperty("items", out var itemsEl) && itemsEl.ValueKind == JsonValueKind.Array)
                        {
                            var cart = System.Text.Json.JsonSerializer.Deserialize<CartOrderCreateDto>(raw, options);
                            if (cart != null)
                            {
                                return await CreateCartOrder(cart);
                            }
                        }
                        if (root.TryGetProperty("order", out var orderEl))
                        {
                            orderDto = orderEl.Deserialize<OrderCreateDto>(options);
                        }
                        else
                        {
                            orderDto = System.Text.Json.JsonSerializer.Deserialize<OrderCreateDto>(raw, options);
                        }
                    }
                }
                catch
                {
                    // ignore and fall through to BadRequest
                }
            }

            if (orderDto is null)
            {
                return BadRequest(new { message = "Invalid order payload" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Basic validation
            if (orderDto.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must be greater than zero" });
            }
            orderDto.BuyerName = (orderDto.BuyerName ?? string.Empty).Trim();
            orderDto.BuyerEmail = (orderDto.BuyerEmail ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(orderDto.BuyerName))
            {
                return BadRequest(new { message = "Buyer name is required" });
            }
            if (string.IsNullOrWhiteSpace(orderDto.BuyerEmail))
            {
                return BadRequest(new { message = "Buyer email is required" });
            }

            // Resolve product by Id or Name
            Product? product = null;
            if (orderDto.ProductId > 0)
            {
                product = await _context.Products.FindAsync(orderDto.ProductId);
            }
            if (product == null && !string.IsNullOrWhiteSpace(orderDto.ProductName))
            {
                var name = orderDto.ProductName.Trim().ToLower();
                product = await _context.Products.OrderByDescending(p => p.Id)
                    .FirstOrDefaultAsync(p => p.Name.ToLower() == name);
            }
            if (product == null)
            {
                return BadRequest(new { message = "Product not found. Provide a valid productId or productName." });
            }

            // Ensure AvailableQuantity has a sensible default if zero but stock is positive
            if (product.AvailableQuantity <= 0 && product.StockQuantity > 0)
            {
                product.AvailableQuantity = product.StockQuantity;
            }

            if (product.AvailableQuantity < orderDto.Quantity)
            {
                return BadRequest(new { message = $"Only {product.AvailableQuantity} units available" });
            }

            // Optionally associate an existing buyer by email
            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Email.ToLower() == orderDto.BuyerEmail.ToLower());
            if (buyer == null)
            {
                buyer = new Buyer { Name = orderDto.BuyerName, Email = orderDto.BuyerEmail, Address = string.Empty, Phone = string.Empty, RegistrationDate = DateTime.UtcNow };
                _context.Buyers.Add(buyer);
                await _context.SaveChangesAsync();
            }

            // Create order
            var order = new Order
            {
                BuyerName = orderDto.BuyerName,
                BuyerEmail = orderDto.BuyerEmail,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                Buyer = buyer
            };

            // Create order item
            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = orderDto.Quantity,
                UnitPrice = product.Price
            };

            order.OrderItems.Add(orderItem);

            // Update product quantity and total
            product.AvailableQuantity -= orderDto.Quantity;
            order.TotalAmount = orderItem.UnitPrice * orderItem.Quantity;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // Alias endpoint to accommodate common frontend route names
        [HttpPost("place")]
        public Task<ActionResult<Order>> PlaceOrder([FromBody] OrderCreateDto? orderDto)
        {
            return CreateOrder(orderDto);
        }

        // Cart checkout: accepts multiple items
        [HttpPost("cart")]
        [Consumes("application/json", "text/plain")]
        public async Task<ActionResult<Order>> CreateCartOrder([FromBody] CartOrderCreateDto? cart)
        {
            if (cart is null)
            {
                try
                {
                    Request.EnableBuffering();
                    Request.Body.Position = 0;
                    using var reader = new StreamReader(Request.Body, leaveOpen: true);
                    var raw = await reader.ReadToEndAsync();
                    Request.Body.Position = 0;
                    if (!string.IsNullOrWhiteSpace(raw))
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        cart = System.Text.Json.JsonSerializer.Deserialize<CartOrderCreateDto>(raw, options);
                    }
                }
                catch { }
            }

            if (cart is null || cart.Items is null || cart.Items.Count == 0)
            {
                return BadRequest(new { message = "Cart is empty or invalid" });
            }

            // normalize buyer fields
            cart.BuyerName = (cart.BuyerName ?? string.Empty).Trim();
            cart.BuyerEmail = (cart.BuyerEmail ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(cart.BuyerName)) return BadRequest(new { message = "Buyer name is required" });
            if (string.IsNullOrWhiteSpace(cart.BuyerEmail)) return BadRequest(new { message = "Buyer email is required" });

            // Resolve or create buyer by email
            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Email.ToLower() == cart.BuyerEmail.ToLower());
            if (buyer == null)
            {
                buyer = new Buyer { Name = cart.BuyerName, Email = cart.BuyerEmail, Address = cart.ShippingAddress ?? string.Empty, Phone = string.Empty, RegistrationDate = DateTime.UtcNow };
                _context.Buyers.Add(buyer);
                await _context.SaveChangesAsync();
            }

            var order = new Order
            {
                BuyerName = cart.BuyerName,
                BuyerEmail = cart.BuyerEmail,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                Buyer = buyer,
                ShippingAddress = cart.ShippingAddress ?? string.Empty
            };

            decimal total = 0m;

            foreach (var item in cart.Items)
            {
                if (item.Quantity <= 0) return BadRequest(new { message = "Each item quantity must be greater than zero" });

                Product? product = null;
                if (item.ProductId > 0)
                {
                    product = await _context.Products.FindAsync(item.ProductId);
                }
                if (product == null && !string.IsNullOrWhiteSpace(item.ProductName))
                {
                    var name = item.ProductName.Trim().ToLower();
                    product = await _context.Products.OrderByDescending(p => p.Id)
                        .FirstOrDefaultAsync(p => p.Name.ToLower() == name);
                }
                if (product == null)
                {
                    return BadRequest(new { message = $"Product not found for cart item: {item.ProductName ?? item.ProductId.ToString()}" });
                }

                // default available qty from stock
                if (product.AvailableQuantity <= 0 && product.StockQuantity > 0)
                {
                    product.AvailableQuantity = product.StockQuantity;
                }
                if (product.AvailableQuantity < item.Quantity)
                {
                    return BadRequest(new { message = $"Only {product.AvailableQuantity} units available for {product.Name}" });
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };
                order.OrderItems.Add(orderItem);
                product.AvailableQuantity -= item.Quantity;
                total += orderItem.UnitPrice * orderItem.Quantity;
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/orders/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTO for creating orders
    public class OrderCreateDto
    {
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class CartOrderCreateDto
    {
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public List<CartItem> Items { get; set; } = new();
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
    }
}