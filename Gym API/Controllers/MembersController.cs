using Gym_API.Data;
using Gym_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
            var members = await _context.Members.ToListAsync();
            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(member);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMember), new {id = member.Id}, member);
        }

        [HttpPut ("{id}")]
        public async Task<IActionResult> UpdateMember(int id, Member member)
        {
            var old =  await _context.Members.FindAsync(id);
            if(old == null)
            {
                return NotFound();
            }
                old.Name = member.Name;
                old.Email = member.Email;
                old.Phone = member.Phone;
                old.JoinDate = member.JoinDate;
                old.IsActive = member.IsActive;
                await _context.SaveChangesAsync();
                return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if(member == null)
            {
                return NotFound();
            }
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}