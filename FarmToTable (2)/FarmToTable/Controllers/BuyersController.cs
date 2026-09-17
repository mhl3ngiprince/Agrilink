using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmToTable.API.Data;
using FarmToTable.Models;

namespace FarmToTable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyersController : ControllerBase
    {
        private readonly FarmToTableContext _context;

        public BuyersController(FarmToTableContext context)
        {
            _context = context;
        }

        // GET: api/Buyers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Buyer>>> GetBuyers()
        {
            return await _context.Buyers.ToListAsync();
        }

        // GET: api/Buyers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Buyer>> GetBuyer(int id)
        {
            var buyer = await _context.Buyers.FindAsync(id);

            if (buyer == null)
            {
                return NotFound();
            }

            return buyer;
        }

        // PUT: api/Buyers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBuyer(int id, Buyer buyer)
        {
            if (id != buyer.Id)
            {
                return BadRequest();
            }

            _context.Entry(buyer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuyerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Buyers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Buyer>> PostBuyer(Buyer buyer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Normalize inputs
            buyer.Name = (buyer.Name ?? string.Empty).Trim();
            buyer.Email = (buyer.Email ?? string.Empty).Trim().ToLower();

            if (string.IsNullOrWhiteSpace(buyer.Name))
            {
                return BadRequest(new { message = "Name is required" });
            }
            if (string.IsNullOrWhiteSpace(buyer.Email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            // Duplicate email check (case-insensitive)
            var exists = await _context.Buyers.AnyAsync(b => b.Email.ToLower() == buyer.Email);
            if (exists)
            {
                return BadRequest(new { message = "A buyer with this email already exists" });
            }

            // Initialize defaults
            if (string.IsNullOrWhiteSpace(buyer.Address)) buyer.Address = string.Empty;
            if (string.IsNullOrWhiteSpace(buyer.Phone)) buyer.Phone = string.Empty;
            if (buyer.RegistrationDate == default) buyer.RegistrationDate = DateTime.UtcNow;

            _context.Buyers.Add(buyer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBuyer", new { id = buyer.Id }, buyer);
        }

        // POST: api/Buyers/register
        [HttpPost("register")]
        public Task<ActionResult<Buyer>> RegisterBuyer(Buyer buyer)
        {
            return PostBuyer(buyer);
        }

        // DELETE: api/Buyers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuyer(int id)
        {
            var buyer = await _context.Buyers.FindAsync(id);
            if (buyer == null)
            {
                return NotFound();
            }

            _context.Buyers.Remove(buyer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BuyerExists(int id)
        {
            return _context.Buyers.Any(e => e.Id == id);
        }
    }
}
