using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmToTable.API.Data;
using FarmToTable.API.Models;

namespace FarmToTable.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FarmersController : ControllerBase
    {
        private readonly FarmToTableContext _context;

        public FarmersController(FarmToTableContext context)
        {
            _context = context;
        }

        // GET: api/Farmers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Farmer>>> GetFarmers()
        {
            return await _context.Farmers.ToListAsync();
        }

        // GET: api/Farmers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Farmer>> GetFarmer(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
                return NotFound();
            return farmer;
        }

        // POST: api/Farmers/register
        [HttpPost("register")]
        public async Task<ActionResult<Farmer>> RegisterFarmer(Farmer farmer)
        {
            if (farmer == null)
            {
                return BadRequest("Invalid farmer data.");
            }

            // Normalize email for comparison (trim + case-insensitive)
            var normalizedEmail = (farmer.Email ?? string.Empty).Trim().ToLower();

            // Check for duplicate email
            if (await _context.Farmers.AnyAsync(f => f.Email.ToLower() == normalizedEmail))
            {
                return BadRequest("A farmer with this email already exists.");
            }

            // Initialize defaults
            farmer.RegistrationDate = DateTime.UtcNow;
            farmer.IsVerified = false;

            _context.Farmers.Add(farmer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFarmer), new { id = farmer.Id }, farmer);
        }

        // PUT: api/Farmers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFarmer(int id, Farmer farmer)
        {
            if (id != farmer.Id)
                return BadRequest();

            _context.Entry(farmer).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Farmers.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        // DELETE: api/Farmers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarmer(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
                return NotFound();

            _context.Farmers.Remove(farmer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}