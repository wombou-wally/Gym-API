using Gym_API.Data;
using Gym_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<ActionResult<IEnumerable<MembershipPlan>>> GetPlans()
        {
            var plans = await _context.Plans.OrderBy(p => p.Id).ToListAsync();
            return Ok(plans);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MembershipPlan>> GetPlan(int id)
        {
            var plan = await _context.Plans.FindAsync(id);
            if(plan == null)
            {
                return NotFound();
            }
            return Ok(plan);
        }
        [HttpPost]
        public async Task<ActionResult<MembershipPlan>> PostPlan(MembershipPlan plan)
        {
            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlan), new { id = plan.Id }, plan);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlan(int id, MembershipPlan plan)
        {
            var old = await _context.Plans.FindAsync(id);
            if(old == null)
            {
                return NotFound();
            }
            old.Name = plan.Name;
            old.Price = plan.Price;
            old.DurationDays = plan.DurationDays;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlan(int id)
        {
            var plan = await _context.Plans.FindAsync(id);
            if(plan == null)
            {
                return NotFound();
            }
            bool hasMemner = await _context.Members.AnyAsync(m => m.MembershipPlanId == id);
            if(hasMemner)
            {
                return Conflict("Can not delate a plan that still has a member");
            }
            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}