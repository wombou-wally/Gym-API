using Gym_API.Data;
using Gym_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly GymContext _context;

        public MembersController(GymContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            var members = await _context.Members
                .Include(m => m.MembershipPlan)
                .Include(m => m.Trainers)
                .OrderBy(m => m.Id)
                .ToListAsync();
            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _context.Members
                .Include(m => m.MembershipPlan)
                .Include(m => m.Trainers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }

        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            if (member.MembershipPlanId != null)
            {
                bool planExists = await _context.Plans.AnyAsync(p => p.Id == member.MembershipPlanId);
                if (!planExists)
                {
                    return BadRequest("Plan does not exist.");
                }
            }

            // Ignore any nested plan object; only MembershipPlanId decides the plan.
            member.MembershipPlan = null;

            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, Member member)
        {
            var existing = await _context.Members.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (member.MembershipPlanId != null)
            {
                bool planExists = await _context.Plans.AnyAsync(p => p.Id == member.MembershipPlanId);
                if (!planExists)
                {
                    return BadRequest("Plan does not exist.");
                }
            }

            existing.Name = member.Name;
            existing.Email = member.Email;
            existing.Phone = member.Phone;
            existing.JoinDate = member.JoinDate;
            existing.IsActive = member.IsActive;
            existing.MembershipPlanId = member.MembershipPlanId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("{id}/checkins")]
        public async Task<ActionResult<IEnumerable<CheckIn>>> History(int id)
        {
            bool memberExists = await _context.Members.AnyAsync(m => m.Id == id);
            if (!memberExists)
            {
                return NotFound();
            }

            var checkIns = await _context.CheckIns
                .Where(c => c.MemberId == id)
                .OrderByDescending(c => c.CheckInTime)
                .ToListAsync();
            return Ok(checkIns);
        }
        [HttpPost("{id}/trainers/{trainerId}")]
        public async Task<IActionResult> AssignTrainer(int id, int trainerId)
        {
            var member = await _context.Members
                .Include(m => m.Trainers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(trainerId);
            if (trainer == null)
            {
                return NotFound();
            }

            if (member.Trainers.Any(t => t.Id == trainerId))
            {
                return Conflict("Trainer already assigned.");
            }

            member.Trainers.Add(trainer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}