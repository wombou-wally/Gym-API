using Gym_API.Data;
using Gym_API.DTOs;
using Gym_API.Models;
using Microsoft.AspNetCore.Authorization;
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

        private IQueryable<MemberResponse> MemberResponses()
        {
            return _context.Members
                .OrderBy(m => m.Id)
                .Select(m => new MemberResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    Phone = m.Phone,
                    JoinDate = m.JoinDate,
                    IsActive = m.IsActive,
                    ExpiryDate = m.ExpiryDate,
                    Plan = m.MembershipPlan == null ? null : new PlanSummary
                        {   
                            Name = m.MembershipPlan.Name,
                            Price = m.MembershipPlan.Price
                        },
                    Trainers = m.Trainers
                        .Select(t => new TrainerSummary
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Speciality = t.Speciality
                        })
                        .ToList()
                });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberResponse>>> GetMembers()
        {
            var members = await MemberResponses().ToListAsync();
            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberResponse>> GetMember(int id)
        {
            var member = await _context.Members
                .Where(m => m.Id == id)
                .Select(m => new MemberResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    Phone = m.Phone,
                    JoinDate = m.JoinDate,
                    IsActive = m.IsActive,
                    ExpiryDate = m.ExpiryDate,
                    Plan = m.MembershipPlan == null ? null : new PlanSummary
                        {
                            Name = m.MembershipPlan.Name,
                            Price = m.MembershipPlan.Price
                        },
                    Trainers = m.Trainers
                        .Select(t => new TrainerSummary
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Speciality = t.Speciality
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }

        [HttpPost]
        public async Task<ActionResult<MemberResponse>> PostMember(CreateMemberRequest request)
        {
            if (request.MembershipPlanId != null)
            {
                bool planExists = await _context.Plans.AnyAsync(p => p.Id == request.MembershipPlanId);
                if (!planExists)
                {
                    return BadRequest("Plan does not exist.");
                }
            }

            var member = new Member
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                JoinDate = request.JoinDate,
                MembershipPlanId = request.MembershipPlanId,
                IsActive = true
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMember), new { id = member.Id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, UpdateMemberRequest request)
        {
            var existing = await _context.Members.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (request.MembershipPlanId != null)
            {
                bool planExists = await _context.Plans.AnyAsync(p => p.Id == request.MembershipPlanId);
                if (!planExists)
                {
                    return BadRequest("Plan does not exist.");
                }
            }

            existing.Name = request.Name;
            existing.Email = request.Email;
            existing.Phone = request.Phone;
            existing.MembershipPlanId = request.MembershipPlanId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
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
        public async Task<ActionResult<IEnumerable<CheckInResponse>>> History(int id)
        {
            bool memberExists = await _context.Members.AnyAsync(m => m.Id == id);
            if (!memberExists)
            {
                return NotFound();
            }

            var checkIns = await _context.CheckIns
                .Where(c => c.MemberId == id)
                .OrderByDescending(c => c.CheckInTime)
                .Select(c => new CheckInResponse
                {
                    Id= c.Id,
                    MemberId = c.MemberId,
                    CheckInTime = c.CheckInTime
                }
                ).ToListAsync();
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

        [HttpPost("{id}/renew")]
        public async Task<ActionResult<RenewResponse>> Renew(int id)
        {
            var member
                = await _context.Members
                .Include(m => m.MembershipPlan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
                return NotFound();
            if (member.MembershipPlan == null)
                return BadRequest("member has no plan");
            DateOnly StartDate = DateOnly.FromDateTime(DateTime.Now);
            if(member.ExpiryDate != null && member.ExpiryDate.Value > StartDate)
            {
                 StartDate = member.ExpiryDate.Value;
            }
            member.ExpiryDate = StartDate.AddDays(member.MembershipPlan.DurationDays);
            member.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new RenewResponse
            {
                MemberId = member.Id,
                ExpiryDate = member.ExpiryDate,
                IsActive = member.IsActive
            });

        }
    }
}