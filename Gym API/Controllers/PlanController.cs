using Gym_API.Data;
using Gym_API.DTOs;
using Gym_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly GymContext _context;

        public PlansController(GymContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlansResponse>>> GetPlans()
        {
            var plans = await _context.Plans
                .OrderBy(p => p.Id)
                .Select(p => new PlansResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DurationDays = p.DurationDays
                })
                .ToListAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlansResponse>> GetPlan(int id)
        {
            var plan = await _context.Plans
                .Where(p => p.Id == id)
                .Select(p => new PlansResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DurationDays = p.DurationDays
                })
                .FirstOrDefaultAsync();

            if (plan == null)
            {
                return NotFound();
            }
            return Ok(plan);
        }

        [HttpPost]
        public async Task<ActionResult<PlansResponse>> PostPlan(CreatePlanRequest request)
        {
            var plan = new MembershipPlan
            {
                Name = request.Name,
                Price = request.Price,
                DurationDays = request.DurationDays
            };

            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();

            var response = new PlansResponse
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                DurationDays = plan.DurationDays
            };
            return CreatedAtAction(nameof(GetPlan), new { id = plan.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlan(int id, UpdatePlanRequest request)
        {
            var existing = await _context.Plans.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = request.Name;
            existing.Price = request.Price;
            existing.DurationDays = request.DurationDays;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var plan = await _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            bool hasMembers = await _context.Members.AnyAsync(m => m.MembershipPlanId == id);
            if (hasMembers)
            {
                return Conflict("Cannot delete a plan that still has members.");
            }

            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}